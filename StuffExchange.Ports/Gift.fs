module StuffExchange.Ports.Gift

open StuffExchange.Contract.Commands
open StuffExchange.Ports.EventStore
open StuffExchange.BusinessRules.Gift

open StuffExchange.BusinessRules.Railway

let routeCommand (command: GiftCommand) =
    let foldable apply currentState event =
        apply event currentState 
        |> function
            | Success newState -> newState
            | Failure _ -> currentState

    match command with
    | AddGift (id, _, _, _) 
    | AddImage (_, id) 
    | ChangeTitle (id, _)
    | UpdateDescription (id, _)
    | AddComment (_, id, _, _, _) ->
        getEventsForAggregate id
        |> List.fold (foldable apply) Init
        |> handle command
        >>= addEventToAggregate id



