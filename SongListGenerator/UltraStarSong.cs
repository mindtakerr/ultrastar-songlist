namespace SongListGenerator
{
    public class UltraStarSong
    {
        public string artist { get; set; }
        public string title { get; set; }
        public int? year { get; set; }
        public int? USDB_ID { get; set; }
        public bool IsDuet { get; set; }

        public string sortableArtist
        {
            get
            {
                if (artist.StartsWith("The "))
                    return artist.Replace("The ", string.Empty);
                return artist;
            }
        }
        public string sortableTitle
        {
            get
            {
                if (title.StartsWith("The "))
                    return title.Replace("The ", string.Empty);
                return title;
            }
        }

        public string decade
        {
            get
            {
                if (year.HasValue)
                    return year.Value.ToString().Substring(0, 3) + "0s";
                return string.Empty;
            }
        }

        public string titlePlusDuet
        {
            get
            {
                if (IsDuet)
                    return title + " (DUET)";
                return title;
            }
        }
        
        public string imageHTML
        {
            get
            {
                string imageURL = "https://usdb.animux.de/data/cover/" + USDB_ID + ".jpg";
                return "<img style='height:50px;' src='" + imageURL + "' alt='" + title + "' title='" + title + "' />'";
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