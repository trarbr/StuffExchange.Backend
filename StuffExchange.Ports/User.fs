module StuffExchange.Ports.User

//open Akka.FSharp

open StuffExchange.Contract.Commands
open StuffExchange.Contract.Railway
open StuffExchange.BusinessRules.User
open StuffExchange.Ports.EventStore


// Random actor samples
//let system = System.create "my-system" (Configuration.load())
//
//let superCharge x =
//    spawn system "my-actor" 
//        <| fun mailbox ->
//            let rec loop() = actor {
//                let! message = mailbox.Receive()
//                let name = mailbox.Self.Path
//                return! loop()
//                }
//            loop()
//
//let handleMessage (mailbox: Actor<'T>) msg =
//    match msg with
//    | Some x -> printf "%A" x
//    | None -> ()
//
//let actorRef = spawn system "my-actor" (actorOf2 handleMessage)
//
//actorRef <! Some "yo dude!"

// receive a user command, send command to users actor. If actor does not exist, spin it up
// This is kinda like a router!

// get the command and <! it to the users supervisor
// user supervisor looks up the actor for the user by id in command (or might be it should be parameter to routeCommand...)
// and <!s the command to that actor
// if that actor aint here, it has to be loaded up first
// the actor knows its own name by mailbox.Self.Path
let routeCommand (command: UserCommand) =
    // read events from eventstore using fold
    // what I want is a state -> event -> state function that uses apply, and returns the old state if it was a failure
    let foldable apply currentState event =
        apply event currentState 
        |> function
            | Success newState -> newState
            | Failure _ -> currentState

    let aggregateId = 
        match command with
        | ActivateUser activation -> activation.Id
//      | AddGift (_, userId, _, _) -> 
        | DeactivateUser deactivation -> deactivation.ID
    
    aggregateId 
    |> getEventsForAggregate 
    |> List.fold (foldable apply) Inactive
    |> handle command
    >>= addEventToAggregate aggregateId