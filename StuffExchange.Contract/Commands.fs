module StuffExchange.Contract.Commands

open StuffExchange.Contract.Types

type Command =
    | UserCommand of UserCommand
and UserCommand =
    | ActivateUser of CustomerId
    | DeactivateUser of CustomerId