using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.City
{
    public static class CityData
    {
        // CityData_now[prefID][cityID] -> List<string>
        public static List<List<List<string>>> GetCities()
        {
            var field = typeof(Mainload).GetField("CityData_now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<List<string>>>) ?? new List<List<List<string>>>();
        }

        public static string GetCityName(List<string> city)
        {
            return city.Count > 0 ? city[0] : "???";
        }
    }
}