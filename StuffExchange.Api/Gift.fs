module StuffExchange.Api.Gift

open Nancy
open Nancy.Security
open Newtonsoft.Json

open StuffExchange.Contract.Commands
open StuffExchange.Ports.Gift
open StuffExchange.Ports.ReadStore

open StuffExchange.BusinessRules.Railway

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
    inherit NancyModule("/gift")

    do x.Get.["/"] <- fun _ ->
        // can't return 'em all when using key-value store :(
        box HttpStatusCode.NotImplemented

    do x.Get.["/{id:guid}"] <- fun parameters ->
        System.Guid((parameters?id).ToString())
        |> getGift
        |> function
            | Success gift -> box gift
            | Failure f -> 
                textResponse HttpStatusCode.BadRequest f
                |> box
        // get from Riak
        //box HttpStatusCode.NotImplemented


    do x.Post.["/"] <- fun _ ->
        x.RequiresAuthentication()
        let request = getRequest<AddGiftRequest> x.Request.Body
        let userId = System.Guid(x.Context.CurrentUser.UserName)

        AddGift (System.Guid.NewGuid(), userId, request.Title, request.Description)
        |> routeCommand
        |> function
            | Success _ -> box HttpStatusCode.OK
            | Failure f -> 
                textResponse HttpStatusCode.BadRequest f
                |> box


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
            AddComment (commentId, giftId, userId, request.Timestamp, request.Content)
            |> routeCommand
            |> function
                | Success _ -> box HttpStatusCode.OK
                | Failure f -> 
                    textResponse HttpStatusCode.BadRequest f
                    |> box
        | "addImage" -> box HttpStatusCode.NotImplemented
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
            ChangeTitle (request.Gift, request.Title)
            |> routeCommand
            |> function 
                | Success _ -> box HttpStatusCode.OK
                | Failure f -> 
                    HttpStatusCode.BadRequest
                    |> box
        | "updateDescription" ->
            let request = getRequest<UpdateDescriptionRequest> x.Request.Body
            UpdateDescription (request.Gift, request.Description)
            |> routeCommand
            |> function
                | Success _ -> box HttpStatusCode.OK
                | Failure f -> 
                    HttpStatusCode.BadRequest
                    |> box
        | _ -> box HttpStatusCode.NotFound


