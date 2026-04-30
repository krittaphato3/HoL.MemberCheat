using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Court
{
    public static class CourtData
    {
        // Guan_JingCheng = Royal Court Ministers (Ranks 1‑3)
        public static List<List<string>> GetMinisters()
        {
            var field = typeof(Mainload).GetField("Guan_JingCheng", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static string GetMinisterName(List<string> minister)
        {
            if (minister == null || minister.Count < 2) return "???";
            // Usually line 1 is the name or line 0 is an ID, line 1 is name
            return minister.Count > 1 ? minister[1] : "???";
        }
    }
}