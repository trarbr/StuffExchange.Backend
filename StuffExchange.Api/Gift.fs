module StuffExchange.Api.Gift

open Nancy
open Nancy.Security
open Newtonsoft.Json

open StuffExchange.Contract.Commands
open StuffExchange.Ports.Gift

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

                // addedgift: 46a467cf-ab43-4974-b5db-c327ff8fb0d0

    do x.Post.["/{id:guid}"] <- fun parameters ->
        x.RequiresAuthentication()
        let userId = System.Guid(x.Context.CurrentUser.UserName)
        let p = parameters :?> Nancy.DynamicDictionary
        let gift = System.Guid(p.["id"].ToString())
        let headers = Seq.toList x.Request.Headers.Accept
        let commandText = getCommandText headers

        match commandText with
        | "addComment" ->
            let request = getRequest<AddCommentRequest> x.Request.Body
            let commentId = System.Guid.NewGuid()
            // todo: fix timestamps serialization
            // 2012-04-23T18:25:43.511Z
            // http://stackoverflow.com/questions/10286204/the-right-json-date-format
            // Seems to be the default anyway
            AddComment (commentId, gift, userId, request.Timestamp, request.Content)
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
        //box HttpStatusCode.NotImplemented

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


