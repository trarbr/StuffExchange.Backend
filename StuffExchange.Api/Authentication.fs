module StuffExchange.Api.Auth 

open System.IO
open Newtonsoft.Json

open Nancy
open Nancy.Authentication.Token
open Nancy.Security

open StuffExchange.Api.Helpers

type UserIdentity(userName, claims) =
    interface IUserIdentity with 
        member x.Claims: System.Collections.Generic.IEnumerable<string> = claims
        member x.UserName: string = userName

let getUser username password = 
    let troels = "5d07438d-4e7e-4fc7-92a6-bce87c60cd27" 
    let anne_marie = "8e047a93-3fce-401c-9768-74c1fb9e9938"
    match username, password with
        | "troels", "1234" -> 
            UserIdentity(troels, ["User"]) |> Some
        | "anne-marie", @"OqkB^K1aW42%n*1tEho427VH1txZaZ" -> 
            UserIdentity(anne_marie, ["User"]) |> Some
        | _ -> None

type TokenResponse = { Token: string; UserId: string }

let getToken (tokenizer : ITokenizer) context user =
    let token = tokenizer.Tokenize(user, context)
    { Token = token; UserId = user.UserName }


[<CLIMutable>]
type LoginRequest = { Username:string; Password:string }

type AuthModule(tokenizer : ITokenizer) as x =
    inherit NancyModule("/auth")

    do x.Get.["/"] <- fun _ ->
        // If RequiresAuthentication fails it will throw an exception, but it can safely be ignored:
        // Nancy will notice and return a 401
        x.RequiresAuthentication()

        // Return a new token for the user, use it to keep logins going
        getToken tokenizer x.Context x.Context.CurrentUser
        |> box

    do x.Post.["/"] <- fun _ ->
        let request = getRequest<LoginRequest> x.Request.Body

        getUser request.Username request.Password 
        |> function
            | Some user -> 
                getToken tokenizer x.Context user 
                |> JsonConvert.SerializeObject
                |> box
            | None -> 
                HttpStatusCode.Unauthorized 
                |> box