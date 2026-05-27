using System.Collections.Generic;
using System.Reflection;

namespace HoLMod.MemberCheat.Royal
{
    public static class RoyalData
    {
        private const string Field_Members = "Member_King";
        private const string Field_Spouses = "Member_King_qu";

        // Member_King line indices (per doc spec)
        public const int IDX_PERSON_KEY = 0;
        public const int IDX_APPEARANCE = 1;
        public const int IDX_COMPOSITE = 2;
        public const int IDX_AGE = 3;
        public const int IDX_WRITING = 4;
        public const int IDX_MIGHT = 5;
        public const int IDX_BUSINESS = 6;
        public const int IDX_ARTS = 7;
        public const int IDX_MOOD = 8;
        public const int IDX_RENOWN = 16;
        public const int IDX_CHARISMA = 18;
        public const int IDX_HEALTH = 19;
        public const int IDX_CUNNING = 21;

        // Lines that are internal/fixed values - hidden from UI
        public static readonly HashSet<int> HiddenIndices = new HashSet<int> { 11, 12, 17, 24, 27, 28 };

        // Composite sub-indices (line 2: Name|Hobby|Talent|TalentVal|Gender|Lifespan|Skill|Luck|Personality|ParentPK)
        public const int SUB_NAME = 0;
        public const int SUB_HOBBY = 1;
        public const int SUB_TALENT_TYPE = 2;
        public const int SUB_TALENT_VALUE = 3;
        public const int SUB_GENDER = 4;
        public const int SUB_LIFESPAN = 5;
        public const int SUB_SKILL_TYPE = 6;
        public const int SUB_LUCK = 7;
        public const int SUB_PERSONALITY = 8;
        public const int SUB_PARENT_PK = 9;

        public static readonly Dictionary<int, string> FieldLabels = new Dictionary<int, string>
        {
            {IDX_PERSON_KEY, "Person Key"},
            {IDX_APPEARANCE, "Appearance"},
            {IDX_AGE, "Age"},
            {IDX_WRITING, "Writing"},
            {IDX_MIGHT, "Might"},
            {IDX_BUSINESS, "Business"},
            {IDX_ARTS, "Arts"},
            {IDX_MOOD, "Mood"},
            {IDX_RENOWN, "Renown"},
            {IDX_CHARISMA, "Charisma"},
            {IDX_HEALTH, "Health"},
            {IDX_CUNNING, "Cunning"},
            {9, "Empire Position"},
            {10, "Fief Title"},
            {13, "Children"},
            {14, "Relationship"},
            {15, "Status"},
            {20, "Marriage"},
            {22, "Stat Gain"},
            {23, "Skill Value"},
            {25, "Biography"},
            {26, "Traits"},
            {29, "Royal Teacher"},
            {30, "Teacher Gain"},
        };

        public static List<List<string>> GetMembers()
        {
            return UIHelpers.GetStaticListField(Field_Members);
        }

        public static List<List<string>> GetSpouses()
        {
            return UIHelpers.GetStaticListField(Field_Spouses);
        }

        public static void SetMembers(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, Field_Members);
        }

        public static void SetSpouses(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, Field_Spouses);
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

        public static string GetCompositeSub(List<string> member, int subIndex)
        {
            var parts = GetCompositeParts(member);
            return (parts.Length > subIndex) ? parts[subIndex] : "";
        }

        public static void SetCompositeSub(List<string> member, int subIndex, string value)
        {
            var parts = GetCompositeParts(member);
            if (parts.Length > subIndex)
            {
                parts[subIndex] = value;
                SetCompositeParts(member, parts);
            }
        }

        public static string GetMemberName(List<string> member)
        {
            var name = GetCompositeSub(member, SUB_NAME);
            if (!string.IsNullOrEmpty(name)) return name;
            if (member == null || member.Count < 3) return "???";
            string[] parts = member[2].Split('|');
            return parts.Length > 0 ? parts[0] : "???";
        }

        public static int GetAge(List<string> member)
        {
            if (member != null && member.Count > 3 && int.TryParse(member[3], out int a)) return a;
            return -1;
        }

        public static string GetFieldLabel(int idx)
        {
            return FieldLabels.TryGetValue(idx, out string label) ? label : null;
        }
    }
}
