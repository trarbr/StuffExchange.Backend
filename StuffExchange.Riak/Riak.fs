module StuffExchange.Riak.Riak

open RiakClient
open RiakClient.Models
open Newtonsoft.Json

open StuffExchange.Core.Railway

// TODO: set app.config value in EXE's app.config
let writeToRiak (bucket: string) (key: string) value = 
    let serializedValue = JsonConvert.SerializeObject(value)
    use cluster : IRiakEndPoint = RiakCluster.FromConfig("riakConfig")
    let client : IRiakClient = cluster.CreateClient()
    let ro = new RiakObject(bucket, key, serializedValue)
    let putResult = client.Put ro
    match putResult.IsSuccess with
    | true -> printfn "Wrote %A to riak" value
    | false -> printfn "Something went wrong"

let readFromRiak<'a> (bucket: string) (key: string) =
    printfn "trying to read"
    printfn "trying to read"
    use cluster : IRiakEndPoint = RiakCluster.FromConfig("riakConfig")
    printfn "using cluster"
    let client : IRiakClient = cluster.CreateClient()
    printfn "made client"
    let result = client.Get(bucket, key)
    match result.IsSuccess with
    | true -> 
        let value = result.Value.Value
        let valueAsString = System.Text.Encoding.UTF8.GetString(value)
        JsonConvert.DeserializeObject<'a>(valueAsString)
        |> Success
    | false -> 
        RiakGetFailed "Something went wrong" 
        |> Failure
