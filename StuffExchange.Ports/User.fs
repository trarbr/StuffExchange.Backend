module StuffExchange.Ports.User

// receive a user command, send command to users actor. If actor does not exist, spin it up
// This is kinda like a router!

open StuffExchange.Contract.Commands
open StuffExchange.BusinessRules.User

let routeCommand (command: UserCommand) =
    handle Active command