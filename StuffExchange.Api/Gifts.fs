module StuffExchange.Api.Gifts

open Nancy
open Nancy.Security
open Newtonsoft.Json

open StuffExchange.Core.Railway
open StuffExchange.Api.ImageProcessing
open StuffExchange.Contract.Commands
open StuffExchange.Contract.Types
open StuffExchange.Ports.ReadStore
open StuffExchange.Api.Helpers

[<CLIMutable>]
type AddGiftRequest = { Title: string; Description: string }
[<CLIMutable>]
type ChangeTitleRequest = { Title: string }
[<CLIMutable>]
type UpdateDescriptionRequest = { Description: string }
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
        let commandName = getCommandName x.Request.Headers

        match commandName with
        | "AddGift" ->
            {GiftAddition.Id = System.Guid.NewGuid(); User = userId; Title = request.Title; Description = request.Description}
            |> AddGiftDto
            |> route
            |> respond
        | _ -> box HttpStatusCode.UnsupportedMediaType




    do x.Post.["/{id:guid}"] <- fun parameters ->
        x.RequiresAuthentication()
        let userId = System.Guid(x.Context.CurrentUser.UserName)
        let giftId = System.Guid((parameters?id).ToString())
        let commandName = getCommandName x.Request.Headers

        match commandName with
        | "AddComment" ->
            let request = getRequest<AddCommentRequest> x.Request.Body
            let commentId = System.Guid.NewGuid()
            {CommentAddition.Id = commentId; Gift = giftId; User = userId; Timestamp = request.Timestamp; Content = request.Content}
            |> AddCommentDto 
            |> route
            |> respond
        | "AddImage" -> 
            // TODO: check that it is a jpg
            let imageId = System.Guid.NewGuid()
            let image = Seq.head x.Request.Files
            let filename = sprintf @"images/%s.jpg" (imageId.ToString())
            let fileStream = System.IO.File.Create(filename)
            image.Value.CopyTo fileStream
            fileStream.Close()
            let thumbnail = saveThumbnail filename
            {ImageAddition.Id = imageId; Gift = giftId}
            |> AddImageDto
            |> route
            |> respond
        | _ -> box HttpStatusCode.UnsupportedMediaType


    do x.Put.["/{id:guid}"] <- fun parameters ->
        x.RequiresAuthentication()
        let userId = System.Guid(x.Context.CurrentUser.UserName)
        let giftId = System.Guid((parameters?id).ToString())
        let commandName = getCommandName x.Request.Headers

        match commandName with
        | "ChangeTitle" ->
            // TODO: request validation - or live with 500 
            // what if this throws?! 500 internal server error.
            let request = getRequest<ChangeTitleRequest> x.Request.Body
            {TitleChange.Gift = giftId; NewTitle = request.Title}
            |> ChangeTitleDto
            |> route
            |> respond
        | "UpdateDescription" ->
            let request = getRequest<UpdateDescriptionRequest> x.Request.Body
            {DescriptionUpdate.Gift = giftId; NewDescription = request.Description}
            |> UpdateDescriptionDto
            |> route
            |> respond
        | _ -> box HttpStatusCode.UnsupportedMediaType


