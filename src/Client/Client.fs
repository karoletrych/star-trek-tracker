module Client

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch

open Thoth.Json

open Shared

open Fulma
open Types


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


let starTrek = [
    {
        Episodes= (List.replicate 60 {Series="1"; Title=""; Length=1;ImdbRating=1.2m; IsWatched = true})
    }
    {
        Episodes= (List.replicate 60 {Series="2"; Title=""; Length=1;ImdbRating=1.2m; IsWatched = false})
    }
    {
        Episodes= (List.replicate 60 {Series="3"; Title=""; Length=1;ImdbRating=1.2m; IsWatched = true})
    }
    {
        Episodes= (List.replicate 60 {Series="4"; Title=""; Length=1;ImdbRating=1.2m; IsWatched = false})
    }
    {
        Episodes= (List.replicate 60 {Series="5"; Title=""; Length=1;ImdbRating=1.2m; IsWatched = true})
    }
    {
        Episodes= (List.replicate 60 {Series="6"; Title=""; Length=1;ImdbRating=1.2m; IsWatched = false})
    }
    ]
let image = Image.image [Image.IsSquare; Image.Is32x32] [img [ Src "Images/star_trek.jpg" ]]

let getColor = function
    | "1" -> IsSuccess
    | "2" -> IsWarning
    | "3" -> IsBlack
    | "4" -> IsGreyLight
    | "5" -> IsPrimary
    | "6" -> IsDanger
    | _ -> failwith ""

let episodeView (e : Episode) =
            Notification.notification [ Notification.Color (getColor e.Series) ] [ str "Column n°1" ] 

let seriesView (series : Series) =
    Columns.columns [Columns.IsMultiline]
        (series.Episodes 
        |> List.map 
            (fun e -> (Column.column [  ( Column.Width (Screen.All, Column.IsNarrow)) ]  
                        [ofType<EpisodeItem.EpisodeItem,_,_> (unbox null) [] ])))
let episodesView (st : Series list) = 
      Column.column [Column.Width (Screen.All, Column.IsFull) ]
        (st |> List.map seriesView)
let hide e =
    ()


let view (model : Model) (dispatch : Msg -> unit) =
    div []
        [ Navbar.navbar [ Navbar.Color IsPrimary ]
            [ Navbar.Item.div [ ]
                [ Heading.h2 [ ]
                    [ str "SAFE Template" ] ] ]

          ofType<Quickview.QuickviewDemo,_,_> (unbox null) []
          episodesView starTrek
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
