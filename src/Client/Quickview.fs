module Quickview
open Fable.Helpers.React
open Fulma
open Fulma.Extensions
open Types


let tryGet get episode = 
    episode |> Option.map get |> Option.toObj
     
let view (model : Model) (dispatch : Msg -> unit) = 
    div [ ]
        [ Quickview.quickview [ Quickview.IsActive model.QuickInfo.IsSome ]
                [ Quickview.header [ ]
                    [ Quickview.title [ ] [ str (model.QuickInfo |> tryGet (fun e -> e.Title)) ]
                      Delete.delete [ Delete.OnClick (fun e -> dispatch CloseQuickview) ] [ ] ]
                  Quickview.body [ ]
                    [ p [ ] [ str (model.QuickInfo |> tryGet (fun e -> e.PlotOutline |>Option.toObj )) ] ]
                  Quickview.footer [ ]
                    [ ] ]
        ]