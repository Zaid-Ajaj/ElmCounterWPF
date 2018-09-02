module App 

open Elmish
open Elmish.WPF
open System
open FsXaml

type State = { Count: int } 

type Msg = 
    | Increment 
    | Decrement
    | IncrementDelayed
    | DoNothing

let init() = { Count = 0 }, Cmd.none

let timeout (n, msg) = async {
    do! Async.Sleep n 
    return msg 
}

let update msg state =  
    match msg with 
    | Increment -> 
        let nextState = { state with Count = state.Count + 1 }
        nextState, Cmd.none 

    | Decrement -> 
        let nextState = { state with Count = state.Count - 1 }
        nextState, Cmd.none 

    | IncrementDelayed -> 
        state, Cmd.ofAsync timeout (1000, Increment) id (fun ex -> DoNothing)

    | DoNothing -> 
        state, Cmd.none

let bindings model dispatch = [
    "Count"     |> Binding.oneWay (fun state -> state.Count)
    "Increment" |> Binding.cmd (fun state -> Increment)
    "Decrement" |> Binding.cmd (fun state -> Decrement)
    "IncrementDelayed" |> Binding.cmd (fun state -> IncrementDelayed)
]

type MainWindow = XAML<"MainWindow.xaml"> 

[<EntryPoint; STAThread>]
let main argv = 
    Program.mkProgram init update bindings
    |> Program.runWindow (MainWindow())