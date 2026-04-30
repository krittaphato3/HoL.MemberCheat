using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Horse
{
    public static class HorseData
    {
        // Horse_Have – player's owned horses
        public static List<List<string>> GetHorses()
        {
            var field = typeof(Mainload).GetField("Horse_Have", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static string GetHorseName(List<string> horse)
        {
            if (horse == null || horse.Count == 0) return "???";
            return horse.Count > 0 ? horse[0] : "???";
        }
    }
}