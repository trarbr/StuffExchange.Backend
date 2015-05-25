module StuffExchange.Ports.CommandHandler

open StuffExchange.Core.Helpers
open StuffExchange.Core.Railway
open StuffExchange.Contract.Commands
open StuffExchange.BusinessRules
open StuffExchange.Ports.Helpers

let foldable apply currentState event =
    apply event currentState 
    |> function
        | Success newState -> newState
        | Failure _ -> currentState

let handleUserCommand (infrastructure: Infrastructure) (command: UserCommand) =
    let aggregateId = 
        match command with
        | ActivateUser activation -> activation.Id
        | DeactivateUser deactivation -> deactivation.Id

    aggregateId
    |> infrastructure.EventReader
    |> List.fold (foldable User.apply) User.Inactive
    |> User.handle command
    >>= infrastructure.EventWriter aggregateId

let handleGiftCommand (infrastructure: Infrastructure) (command: GiftCommand) =
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
    |> infrastructure.EventReader
    |> List.fold (foldable Gift.apply) Gift.Init
    |> Gift.handle command
    >>= infrastructure.EventWriter aggregateId