using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.AttackPrefecture
{
    public static class AttackPrefectureData
    {
        // CityID_CanAttack – list of hostile prefecture IDs
        public static List<int> GetAttackPrefectures()
        {
            var field = typeof(Mainload).GetField("CityID_CanAttack", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<int>) ?? new List<int>();
        }
    }
}