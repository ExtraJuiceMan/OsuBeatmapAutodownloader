using Microsoft.Xna.Framework.Graphics;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bancho2
{
    // TODO: Automatically get Username/Password instead of manual input
    public class Bancho2
    {
        private const string CAPTION = @"Not_osu!direct";
        public static readonly string osuPath = Path.GetDirectoryName(typeof(osu.OsuModes).Assembly.Location);

        public static string username;
        public static string password;

        static void Main(string[] args)
        {
            MessageBox("Finding method to patch...");

            var method = OsuMethods.osuDirectFallbackMethod;

            if (method == null)
            {
                MessageBox($"Could not find method to patch, aborting.");
                return;
            }

            PatchMethod(method, typeof(Bancho2Downloader).GetMethod(nameof(Bancho2Downloader.BeatmapDownloadHandler)));

            MessageBox($"Patched method {method.DeclaringType.FullName}.{method.Name}.\nosu! Path:{osuPath}");

            Console.WriteLine("You will now be prompted to enter your osu! username and password. They are available from the game, but I don't think that they will stay in the same location every update. The username and password is required to auth into osu!'s download urls.");

            Console.WriteLine("Username:");
            username = Console.ReadLine();
            Console.WriteLine("Password:");
            // Only the hash is sent to the server
            password = HashTool.GenerateMd5Hash(Console.ReadLine());
        }

        public static void PatchMethod(MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
            RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);

            var add = IntPtr.Size == 4 ? 2 : 1;

            unsafe
            {
                int* newMethod = (int*)methodToInject.MethodHandle.Value.ToPointer() + add;
                int* toReplace = (int*)methodToReplace.MethodHandle.Value.ToPointer() + add;
                *toReplace = *newMethod;
            }
        }

        public static void MessageBox(string msg) => System.Windows.Forms.MessageBox.Show(msg, CAPTION);
    }
}
