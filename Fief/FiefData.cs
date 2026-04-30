using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Fief
{
    public static class FiefData
    {
        // Fengdi_now[prefID] -> List<string> (one fief per prefecture)
        public static List<List<string>> GetFiefs()
        {
            var field = typeof(Mainload).GetField("Fengdi_now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static string GetFiefName(List<string> fief)
        {
            return fief.Count > 0 ? fief[0] : "???";
        }
    }
}