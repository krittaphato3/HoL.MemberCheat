using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Royal
{
    public static class RoyalData
    {
        private const string Field_Members = "Member_King";
        private const string Field_Spouses = "Member_King_qu";

        public static List<List<string>> GetMembers()
        {
            return UIHelpers.GetStaticListField(Field_Members);
        }

        public static List<List<string>> GetSpouses()
        {
            return UIHelpers.GetStaticListField(Field_Spouses);
        }

        public static void SetMembers(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, Field_Members);
        }

        public static void SetSpouses(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, Field_Spouses);
        }

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
