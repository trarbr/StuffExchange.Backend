module StuffExchange.Ports.EventStore

open StuffExchange.EventStore.AsyncExtensions
open StuffExchange.EventStore.Store
open StuffExchange.BusinessRules.Railway
open StuffExchange.Contract.Events


let getEventsForAggregate id =
    // get in touch with eventstore, read the stream, return a list of all deserialized events
    // connect()
    id.ToString()
    |> readStream 

let addEventToAggregate event =
    let aggregateId =
        match event with
        | UserActivated id 
        | UserDeactivated id 
        | GiftAdded (id, _, _, _)
        | CommentAdded (_, id, _, _, _) -> id
        | _ -> failwith "Not yet implemented"
    
    appendToStream (aggregateId.ToString()) event |> Success