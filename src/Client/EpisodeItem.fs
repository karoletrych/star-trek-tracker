module EpisodeItem
open Fable.Import
open Fable.Helpers.React
open Fulma
open Fulma.Extensions
open Fable.Helpers.React.Props
open Types

type EpisodeItemProps =
    { 
        Dispatch : Msg -> unit
        Episode : Episode
    }

type EpisodeItemState =
    { 
        IsWatched : bool
        Episode : Episode
    }


type EpisodeItem(props) =
    inherit React.Component<EpisodeItemProps, EpisodeItemState>(props)
    do base.setInitState({ IsWatched = false; Episode = props.Episode})

    member this.SetWatched _ =
        this.setState (fun s _ -> {s with IsWatched = true})

    member this.UnsetWatched _ =
        this.setState (fun s _ -> {s with IsWatched = false})
    member this.ToggleWatched _ =
        this.setState (fun s _ -> {s with IsWatched = not s.IsWatched})
    member this.ShowInfo _ =
        this.props.Dispatch (ShowQuickview this.state.Episode)
    override this.componentDidMount () =
        ()
    override this.render () =
        div [ ]
            [ Image.image [
                Image.IsSquare
                Image.Is32x32
                (Image.Props [OnClick (fun _ -> this.ShowInfo())]) 
                ]
                [img [ Src (
                        if not this.state.IsWatched 
                        then "Images/episode.jpg" 
                        else "Images/episode_watched.jpg") ]] 
                        ]
            


