module StuffExchange.BusinessRules.Gift

open StuffExchange.Contract
open Types
open Commands
open Events

open Railway
open Helpers

type GiftState =
    | Init
    | Available of Id: Id * User: Id * Title: string * Description: string

let handle command state : Result<Event> =
    match state with
    | Init ->
        match command with
        | AddGift (id, user, title, description) -> 
            GiftAdded (id, user, title, description) |> Success
        | _ -> invalidStateFail state command
    | Available (giftId, user, title, description) ->
        match command with
        | ChangeTitle (id, newTitle) -> 
            TitleChanged (id, newTitle) |> Success
        | UpdateDescription (id, newDescription) ->
            DescriptionUpdated (id, newDescription) |> Success
        | AddImage (id, gift) ->
            ImageAdded (id, gift) |> Success
        | AddComment (id, gift, user, timestamp, content) ->
            CommentAdded (id, gift, user, timestamp, content) |> Success
        | _ -> invalidStateFail state command

let apply event state : Result<GiftState> =
    match state with
    | Init ->
        match event with
        | GiftAdded (id, user, title, description) ->
            Available (id, user, title, description) |> Success
        | _ -> stateTransitionFail state event
    | Available (id, user, title, description) ->
        match event with
        | TitleChanged (gift, newTitle) ->
            Available (gift, user, newTitle, description) |> Success
        | DescriptionUpdated (gift, newDescription) ->
            Available (gift, user, title, newDescription) |> Success
        | ImageAdded (id, gift) ->
            Available (id, user, title, description) |> Success
        | CommentAdded (id, gift, user, timestamp, content) ->
            Available (id, user, title, description) |> Success
        | _ -> stateTransitionFail state event


        