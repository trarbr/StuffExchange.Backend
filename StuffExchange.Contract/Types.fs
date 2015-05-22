module StuffExchange.Contract.Types

type Id = System.Guid

[<CLIMutable>]
type User = { Id: Id; Username: string }

[<CLIMutable>]
type Comment = {Id: Id; Username: string; Timestamp: System.DateTime; Content: string}

// Instead of separate userid and username just put in a user?!
[<CLIMutable>]
type Gift = { Id: Id; User: Id; Username: string; Title: string; Description: string; 
    Images: string list; Comments: Comment list;}

// How gifts are passed on:
// First a gift is posted by user A
// User B finds the gift and "puts it on wishlist" (command handled by gift as it must be available) - MakeWish
// This will update the read model for the gift and user B, and possibly notify user A
// User A then clicks to give it away to user B (again command handled by gift as user must be on the list of wishers) - MakeOffer
// This will put the Gift in a state of "Offered" and it must be accepted by user B so he can receive it
// (Read Model should maybe have a state field indicating if it's available, offered or givenaway)
// If Offered, others can still MakeWish, but not when GivenAway
// User B can either AcceptOffer og DeclineOffer, if DeclineOffer it goes back to available, with user B off the list of wishers
// Gift states: Init -> Available -> Offered -> GivenAway
// MakeWish can be undone with TakeBackWish. If TakeBackWish is received while in state of Offered, does it fail or just decline?
// If it fails, how would you tell the user?
// How would notifications work? Like messages? MakeWish => "Hi user A, I would like this. Regards user B". Store messages on user?
// Or a separate "inbox" bucket keyed by userId? I'd say separate, don't need it for most things. 
// And messages could be full text indexed...
//[<CLIMutable>]
//type GiveAwayGift = { Id: Id; User: Id; Username: string; Title: string; Description: string; 
//    Images: string list; Comments: Comment list; Wishers: (Id * string)}


// For location-aware gifts - possible to turn Lat/Lon into address?
//[<CLIMutable>]
//type Location = {Lat: double; Lon: double}
//
//[<CLIMutable>]
//type GeoGift = { Id: Id; User: Id; Username: string; Title: string; Description: string; 
//    Images: string list; Comments: Comment list; Location: Location; Location_p: string}

type GiftAddition = {Id: Id; User: Id; Title: string; Description: string}
type TitleChange = {Gift: Id; NewTitle: string}
type DescriptionUpdate = {Gift: Id; NewDescription: string}
type ImageAddition = {Id: Id; Gift: Id}
type CommentAddition = {Id: Id; Gift: Id; User: Id; Timestamp: System.DateTime; Content: string}
type UserActivation = {Id: Id}
type UserDeactivation = {Id: Id}
