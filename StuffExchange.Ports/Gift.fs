module StuffExchange.Ports.Gift

open StuffExchange.Contract.Commands

open StuffExchange.BusinessRules.Railway

let routeCommand (command: GiftCommand) =
    Failure (AggregateNotFound "You suck!")
