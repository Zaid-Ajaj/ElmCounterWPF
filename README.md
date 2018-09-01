# Elm counter in WPF
Pure F#/Xaml counter using Elmish.WPF and XAML type provider

```fs
// Program.fs
module App 

open Elmish
open Elmish.WPF
open System
open FsXaml

type State = { Count : int } 

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
```
and 
```xml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        mc:Ignorable="d"
        Title="MainWindow" Height="300" Width="500">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="*" />
            <RowDefinition Height="*" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>
        <TextBlock Text="{Binding Count}" TextAlignment="Center" FontSize="18" Grid.Row="0" />
        <Button Command="{Binding Increment}" FontSize="18" Content="Increment" Grid.Row="1" />
        <Button Command="{Binding Decrement}" FontSize="18" Content="Decrement" Grid.Row="2" />
        <Button Command="{Binding IncrementDelayed}" FontSize="18" Content="Increment (Delayed)" Grid.Row="3" />
    </Grid>
</Window>
```
