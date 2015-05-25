module StuffExchange.Contract.Types

type Id = System.Guid

[<CLIMutable>]
type UserIdentity = {Id: Id; Username: string}
[<CLIMutable>]
type User = {UserIdentity: UserIdentity; Gifts: Id list; Wishlist: Id list; Offers: Id list}
[<CLIMutable>]
type Comment = {Id: Id; Username: string; Timestamp: System.DateTime; Content: string}

type GiftState = 
    | Available = 0
    | Offered = 1
    | GivenAway = 2
[<CLIMutable>]
type Gift = { Id: Id; User: UserIdentity; Title: string; Description: string; 
    Images: Id list; Comments: Comment list; Wishers: Id list; OfferedTo: Id option;
    State: GiftState}

// For location-aware gifts - possible to turn Lat/Lon into address?
//[<CLIMutable>]
//type Location = {Lat: double; Lon: double}
//
//[<CLIMutable>]
//type GeoGift = { Id: Id; User: Id; Username: string; Title: string; Description: string; 
//    Images: string list; Comments: Comment list; Location: Location; Location_p: string}

type UserActivation = {Id: Id}
type UserDeactivation = {Id: Id}
type GiftAddition = {Id: Id; User: Id; Title: string; Description: string}
type TitleChange = {Gift: Id; NewTitle: string}
type DescriptionUpdate = {Gift: Id; NewDescription: string}
type ImageAddition = {Id: Id; Gift: Id}
type CommentAddition = {Id: Id; Gift: Id; User: Id; Timestamp: System.DateTime; Content: string}
type WishMaking = {Gift: Id; User: Id}
type WishUnmaking = {Gift: Id; User: Id}
type OfferMaking = {Gift: Id; User: Id}
type OfferAcceptance = {Gift: Id; User: Id}
type OfferDeclination = {Gift: Id; User: Id}