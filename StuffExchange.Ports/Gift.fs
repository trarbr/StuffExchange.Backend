module StuffExchange.Ports.Gift

open StuffExchange.Contract.Commands
open StuffExchange.Core.Railway
open StuffExchange.Ports.EventStore
open StuffExchange.BusinessRules.Gift


let routeCommand (command: GiftCommand) =
    let foldable apply currentState event =
        apply event currentState 
        |> function
            | Success newState -> newState
            | Failure _ -> currentState

    let aggregateId = 
        match command with
        | AddGift gift -> gift.Id
        | AddImage image -> image.Gift
        | ChangeTitle title -> title.Gift
        | UpdateDescription description -> description.Gift
        | AddComment comment -> comment.Gift
        | MakeWish wish -> wish.Gift
        | UnmakeWish wish -> wish.Gift
        | MakeOffer offer -> offer.Gift
        | AcceptOffer offer -> offer.Gift
        | DeclineOffer offer -> offer.Gift

    aggregateId
    |> getEventsForAggregate
    |> List.fold (foldable apply) Init
    |> handle command
    >>= addEventToAggregate aggregateId



