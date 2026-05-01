using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Courtesan
{
    public static class CourtesanData
    {
        // Member_Qinglou[cityIndex][memberIndex] -> List<string>
        public static List<List<List<string>>> GetCourtesans()
        {
            var field = typeof(Mainload).GetField("Member_Qinglou", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<List<string>>>) ?? new List<List<List<string>>>();
        }

        public static string GetName(List<string> member)
        {
            if (member == null || member.Count < 3) return "???";
            // line 2 is composite, sub 0 is name
            var parts = member.Count > 2 ? member[2].Split('|') : new string[0];
            return parts.Length > 0 ? parts[0] : "???";
        }

        public static int GetAge(List<string> member)
        {
            return member.Count > 3 && int.TryParse(member[3], out int a) ? a : -1;
        }
    }
}