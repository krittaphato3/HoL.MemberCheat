using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Mine
{
    public static class MineData
    {
        // Kuang_now[mineID] -> List<string>
        public static List<List<string>> GetMines()
        {
            var field = typeof(Mainload).GetField("Kuang_now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static string GetMineName(List<string> mine)
        {
            return mine.Count > 0 ? mine[0] : "???";
        }
    }
}