module StuffExchange.BusinessRules.User

open StuffExchange.Contract
open Types
open Commands
open Events

open Railway
open Helpers

type UserState =
    | Inactive
    | Active

let handle state command : Result<Event> =
    match state with
    | Inactive ->
        match command with
        | ActivateUser userId -> UserActivated userId |> Success
        | DeactivateUser userId -> invalidStateFail state command
    | Active ->
        match command with
        | ActivateUser userId -> invalidStateFail state command 
        | DeactivateUser userId -> UserDeactivated userId |> Success

let apply state event : Result<UserState> =
    match state with
    | Inactive -> 
        match event with
        | UserActivated _ -> Active |> Success
        | UserDeactivated _ -> stateTransitionFail state event
    | Active ->
        match event with
        | UserActivated _ -> stateTransitionFail state event
        | UserDeactivated _ -> Inactive |> Success