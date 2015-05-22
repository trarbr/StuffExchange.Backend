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
        | AddGift giftAddition -> 
            GiftAdded giftAddition |> Success
        | _ -> invalidStateFail state command
    | Available (giftId, user, title, description) ->
        match command with
        | ChangeTitle titleChange -> 
            TitleChanged titleChange |> Success
        | UpdateDescription descriptionUpdate ->
            DescriptionUpdated descriptionUpdate |> Success
        | AddImage imageAddition ->
            ImageAdded imageAddition |> Success
        | AddComment commentAddition ->
            CommentAdded commentAddition |> Success
        | _ -> invalidStateFail state command

let apply event state : Result<GiftState> =
    match state with
    | Init ->
        match event with
        | GiftAdded gift ->
            Available (gift.Id, gift.User, gift.Title, gift.Description) |> Success
        | _ -> stateTransitionFail state event
    | Available (id, user, title, description) ->
        match event with
        | TitleChanged newTitle ->
            Available (id, user, newTitle.NewTitle, description) |> Success
        | DescriptionUpdated newDescription  ->
            Available (id, user, title, newDescription.NewDescription) |> Success
        | ImageAdded imageAddition ->
            Available (id, user, title, description) |> Success
        | CommentAdded commentAddition ->
            Available (id, user, title, description) |> Success
        | _ -> stateTransitionFail state event


        