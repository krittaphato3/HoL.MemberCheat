using System.Collections.Generic;

namespace HoLMod.MemberCheat.Fief
{
    public static class FiefData
    {
        private const string FieldName = "Fengdi_now";

        public static List<List<string>> GetFiefs()
        {
            return UIHelpers.GetStaticListField(FieldName);
        }

        public static void SetFiefs(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, FieldName);
        }

        public static string GetFiefName(List<string> fief)
        {
            return fief.Count > 0 ? fief[0] : "???";
        }
    }
}
