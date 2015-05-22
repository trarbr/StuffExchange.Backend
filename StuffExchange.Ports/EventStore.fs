module StuffExchange.Ports.EventStore

open StuffExchange.EventStore.AsyncExtensions
open StuffExchange.EventStore.Store
open StuffExchange.Contract.Railway
open StuffExchange.Contract.Events


let getEventsForAggregate id =
    id.ToString()
    |> readStream 

let addEventToAggregate id event =
    appendToStream (id.ToString()) event |> Success

let subscribeToEventType eventType eventHandler =
    let streamId = sprintf "$et-%s" eventType
    subscribeToStream streamId eventHandler

let subscribeToDomainEvents eventHandler =
    let streamId = "domainEvents" 
    subscribeToStream streamId eventHandler

(* The domainProjection.
   Remember to run with --run-projections=all
   and enable emit for the projection
   and set mode as Continous
fromAll().when({
    'GiftAdded': handle,
    'TitleChanged': handle,
    'DescriptionUpdated': handle,
    'ImageAdded': handle,
    'CommentAdded': handle
});

function handle(state, ev) {
    linkTo("domainEvents", ev);
}
*)

    