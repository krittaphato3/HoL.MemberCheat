using System.Collections.Generic;

namespace HoLMod.MemberCheat.HanMen
{
    public static class HanMenData
    {
        private const string FieldName = "Member_HanMen";

        // Member_HanMen line indices (per doc spec)
        public const int IDX_PERSON_KEY = 0;
        public const int IDX_APPEARANCE = 1;
        public const int IDX_COMPOSITE = 2;
        public const int IDX_AGE = 3;
        public const int IDX_WRITING = 4;
        public const int IDX_MIGHT = 5;
        public const int IDX_BUSINESS = 6;
        public const int IDX_ARTS = 7;
        public const int IDX_MOOD = 8;
        public const int IDX_EMPIRE_POSITION = 9;
        public const int IDX_SCHOLARSHIP = 10;
        public const int IDX_FIEF_TITLE = 11;
        public const int IDX_STATUS = 16;
        public const int IDX_RENOWN = 17;
        public const int IDX_CHARISMA = 19;
        public const int IDX_HEALTH = 20;
        public const int IDX_CUNNING = 22;
        public const int IDX_STAT_GAIN = 23;
        public const int IDX_SCHOOL = 25;

        // Lines that are internal/fixed values - hidden from UI
        public static readonly HashSet<int> HiddenIndices = new HashSet<int> { 12, 13, 14, 15, 18, 21, 24, 26 };

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
            {IDX_EMPIRE_POSITION, "Empire Position"},
            {IDX_SCHOLARSHIP, "Scholarship"},
            {IDX_FIEF_TITLE, "Fief Title"},
            {IDX_STATUS, "Status"},
            {IDX_RENOWN, "Renown"},
            {IDX_CHARISMA, "Charisma"},
            {IDX_HEALTH, "Health"},
            {IDX_CUNNING, "Cunning"},
            {IDX_STAT_GAIN, "Stat Gain"},
            {IDX_SCHOOL, "School"},
        };

        public static List<List<string>> GetMembers()
        {
            return UIHelpers.GetStaticListField(FieldName);
        }

        public static void SetMembers(List<List<string>> list)
        {
            UIHelpers.WriteBackSetData(list, FieldName);
        }

        public static string GetName(List<string> member)
        {
            if (member == null || member.Count < 3) return "???";
            var parts = member.Count > IDX_COMPOSITE ? member[IDX_COMPOSITE].Split('|') : new string[0];
            return parts.Length > 0 ? parts[0] : "???";
        }

        public static int GetAge(List<string> member)
        {
            return member.Count > IDX_AGE && int.TryParse(member[IDX_AGE], out int a) ? a : -1;
        }

        public static string GetFieldLabel(int idx)
        {
            return FieldLabels.TryGetValue(idx, out string label) ? label : null;
        }
    }
}
