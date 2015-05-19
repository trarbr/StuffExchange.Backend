module StuffExchange.Contract.Commands

open StuffExchange.Contract.Types

//type AddComment = {Id: CommentId; Gift: GiftId; User: UserId; Timestamp: System.DateTime; Content: string}

type Command =
    | UserCommand of UserCommand
    | GiftCommand of GiftCommand
and UserCommand =
    | ActivateUser of Id
    | DeactivateUser of Id
and GiftCommand =
    | AddGift of Id: Id * User: Id * Title: string * Description: string
    | AddImage of Id: Id * Gift: Id
    | ChangeTitle of Gift: Id * NewTitle: string
    | UpdateDescription of Gift: Id * NewDescription: string
    //| AddComment of AddComment
    | AddComment of 
        Id: Id * Gift: Id * User: Id * 
        Timestamp: System.DateTime * Content: string
