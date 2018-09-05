module App 

open Elmish
open Elmish.WPF
open System
open FsXaml

type State = { Count: int; State: string } 

type GoOnMsg = {Msg: string; ToGo: int}
type GoOnAdding = {Adder: int; ToGo: int}

type Msg = 
    | Increment 
    | Decrement
    | IncrementDelayed
    | IncrementTimes of int
    | ShowMsg of string
    | ShowMsgButGoOn of GoOnMsg
    | AddAndGoOn of GoOnAdding

let rnd = Random()

let init() = { Count = 0; State="Ready" }, Cmd.none

let simulateException level = 
    if rnd.Next(level) = level-1 
    then failwith "intentional randomic exception"

let timeout (n, msg) = async {
    do! Async.Sleep n 
    simulateException 5
    return msg 
}

let justSleep secs = async {
    simulateException 4
    do! Async.Sleep secs 
}

let update msg state =  
    match msg with 
    | Increment -> 
        let nextState = { state with Count = state.Count + 1; State="Incremented by 1" }
        nextState, Cmd.none 

    | Decrement -> 
        let nextState = { state with Count = state.Count - 1; State="Decremented by 1" }
        nextState, Cmd.none 

    | IncrementDelayed -> 
        state, Cmd.ofAsync timeout (1000, Increment) id (fun ex -> ShowMsg ex.Message)

    | IncrementTimes n ->
        if n>0
        then 
            { state with State= sprintf "%d iterations to go" (n-1)}, 
            Cmd.ofAsync justSleep  2000 (fun () -> AddAndGoOn {Adder=1; ToGo=n-1}) (fun ex -> ShowMsgButGoOn {Msg=ex.Message; ToGo=n-1})
        else
            state, Cmd.ofMsg (ShowMsg "Iteration Completed")

    | ShowMsg msg -> 
        {state with State=msg }, Cmd.none

    | ShowMsgButGoOn msg -> 
        {state with State=msg.Msg }, Cmd.ofMsg (IncrementTimes msg.ToGo)
    
    | AddAndGoOn msg ->
        {state with Count = state.Count + msg.Adder }, Cmd.ofMsg (IncrementTimes msg.ToGo)

let bindings model dispatch = [
    "Count"     |> Binding.oneWay (fun state -> state.Count)
    "State"     |> Binding.oneWay (fun state -> state.State)
    "Increment" |> Binding.cmd (fun state -> Increment)
    "Decrement" |> Binding.cmd (fun state -> Decrement)
    "IncrementDelayed" |> Binding.cmd (fun state -> IncrementDelayed)
    "IncrementX10" |> Binding.cmd (fun state -> IncrementTimes 10)
]

type MainWindow = XAML<"MainWindow.xaml"> 

[<EntryPoint; STAThread>]
let main argv = 
    Program.mkProgram init update bindings
    |> Program.runWindow (MainWindow())