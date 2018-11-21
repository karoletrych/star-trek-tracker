module Types
open Shared


type Episode = {
    Title : string;
    EpisodeID: string;
    Plot: string array option;
    ImdbRating: decimal option;
    OriginalAirDate : string option;
    ImdbURL : string;
    PlotOutline : string option;
}

type Season = Episode list

type Series = {
    MovieID: string
    Title: string
    Seasons : Season list
}
type StarTrek = {
    TOS: Series
    TAS: Series
    TNG: Series
    DSN: Series
    STV: Series
    STE: Series
    STD: Series
}


type Model = { 
    StarTrekData: string option;
    Quickview: Episode option
}

type Msg =
| StarTrekDataLoaded of Result<string, exn>
| ToggleEpisodeItem
| ShowQuickview of Episode
| CloseQuickview

