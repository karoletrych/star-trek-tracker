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
    | _, StarTrekDataLoaded (Ok starTrekData)->
        let nextModel = { StarTrekData = Some starTrekData }
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

let seriesDecoder : Decode.Decoder<Series> = 
    Decode.Auto.generateDecoder<Series>(true)
let starTrekDecoder: Decode.Decoder<StarTrek> =
    Decode.object (fun get ->
        { 
            TOS = get.Required.Field "Star Trek" seriesDecoder
            TNG = get.Required.Field "Star Trek: The Next Generation" seriesDecoder 
            STD = get.Required.Field "Star Trek: Discovery" seriesDecoder 
            DSN = get.Required.Field "Star Trek: Deep Space Nine" seriesDecoder 
            STV = get.Required.Field "Star Trek: Voyager" seriesDecoder 
            STE = get.Required.Field "Star Trek: Enterprise" seriesDecoder 
            TAS = get.Required.Field "Star Trek: The Animated Series" seriesDecoder 
        })

let starTrek = function
| { StarTrekData = Some x } -> 
    match Thoth.Json.Decode.fromString starTrekDecoder x with
    | Ok x -> Some x
    | Error e -> None
| { StarTrekData = None   } -> None


let seriesView (series : Series) =
    Columns.columns [Columns.IsMultiline]
        (series.Seasons |> List.collect (fun s -> s) 
        |> List.map 
            (fun e -> (Column.column [  ( Column.Width (Screen.All, Column.IsNarrow)) ]  
                        [ofType<EpisodeItem.EpisodeItem,_,_> (unbox null) [] ])))
let starTrekView (st : StarTrek option) = 
      match st with
      | Some st -> 
          Column.column [Column.Width (Screen.All, Column.IsFull) ]
            ([st.DSN; st.STD; st.STE; st.STV; st.TAS; st.TNG; st.TOS] |> List.map seriesView)
      | None -> 
        Column.column [] []
let hide e =
    ()


let view (model : Model) (dispatch : Msg -> unit) =
    div []
        [ Navbar.navbar [ Navbar.Color IsPrimary ]
            [ Navbar.Item.div [ ]
                [ Heading.h2 [ ]
                    [ str "SAFE Template" ] ] ]
          ofType<Quickview.QuickviewDemo,_,_> (unbox null) []
          starTrekView (starTrek model)
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
