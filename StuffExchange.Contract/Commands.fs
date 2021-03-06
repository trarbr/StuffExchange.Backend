﻿module StuffExchange.Contract.Commands

open StuffExchange.Contract.Types

type Command =
    | UserCommand of UserCommand
    | GiftCommand of GiftCommand
and UserCommand =
    | ActivateUser of UserActivation
    | DeactivateUser of UserDeactivation
and GiftCommand =
    | AddGift of GiftAddition
    | AddImage of ImageAddition
    | ChangeTitle of TitleChange
    | UpdateDescription of DescriptionUpdate
    | AddComment of CommentAddition
    // For handing over gifts:
    | MakeWish of WishMaking
    | UnmakeWish of WishUnmaking
    | MakeOffer of OfferMaking
    | AcceptOffer of OfferAcceptance
    | DeclineOffer of OfferDeclination
