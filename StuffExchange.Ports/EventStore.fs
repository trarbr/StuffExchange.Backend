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

let addEventToAggregate id event =
    appendToStream (id.ToString()) event |> Success

let subscribeToEventType eventType eventHandler =
    let streamId = sprintf "$et-%s" eventType
    subscribeToStream streamId eventHandler

    