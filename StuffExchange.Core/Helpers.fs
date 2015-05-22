module StuffExchange.Core.Helpers

let rec removeFromList list item =
    match list with
    | head :: tail when head = item -> tail
    | head :: tail -> head :: removeFromList tail item
    | _ -> []
