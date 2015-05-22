open StuffExchange.Ports.EventStore
open StuffExchange.Ports.ReadStore
open StuffExchange.Contract
open Types
open Events
open Railway

let giftAddedHandler (gift: GiftAddition) =
    let user = getUser gift.User
    match user with
    | Success user -> 
        let gift = {Id = gift.Id; User = user.Id; Username = user.Username; Title = gift.Title; 
            Description = gift.Description; Images = []; Comments = []}
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
        { Comment.Id = comment.Id; Username = user.Username; Timestamp = comment.Timestamp; Content = comment.Content }
        |> addCommentToGift gift
    | _ -> ()

let domainEventHandler (event: Event) =
    match event with
    | GiftAdded gift -> giftAddedHandler gift
    | TitleChanged title -> titleChangedHandler title
    | DescriptionUpdated description -> descriptionUpdatedHandler description
    | ImageAdded image -> imageAddedHandler image
    | CommentAdded comment -> commentAddedHandler comment
    | _ -> ()
            

[<EntryPoint>]
let main argv = 
    subscribeToDomainEvents domainEventHandler
    System.Console.ReadLine() |> ignore
    0 // return an integer exit code


