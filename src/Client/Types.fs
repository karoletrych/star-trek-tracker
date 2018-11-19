module Types
open Shared


// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model = { StarTrekData: string option }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
| StarTrekDataLoaded of Result<string, exn>
| ToggleEpisodeItem



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


