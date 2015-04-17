﻿module StuffExchange.BusinessRules.Railway

type Error =
    | InvalidState of string
    | InvalidStateTransition of string

type Result<'T> =
    | Success of 'T
    | Failure of Error

let bind switchFunction result = 
    match result with
    | Success s -> switchFunction s
    | Failure f -> result

let (>>=) result switchFunction = bind switchFunction result