using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bancho2
{
    public static class OsuNotifications
    {
        public static void NotifyBox(string msg, Color color, int durationInMilliseconds = 3000)
        {
            try
            {
                OsuMethods.osuNotificationMethod.Invoke(null, new object[] { msg, color, durationInMilliseconds, null });
            }
            catch (Exception e)
            {
                Bancho2.MessageBox(e.ToString() + e.GetType().ToString());
            }
        }

        public static void NotifyBoxError(string msg, int durationInMilliseconds = 3000) => NotifyBox(msg, Color.Red, durationInMilliseconds);
        public static void NotifyBoxSuccess(string msg, int durationInMilliseconds = 3000) => NotifyBox(msg, Color.Green, durationInMilliseconds);
        public static void NotifyBoxAlert(string msg, int durationInMilliseconds = 3000) => NotifyBox(msg, Color.LightYellow, durationInMilliseconds);
    }
}
