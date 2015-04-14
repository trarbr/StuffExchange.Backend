module StuffExchange.Api.User

open Nancy
open Nancy.Security

open StuffExchange.Contract.Commands
open StuffExchange.Ports.User

open StuffExchange.BusinessRules.Railway

type UserModule() as x =
    inherit NancyModule("/user")

    do x.Put.["/"] <- fun _ ->
        // format goes like: application/vnd.stuffexchange[.version].command+json
        // if you get the format wrong, you die
        // api version goes v1, v2, v3 etc
        // might be simpler to spit on ; and look for command= ?
        x.RequiresAuthentication()
        let headers = Seq.toList x.Request.Headers.Accept
//        box headers
        let header, _ = headers.[0] 
        let commandText = header.Split('/').[1].Split('.').[2].Split('+').[0]
        match commandText with
            | "activate" -> 
                // parse request body, create command, send to port
                let userId = System.Guid(x.Context.CurrentUser.UserName)
                let command = ActivateUser userId
                let result = routeCommand command 
                // how to unpack the result!?
                match result with
                    | Success s -> box (s.GetType().Name, s)
                    | Failure f -> box (f.GetType().Name, f)
            | _ -> box HttpStatusCode.NotFound

    do x.Post.["/"] <- fun _ ->
        // create a new user
        box HttpStatusCode.NotImplemented