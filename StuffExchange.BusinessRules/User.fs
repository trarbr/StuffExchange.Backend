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

let handle command state : Result<Event> =
    match state with
    | Inactive ->
        match command with
        | ActivateUser userId -> UserActivated userId |> Success
        | DeactivateUser _ 
        | CreateGift _ -> invalidStateFail state command
    | Active ->
        match command with
        | ActivateUser userId -> invalidStateFail state command 
        | DeactivateUser userId -> UserDeactivated userId |> Success
        | CreateGift (userId, name, description) -> 
            GiftAdded (userId, name, description) |> Success

let apply event state : Result<UserState> =
    match state with
    | Inactive -> 
        match event with
        | UserActivated _ -> Active |> Success
        | UserDeactivated _ 
        | GiftAdded _ -> stateTransitionFail state event
    | Active ->
        match event with
        | UserActivated _ -> stateTransitionFail state event
        | UserDeactivated _ -> Inactive |> Success
        | GiftAdded (userId, name, description) -> Active |> Success