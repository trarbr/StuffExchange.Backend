module StuffExchange.Ports.ReadStore

open StuffExchange.Riak.Riak
open StuffExchange.BusinessRules.Railway
open StuffExchange.Contract.Types

let getUser userId =
    userId.ToString()
    |> readFromRiak<User> "users"

let getGift giftId =
    giftId.ToString()
    |> readFromRiak<Gift> "gifts"

let putGift (gift: Gift) =
    let giftId = gift.Id.ToString()
    writeToRiak "gifts" giftId gift
