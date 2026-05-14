using System.Collections.Generic;

namespace HoLMod.MemberCheat.Farm
{
    public static class FarmData
    {
        private const string FieldName = "NongZ_now";

        public static List<List<List<string>>> GetFarmList()
        {
            var field = typeof(Mainload).GetField(FieldName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return (field?.GetValue(null) as List<List<List<string>>>) ?? new List<List<List<string>>>();
        }

        public static void SetFarmList(List<List<List<string>>> data)
        {
            var field = typeof(Mainload).GetField(FieldName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            field?.SetValue(null, data);
            UIHelpers.InvokeReadSetData();
        }

        public static string GetFarmName(List<string> farm)
        {
            return farm.Count > 6 ? farm[6] : "???";
        }

        public static int GetFarmSize(List<string> farm)
        {
            return farm.Count > 5 && int.TryParse(farm[5], out int s) ? s : 0;
        }

        public static bool IsPlayerFarm(List<string> farm, int regionIndex)
        {
            if (regionIndex == 0)
                return farm.Count > 0 && farm[0] == "-1";
            return true;
        }
    }
}
