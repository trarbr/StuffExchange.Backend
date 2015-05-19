module StuffExchange.Api.User

open Nancy
open Nancy.Security
open Newtonsoft.Json

open StuffExchange.Contract.Commands
open StuffExchange.Ports.User
open StuffExchange.Api.Helpers

open StuffExchange.BusinessRules.Railway

type UserModule() as x =
    inherit NancyModule("/user")

    do x.Put.["/"] <- fun _ ->
        x.RequiresAuthentication()
        let headers = Seq.toList x.Request.Headers.Accept
        let commandText = getCommandText headers

        match commandText with
        | "activate" -> 
            System.Guid(x.Context.CurrentUser.UserName) 
            |> ActivateUser
            |> routeCommand
            |> function
                | Success _ -> box HttpStatusCode.OK
                | Failure f ->
                    textResponse HttpStatusCode.BadRequest f
                    |> box
        | _ -> box HttpStatusCode.NotFound

    do x.Post.["/"] <- fun _ ->
        // create a new user
        box HttpStatusCode.NotImplemented