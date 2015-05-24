// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open StuffExchange.Contract.Types
open StuffExchange.Ports.ReadStore

[<EntryPoint>]
let main argv = 
    let id = System.Guid("5d07438d-4e7e-4fc7-92a6-bce87c60cd27")
    let userId = {UserIdentity.Id = id; Username = "troels"}
    let user = {User.UserIdentity = userId; Gifts = []; Wishlist = []; Offers = []}
    putUser user
    printfn "Put %A" user
    ignore (System.Console.ReadLine())
    0 // return an integer exit code
