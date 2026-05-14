using System.Collections.Generic;

namespace HoLMod.MemberCheat.Estate
{
    public static class EstateData
    {
        private const string FieldName = "Fudi_now";

        public static List<List<string>> GetEstates()
        {
            return UIHelpers.GetStaticListField(FieldName);
        }

        public static void SetEstates(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, FieldName);
        }

        public static string GetEstateName(List<string> estate)
        {
            return estate.Count > 0 ? estate[0] : "???";
        }
    }
}
