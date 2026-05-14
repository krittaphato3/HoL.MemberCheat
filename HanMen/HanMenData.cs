using System.Collections.Generic;

namespace HoLMod.MemberCheat.HanMen
{
    public static class HanMenData
    {
        private const string FieldName = "Member_HanMen";

        public static List<List<string>> GetMembers()
        {
            return UIHelpers.GetStaticListField(FieldName);
        }

        public static void SetMembers(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, FieldName);
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
