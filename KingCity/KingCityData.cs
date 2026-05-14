using System.Collections.Generic;

namespace HoLMod.MemberCheat.KingCity
{
    public static class KingCityData
    {
        private const string FieldName = "KingCityData_now";

        public static List<string> GetKingCity()
        {
            var field = typeof(Mainload).GetField(FieldName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return (field?.GetValue(null) as List<string>) ?? new List<string>();
        }

        public static void SetKingCity(List<string> data)
        {
            var field = typeof(Mainload).GetField(FieldName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            field?.SetValue(null, data);
            UIHelpers.InvokeReadSetData();
        }

        public static readonly Dictionary<int, string> FieldLabels = new Dictionary<int, string>
        {
            {0, "City Name"}, {1, "Garrison"}, {2, "Commander"}, {3, "Food"},
            {4, "Gold"}, {5, "Troop Count"}, {6, "Morale"}, {7, "Defenses"}
        };
    }
}
