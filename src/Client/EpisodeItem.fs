module EpisodeItem
open Fable.Import
open Fable.Helpers.React
open Fulma
open Fulma.Extensions
open Fable.Helpers.React.Props

type EpisodeItemProps =
    interface end

type EpisodeItemState =
    { IsWatched : bool }


type EpisodeItem(props) =
    inherit React.Component<EpisodeItemProps, EpisodeItemState>(props)
    do base.setInitState({ IsWatched = false })

    member this.SetWatched _ =
        this.setState (fun s _ -> {s with IsWatched = true})

    member this.UnsetWatched _ =
        this.setState (fun s _ -> {s with IsWatched = false})

    override this.render () =
        div [ ]
            [ Image.image [Image.IsSquare; Image.Is32x32] 
                [img [ Src (
                        if not this.state.IsWatched 
                        then "Images/episode.jpg" 
                        else "Images/episode_watched.jpg") ]] ]


