using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace SongListGenerator
{
    internal class Program
    {
        static string ULTRASTAR_SONG_DIR = @"C:\Program Files (x86)\UltraStar WorldParty\songs";
        static string OUTPUT_DIR = @"C:\Users\Graph\source\repos\mindtakerr.github.io";

        static void Main(string[] args)
        {
            //var SongList = CreateListOfSongs();
            //string body = CreateSongTableBody(SongList);
            string ShellHTML = File.ReadAllText("shell.html", Encoding.UTF8);
            //string OutputHTML = ShellHTML.Replace("-- Will Be Replaced --", body);
            File.WriteAllText(OUTPUT_DIR + "\\index.html", ShellHTML, Encoding.UTF8);
            File.WriteAllText(OUTPUT_DIR + "\\songlist.json", CreateJsonOfSongs(), Encoding.UTF8);

        }

        //private static string CreateSongTableBody(List<UltraStarSong> SongList)
        //{
        //    StringBuilder TableHTML = new StringBuilder();
        //    foreach (var song in SongList)
        //    {
        //        TableHTML.Append("<tr>");
        //        TableHTML.Append("<td></td>");
        //        TableHTML.Append("<td>" + song.imageHTML + "</td>");
        //        TableHTML.Append("<td>" + song.artist + "</td>");
        //        TableHTML.Append("<td>" + song.titlePlusDuet + "</td>");
        //        TableHTML.Append("<td>" + song.year + "</td>");
        //        TableHTML.Append("<td>" + song.decade + "</td>");
        //        TableHTML.Append("<td>" + song.usdbHTML + "</td>");
        //        TableHTML.Append("</tr>");
        //        TableHTML.AppendLine();
        //    }

        //    return TableHTML.ToString();
        //}

        private static List<UltraStarSong> CreateListOfSongs()
        {
            string SongoPatho = ULTRASTAR_SONG_DIR;
            var DirList = Directory.GetDirectories(SongoPatho, "*", SearchOption.AllDirectories).ToList();
            DirList.Sort();

            List<UltraStarSong> songs = new List<UltraStarSong>();

            foreach (var Dir in DirList)
            {
                UltraStarSong song = new UltraStarSong();

                string SongTextPath = Directory.GetFiles(Dir, "*.txt").FirstOrDefault();
                if (SongTextPath == null)
                    continue;

                var lines = File.ReadAllLines(SongTextPath);
                string yearline = lines.SingleOrDefault(x => x.StartsWith("#YEAR"));
                if (yearline == null)
                {
                    song.year = null;
                }
                else
                    song.year = int.Parse(yearline.Split(':')[1].Substring(0, 4));

                string artistLine = lines.SingleOrDefault(x => x.StartsWith("#ARTIST"));
                song.artist = artistLine.Split(':')[1];

                string titleLine = lines.SingleOrDefault(x => x.StartsWith("#TITLE"));
                song.title = titleLine.Split(':')[1];

                string duetLine = lines.SingleOrDefault(x => x.ToUpper().Contains("DUET") && x.ToUpper().Contains("EDITION"));
                if (!string.IsNullOrEmpty(duetLine))
                    song.IsDuet = true;

                string usdb_file = Directory.GetFiles(Dir, "*.usdb").FirstOrDefault();
                if (usdb_file == null)
                    song.USDB_ID = null;
                else
                    song.USDB_ID = int.Parse(Path.GetFileNameWithoutExtension(usdb_file));

                string cover_file = Directory.GetFiles(Dir, "*").FirstOrDefault(x => x.Contains("[CO]"));
                if (cover_file == null|| cover_file.Length > 260)
                {
                    if (cover_file != null)
                        Console.WriteLine(song.title + " full path is " + cover_file.Length + " characters. Defaulting to USDB URL.");
                    song.imageHTML = "https://usdb.animux.de/data/cover/" + song.USDB_ID + ".jpg";
                }
                else
                {
                    using (Image cover = Image.FromFile(cover_file))
                    {
                        Bitmap resized = ResizeImage(cover, 200, 200);
                        var coverBytes = ImageToByte(resized);
                        var base64 = Convert.ToBase64String(coverBytes);
                        song.imageHTML = "<img style='height:50px;' src=\"data:image/jpg;base64, " + base64 + "\" alt='" + song.title + "' title='" + song.title + "' />'";
                    }
                }

                songs.Add(song);
            }

            songs = songs.OrderBy(x => x.sortableArtist).ThenBy(x => x.sortableTitle).ToList();
            return songs;
        }

        private static string CreateJsonOfSongs()
        {
            var songs = CreateListOfSongs();
            var ToReturn = new { data = songs };
            return JsonConvert.SerializeObject(ToReturn, Formatting.Indented);
        }

        private static byte[] ImageToByte(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
