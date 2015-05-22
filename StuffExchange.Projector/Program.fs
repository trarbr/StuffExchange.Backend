open StuffExchange.Ports.EventStore
open StuffExchange.Ports.ReadStore
open StuffExchange.Contract
open Types
open Events
open Railway

let giftAddedHandler (event: Event) =
    match event with
    | GiftAdded (id, userId, title, description) ->
        printfn "Got event: %A" event
        let user = getUser userId
        match user with
        | Success user -> 
            let gift = {Id = id; User = user.Id; Username = user.Username; Title = title; 
                Description = description; Images = []; Comments = []}
            putGift gift
        | Failure f ->
            printfn "%A" f
    | _ -> ()

let titleChangedHandler event =
    match event with
    | TitleChanged (giftId, newTitle) ->
        let gift = getGift giftId
        match gift with
        | Success gift ->
            {gift with Title = newTitle }
            |> putGift 
        | _ -> ()
    | _ -> ()
    
let descriptionUpdatedHandler event =
    match event with
    | DescriptionUpdated (giftId, newDescription) ->
        let gift = getGift giftId
        match gift with
        | Success gift ->
            {gift with Description = newDescription }
            |> putGift
        | _ -> ()
    | _ -> ()

let imageAddedHandler event =
    match event with
    | ImageAdded (id, giftId) ->
        let gift = getGift giftId
        match gift with
        | Success gift ->
            {gift with Images = (id.ToString()) :: gift.Images}
            |> putGift
        | _ -> ()
    | _ -> ()


let commentAddedHandler event =
    let addCommentToGift gift comment =
        { gift with Comments = comment :: gift.Comments } 
        |> putGift

    match event with 
    | CommentAdded (id, giftId, userId, timestamp, content) ->
        let gift = getGift giftId
        let user = getUser userId
        match (gift, user) with
        | (Success gift, Success user) ->
            { Id = id; Username = user.Username; Timestamp = timestamp; Content = content }
            |> addCommentToGift gift
        | _ -> ()
    | _ -> ()

//let domainEventHandler event =
//    match event with
//    | CommentAdded (_, _, _, _, _) -> commentAddedHandler event
            

[<EntryPoint>]
let main argv = 
    // define functions for each eventtype
    // subscribe to event store
    // way too many connections here!
    // make a projection inside EventStore that will linkTo on all domain events, and just subscribe to that instead
    subscribeToEventType "GiftAdded" giftAddedHandler
    subscribeToEventType "TitleChanged" titleChangedHandler
    subscribeToEventType "DescriptionUpdated" descriptionUpdatedHandler
    subscribeToEventType "ImageAdded" imageAddedHandler
    subscribeToEventType "CommentAdded" commentAddedHandler

    System.Console.ReadLine() |> ignore
    0 // return an integer exit code
