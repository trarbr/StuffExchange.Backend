module StuffExchange.Api.Auth 

open System.IO
open Newtonsoft.Json

open Nancy
open Nancy.Authentication.Token
open Nancy.Security

type UserIdentity(userName, claims) =
    interface IUserIdentity with 
        member x.Claims: System.Collections.Generic.IEnumerable<string> = claims
        member x.UserName: string = userName

let getUser username password = 
    let troels = "5D07438D-4E7E-4FC7-92A6-BCE87C60CD27" // es stream: 5d07438d-4e7e-4fc7-92a6-bce87c60cd27
    let anne_marie = "8E047A93-3FCE-401C-9768-74C1FB9E9938"
    match username, password with
        | "troels", "1234" -> 
            UserIdentity(troels, ["User"]) |> Some
        | "anne-marie", @"OqkB^K1aW42%n*1tEho427VH1txZaZ" -> 
            UserIdentity(anne_marie, ["User"]) |> Some
        | _ -> None

type TokenResponse = { Token:string }

let getToken (tokenizer : ITokenizer) context user =
    let token = tokenizer.Tokenize(user, context)
    { Token = token }


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
        use rdr = new StreamReader(x.Request.Body)
        let s = rdr.ReadToEnd()
        let request = JsonConvert.DeserializeObject<LoginRequest>(s)

        getUser request.Username request.Password 
        |> function
            | Some user -> 
                getToken tokenizer x.Context user 
                |> JsonConvert.SerializeObject
                |> box
            | None -> 
                HttpStatusCode.Unauthorized 
                |> box