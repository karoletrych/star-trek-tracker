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


type Layout = 
| Clustered
| GroupedBySeries
| GroupedBySeason

let episodeComponent dispatch e  = ofType<EpisodeItem.EpisodeItem,_,_> ({Episode = e; Dispatch = dispatch}) []


let starTrekView dispatch (st : StarTrek option) layout = 
    let episodeColumn e= (Column.column [  ( Column.Width (Screen.All, Column.IsNarrow)) ]  
                                [ episodeComponent dispatch e])
    let seasonToColumns (season : Season) = 
        Columns.columns [Columns.IsMultiline] (season |> List.map episodeColumn)

    match st with
    | Some st -> 
        match layout with
        | Clustered ->
            Column.column [Column.Width (Screen.All, Column.IsFull) ]
                [Columns.columns [Columns.IsMultiline]
                    ([st.TOS; st.TAS; st.TNG; st.DSN; st.STV; st.STE; st.STD; ]
                    |> List.collect (fun series -> 
                        series.Seasons 
                        |> List.collect id
                        |> List.map episodeColumn))]
                
        | _ ->
            Column.column [Column.Width (Screen.All, Column.IsFull) ]
                ([st.TOS; st.TAS; st.TNG; st.DSN; st.STV; st.STE; st.STD; ] |> List.map (
                    fun series ->
                    match layout with
                    | GroupedBySeries -> 
                        Section.section [  ]
                                [ h1 [ ] [str series.Title];
                                     Columns.columns [Columns.IsMultiline]
                                        (series.Seasons 
                                        |> List.map seasonToColumns ) 
                                ] 
                    | GroupedBySeason -> 
                        Section.section [  ]
                                [ h1 [ ] [str series.Title];
                                     (series.Seasons
                                     |> List.map seasonToColumns 
                                     |> (fun x -> div [] x))
                                ] 
                    ))
   
        

    | None -> 
        Column.column [] []


let view (model : Model) (dispatch : Msg -> unit) =
    div []
        [ Navbar.navbar [ Navbar.Color IsPrimary ]
            [ Navbar.Item.div [ ]
                [ Heading.h2 [ ]
                    [ str "Trek Tracker" ] ] ]
          starTrekView dispatch (JsonParser.starTrek model) Clustered
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
