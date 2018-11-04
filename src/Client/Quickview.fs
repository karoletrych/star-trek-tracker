module Quickview
open Fable.Import
open Fable.Helpers.React
open Fulma
open Fulma.Extensions

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