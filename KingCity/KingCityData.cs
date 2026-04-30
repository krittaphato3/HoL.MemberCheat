using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.KingCity
{
    public static class KingCityData
    {
        // KingCityData_now – Empire capital info (army, commanders, etc.)
        public static List<string> GetKingCity()
        {
            var field = typeof(Mainload).GetField("KingCityData_now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<string>) ?? new List<string>();
        }
    }
}