using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace HoLMod.MemberCheat.ClanMember
{
    public static class ClanMemberData
    {
        public const int IDX_APPEARANCE = 1;
        public const int IDX_CHILD_IDS = 2;
        public const int IDX_ESTATE = 3;
        public const int IDX_COMPOSITE = 4;
        public const int IDX_PERSONALITY = 5;
        public const int IDX_AGE = 6;
        public const int IDX_WRITING = 7;
        public const int IDX_MIGHT = 8;
        public const int IDX_BUSINESS = 9;
        public const int IDX_ARTS = 10;
        public const int IDX_MOOD = 11;
        public const int IDX_SCHOLARSHIP = 13;
        public const int IDX_FIEF_TITLE = 14;
        public const int IDX_STATUS = 15;
        public const int IDX_RENOWN = 16;
        public const int IDX_STATUS_DURATION = 18;
        public const int IDX_BOOK_PROGRESS = 19;
        public const int IDX_HEALTH = 20;
        public const int IDX_CHARISMA = 21;
        public const int IDX_CLAN_LEADER = 22;
        public const int IDX_TRAITS = 23;
        public const int IDX_RECENT_EVENTS = 24;
        public const int IDX_PREGNANCY = 25;
        public const int IDX_MARRIAGE = 26;
        public const int IDX_CUNNING = 27;
        public const int IDX_STAMINA = 30;
        public const int IDX_BASIC_STAT_GAIN = 31;
        public const int IDX_SCHOOL_VALUES = 32;
        public const int IDX_SKILL_VALUE = 33;
        public const int IDX_PREGNANCY_COOLDOWN = 34;
        public const int IDX_BIOGRAPHY = 36;
        public const int IDX_STUDY_SCHOOL = 40;
        public const int IDX_CLAN_DUTY = 41;

        public const int SUB_NAME = 0;
        public const int SUB_GENERATION = 1;
        public const int SUB_TALENT_TYPE = 2;
        public const int SUB_TALENT_VALUE = 3;
        public const int SUB_GENDER = 4;
        public const int SUB_LIFESPAN = 5;
        public const int SUB_SKILL_TYPE = 6;
        public const int SUB_LUCK = 7;
        public const int SUB_HOBBY = 9;

        public static readonly Dictionary<int, string> MainStats = new Dictionary<int, string>
        {
            {IDX_WRITING, "Writing"}, {IDX_MIGHT, "Might"}, {IDX_BUSINESS, "Business"}, {IDX_ARTS, "Arts"},
            {IDX_MOOD, "Mood"}, {IDX_RENOWN, "Renown"}, {IDX_HEALTH, "Health"}, {IDX_CHARISMA, "Charisma"},
            {IDX_CUNNING, "Cunning"}, {IDX_STAMINA, "Stamina"}, {IDX_SKILL_VALUE, "Skill Value"},
        };

        public static readonly List<int> UpperStats = new List<int> { IDX_WRITING, IDX_MIGHT, IDX_BUSINESS, IDX_ARTS };
        public static readonly List<int> LowerStats = new List<int> { IDX_MOOD, IDX_RENOWN, IDX_HEALTH, IDX_CHARISMA, IDX_CUNNING, IDX_STAMINA };

        public static readonly Dictionary<int, string> GenderOptions = new Dictionary<int, string> { { 0, "Female" }, { 1, "Male" } };
        public static readonly Dictionary<int, string> TalentTypeOptions = new Dictionary<int, string> { { 1, "Writing" }, { 2, "Might" }, { 3, "Business" }, { 4, "Arts" } };
        public static readonly Dictionary<int, string> SkillTypeOptions = new Dictionary<int, string> { { 0, "None" }, { 1, "Sorcery" }, { 2, "Medicine" }, { 3, "Daoism" }, { 4, "Divination" }, { 5, "Charisma" }, { 6, "Technology" } };
        public static readonly Dictionary<int, string> HobbyOptions = new Dictionary<int, string> { { 0, "Rouge" }, { 1, "Ink" }, { 2, "Art" }, { 3, "Antique" }, { 4, "Tea Set" }, { 5, "Incense" }, { 6, "Vase" }, { 7, "Wine" }, { 8, "Music" }, { 9, "Pelt" } };
        public static readonly Dictionary<int, string> PersonalityOptions = new Dictionary<int, string> { { 0, "Unclear" }, { 1, "Proud" }, { 2, "Righteous" }, { 3, "Lively" }, { 4, "Kind" }, { 5, "Honest" }, { 6, "Carefree" }, { 7, "Cold" }, { 8, "Insecure" }, { 9, "Timid" }, { 10, "Shy" }, { 11, "Mean" }, { 12, "Fickle" }, { 13, "Gloomy" }, { 14, "Paranoid" } };

        public static readonly Dictionary<int, string> StatusOptions = new Dictionary<int, string>
        {
            {0, "Available"}, {1, "Demoted"}, {2, "Study Tour"}, {3, "Caning"}, {4, "Imprisoned"},
            {5, "Exile 500m"}, {6, "Exile 1k"}, {7, "Exile 2k"}, {8, "Exile 3k"}, {9, "Beheaded"},
            {10, "Military Mission"}, {11, "Travelling"}, {12, "Visit"}, {13, "Runaway"},
            {15, "Commerce"}, {16, "Official"}, {17, "Gigs"}, {18, "Street Trading"},
            {19, "Intel"}, {20, "Rumors"}, {21, "Private Teacher"}, {22, "Stage Actor"},
            {23, "Clan Teacher"}, {24, "Training Coach"}
        };

        public static readonly Dictionary<int, string> MarriageOptions = new Dictionary<int, string>
        {
            {0, "Single"}, {1, "Married"}, {2, "Widowed"}, {3, "Remarried"},
            {4, "Separated"}, {5, "Discarded"}, {6, "Divorced"}
        };

        public static readonly Dictionary<int, string> ScholarshipTitles = new Dictionary<int, string>
        {
            {0, "None"}, {1, "Xiucai"}, {2, "Juren"}, {3, "Xieyuan"},
            {4, "Gongshi"}, {5, "Huiyuan"}, {6, "Jinshi"}, {7, "Tanhua"},
            {8, "Bangyan"}, {9, "Zhuangyuan"}, {10, "M.Xiucai"}, {11, "M.Juren"},
            {12, "M.Xieyuan"}, {13, "M.Gongshi"}, {14, "M.Huiyuan"}, {15, "M.Jinshi"},
            {16, "M.Tanhua"}, {17, "M.Bangyan"}, {18, "M.Zhuangyuan"}
        };

        public static readonly Dictionary<int, string> FiefLevels = new Dictionary<int, string>
        {
            {0, "None"}, {1, "Earl"}, {2, "Marquis"}, {3, "Duke"}, {4, "Vassal Prince"}
        };

        public static readonly Dictionary<int, string> StudySchools = new Dictionary<int, string>
        {
            {0, "None"}, {1, "Mingli"}, {2, "Jiuyuan"}, {3, "Jinwen"}
        };

        // ========== Data access ==========
        public static List<List<string>> GetMemberList(string category)
        {
            var field = typeof(Mainload).GetField(category, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static void SetMemberList(string category, List<List<string>> list)
        {
            var field = typeof(Mainload).GetField(category, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            field?.SetValue(null, list);
            typeof(Mainload).GetMethod("ReadSetData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, null);
        }

        public static List<string> GetFamilyData()
        {
            var field = typeof(Mainload).GetField("FamilyData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return field?.GetValue(null) as List<string> ?? new List<string>();
        }
        public static void SetFamilyData(List<string> data)
        {
            var field = typeof(Mainload).GetField("FamilyData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            field?.SetValue(null, data);
            typeof(Mainload).GetMethod("ReadSetData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, null);
        }

        public static List<string> GetCGNum()
        {
            var field = typeof(Mainload).GetField("CGNum", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return field?.GetValue(null) as List<string> ?? new List<string>();
        }
        public static void SetCGNum(List<string> data)
        {
            var field = typeof(Mainload).GetField("CGNum", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            field?.SetValue(null, data);
            typeof(Mainload).GetMethod("ReadSetData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, null);
        }

        public static List<string> GetZhiZeData_ZhangMu()
        {
            var field = typeof(Mainload).GetField("ZhiZeData_ZhangMu", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return field?.GetValue(null) as List<string> ?? new List<string>();
        }
        public static void SetZhiZeData_ZhangMu(List<string> data)
        {
            var field = typeof(Mainload).GetField("ZhiZeData_ZhangMu", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            field?.SetValue(null, data);
            typeof(Mainload).GetMethod("ReadSetData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, null);
        }

        public static List<string> GetPerData()
        {
            var field = typeof(Mainload).GetField("PerData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return field?.GetValue(null) as List<string> ?? new List<string>();
        }
        public static void SetPerData(List<string> data)
        {
            var field = typeof(Mainload).GetField("PerData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            field?.SetValue(null, data);
            typeof(Mainload).GetMethod("ReadSetData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, null);
        }

        // ========== Composite field helpers ==========
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
            string name = GetCompositeSub(member, SUB_NAME);
            return !string.IsNullOrEmpty(name) ? name : (member != null && member.Count > 0 ? member[0] : "???");
        }

        public static int GetAge(List<string> member)
        {
            if (member != null && member.Count > IDX_AGE && int.TryParse(member[IDX_AGE], out int age)) return age;
            return -1;
        }
    }
}