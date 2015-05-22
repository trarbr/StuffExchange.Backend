module StuffExchange.Api.Helpers

open Nancy
open System.IO
open Newtonsoft.Json

open StuffExchange.Core.Railway

let (?) (parameters: obj) param =
    (parameters :?> Nancy.DynamicDictionary).[param]


let jsonResponse (statusCode: HttpStatusCode) body =
    let body = JsonConvert.SerializeObject(body)
    let response = new Nancy.Responses.TextResponse(statusCode, body)
    response.ContentType <- "application/json"
    box response

let respond result =
    match result with
    | Success s ->
        jsonResponse HttpStatusCode.OK s
    | Failure f ->
        jsonResponse HttpStatusCode.BadRequest f

let getCommandText (headers: List<(string * decimal)>) =
    // format goes like: application/vnd.stuffexchange.command+json - THIS IS NOT 5LMT
    // might be simpler to spit on ; and look for command= ?
    // crap that should be in ContentType header not Accept header! (except ContentType is only on response?!)
    // it's only the version of the api that goes in the Accept header
    let header, _ = headers.[0]
    header.Split('/').[1].Split('.').[2].Split('+').[0]

let getRequest<'a> (body:IO.RequestStream) =
        use rdr = new StreamReader(body)
        let s = rdr.ReadToEnd()
        JsonConvert.DeserializeObject<'a>(s)
