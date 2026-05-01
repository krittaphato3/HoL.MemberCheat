using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.HanMen
{
    public static class HanMenData
    {
        // Member_HanMen – list of civilian clan members
        public static List<List<string>> GetMembers()
        {
            var field = typeof(Mainload).GetField("Member_HanMen", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static string GetName(List<string> member)
        {
            if (member == null || member.Count < 3) return "???";
            var parts = member.Count > 2 ? member[2].Split('|') : new string[0];
            return parts.Length > 0 ? parts[0] : "???";
        }

        public static int GetAge(List<string> member)
        {
            return member.Count > 3 && int.TryParse(member[3], out int a) ? a : -1;
        }
    }
}