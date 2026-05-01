using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Prop
{
    public static class PropData
    {
        // PropData_Enter – player's inventory items
        public static List<List<string>> GetProps()
        {
            var field = typeof(Mainload).GetField("PropData_Enter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static string GetPropName(List<string> prop)
        {
            // Typically line 0 is the item name
            return prop.Count > 0 ? prop[0] : "???";
        }
    }
}