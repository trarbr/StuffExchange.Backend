module StuffExchange.Contract.Commands

open StuffExchange.Contract.Types

type Command =
    | UserCommand of UserCommand
and UserCommand =
    | ActivateUser of UserId
    | DeactivateUser of UserId
    | CreateGift of UserId: UserId * Name: string * Description: string