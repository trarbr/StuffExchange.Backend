module StuffExchange.BusinessRules.Gift

open StuffExchange.Contract
open Types
open Commands
open Events
open Railway

open Helpers

// gitstate must keep track of offers made, its own id, its owner/user
type Available = {Id: Id; User: Id; Images: int; Wishers: Id list; }
type Offered = {Id: Id; User: Id; Images: int; Wishers: Id list; OfferedTo: Id}

type GiftState =
    | Init
    | Available of Available
    | Offered of Offered
    | GivenAway


let handle command state : Result<Event> =
    match state with
    | Init ->
        match command with
        | AddGift giftAddition -> 
            GiftAdded giftAddition |> Success
        | _ -> invalidStateFail state command
    | Available giftState ->
        match command with
        | ChangeTitle titleChange -> 
            TitleChanged titleChange |> Success
        | UpdateDescription descriptionUpdate ->
            DescriptionUpdated descriptionUpdate |> Success
        | AddImage imageAddition ->
            // Don't allow more than 5 images
            if giftState.Images < 5 
            then ImageAdded imageAddition |> Success
            else invalidStateFail state command
        | AddComment commentAddition ->
            CommentAdded commentAddition |> Success
        | MakeWish wishMaking ->
            WishMade wishMaking |> Success
        | UnmakeWish wishUnmaking ->
            if List.exists (fun userId -> userId = wishUnmaking.User) giftState.Wishers
            then WishUnmade wishUnmaking |> Success
            else invalidStateFail state command
        | MakeOffer offerMaking ->
            // test that the offer is made to someone on list of wishers
            // might also want to check id of who submits the command!
            if List.exists (fun userId -> userId = offerMaking.User) giftState.Wishers
            then OfferMade offerMaking |> Success
            else invalidStateFail state command
        | _ -> invalidStateFail state command
    | Offered giftState ->
        match command with
        | ChangeTitle titleChange -> 
            TitleChanged titleChange |> Success
        | UpdateDescription descriptionUpdate ->
            DescriptionUpdated descriptionUpdate |> Success
        | AddImage imageAddition ->
            // Don't allow more than 5 images
            if giftState.Images < 5 
            then ImageAdded imageAddition |> Success
            else invalidStateFail state command
        | AddComment commentAddition ->
            CommentAdded commentAddition |> Success
        | MakeWish wishMaking ->
            WishMade wishMaking |> Success
        | UnmakeWish wishUnmaking ->
            // what if gift is offered to that person? DeclineOffer
            let isWisher = List.exists (fun userId -> userId = wishUnmaking.User) giftState.Wishers
            let offeredTo = giftState.OfferedTo
            match isWisher with
            | true ->
                match wishUnmaking.User with
                | offeredTo ->
                    OfferDeclined {Gift = wishUnmaking.Gift; User = wishUnmaking.User} 
                    |> Success
                | _ ->
                    WishUnmade wishUnmaking |> Success
            | false ->
                invalidStateFail state command
        | AcceptOffer offerAcceptance ->
            if giftState.OfferedTo = offerAcceptance.User
            then OfferAccepted offerAcceptance |> Success
            else invalidStateFail state command
        | DeclineOffer offerDeclination ->
            if giftState.OfferedTo = offerDeclination.User
            then OfferDeclined offerDeclination |> Success
            else invalidStateFail state command
        | _ -> invalidStateFail state command
    | GivenAway -> invalidStateFail state command

let apply event state : Result<GiftState> =
    match state with
    | Init ->
        match event with
        | GiftAdded gift ->
            Available {Id = gift.Id; User = gift.User; Images = 0; Wishers = []}
            |> Success
        | _ -> stateTransitionFail state event
    | Available giftState ->
        match event with
        | TitleChanged newTitle ->
            state |> Success
        | DescriptionUpdated newDescription  ->
            state |> Success
        | ImageAdded imageAddition ->
            Available {giftState with Images = 1 + giftState.Images} |> Success
        | CommentAdded commentAddition ->
            state |> Success
        | WishMade wishMaking ->
            Available {giftState with Wishers = wishMaking.User :: giftState.Wishers}
            |> Success
        | WishUnmade wishUnmaking ->
            let rec removeWisher wishers wisher =
                match wishers with
                | head :: tail when head = wisher -> tail
                | head :: tail -> head :: removeWisher tail wisher
                | _ -> []
            let wishers = removeWisher giftState.Wishers wishUnmaking.User
            Available {giftState with Wishers = wishers}
            |> Success
        | OfferMade offerMaking ->
            Offered {Id = giftState.Id; User = giftState.User; 
            Images = giftState.Images; Wishers = giftState.Wishers; 
            OfferedTo = offerMaking.User }
            |> Success
        | _ -> stateTransitionFail state event
    | Offered giftState ->
        match event with
        | TitleChanged newTitle ->
            state |> Success
        | DescriptionUpdated newDescription  ->
            state |> Success
        | ImageAdded imageAddition ->
            Offered {giftState with Images = 1 + giftState.Images} |> Success
        | CommentAdded commentAddition ->
            state |> Success
        | WishMade wishMaking ->
            Offered {giftState with Wishers = wishMaking.User :: giftState.Wishers}
            |> Success
        | WishUnmade wishUnmaking ->
            let wishers = removeFromList giftState.Wishers wishUnmaking.User
            Offered {giftState with Wishers = wishers}
            |> Success
        | OfferAccepted offerAcceptance ->
            GivenAway |> Success
        | OfferDeclined offerDecliation ->
            let wishers = removeFromList giftState.Wishers offerDecliation.User
            Available {Id = giftState.Id; User = giftState.User; Images = giftState.Images;
            Wishers = giftState.Wishers}
            |> Success
        | _ -> stateTransitionFail state event
    | GivenAway -> stateTransitionFail state event

        