module StuffExchange.BusinessRules.Helpers

open Microsoft.FSharp.Reflection
open StuffExchange.Core.Railway
open StuffExchange.Contract.Events

let stateTransitionFail state event : Result<'a> = 
    sprintf "Invalid event %s for state %s" (event.GetType().Name) (state.GetType().Name)
    |> InvalidStateTransition
    |> Failure

let invalidStateFail state command : Result<'a> =
    let case, _ = FSharpValue.GetUnionFields(state, state.GetType())
    sprintf "Invalid command %s for state %s" (command.GetType().Name) (case.ToString())
    |> InvalidState
    |> Failure

