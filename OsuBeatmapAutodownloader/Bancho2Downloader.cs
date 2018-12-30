using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bancho2
{
    public static class Bancho2Downloader
    {
        /*
        osu! LinkType enum
        0    Set,
	    1    Beatmap,
	    2    Topic,
	    3    Post,
	    4    Checksum
        */

        private readonly static Dictionary<int, char> osuLinkEnum = new Dictionary<int, char>
        {
            { 0, 's' },
            { 1, 'b'},
            { 2, 't' },
            { 3, 'p' },
            { 4, 'c' }
        };

        private static char GetUrlLetter(int num) => osuLinkEnum.TryGetValue(num, out var ret) ? ret : 's';

        private static bool isDownloading;
        private static readonly object downloadLock = new object();

        // The backing type of enum is int, and it hasn't been changed in the linktype enum.
        // passedEnum: Link type
        // passedInt: ID
        public static void BeatmapDownloadHandler(int passedEnum, int passedInt)
        {
            var letter = GetUrlLetter(passedEnum);

            if (letter != 'b' && letter != 's')
            {
                Process.Start($"https://osu.ppy.sh/forum/t/{letter}/{passedInt}");
                return;
            }

            try
            {
                lock (downloadLock)
                {
                    if (isDownloading)
                        return;

                    isDownloading = true;
                }

                new Thread(() =>
                {
                    OsuNotifications.NotifyBoxAlert($"{letter} | {passedInt}");

                    string data;

                    using (WebClient http = new WebClient())
                    {
                        data = http.DownloadString(String.Format(OsuUrls.DIRECT_SEARCH_SET, Bancho2.username, Bancho2.password, letter, passedInt));
                    }

                    var split = data.Split('|');

                    var setId = split[0].Split('.')[0];

                    using (WebClient download = new WebClient())
                    {
                        download.DownloadProgressChanged += OnProgressChanged;
                        download.DownloadFileCompleted += OnDownloadCompleted;
                        
                        download.DownloadFileAsync(new Uri(String.Format(OsuUrls.BEATMAP_DOWNLOAD, setId, Bancho2.username, Bancho2.password)), Path.Combine(Bancho2.osuPath, "download.osz"));
                    }
                }).Start();

                return;
            }
            catch (Exception e)
            {
                lock (downloadLock)
                {
                    isDownloading = false;
                }

                new Thread(() => Bancho2.MessageBox(e.ToString())).Start();
            }
        }

        private static void OnProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                OsuNotifications.NotifyBoxAlert($"Download {e.ProgressPercentage}% complete", 1000);
            }
            catch (Exception ex)
            {
                new Thread(() => Bancho2.MessageBox(ex.ToString())).Start();
            }
        }

        private static void OnDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                lock (downloadLock)
                {
                    isDownloading = false;
                }

                if (e.Error != null)
                {
                    OsuNotifications.NotifyBoxError($"Failed to download beatmap due to an error.");
                    return;
                }

                OsuNotifications.NotifyBoxSuccess($"Download completed!");
                Process.Start(Path.Combine(Bancho2.osuPath, "osu!.exe"), Path.Combine(Bancho2.osuPath, "download.osz"));
            }
            catch (Exception ex)
            {
                new Thread(() => Bancho2.MessageBox(ex.ToString())).Start();
            }
        }
    }
}
