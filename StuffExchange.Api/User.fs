module StuffExchange.Api.User

open Nancy
open Nancy.Security
open Newtonsoft.Json

open StuffExchange.Contract.Commands
open StuffExchange.Contract.Types
open StuffExchange.Core.Railway
open StuffExchange.Ports.User
open StuffExchange.Api.Helpers


type UserModule() as x =
    inherit NancyModule("/users")

    do x.Put.["/"] <- fun _ ->
        x.RequiresAuthentication()
        let headers = Seq.toList x.Request.Headers.Accept
        let commandText = getCommandText headers

        match commandText with
        | "activate" -> 
            {UserActivation.Id = System.Guid(x.Context.CurrentUser.UserName)}
            |> ActivateUser
            |> routeCommand
            |> function
                // TODO: If success respond with 202 Accepted and id of command / url for looking up result
                | Success _ -> box HttpStatusCode.OK
                | Failure f ->
                    jsonResponse HttpStatusCode.BadRequest f
                    |> box
        | _ -> box HttpStatusCode.NotFound

    do x.Post.["/"] <- fun _ ->
        // create a new user
        box HttpStatusCode.NotImplemented