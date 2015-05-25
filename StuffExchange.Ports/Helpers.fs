module StuffExchange.Ports.Helpers

open StuffExchange.Core.Railway
open StuffExchange.Contract.Events
open StuffExchange.Contract.Types

type Infrastructure = {EventReader: Id ->  Event list; EventWriter: Id -> Event -> Result<Event>}
