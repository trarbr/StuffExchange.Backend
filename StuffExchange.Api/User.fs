module StuffExchange.Api.User

open Nancy
open Nancy.Security
open Newtonsoft.Json

open StuffExchange.Contract.Commands
open StuffExchange.Ports.User

open StuffExchange.BusinessRules.Railway

let textResponse (statusCode: HttpStatusCode) (body: string) =
    new Nancy.Responses.TextResponse(statusCode, body)

type UserModule() as x =
    inherit NancyModule("/user")

    do x.Put.["/"] <- fun _ ->
        // format goes like: application/vnd.stuffexchange.command+json
        // if you get the format wrong, you die
        // api version goes v1, v2, v3 etc
        // might be simpler to spit on ; and look for command= ?
        x.RequiresAuthentication()
        let headers = Seq.toList x.Request.Headers.Accept
        let header, _ = headers.[0] 
        let commandText = header.Split('/').[1].Split('.').[2].Split('+').[0]
        match commandText with
            | "activate" -> 
                System.Guid(x.Context.CurrentUser.UserName) 
                |> ActivateUser
                |> routeCommand
                |> function
                    | Success _ -> box HttpStatusCode.OK
                    | Failure f ->
                        JsonConvert.SerializeObject(f)
                        |> textResponse HttpStatusCode.BadRequest
                        |> box
            | _ -> box HttpStatusCode.NotFound

    do x.Post.["/"] <- fun _ ->
        // create a new user
        box HttpStatusCode.NotImplemented