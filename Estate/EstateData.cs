using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Estate
{
    public static class EstateData
    {
        // Fudi_now[estateID] -> List<string>
        public static List<List<string>> GetEstates()
        {
            var field = typeof(Mainload).GetField("Fudi_now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static string GetEstateName(List<string> estate)
        {
            return estate.Count > 0 ? estate[0] : "???";
        }
    }
}