module StuffExchange.Core.Helpers

open StuffExchange.Core.Railway
open StuffExchange.Contract.Types
open StuffExchange.Contract.Events

type Infrastructure = {EventReader: Id ->  Event list; EventWriter: Id -> Event -> Result<Event>}

let rec removeFromList list item =
    match list with
    | head :: tail when head = item -> tail
    | head :: tail -> head :: removeFromList tail item
    | _ -> []
