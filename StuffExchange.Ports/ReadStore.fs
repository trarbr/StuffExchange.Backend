﻿module StuffExchange.Ports.ReadStore

open StuffExchange.Riak.Riak
open StuffExchange.Contract.Types


let getUser userId =
    userId.ToString()
    |> readFromRiak<User> "users"

let putUser (user: User) =
    let userId = user.UserIdentity.Id.ToString()
    writeToRiak "users" userId user

let getGift giftId =
    giftId.ToString()
    |> readFromRiak<Gift> "gifts"

let putGift (gift: Gift) =
    let giftId = gift.Id.ToString()
    writeToRiak "gifts" giftId gift

let getGifts() =
    readFromRiak<List<Id>> "gifts" "all"

let putGifts gifts =
    writeToRiak "gifts" "all" gifts
