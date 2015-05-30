module StuffExchange.Api.Users

open Nancy
open Nancy.Security
open Newtonsoft.Json

open StuffExchange.Contract.Types
open StuffExchange.Core.Railway
open StuffExchange.Api.Helpers
open StuffExchange.Ports.ReadStore


type UserModule() as x =
    inherit NancyModule("/users")

    do x.Get.["/{id:guid}"] <- fun parameters ->
        x.RequiresAuthentication()
        let userId = System.Guid((parameters?id).ToString())
        if x.Context.CurrentUser.UserName = userId.ToString()
        then
            userId
            |> getUser
            |> respond
        else
            box HttpStatusCode.Unauthorized

    do x.Put.["/{id:guid}"] <- fun _ ->
        x.RequiresAuthentication()
        let commandName = getCommandName x.Request.Headers
        let userId = System.Guid(x.Context.CurrentUser.UserName)

        match commandName with
        | "ActivateUser" -> 
            {UserActivation.Id = userId}
            |> ActivateUserDto
            |> route
            |> respond
        | "DeactivateUser" -> box HttpStatusCode.NotImplemented
        | _ -> box HttpStatusCode.UnsupportedMediaType

    do x.Post.["/"] <- fun _ ->
        // create a new user
        box HttpStatusCode.NotImplemented