using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Farm
{
    public static class FarmData
    {
        public static List<List<List<string>>> GetFarmList()
        {
            var field = typeof(Mainload).GetField("NongZ_now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<List<string>>>) ?? new List<List<List<string>>>();
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
            return true; // Fief farms are player-owned
        }

        public static string GetFarmLocation(List<string> farm)
        {
            return farm.Count > 4 ? farm[4] : "?";
        }
    }
}