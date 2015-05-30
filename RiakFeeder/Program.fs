// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open StuffExchange.Contract.Types
open StuffExchange.Ports.ReadStore

let putTroels() =
    let id = System.Guid("5d07438d-4e7e-4fc7-92a6-bce87c60cd27")
    let userId = {UserIdentity.Id = id; Username = "troels"}
    let user = {User.UserIdentity = userId; Gifts = []; Wishlist = []; Offers = []}
    putUser user
    printfn "Put %A" user

let putAnneMarie() =
    let id = System.Guid("8e047a93-3fce-401c-9768-74c1fb9e9938")
    let userId = {UserIdentity.Id = id; Username = "anne-marie"}
    let user = {User.UserIdentity = userId; Gifts = []; Wishlist = []; Offers = []}
    putUser user
    printfn "Put %A" user

[<EntryPoint>]
let main argv = 
    putAnneMarie()
    ignore (System.Console.ReadLine())
    0 // return an integer exit code
