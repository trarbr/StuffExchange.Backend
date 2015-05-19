module StuffExchange.Contract.Events

open StuffExchange.Contract.Types

type Event = 
    | UserActivated of UserId
    | UserDeactivated of UserId
    | GiftAdded of UserId: UserId * Name: string * Description: string

