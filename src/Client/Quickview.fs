module Quickview
open Fable.Import
open Fable.Helpers.React
open Fulma
open Fulma.Extensions
open Types

type QuickviewDemoProps = interface end

type QuickviewDemoState =
    { IsActive : bool }


type QuickviewDemo(props) =
    inherit React.Component<QuickviewDemoProps, QuickviewDemoState>(props)
    do base.setInitState({ IsActive = false })

    member this.Show _ =
        this.setState (fun s _ -> { s with IsActive = true })
        

    member this.Hide _ =
        this.setState (fun s _ -> { s with IsActive = false })

    override this.render () =
        div [ ]
            [ Quickview.quickview [ Quickview.IsActive this.state.IsActive ]
                    [ Quickview.header [ ]
                        [ Quickview.title [ ] [ str "Testing..." ]
                          Delete.delete [ Delete.OnClick this.Hide ] [ ] ]
                      Quickview.body [ ]
                        [ p [ ] [ str "The body" ] ]
                      Quickview.footer [ ]
                        [ Button.button [ Button.OnClick this.Hide ]
                                        [ str "Hide the quickview!" ] ] ]
              Button.button [ Button.Color IsPrimary
                              Button.OnClick this.Show ]
                            [ str "Show the Quickview!" ] ]