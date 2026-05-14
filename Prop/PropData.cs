using System.Collections.Generic;

namespace HoLMod.MemberCheat.Prop
{
    public static class PropData
    {
        private const string FieldName = "PropData_Enter";

        public static List<List<string>> GetProps()
        {
            return UIHelpers.GetStaticListField(FieldName);
        }

        public static void SetProps(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, FieldName);
        }

        public static string GetPropName(List<string> prop)
        {
            return prop.Count > 0 ? prop[0] : "???";
        }
    }
}
