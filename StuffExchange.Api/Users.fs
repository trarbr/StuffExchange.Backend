module StuffExchange.Api.Users

open Nancy
open Nancy.Security
open Newtonsoft.Json

open StuffExchange.Contract.Types
open StuffExchange.Core.Railway
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
            |> ActivateUserDto
            |> route
            |> respond
        | _ -> box HttpStatusCode.NotFound

    do x.Post.["/"] <- fun _ ->
        // create a new user
        box HttpStatusCode.NotImplemented