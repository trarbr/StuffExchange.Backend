module StuffExchange.Contract.Events

open StuffExchange.Contract.Types

//type CommentAdded = {Id: CommentId; Gift: GiftId; User: UserId; Timestamp: System.DateTime; Content: string}

type Event = 
    | UserActivated of Id: Id
    | UserDeactivated of Id: Id
    | GiftAdded of Id: Id * User: Id * Title: string * Description: string
    | TitleChanged of Gift: Id * NewTitle: string
    | DescriptionUpdated of Gift: Id * NewDescription: string
    | ImageAdded of Id: Id * Gift: Id
    //| CommentAdded of CommentAdded
    | CommentAdded of 
        Id: Id * Gift: Id * User: Id * 
        Timestamp: System.DateTime * Content: string


