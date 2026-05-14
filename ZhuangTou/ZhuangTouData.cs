using System.Collections.Generic;

namespace HoLMod.MemberCheat.ZhuangTou
{
    public static class ZhuangTouData
    {
        private const string FieldName = "ZhuangTou_now";

        public static List<List<List<List<string>>>> GetManagers()
        {
            var field = typeof(Mainload).GetField(FieldName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return (field?.GetValue(null) as List<List<List<List<string>>>>) ?? new List<List<List<List<string>>>>();
        }

        public static void SetManagers(List<List<List<List<string>>>> data)
        {
            var field = typeof(Mainload).GetField(FieldName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            field?.SetValue(null, data);
            UIHelpers.InvokeReadSetData();
        }

        public static string GetManagerName(List<string> manager)
        {
            if (manager == null || manager.Count < 3) return "???";
            var parts = manager[2].Split('|');
            return parts.Length > 0 ? parts[0] : "???";
        }

        public static int GetAge(List<string> manager)
        {
            return manager.Count > 3 && int.TryParse(manager[3], out int a) ? a : -1;
        }
    }
}
