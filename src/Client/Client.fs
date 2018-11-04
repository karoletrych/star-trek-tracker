module Client

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch
open Fable.Import

open Thoth.Json

open Shared

open Fulma
open Fulma.Extensions

// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model = { Counter: Counter option }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
| Increment
| Decrement
| InitialCountLoaded of Result<Counter, exn>



// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = { Counter = None }
    let loadCountCmd =
        Cmd.ofPromise
            (fetchAs<Counter> "/api/init" Decode.int)
            []
            (Ok >> InitialCountLoaded)
            (Error >> InitialCountLoaded)
    initialModel, loadCountCmd

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match currentModel.Counter, msg with
    | Some x, Increment ->
        let nextModel = { currentModel with Counter = Some (x + 1) }
        nextModel, Cmd.none
    | Some x, Decrement ->
        let nextModel = { currentModel with Counter = Some (x - 1) }
        nextModel, Cmd.none
    | _, InitialCountLoaded (Ok initialCount)->
        let nextModel = { Counter = Some initialCount }
        nextModel, Cmd.none

    | _ -> currentModel, Cmd.none


let safeComponents =
    let components =
        span [ ]
           [
             a [ Href "https://saturnframework.github.io" ] [ str "Saturn" ]
             str ", "
             a [ Href "http://fable.io" ] [ str "Fable" ]
             str ", "
             a [ Href "https://elmish.github.io/elmish/" ] [ str "Elmish" ]
             str ", "
             a [ Href "https://mangelmaxime.github.io/Fulma" ] [ str "Fulma" ]
           ]

    p [ ]
        [ strong [] [ str "SAFE Template" ]
          str " powered by: "
          components ]

let show = function
| { Counter = Some x } -> string x
| { Counter = None   } -> "Loading..."

let tile = 
   Tile.ancestor [ ]
        [ Tile.parent [  Tile.IsVertical ]
            (List.replicate 19 (Tile.parent [
                Tile.Size Tile.Is1
            ]  
                (List.replicate 60 (Tile.child [] [Image.image [Image.IsSquare; Image.Is32x32] [img [ Src "Images/star_trek.jpg" ]]] ))
              ))
           ]
let hide e =
    ()

type QuickviewDemoProps =
    interface end

type QuickviewDemoState =
    { IsActive : bool }


type QuickviewDemo(props) =
    inherit React.Component<QuickviewDemoProps, QuickviewDemoState>(props)
    do base.setInitState({ IsActive = false })

    member this.show _ =
        { this.state with
                        IsActive = true }
        |> this.setState

    member this.hide _ =
        { this.state with
                        IsActive = false }
        |> this.setState

    override this.render () =
        div [ ]
            [ Quickview.quickview [ Quickview.IsActive this.state.IsActive ]
                    [ Quickview.header [ ]
                        [ Quickview.title [ ] [ str "Testing..." ]
                          Delete.delete [ Delete.OnClick this.hide ] [ ] ]
                      Quickview.body [ ]
                        [ p [ ] [ str "The body" ] ]
                      Quickview.footer [ ]
                        [ Button.button [ Button.OnClick this.hide ]
                                        [ str "Hide the quickview!" ] ] ]
              Button.button [ Button.Color IsPrimary
                              Button.OnClick this.show ]
                            [ str "Show the Quickview!" ] ]

let view (model : Model) (dispatch : Msg -> unit) =
    div []
        [ Navbar.navbar [ Navbar.Color IsPrimary ]
            [ Navbar.Item.div [ ]
                [ Heading.h2 [ ]
                    [ str "SAFE Template" ] ] ]

          ofType<QuickviewDemo,_,_> (unbox null) []
          tile
          Footer.footer [ ]
                [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [ safeComponents ] ] ]


#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
