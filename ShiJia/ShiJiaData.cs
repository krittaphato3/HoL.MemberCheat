using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.ShiJia
{
    public static class ShiJiaData
    {
        public static List<List<string>> GetClanList()
        {
            var field = typeof(Mainload).GetField("ShiJia_Now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static List<List<string>> GetMembers(int clanID)
        {
            var field = typeof(Mainload).GetField("Member_other", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var all = field?.GetValue(null) as List<List<List<string>>>;
            if (all != null && clanID >= 0 && clanID < all.Count)
                return all[clanID];
            return new List<List<string>>();
        }

        public static List<List<string>> GetSpouses(int clanID)
        {
            var field = typeof(Mainload).GetField("Member_Other_qu", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var all = field?.GetValue(null) as List<List<List<string>>>;
            if (all != null && clanID >= 0 && clanID < all.Count)
                return all[clanID];
            return new List<List<string>>();
        }

        // ========== Composite field helpers ==========
        public const int IDX_COMPOSITE = 2;
        public const int SUB_NAME = 0;
        public const int SUB_TALENT_TYPE = 2;
        public const int SUB_TALENT_VALUE = 3;
        public const int SUB_GENDER = 4;
        public const int SUB_SKILL_TYPE = 5;
        public const int SUB_LUCK = 6;
        public const int SUB_PERSONALITY = 7;
        public const int SUB_HOBBY = 9;

        public static string[] GetCompositeParts(List<string> member)
        {
            if (member == null || member.Count <= IDX_COMPOSITE) return new string[0];
            string field = member[IDX_COMPOSITE];
            return string.IsNullOrEmpty(field) ? new string[0] : field.Split('|');
        }

        public static void SetCompositeParts(List<string> member, string[] parts)
        {
            if (member == null || member.Count <= IDX_COMPOSITE) return;
            member[IDX_COMPOSITE] = string.Join("|", parts);
        }

        public static string GetCompositeSub(List<string> member, int subIndex)
        {
            var parts = GetCompositeParts(member);
            return (parts.Length > subIndex) ? parts[subIndex] : "";
        }

        public static void SetCompositeSub(List<string> member, int subIndex, string value)
        {
            var parts = GetCompositeParts(member);
            if (parts.Length > subIndex)
            {
                parts[subIndex] = value;
                SetCompositeParts(member, parts);
            }
        }

        public static string GetMemberName(List<string> member)
        {
            var name = GetCompositeSub(member, SUB_NAME);
            if (!string.IsNullOrEmpty(name)) return name;
            if (member == null || member.Count < 3) return "???";
            string[] parts = member[2].Split('|');
            return parts.Length > 0 ? parts[0] : "???";
        }

        public static int GetAge(List<string> member)
        {
            if (member != null && member.Count > 3 && int.TryParse(member[3], out int a)) return a;
            return -1;
        }
    }
}