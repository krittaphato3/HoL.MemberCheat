using System.Collections.Generic;

namespace HoLMod.MemberCheat.Decree
{
    public static class DecreeData
    {
        private const string FieldName = "ZhengLing_Now";

        public static List<List<string>> GetDecrees()
        {
            return UIHelpers.GetStaticListField(FieldName);
        }

        public static void SetDecrees(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, FieldName);
        }

        public static string GetDecreeName(List<string> decree)
        {
            if (decree == null || decree.Count == 0) return "???";
            return decree[0];
        }
    }
}
