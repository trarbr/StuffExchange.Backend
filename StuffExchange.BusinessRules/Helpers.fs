module StuffExchange.BusinessRules.Helpers

open Microsoft.FSharp.Reflection
open Railway

let stateTransitionFail state event = 
    sprintf "Invalid event %s for state %s" (event.GetType().Name) (state.GetType().Name)
    |> InvalidStateTransition
    |> Failure

let invalidStateFail state command =
    let case, _ = FSharpValue.GetUnionFields(state, state.GetType())
    sprintf "Invalid command %s for state %s" (command.GetType().Name) (case.ToString())
    |> InvalidState
    |> Failure
