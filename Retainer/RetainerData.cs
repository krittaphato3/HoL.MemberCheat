using System;
using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Retainer
{
    public static class RetainerData
    {
        public const string Category = "MenKe_Now";

        public const int IDX_ID = 0;
        public const int IDX_APPEARANCE = 1;
        public const int IDX_COMPOSITE = 2;
        public const int IDX_AGE = 3;
        public const int IDX_WRITING = 4;
        public const int IDX_MIGHT = 5;
        public const int IDX_BUSINESS = 6;
        public const int IDX_ARTS = 7;
        public const int IDX_MOOD = 8;
        public const int IDX_TEACHING = 9;
        public const int IDX_STATUS = 10;
        public const int IDX_RENOWN = 11;
        public const int IDX_UNKNOWN_12 = 12;
        public const int IDX_CHARISMA = 13;
        public const int IDX_HEALTH = 14;
        public const int IDX_CUNNING = 15;
        public const int IDX_SKILL_POINTS = 16;
        public const int IDX_PREGNANCY = 17;
        public const int IDX_SALARY = 18;
        public const int IDX_STAMINA = 19;
        public const int IDX_UNKNOWN_20 = 20;
        public const int IDX_SPECIAL_TAG = 21;

        public const int SUB_NAME = 0;
        public const int SUB_TALENT_TYPE = 2;
        public const int SUB_TALENT_VALUE = 3;
        public const int SUB_GENDER = 4;
        public const int SUB_LIFESPAN = 5;
        public const int SUB_SKILL_TYPE = 6;
        public const int SUB_LUCK = 7;
        public const int SUB_PERSONALITY = 8;

        public static List<List<string>> GetList()
        {
            var field = typeof(Mainload).GetField(Category, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static void SetList(List<List<string>> list)
        {
            var field = typeof(Mainload).GetField(Category, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            field?.SetValue(null, list);
            typeof(Mainload).GetMethod("ReadSetData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, null);
        }

        public static string[] GetCompositeParts(List<string> member)
        {
            if (member == null || member.Count <= IDX_COMPOSITE) return new string[0];
            string field = member[IDX_COMPOSITE];
            return string.IsNullOrEmpty(field) ? new string[0] : field.Split('|');
        }

        public static void SetCompositeParts(List<string> member, string[] parts)
        {
            if (member == null || member.Count <= IDX_COMPOSITE) return;
            member[IDX_COMPOSITE] = string.Join("|", parts);
        }

        public static string GetCompositeSub(List<string> member, int subIdx)
        {
            var parts = GetCompositeParts(member);
            return (parts.Length > subIdx) ? parts[subIdx] : "";
        }

        public static void SetCompositeSub(List<string> member, int subIdx, string value)
        {
            var parts = GetCompositeParts(member);
            if (parts.Length > subIdx)
            {
                parts[subIdx] = value;
                SetCompositeParts(member, parts);
            }
        }

        public static string GetName(List<string> member) => GetCompositeSub(member, SUB_NAME);

        public static int GetAge(List<string> member)
        {
            if (member != null && member.Count > IDX_AGE && int.TryParse(member[IDX_AGE], out int age)) return age;
            return -1;
        }
    }
}