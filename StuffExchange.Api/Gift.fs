module StuffExchange.Api.Gift

open Nancy
open Nancy.Security
open Newtonsoft.Json

open StuffExchange.Core.Railway
open StuffExchange.Core.ImageProcessing
open StuffExchange.Contract.Commands
open StuffExchange.Contract.Types
open StuffExchange.Ports.Gift
open StuffExchange.Ports.ReadStore


open StuffExchange.Api.Helpers

[<CLIMutable>]
type AddGiftRequest = { Title: string; Description: string }
[<CLIMutable>]
type ChangeTitleRequest = { Gift: System.Guid; Title: string }
[<CLIMutable>]
type UpdateDescriptionRequest = { Gift: System.Guid; Description: string }
[<CLIMutable>]
type AddCommentRequest = { Timestamp: System.DateTime; Content: string }

type GiftModule() as x =
    inherit NancyModule("/gifts")

    do x.Get.["/"] <- fun _ ->
        getGifts()
        |> respond

    do x.Get.["/{id:guid}"] <- fun parameters ->
        System.Guid((parameters?id).ToString())
        |> getGift
        |> respond


    do x.Post.["/"] <- fun _ ->
        x.RequiresAuthentication()
        let request = getRequest<AddGiftRequest> x.Request.Body
        let userId = System.Guid(x.Context.CurrentUser.UserName)

        {GiftAddition.Id = System.Guid.NewGuid(); User = userId; Title = request.Title; Description = request.Description}
        |> AddGift
        |> routeCommand
        |> respond


    do x.Post.["/{id:guid}"] <- fun parameters ->
        x.RequiresAuthentication()
        let userId = System.Guid(x.Context.CurrentUser.UserName)
        let giftId = System.Guid((parameters?id).ToString())
        let headers = Seq.toList x.Request.Headers.Accept
        let commandText = getCommandText headers

        match commandText with
        | "addComment" ->
            let request = getRequest<AddCommentRequest> x.Request.Body
            let commentId = System.Guid.NewGuid()
            {CommentAddition.Id = commentId; Gift = giftId; User = userId; Timestamp = request.Timestamp; Content = request.Content}
            |> AddComment 
            |> routeCommand
            |> respond
        | "addImage" -> 
            // TODO: check that it is a jpg
            let image = Seq.head x.Request.Files
            let imageId = System.Guid.NewGuid()
            let filename = sprintf @"images\%s.jpg" (imageId.ToString())
            let fileStream = System.IO.File.Create(filename)
            image.Value.CopyTo fileStream
            fileStream.Close()
            let thumbnail = saveThumbnail filename
            {ImageAddition.Id = imageId; Gift = giftId}
            |> AddImage
            |> routeCommand
            |> respond
        | _ -> box HttpStatusCode.NotFound

        // non-idempotent commands are POST'ed to the id of the gift, but you still need
        // to set command in accept header
        // but then, why not have id in url for put?

    do x.Put.["/"] <- fun _ ->
        x.RequiresAuthentication()
        let userId = System.Guid(x.Context.CurrentUser.UserName)
        let headers = Seq.toList x.Request.Headers.Accept
        let commandText = getCommandText headers

        match commandText with
        | "changeTitle" ->
            // what if this throws?!
            let request = getRequest<ChangeTitleRequest> x.Request.Body
            {TitleChange.Gift = request.Gift; NewTitle = request.Title}
            |> ChangeTitle
            |> routeCommand
            |> respond
        | "updateDescription" ->
            let request = getRequest<UpdateDescriptionRequest> x.Request.Body
            {DescriptionUpdate.Gift = request.Gift; NewDescription = request.Description}
            |> UpdateDescription
            |> routeCommand
            |> respond
        | _ -> box HttpStatusCode.NotFound


