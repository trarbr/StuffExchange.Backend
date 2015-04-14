module StuffExchange.BusinessRules.Helpers

open Railway

let stateTransitionFail state event = 
    sprintf "Invalid event %s for state %s" (event.GetType().Name) (state.GetType().Name)
    |> InvalidStateTransition
    |> Failure

let invalidStateFail state command =
    sprintf "Invalid command %s for state %s" (command.GetType().Name) (state.GetType().Name)
    |> InvalidState
    |> Failure
