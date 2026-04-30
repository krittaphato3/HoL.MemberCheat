using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Camp
{
    public static class CampData
    {
        // JunYing_now[campID] -> List<string>
        public static List<List<string>> GetCamps()
        {
            var field = typeof(Mainload).GetField("JunYing_now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static string GetCampName(List<string> camp)
        {
            return camp.Count > 0 ? camp[0] : "???";
        }
    }
}