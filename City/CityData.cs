using System.Collections.Generic;

namespace HoLMod.MemberCheat.City
{
    public static class CityData
    {
        private const string FieldName = "CityData_now";

        public static List<List<List<string>>> GetCities()
        {
            var field = typeof(Mainload).GetField(FieldName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return (field?.GetValue(null) as List<List<List<string>>>) ?? new List<List<List<string>>>();
        }

        public static void SetCities(List<List<List<string>>> data)
        {
            var field = typeof(Mainload).GetField(FieldName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            field?.SetValue(null, data);
            UIHelpers.InvokeReadSetData();
        }

        public static string GetCityName(List<string> city)
        {
            return city.Count > 0 ? city[0] : "???";
        }
    }
}
