using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.VassalPrince
{
    public static class VassalPrinceData
    {
        // WangGData_now[prefID] -> List<string>
        public static List<List<string>> GetPrinces()
        {
            var field = typeof(Mainload).GetField("WangGData_now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static string GetPrinceName(List<string> prince)
        {
            return prince.Count > 0 ? prince[0] : "???";
        }
    }
}