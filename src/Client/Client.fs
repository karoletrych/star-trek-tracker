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
open System
open Fable.Core
open Fable.Core
open Fable.Import


// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = { StarTrekData = None }
    let loadCountCmd =
        Cmd.ofPromise
            (fetchAs<string> "/api/star-trek-data" Decode.string)
            []
            (Ok >> StarTrekDataLoaded)
            (Error >> StarTrekDataLoaded)
    initialModel, loadCountCmd

let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    JS.console.log("UPDATE")
    match currentModel.StarTrekData, msg with
    | _, StarTrekDataLoaded (Ok starTrekData) ->
        let nextModel = { StarTrekData = Some starTrekData }
        nextModel, Cmd.none
    | _, ShowQuickInfo e -> 
        let nextModel = { StarTrekData = Some starTrekData; Quickview = { }
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

let seasonsDecoder = Decode.Auto.generateDecoder<Season list>( true)
let seriesDecoder title : Decode.Decoder<Series> = 
    Decode.object (fun s -> 
                            { 
                                MovieID = s.Required.Field "movieID" Decode.string
                                Title = title
                                Seasons = s.Required.Field "seasons" seasonsDecoder
                            })
let starTrekDecoder: Decode.Decoder<StarTrek> =
    Decode.object (fun get ->
        { 
            TOS = get.Required.Field "Star Trek" (seriesDecoder "Star Trek")
            TNG = get.Required.Field "Star Trek: The Next Generation" (seriesDecoder "Star Trek: The Next Generation")
            STD = get.Required.Field "Star Trek: Discovery" (seriesDecoder  "Star Trek: Discovery")
            DSN = get.Required.Field "Star Trek: Deep Space Nine" (seriesDecoder "Star Trek: Deep Space Nine")
            STV = get.Required.Field "Star Trek: Voyager" (seriesDecoder "Star Trek: Voyager")
            STE = get.Required.Field "Star Trek: Enterprise" (seriesDecoder "Star Trek: Enterprise")
            TAS = get.Required.Field "Star Trek: The Animated Series" (seriesDecoder "Star Trek: The Animated Series")
        })

let starTrek = function
| { StarTrekData = Some x } -> 
    match Thoth.Json.Decode.fromString starTrekDecoder x with
    | Ok x -> Some x
    | Error e -> failwith e
| { StarTrekData = None   } -> None

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
    JS.console.log("VIEW")
    JS.console.log(model)
    div []
        [ Navbar.navbar [ Navbar.Color IsPrimary ]
            [ Navbar.Item.div [ ]
                [ Heading.h2 [ ]
                    [ str "SAFE Template" ] ] ]
          ofType<Quickview.QuickviewDemo,_,_> (unbox null) []
          starTrekView dispatch (starTrek model)
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
