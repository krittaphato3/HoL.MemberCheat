using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.AttackPrefecture
{
    public static class AttackPrefectureData
    {
        private const string FieldName = "CityID_CanAttack";

        public static List<int> GetAttackPrefectures()
        {
            var field = typeof(Mainload).GetField(FieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<int>) ?? new List<int>();
        }

        public static void SetAttackPrefectures(List<int> list)
        {
            var field = typeof(Mainload).GetField(FieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            field?.SetValue(null, list);
            UIHelpers.InvokeReadSetData();
        }
    }
}
