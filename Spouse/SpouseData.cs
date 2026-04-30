using System;
using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Spouse
{
    public static class SpouseData
    {
        public const string Category = "Member_qu";

        public const int IDX_PERSON_ID = 0;
        public const int IDX_APPEARANCE = 1;
        public const int IDX_COMPOSITE = 2;
        public const int IDX_CHILD_IDS = 3;
        public const int IDX_HOUSING = 4;
        public const int IDX_AGE = 5;
        public const int IDX_WRITING = 6;
        public const int IDX_MIGHT = 7;
        public const int IDX_BUSINESS = 8;
        public const int IDX_ARTS = 9;
        public const int IDX_MOOD = 10;
        public const int IDX_STATUS = 11;
        public const int IDX_RENOWN = 12;
        public const int IDX_STATUS_DURATION = 13;
        public const int IDX_EQUIPMENT = 14;
        public const int IDX_CHARISMA = 15;
        public const int IDX_HEALTH = 16;
        public const int IDX_RECENT_EVENTS = 17;
        public const int IDX_PREGNANCY = 18;
        public const int IDX_CUNNING = 19;
        public const int IDX_STAMINA = 20;
        public const int IDX_SKILL_POINTS = 23;
        public const int IDX_PREGNANCY_PROB = 24;
        public const int IDX_TRAITS = 26;
        public const int IDX_OFFICIAL_POS = 30;
        public const int IDX_MARITAL_HARMONY = 31;
        public const int IDX_CLAN_DUTY = 32;

        public const int SUB_NAME = 0;
        public const int SUB_TALENT_TYPE = 2;
        public const int SUB_TALENT_VALUE = 3;
        public const int SUB_GENDER = 4;
        public const int SUB_SKILL_TYPE = 5;
        public const int SUB_LUCK = 6;
        public const int SUB_PERSONALITY = 7;
        public const int SUB_HOBBY = 9;

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

        public static string GetMaritalHarmony(List<string> member)
        {
            if (member == null || member.Count <= IDX_MARITAL_HARMONY) return "0";
            return member[IDX_MARITAL_HARMONY].Split('|')[0];
        }

        public static void SetMaritalHarmony(List<string> member, string value)
        {
            if (member == null || member.Count <= IDX_MARITAL_HARMONY) return;
            var parts = member[IDX_MARITAL_HARMONY].Split('|');
            if (parts.Length > 0) parts[0] = value;
            else parts = new string[] { value };
            member[IDX_MARITAL_HARMONY] = string.Join("|", parts);
        }
    }
}