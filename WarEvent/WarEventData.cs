using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.WarEvent
{
    public static class WarEventData
    {
        // WarEvent_Now – ongoing military conflicts
        public static List<List<string>> GetWarEvents()
        {
            var field = typeof(Mainload).GetField("WarEvent_Now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static string GetEventName(List<string> warEvent)
        {
            if (warEvent == null || warEvent.Count == 0) return "???";
            return warEvent.Count > 0 ? warEvent[0] : "???";
        }
    }
}