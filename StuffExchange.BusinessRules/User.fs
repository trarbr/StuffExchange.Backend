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
//        | AddGift _
        | DeactivateUser _ -> invalidStateFail state command 
    | Active ->
        match command with
        | ActivateUser userId -> invalidStateFail state command 
        | DeactivateUser userId -> UserDeactivated userId |> Success
//        | AddGift (giftId, userId, name, description) -> 
//            GiftAdded (giftId, userId, name, description) |> Success

let apply event state : Result<UserState> =
    match state with
    | Inactive -> 
        match event with
        | UserActivated _ -> Active |> Success
//        | GiftAdded _ 
        | UserDeactivated _ -> stateTransitionFail state event 
        | _ -> stateTransitionFail state event
    | Active ->
        match event with
        | UserActivated _ -> stateTransitionFail state event
        | UserDeactivated _ -> Inactive |> Success
//        | GiftAdded (giftId, userId, name, description) -> Active |> Success
        | _ -> stateTransitionFail state event