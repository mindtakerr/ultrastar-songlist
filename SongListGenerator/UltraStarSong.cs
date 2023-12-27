namespace SongListGenerator
{
    public class UltraStarSong
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public int? USDB_ID { get; set; }
        public bool IsDuet { get; set; }

        public string SortableArtist
        {
            get
            {
                if (Artist.StartsWith("The "))
                    return Artist.Replace("The ", string.Empty);
                return Artist;
            }
        }
        public string SortableTitle
        {
            get
            {
                if (Title.StartsWith("The "))
                    return Title.Replace("The ", string.Empty);
                return Title;
            }
        }

        public string Decade
        {
            get
            {
                if (Year.HasValue)
                    return Year.Value.ToString().Substring(0, 3) + "0s";
                return string.Empty;
            }
        }

        public string TitlePlusDuet
        {
            get
            {
                if (IsDuet)
                    return Title + " (DUET)";
                return Title;
            }
        }

        public string ImageURL
        {
            get
            {
                return "https://usdb.animux.de/data/cover/" + USDB_ID + ".jpg";
            }
        }

        public string ImageHTML
        {
            get
            {
                return "<img style='height:50px;' src='" + ImageURL + "' alt='" + Title + "' title='" + Title + "' />'";
            }
        }

        public string usdbHTML
        {
            get
            {
                return "<a target='_blank' href='https://usdb.animux.de/index.php?link=detail&id=" + USDB_ID + "'>View on USDB</a>";
            }
        }

    }
}