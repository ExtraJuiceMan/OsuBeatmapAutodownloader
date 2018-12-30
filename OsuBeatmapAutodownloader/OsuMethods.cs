using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bancho2
{
    public static class OsuMethods
    {
        /*
        private const string DIRECT_FALLBACK_METHOD = @"#=zHHo_wrVa9_GVPGQ6qw==";
        private const string DIRECT_FALLBACK_CLASS = @"#=zJZ2K$wVJI7tSAKMgB8ez3zo=";

        private const string PROCESS_START_METHOD = @"#=zjjRz2xI=";
        private const string PROCESS_START_CLASS = @"#=z1KSuZq74CTCffBe1Kw==";

        private const string NOTIFICATION_METHOD = @"#=zWeAJdwg=";
        private const string NOTIFICATION_CLASS = @"#=z86rw7OFSW3EyOjTo_LZx02M=";
        */

        // Not true pattern scanning, just want to get around any scrambling of method names on build.
        public static readonly MethodInfo osuDirectFallbackMethod = 
            FindStaticMethod(il =>
                il[1].OpCode == System.Reflection.Emit.OpCodes.Switch
                && il[0].OpCode == System.Reflection.Emit.OpCodes.Ldarg_0
                && il[4].OpCode == System.Reflection.Emit.OpCodes.Call,
                5);

        public static readonly MethodInfo osuProcessStart = 
            FindStaticMethod(il => 
                il[0].OpCode == System.Reflection.Emit.OpCodes.Newobj
                && il[3].OpCode == System.Reflection.Emit.OpCodes.Stfld
                && il[6].OpCode == System.Reflection.Emit.OpCodes.Stfld
                && il[9].OpCode == System.Reflection.Emit.OpCodes.Call,
                10);

        public static readonly MethodInfo osuNotificationMethod = 
           FindStaticMethod(il => 
                il[10].OpCode == System.Reflection.Emit.OpCodes.Stfld
                && il[19].OpCode == System.Reflection.Emit.OpCodes.Callvirt
                && il[17].OpCode == System.Reflection.Emit.OpCodes.Newobj
                && il[16].OpCode == System.Reflection.Emit.OpCodes.Ldftn,
                20);

        private static MethodInfo FindStaticMethod(Func<IList<Instruction>, bool> predicate, int minIntructions) =>
            typeof(osu.OsuModes).Assembly
                .GetTypes()
                .SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
                .FirstOrDefault(x =>
                {
                    if (x.GetMethodBody() == null)
                        return false;

                    var il = x.GetInstructions();

                    if (il.Count < minIntructions)
                        return false;

                    return predicate(il);
                });
    }
}
