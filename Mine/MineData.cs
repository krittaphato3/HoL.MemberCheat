using System.Collections.Generic;

namespace HoLMod.MemberCheat.Mine
{
    public static class MineData
    {
        private const string FieldName = "Kuang_now";

        public static List<List<string>> GetMines()
        {
            return UIHelpers.GetStaticListField(FieldName);
        }

        public static void SetMines(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, FieldName);
        }

        public static string GetMineName(List<string> mine)
        {
            return mine.Count > 0 ? mine[0] : "???";
        }
    }
}
