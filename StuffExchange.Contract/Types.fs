module StuffExchange.Contract.Types

type Id = System.Guid

[<CLIMutable>]
type Comment = {Id: Id; Username: string; Timestamp: System.DateTime; Content: string}

[<CLIMutable>]
type Gift = { Id: Id; User: Id; Username: string; Title: string; Description: string; 
    Images: string list; Comments: Comment list;}

[<CLIMutable>]
type User = { Id: Id; Username: string }
