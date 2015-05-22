module StuffExchange.Core.Railway

type Error =
    | InvalidState of string
    | InvalidStateTransition of string
    | AggregateNotFound of string
    | RiakGetFailed of string

type Result<'T> =
    | Success of 'T
    | Failure of Error

let bind switchFunction result = 
    match result with
    | Success s -> switchFunction s
    | Failure f -> Failure f

let (>>=) result switchFunction = bind switchFunction result


