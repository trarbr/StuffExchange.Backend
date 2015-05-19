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
    match event with
    | UserActivated userId 
    | UserDeactivated userId 
    | GiftAdded (userId, _, _) ->
        appendToStream (userId.ToString()) event |> Success


