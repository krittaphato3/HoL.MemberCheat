using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.ZhuangTou
{
    public static class ZhuangTouData
    {
        // ZhuangTou_now[fiefID][farmID][managerIndex] -> List<string>
        public static List<List<List<List<string>>>> GetManagers()
        {
            var field = typeof(Mainload).GetField("ZhuangTou_now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<List<List<string>>>>) ?? new List<List<List<List<string>>>>();
        }

        public static string GetManagerName(List<string> manager)
        {
            // From ZhuangTouInfoPanel: composite at index 2, sub 0 is name
            if (manager == null || manager.Count < 3) return "???";
            var parts = manager[2].Split('|');
            return parts.Length > 0 ? parts[0] : "???";
        }

        public static int GetAge(List<string> manager)
        {
            return manager.Count > 3 && int.TryParse(manager[3], out int a) ? a : -1;
        }
    }
}