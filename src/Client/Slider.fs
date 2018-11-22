module Slider

open Fable.Helpers.React
open Fulma
open Fulma.Extensions
open Types
open Fable.Import
open Fable.Core.JsInterop

let toLayout =
    function
    | 0 -> Layout.Clustered
    | 1 -> Layout.GroupedBySeason
    | 2 -> Layout.GroupedBySeries
    | _ -> failwith "impossibru"
let onSlide dispatch (event : React.FormEvent) =
    let v = unbox<int> (unbox<JS.Object> event.currentTarget) ?value
    v
    |> toLayout
    |> SliderChanged
    |> dispatch

let view (model : Model) (dispatch : Msg -> unit) = 
    let onSlide = onSlide dispatch
    div [ ]
        [ 
            Slider.slider [ 
                Slider.Min 0. 
                Slider.Max 2.
                Slider.Step 1.
                Slider.OnChange onSlide 
                ]
        ]