module JsonParser
open Types
open Thoth.Json

let seasonsDecoder = Decode.Auto.generateDecoder<Season list>( true)
let seriesDecoder title : Decode.Decoder<Series> = 
    Decode.object (fun s -> 
                            { 
                                MovieID = s.Required.Field "movieID" Decode.string
                                Title = title
                                Seasons = s.Required.Field "seasons" seasonsDecoder
                            })
let starTrekDecoder: Decode.Decoder<StarTrek> =
    Decode.object (fun get ->
        { 
            TOS = get.Required.Field "Star Trek" (seriesDecoder "Star Trek")
            TNG = get.Required.Field "Star Trek: The Next Generation" (seriesDecoder "Star Trek: The Next Generation")
            STD = get.Required.Field "Star Trek: Discovery" (seriesDecoder  "Star Trek: Discovery")
            DSN = get.Required.Field "Star Trek: Deep Space Nine" (seriesDecoder "Star Trek: Deep Space Nine")
            STV = get.Required.Field "Star Trek: Voyager" (seriesDecoder "Star Trek: Voyager")
            STE = get.Required.Field "Star Trek: Enterprise" (seriesDecoder "Star Trek: Enterprise")
            TAS = get.Required.Field "Star Trek: The Animated Series" (seriesDecoder "Star Trek: The Animated Series")
        })

let starTrek = function
| { StarTrekData = Some x } -> 
    match Thoth.Json.Decode.fromString starTrekDecoder x with
    | Ok x -> Some x
    | Error e -> failwith e
| { StarTrekData = None   } -> None
