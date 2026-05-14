using System.Collections.Generic;

namespace HoLMod.MemberCheat.Horse
{
    public static class HorseData
    {
        private const string FieldName = "Horse_Have";

        public static List<List<string>> GetHorses()
        {
            return UIHelpers.GetStaticListField(FieldName);
        }

        public static void SetHorses(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, FieldName);
        }

        public static string GetHorseName(List<string> horse)
        {
            return horse?.Count > 0 ? horse[0] : "???";
        }
    }
}
