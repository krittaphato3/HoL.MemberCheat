using System.Collections.Generic;

namespace HoLMod.MemberCheat.VassalPrince
{
    public static class VassalPrinceData
    {
        private const string FieldName = "WangGData_now";

        public static List<List<string>> GetPrinces()
        {
            return UIHelpers.GetStaticListField(FieldName);
        }

        public static void SetPrinces(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, FieldName);
        }

        public static string GetPrinceName(List<string> prince)
        {
            return prince.Count > 0 ? prince[0] : "???";
        }
    }
}
