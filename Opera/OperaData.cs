using System.Collections.Generic;

namespace HoLMod.MemberCheat.Opera
{
    public static class OperaData
    {
        private const string FieldName = "XiQuID_Enter";

        public static List<List<string>> GetOperas()
        {
            return UIHelpers.GetStaticListField(FieldName);
        }

        public static void SetOperas(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, FieldName);
        }

        public static string GetOperaName(List<string> opera)
        {
            return opera.Count > 0 ? opera[0] : "???";
        }
    }
}
