using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Decree
{
    public static class DecreeData
    {
        // ZhengLing_Now = Imperial Decrees / Edicts
        public static List<List<string>> GetDecrees()
        {
            var field = typeof(Mainload).GetField("ZhengLing_Now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static string GetDecreeName(List<string> decree)
        {
            if (decree == null || decree.Count == 0) return "???";
            return decree.Count > 0 ? decree[0] : "???";
        }
    }
}