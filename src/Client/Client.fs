module Client

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch
open Fable.Import


open Shared

open Fulma
open Types
open JsonParser
open Thoth.Json

open Fulma.Extensions


// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = { StarTrekData = None; Quickview = None }
    let loadCountCmd =
        Cmd.ofPromise
            (fetchAs<string> "/api/star-trek-data" Decode.string)
            []
            (Ok >> StarTrekDataLoaded)
            (Error >> StarTrekDataLoaded)
    initialModel, loadCountCmd

let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match currentModel.StarTrekData, msg with
    | _, StarTrekDataLoaded (Ok starTrekData) ->
        let nextModel = {currentModel with  StarTrekData = Some starTrekData }
        nextModel, Cmd.none
    | _, ShowQuickview e -> 
        let nextModel = { currentModel with Quickview = Some e}
        nextModel, Cmd.none
    | _, CloseQuickview -> 
        let nextModel = { currentModel with Quickview = None}
        nextModel, Cmd.none


    | _ -> currentModel, Cmd.none



let episodeComponent dispatch e  = ofType<EpisodeItem.EpisodeItem,_,_> ({Episode = e; Dispatch = dispatch}) []

let tvSeriesView dispatch (series : Series) =
    Section.section [  ]
            [ h1 [ ] [str series.Title];
                 Columns.columns [Columns.IsMultiline]
                    (series.Seasons |> List.collect (id) 
                    |> List.map 
                        (fun e -> (Column.column [  ( Column.Width (Screen.All, Column.IsNarrow)) ]  
                                    [ episodeComponent dispatch e]))) 
                      ] 
    
let starTrekView dispatch (st : StarTrek option) = 
      match st with
      | Some st -> 
          Column.column [Column.Width (Screen.All, Column.IsFull) ]
            ([st.TOS; st.TAS; st.TNG; st.DSN; st.STV; st.STE; st.STD; ] |> List.map (tvSeriesView dispatch))
      | None -> 
          Column.column [] []


let view (model : Model) (dispatch : Msg -> unit) =
    div []
        [ Navbar.navbar [ Navbar.Color IsPrimary ]
            [ Navbar.Item.div [ ]
                [ Heading.h2 [ ]
                    [ str "Trek Tracker" ] ] ]
          starTrekView dispatch (JsonParser.starTrek model)
          Quickview.view model dispatch
        ]

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
