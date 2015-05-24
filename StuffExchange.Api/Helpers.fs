module StuffExchange.Api.Helpers

open Nancy
open System.IO
open Newtonsoft.Json

open StuffExchange.Contract.Commands
open StuffExchange.Contract.Types
open StuffExchange.Core.Helpers
open StuffExchange.Core.Railway
open StuffExchange.Ports.EventStore
open StuffExchange.Ports.CommandHandler


let (?) (parameters: obj) param =
    (parameters :?> Nancy.DynamicDictionary).[param]


let jsonResponse (statusCode: HttpStatusCode) body =
    let body = JsonConvert.SerializeObject(body)
    let response = new Nancy.Responses.TextResponse(statusCode, body)
    response.ContentType <- "application/json"
    box response

let respond result =
    match result with
    | Success s ->
        jsonResponse HttpStatusCode.OK s
    | Failure f ->
        jsonResponse HttpStatusCode.BadRequest f

let getCommandText (headers: List<(string * decimal)>) =
    // format goes like: application/vnd.stuffexchange.command+json - THIS IS NOT 5LMT
    // might be simpler to spit on ; and look for command= ?
    // crap that should be in ContentType header not Accept header! (except ContentType is only on response?!)
    // it's only the version of the api that goes in the Accept header
    let header, _ = headers.[0]
    header.Split('/').[1].Split('.').[2].Split('+').[0]

let getRequest<'a> (body:IO.RequestStream) =
        use rdr = new StreamReader(body)
        let s = rdr.ReadToEnd()
        JsonConvert.DeserializeObject<'a>(s)

let infrastructure = {EventReader = getEventsForAggregate; EventWriter = addEventToAggregate }

type CommandDto =
    | ActivateUserDto of UserActivation
    | DeactivateUserDto of UserDeactivation
    | AddGiftDto of GiftAddition
    | AddImageDto of ImageAddition
    | ChangeTitleDto of TitleChange
    | UpdateDescriptionDto of DescriptionUpdate
    | AddCommentDto of CommentAddition
    | MakeWishDto of WishMaking
    | UnmakeWishDto of WishUnmaking
    | MakeOfferDto of OfferMaking
    | AcceptOfferDto of OfferAcceptance
    | DeclineOfferDto of OfferDeclination
    

let route commandDto =
    let userHandler = handleUserCommand infrastructure
    let giftHandler = handleGiftCommand infrastructure

    match commandDto with
    | ActivateUserDto d -> ActivateUser d |> userHandler
    | DeactivateUserDto d -> DeactivateUser d |> userHandler
    | AddGiftDto d -> AddGift d |> giftHandler
    | AddImageDto d -> AddImage d |> giftHandler
    | ChangeTitleDto d -> ChangeTitle d |> giftHandler
    | UpdateDescriptionDto d -> UpdateDescription d |> giftHandler
    | AddCommentDto d -> AddComment d |> giftHandler
    | MakeWishDto d -> MakeWish d |> giftHandler
    | UnmakeWishDto d -> UnmakeWish d |> giftHandler
    | MakeOfferDto d -> MakeOffer d |> giftHandler
    | AcceptOfferDto d -> AcceptOffer d |> giftHandler
    | DeclineOfferDto d -> DeclineOffer d |> giftHandler