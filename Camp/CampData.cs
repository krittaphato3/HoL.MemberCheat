using System.Collections.Generic;

namespace HoLMod.MemberCheat.Camp
{
    public static class CampData
    {
        private const string FieldName = "JunYing_now";

        public static List<List<string>> GetCamps()
        {
            return UIHelpers.GetStaticListField(FieldName);
        }

        public static void SetCamps(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, FieldName);
        }

        public static string GetCampName(List<string> camp)
        {
            return camp.Count > 0 ? camp[0] : "???";
        }
    }
}
