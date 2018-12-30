using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bancho2
{
    public static class OsuUrls
    {
        public const string DIRECT_SEARCH_SET = @"https://osu.ppy.sh/web/osu-search-set.php?u={0}&h={1}&{2}={3}";
        public const string BEATMAP_DOWNLOAD = @"https://osu.ppy.sh/d/{0}n?u={1}&h={2}";
    }
}
