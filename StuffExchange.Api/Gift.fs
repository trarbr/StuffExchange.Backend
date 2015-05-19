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
