from imdb import IMDb
import json

ia = IMDb()
data = {}
seriesIds = ["0060028", "0092455", "5171438", "0106145", "0112178", "0244365", "0069637"]
count = 0


for sId in seriesIds:
    m = ia.get_movie(sId)
    seriesTitle = m["title"]
    data[seriesTitle] = {}
    data[seriesTitle]["movieID"] = m.movieID
    data[seriesTitle]["seasons"] = []

    ia.update(m, 'episodes')
    for season in m["episodes"].values():
        data[seriesTitle]["seasons"].append([])
        for epNumber, ep in season.items():
            epDetails = ia.get_movie(ep.movieID)
            data[seriesTitle]["seasons"][-1].append(
                                {
                                "title": ep["title"],
                                "episodeID": ep.movieID,
                                "plot": epDetails.get("plot", None),
                                "imdbRating": epDetails.get("rating",None),
                                "originalAirDate": ep.get("original air date", None),
                                "imdbURL": ia.get_imdbURL(ep),
                                "plotOutline": epDetails.get("plot outline", None),
                                "runtime": epDetails.get("runtimes", None)[0]
                                })
            count = count + 1
            print(count)

with open("star_trek_data.txt", "w") as text_file:
    json.dump(data, text_file, indent=4)