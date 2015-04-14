module StuffExchange.Contract.Events

open StuffExchange.Contract.Types

type Event = 
    | UserActivated of CustomerId
    | UserDeactivated of CustomerId

