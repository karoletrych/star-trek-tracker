module Quickview
open Fable.Helpers.React
open Fulma
open Fulma.Extensions
open Types


let tryGet get option = 
    option |> Option.map get |> Option.toObj

let body =
    function
    | Some (e : Episode) ->
      sprintf "%s %A" (e.PlotOutline |> Option.toObj) (e.ImdbRating)
    | None ->
      null
let view (model : Model) (dispatch : Msg -> unit) = 
    div [ ]
        [ Quickview.quickview [ Quickview.IsActive model.Quickview.IsSome ]
                [ Quickview.header [ ]
                    [ Quickview.title [ ] [ str (model.Quickview |> tryGet (fun e -> e.Title)) ]
                      Delete.delete [ Delete.OnClick (fun e -> dispatch CloseQuickview) ] [ ] ]
                  Quickview.body [ ]
                    [ p [ ] [ str (model.Quickview |> body) ] ]
                  Quickview.footer [ ]
                    [ ] ]
        ]