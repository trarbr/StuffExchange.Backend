open StuffExchange.Ports.EventStore
open StuffExchange.Ports.ReadStore
open StuffExchange.Contract
open Types
open Events
open StuffExchange.Core.Railway
open StuffExchange.Core.Helpers

let giftAddedHandler (gift: GiftAddition) =
    let user = getUser gift.User
    match user with
    | Success user -> 
        let gift = {Id = gift.Id; User = user.UserIdentity; Title = gift.Title; 
            Description = gift.Description; Images = []; Comments = []; Wishers = [];
            OfferedTo = None; State = Available}
        putGift gift
        let user = {user with Gifts = gift.Id :: user.Gifts}
        putUser user
    | Failure f ->
        printfn "%A" f

let titleChangedHandler (title: TitleChange) =
    let gift = getGift title.Gift
    match gift with
    | Success gift ->
        {gift with Title = title.NewTitle }
        |> putGift 
    | _ -> ()
    
let descriptionUpdatedHandler (description: DescriptionUpdate) =
    let gift = getGift description.Gift
    match gift with
    | Success gift ->
        {gift with Description = description.NewDescription }
        |> putGift
    | _ -> ()

let imageAddedHandler (image: ImageAddition) =
    let gift = getGift image.Gift
    match gift with
    | Success gift ->
        {gift with Images = image.Id :: gift.Images}
        |> putGift
    | _ -> ()


let commentAddedHandler (comment: CommentAddition) =
    let addCommentToGift gift comment =
        { gift with Comments = comment :: gift.Comments } 
        |> putGift

    let gift = getGift comment.Gift
    let user = getUser comment.User
    match (gift, user) with
    | (Success gift, Success user) ->
        { Comment.Id = comment.Id; Username = user.UserIdentity.Username; 
        Timestamp = comment.Timestamp; Content = comment.Content }
        |> addCommentToGift gift
    | _ -> ()

let wishMadeHandler (wish: WishMaking) =
    let gift = getGift wish.Gift
    match gift with
    | Success gift ->
        {gift with Wishers = wish.User :: gift.Wishers}
        |> putGift
    | _ -> ()
    let user = getUser wish.User
    match user with
    | Success user ->
        {user with Wishlist = wish.Gift :: user.Wishlist}
        |> putUser
    | _ -> ()

let wishUnmadeHandler (wish: WishUnmaking) =
    let gift = getGift wish.Gift
    match gift with
    | Success gift ->
        let wishers = removeFromList gift.Wishers wish.User
        {gift with Wishers = wishers}
        |> putGift
    | _ -> ()
    let user = getUser wish.User
    match user with
    | Success user ->
        let wishlist = removeFromList user.Wishlist wish.Gift
        {user with Wishlist = wishlist}
        |> putUser
    | _ -> ()

let offerMadeHandler (offer: OfferMaking) =
    let gift = getGift offer.Gift
    match gift with
    | Success gift ->
        {gift with OfferedTo = Some offer.User; State = Offered}
        |> putGift
    | _ -> ()
    let user = getUser offer.User
    match user with
    | Success user ->
        {user with Offers = offer.Gift :: user.Offers}
        |> putUser 
    | _ -> ()

let offerAcceptedHandler (offer: OfferAcceptance) =
    let gift = getGift offer.Gift
    match gift with
    | Success gift ->
        {gift with State = GivenAway}
        |> putGift
    | _ -> ()
    let user = getUser offer.User
    match user with
    | Success user ->
        let offers = removeFromList user.Offers offer.Gift
        {user with Offers = offers}
        |> putUser
    | _ -> ()

let offerDeclinedHandler (offer: OfferDeclination) =
    let gift = getGift offer.Gift
    match gift with
    | Success gift ->
        {gift with State = Available; OfferedTo = None}
        |> putGift
    | _ -> ()
    let user = getUser offer.User
    match user with
    | Success user ->
        let offers = removeFromList user.Offers offer.Gift
        {user with Offers = offers}
        |> putUser
    | _ -> ()

let domainEventHandler (event: Event) =
    match event with
    | GiftAdded gift -> giftAddedHandler gift
    | TitleChanged title -> titleChangedHandler title
    | DescriptionUpdated description -> descriptionUpdatedHandler description
    | ImageAdded image -> imageAddedHandler image
    | CommentAdded comment -> commentAddedHandler comment
    | WishMade wish -> wishMadeHandler wish
    | WishUnmade wish -> wishUnmadeHandler wish
    | OfferMade offer -> offerMadeHandler offer
    | OfferAccepted offer -> offerAcceptedHandler offer
    | OfferDeclined offer -> offerDeclinedHandler offer
    | _ -> ()
            

[<EntryPoint>]
let main argv = 
    subscribeToDomainEvents domainEventHandler
    System.Console.ReadLine() |> ignore
    0 // return an integer exit code


