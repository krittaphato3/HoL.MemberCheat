using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Opera
{
    public static class OperaData
    {
        // XiQuID_Enter – opera/drama performances
        public static List<List<string>> GetOperas()
        {
            var field = typeof(Mainload).GetField("XiQuID_Enter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static string GetOperaName(List<string> opera)
        {
            return opera.Count > 0 ? opera[0] : "???";
        }
    }
}