module StuffExchange.Contract.Events

open StuffExchange.Contract.Types

type Event = 
    | UserActivated of UserActivation
    | UserDeactivated of UserDeactivation
    | GiftAdded of GiftAddition
    | TitleChanged of TitleChange
    | DescriptionUpdated of DescriptionUpdate
    | ImageAdded of ImageAddition
    | CommentAdded of CommentAddition


