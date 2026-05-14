using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Court
{
    public static class CourtData
    {
        private const string FieldName = "Guan_JingCheng";

        public static List<List<string>> GetMinisters()
        {
            return UIHelpers.GetStaticListField(FieldName);
        }

        public static void SetMinisters(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, FieldName);
        }

        public static string GetMinisterName(List<string> minister)
        {
            if (minister == null || minister.Count < 2) return "???";
            return minister.Count > 1 ? minister[1] : "???";
        }

        public static readonly Dictionary<int, string> FieldLabels = new Dictionary<int, string>
        {
            {0, "ID"}, {1, "Name"}, {2, "Position"}, {3, "Salary"},
            {4, "Loyalty"}, {5, "Competence"}, {6, "Term"}
        };
    }
}
