using System.Collections.Generic;

namespace HoLMod.MemberCheat.WarEvent
{
    public static class WarEventData
    {
        private const string FieldName = "WarEvent_Now";

        public static List<List<string>> GetWarEvents()
        {
            return UIHelpers.GetStaticListField(FieldName);
        }

        public static void SetWarEvents(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, FieldName);
        }

        public static string GetEventName(List<string> warEvent)
        {
            return warEvent?.Count > 0 ? warEvent[0] : "???";
        }
    }
}
