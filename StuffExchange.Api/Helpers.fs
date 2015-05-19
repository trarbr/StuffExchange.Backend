module StuffExchange.Api.Helpers

open Nancy
open System.IO
open Newtonsoft.Json

let textResponse (statusCode: HttpStatusCode) failure =
    let body = JsonConvert.SerializeObject(failure)
    new Nancy.Responses.TextResponse(statusCode, body)

let getCommandText (headers: List<(string * decimal)>) =
    // format goes like: application/vnd.stuffexchange.command+json
    // might be simpler to spit on ; and look for command= ?
    let header, _ = headers.[0]
    header.Split('/').[1].Split('.').[2].Split('+').[0]

let getRequest<'a> (body:IO.RequestStream) =
        use rdr = new StreamReader(body)
        let s = rdr.ReadToEnd()
        JsonConvert.DeserializeObject<'a>(s)
