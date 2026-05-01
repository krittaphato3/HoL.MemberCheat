using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.ClanMember
{
    public static class ClanMemberUI
    {
        private static int selectedMemberIndex = -1;
        private static List<List<string>> memberList = new List<List<string>>();
        private static string[] memberNames = new string[0];
        private static Vector2 scrollMember, scrollEditor;
        private static string currentSubCategory = "Member_now";
        private static string currentSubName = "Current Family";
        private static bool showingFamily = false;
        private static bool showingFinances = false;
        private static bool needsRefresh = true;
        private static string searchText = "";

        private static readonly Dictionary<int, string> familyDataLabels = new Dictionary<int, string>
        {
            {0, "Location"}, {1, "Clan Name"}, {2, "Clan Level"}, {3, "Clan Renown"},
            {4, "Court Influence"}, {5, "Warehouse Storage"}, {6, "Barn Space"}
        };

        private static readonly long[] coinAddAmounts = { 100_000, 1_000_000, 10_000_000, 1_000_000_000 };
        private static readonly string[] coinAddLabels = { "+100K", "+1M", "+10M", "+1B" };
        private static readonly string[] coinSubLabels = { "-100K", "-1M", "-10M", "-1B" };
        private static readonly int[] goldAddAmounts = { 100, 1_000, 10_000, 100_000 };
        private static readonly string[] goldAddLabels = { "+100", "+1K", "+10K", "+100K" };
        private static readonly string[] goldSubLabels = { "-100", "-1K", "-10K", "-100K" };

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Family Data")) { showingFamily = true; showingFinances = false; }
            if (GUILayout.Button("Finances")) { showingFinances = true; showingFamily = false; }
            if (GUILayout.Button("Current Family")) { showingFamily = false; showingFinances = false; currentSubCategory = "Member_now"; currentSubName = "Current Family"; Refresh(); }
            if (GUILayout.Button("Branch Family")) { showingFamily = false; showingFinances = false; currentSubCategory = "Member_Ci"; currentSubName = "Branch Family"; Refresh(); }
            if (GUILayout.Button("Refresh")) { if (!showingFamily && !showingFinances) Refresh(); }
            GUILayout.EndHorizontal();

            if (showingFamily)
            {
                DrawFamilyData();
                return;
            }

            if (showingFinances)
            {
                DrawFinances();
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(60));
            string newSearch = GUILayout.TextField(searchText, GUILayout.Width(150));
            if (newSearch != searchText) searchText = newSearch;
            GUILayout.EndHorizontal();

            string[] displayNames = memberNames;
            if (!string.IsNullOrEmpty(searchText))
                displayNames = memberNames.Where(n => n.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();

            GUILayout.Label($"{currentSubName}: {memberList.Count} members", GUI.skin.box);
            scrollMember = GUILayout.BeginScrollView(scrollMember, GUILayout.Height(120));
            int displaySel = -1;
            if (selectedMemberIndex >= 0 && selectedMemberIndex < memberNames.Length)
                displaySel = Array.IndexOf(displayNames, memberNames[selectedMemberIndex]);
            int newSel = GUILayout.SelectionGrid(displaySel, displayNames, 1);
            if (newSel >= 0 && newSel < displayNames.Length && newSel != displaySel)
            {
                string chosen = displayNames[newSel];
                int realIdx = Array.IndexOf(memberNames, chosen);
                if (realIdx >= 0) selectedMemberIndex = realIdx;
            }
            GUILayout.EndScrollView();

            if (selectedMemberIndex >= 0 && selectedMemberIndex < memberList.Count)
            {
                var member = memberList[selectedMemberIndex];
                string displayName = ClanMemberData.GetMemberName(member);
                GUILayout.Label($"Editing: {displayName}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

                if (GUILayout.Button("Dump Raw Data to Log"))
                    YuanLogger.LogInfo($"Raw ({displayName}): {string.Join(" | ", member.Select((v, i) => $"[{i}]={v}"))}");

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Max All (100)")) MaxAllStats(member, 100);
                if (GUILayout.Button("Boost +10")) BoostAllStats(member, 10);
                GUILayout.EndHorizontal();

                scrollEditor = GUILayout.BeginScrollView(scrollEditor, GUILayout.Height(580));
                DrawNameAge(member);
                DrawCompositeEditor(member);
                DrawPersonalityEditor(member);
                DrawStatGroup(member, "Core Talents", ClanMemberData.UpperStats, 100);
                DrawStatGroup(member, "Other Attributes", ClanMemberData.LowerStats, 100, true);
                DrawClanLeader(member);

                // --- NEW SECTIONS ---
                if (member.Count > ClanMemberData.IDX_STATUS)
                    DrawStatusEditor(member);
                if (member.Count > ClanMemberData.IDX_MARRIAGE)
                    DrawMarriageEditor(member);
                if (member.Count > ClanMemberData.IDX_PREGNANCY)
                    DrawPregnancyEditor(member, ClanMemberData.IDX_PREGNANCY);
                if (member.Count > ClanMemberData.IDX_SCHOLARSHIP)
                    DrawScholarshipEditor(member);
                if (member.Count > ClanMemberData.IDX_FIEF_TITLE)
                    DrawFiefEditor(member);
                if (member.Count > ClanMemberData.IDX_TRAITS)
                    DrawTraitsEditor(member);
                if (member.Count > ClanMemberData.IDX_CLAN_DUTY)
                    DrawClanDutyEditor(member);
                if (member.Count > ClanMemberData.IDX_STUDY_SCHOOL)
                    DrawStudySchoolEditor(member);

                // --- EXTRA INTERNAL DATA SECTIONS ---
                DrawInternalDataSection("Appearance", member, ClanMemberData.IDX_APPEARANCE);
                DrawInternalDataSection("Children IDs", member, ClanMemberData.IDX_CHILD_IDS);
                DrawInternalDataSection("Estate / School", member, ClanMemberData.IDX_ESTATE);
                DrawIntFieldSection("Status Duration", member, ClanMemberData.IDX_STATUS_DURATION);
                DrawIntFieldSection("Book Progress", member, ClanMemberData.IDX_BOOK_PROGRESS);
                DrawInternalDataSection("Recent Events", member, ClanMemberData.IDX_RECENT_EVENTS);
                DrawInternalDataSection("Basic Stat Gain", member, ClanMemberData.IDX_BASIC_STAT_GAIN);
                DrawInternalDataSection("School Values", member, ClanMemberData.IDX_SCHOOL_VALUES);
                DrawIntFieldSection("Preg. Cooldown", member, ClanMemberData.IDX_PREGNANCY_COOLDOWN);
                DrawLongTextField("Biography", member, ClanMemberData.IDX_BIOGRAPHY);

                DrawRankEditor(member);
                GUILayout.EndScrollView();

                if (GUILayout.Button("Exile Member"))
                {
                    memberList.RemoveAt(selectedMemberIndex);
                    ApplyChanges();
                    selectedMemberIndex = -1;
                    Refresh();
                }
            }
            else
            {
                GUILayout.Label("Select a member above.");
            }
        }

        // ===================== FAMILY DATA =====================
        private static void DrawFamilyData()
        {
            scrollEditor = GUILayout.BeginScrollView(scrollEditor, GUILayout.Height(580));
            GUILayout.Label("── Family Data ──", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            var familyData = ClanMemberData.GetFamilyData();
            int maxIndex = Mathf.Min(familyData.Count - 1, 6);
            for (int i = 0; i <= maxIndex; i++)
            {
                string label = familyDataLabels.ContainsKey(i) ? familyDataLabels[i] : $"Index {i}";
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{label}:", GUILayout.Width(140));
                string newVal = GUILayout.TextField(familyData[i], GUILayout.Width(200));
                if (newVal != familyData[i]) { familyData[i] = newVal; ClanMemberData.SetFamilyData(familyData); }
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(10);
            GUILayout.Label("── Treasure ──", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            var cgNum = ClanMemberData.GetCGNum();
            while (cgNum.Count < 3) cgNum.Add("0");
            long underBillions = 0, billions = 0;
            long.TryParse(cgNum[0], out underBillions);
            long.TryParse(cgNum[2], out billions);
            long totalCoins = (billions * 1_000_000_000L) + underBillions;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Coins:", GUILayout.Width(140));
            string coinsStr = GUILayout.TextField(totalCoins.ToString(), GUILayout.Width(200));
            if (long.TryParse(coinsStr, out long newTotal) && newTotal >= 0 && newTotal != totalCoins)
            {
                cgNum[2] = (newTotal / 1_000_000_000L).ToString();
                cgNum[0] = (newTotal % 1_000_000_000L).ToString();
                ClanMemberData.SetCGNum(cgNum);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(140));
            for (int i = 0; i < coinAddAmounts.Length; i++)
                if (GUILayout.Button(coinAddLabels[i], GUILayout.Width(55)))
                {
                    totalCoins += coinAddAmounts[i];
                    cgNum[2] = (totalCoins / 1_000_000_000L).ToString();
                    cgNum[0] = (totalCoins % 1_000_000_000L).ToString();
                    ClanMemberData.SetCGNum(cgNum);
                }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(140));
            for (int i = 0; i < coinAddAmounts.Length; i++)
                if (GUILayout.Button(coinSubLabels[i], GUILayout.Width(55)))
                {
                    totalCoins = Math.Max(0, totalCoins - coinAddAmounts[i]);
                    cgNum[2] = (totalCoins / 1_000_000_000L).ToString();
                    cgNum[0] = (totalCoins % 1_000_000_000L).ToString();
                    ClanMemberData.SetCGNum(cgNum);
                }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            int goldBars = 0;
            int.TryParse(cgNum[1], out goldBars);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Gold Bars:", GUILayout.Width(140));
            string goldStr = GUILayout.TextField(goldBars.ToString(), GUILayout.Width(200));
            if (int.TryParse(goldStr, out int newGold) && newGold >= 0 && newGold != goldBars)
            {
                cgNum[1] = newGold.ToString();
                ClanMemberData.SetCGNum(cgNum);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(140));
            for (int i = 0; i < goldAddAmounts.Length; i++)
                if (GUILayout.Button(goldAddLabels[i], GUILayout.Width(55)))
                    if (int.TryParse(cgNum[1], out int curGold))
                    {
                        cgNum[1] = (curGold + goldAddAmounts[i]).ToString();
                        ClanMemberData.SetCGNum(cgNum);
                    }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(140));
            for (int i = 0; i < goldAddAmounts.Length; i++)
                if (GUILayout.Button(goldSubLabels[i], GUILayout.Width(55)))
                    if (int.TryParse(cgNum[1], out int curGold))
                    {
                        curGold = Math.Max(0, curGold - goldAddAmounts[i]);
                        cgNum[1] = curGold.ToString();
                        ClanMemberData.SetCGNum(cgNum);
                    }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        // ===================== FINANCES =====================
        private static void DrawFinances()
        {
            scrollEditor = GUILayout.BeginScrollView(scrollEditor, GUILayout.Height(580));

            GUILayout.Label("── Income (ZhangMu) ──", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            var zhiZeData = ClanMemberData.GetZhiZeData_ZhangMu();
            if (zhiZeData != null)
            {
                string[] zhiZeLabels = { "Auto‑Purchase", "Social", "Entertainment", "Trade", null, null, null, null, null, null };
                for (int i = 0; i < zhiZeData.Count; i++)
                {
                    if (i >= zhiZeLabels.Length || zhiZeLabels[i] == null) continue;
                    string label = zhiZeLabels[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{label}:", GUILayout.Width(110));
                    string val = GUILayout.TextField(zhiZeData[i], GUILayout.Width(200));
                    if (val != zhiZeData[i]) { zhiZeData[i] = val; ClanMemberData.SetZhiZeData_ZhangMu(zhiZeData); }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("── Spending (PerData) ──", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            var perData = ClanMemberData.GetPerData();
            if (perData != null)
            {
                string[] perLabels = { "Vassal Tax", "Land Tax", null, null, null, null, null, null, null, null };
                for (int i = 0; i < perData.Count; i++)
                {
                    if (i >= perLabels.Length || perLabels[i] == null) continue;
                    string label = perLabels[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{label}:", GUILayout.Width(110));
                    string val = GUILayout.TextField(perData[i], GUILayout.Width(200));
                    if (val != perData[i]) { perData[i] = val; ClanMemberData.SetPerData(perData); }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
        }

        // ===================== MEMBER EDITOR =====================
        private static void Refresh()
        {
            scrollMember = Vector2.zero;
            scrollEditor = Vector2.zero;
            memberList = ClanMemberData.GetMemberList(currentSubCategory);
            BuildMemberNames();
        }

        private static void BuildMemberNames()
        {
            memberNames = new string[memberList.Count];
            for (int i = 0; i < memberList.Count; i++)
            {
                var m = memberList[i];
                string name = ClanMemberData.GetMemberName(m);
                int age = ClanMemberData.GetAge(m);
                string ageStr = age >= 0 ? $" (Age {age})" : "";
                memberNames[i] = $"{i}. {name}{ageStr}";
            }
        }

        private static void DrawNameAge(List<string> member)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GUILayout.Width(50));
            string name = GUILayout.TextField(ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_NAME), GUILayout.Width(120));
            if (name != ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_NAME))
            { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_NAME, name); ApplyChanges(); }
            GUILayout.Label("Age:", GUILayout.Width(35));
            string ageStr = GUILayout.TextField(member[ClanMemberData.IDX_AGE], GUILayout.Width(40));
            if (ageStr != member[ClanMemberData.IDX_AGE] && int.TryParse(ageStr, out int na))
            { member[ClanMemberData.IDX_AGE] = na.ToString(); ApplyChanges(); }
            if (GUILayout.Button("-1")) ChangeAge(member, -1);
            if (GUILayout.Button("+1")) ChangeAge(member, +1);
            if (GUILayout.Button("18")) SetAge(member, 18);
            if (GUILayout.Button("30")) SetAge(member, 30);
            GUILayout.EndHorizontal();
        }

        private static void DrawCompositeEditor(List<string> member)
        {
            GUILayout.Label("Basic Info", GUI.skin.label);
            // Gender
            GUILayout.BeginHorizontal();
            GUILayout.Label("Gender:", GUILayout.Width(60));
            string genderStr = ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_GENDER);
            int.TryParse(genderStr, out int gender);
            string genderLabel = ClanMemberData.GenderOptions.ContainsKey(gender) ? ClanMemberData.GenderOptions[gender] : "?";
            GUILayout.Label(genderLabel, GUILayout.Width(60));
            if (GUILayout.Button("Male")) { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_GENDER, "1"); ApplyChanges(); }
            if (GUILayout.Button("Female")) { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_GENDER, "0"); ApplyChanges(); }
            GUILayout.EndHorizontal();
            // Talent Type
            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent:", GUILayout.Width(60));
            string talentStr = ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_TALENT_TYPE);
            int.TryParse(talentStr, out int talent);
            string talentLabel = ClanMemberData.TalentTypeOptions.ContainsKey(talent) ? ClanMemberData.TalentTypeOptions[talent] : "?";
            GUILayout.Label(talentLabel, GUILayout.Width(70));
            foreach (var opt in ClanMemberData.TalentTypeOptions)
                if (GUILayout.Button(opt.Value))
                { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_TALENT_TYPE, opt.Key.ToString()); ApplyChanges(); }
            GUILayout.EndHorizontal();
            // Talent Value
            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent Value:", GUILayout.Width(90));
            string tvStr = GUILayout.TextField(ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_TALENT_VALUE), GUILayout.Width(40));
            if (tvStr != ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_TALENT_VALUE))
            { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_TALENT_VALUE, tvStr); ApplyChanges(); }
            if (GUILayout.Button("MAX"))
            { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_TALENT_VALUE, "100"); ApplyChanges(); }
            GUILayout.EndHorizontal();
            // Skill Type
            GUILayout.BeginHorizontal();
            GUILayout.Label("Skill:", GUILayout.Width(60));
            string skillStr = ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_SKILL_TYPE);
            int.TryParse(skillStr, out int skill);
            string skillLabel = ClanMemberData.SkillTypeOptions.ContainsKey(skill) ? ClanMemberData.SkillTypeOptions[skill] : "?";
            GUILayout.Label(skillLabel, GUILayout.Width(90));
            foreach (var opt in ClanMemberData.SkillTypeOptions)
                if (GUILayout.Button(opt.Value))
                { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_SKILL_TYPE, opt.Key.ToString()); ApplyChanges(); }
            GUILayout.EndHorizontal();
            // Skill Value
            int skValIdx = ClanMemberData.IDX_SKILL_VALUE;
            if (skValIdx < member.Count)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Skill Value:", GUILayout.Width(90));
                string skValStr = GUILayout.TextField(member[skValIdx], GUILayout.Width(50));
                if (skValStr != member[skValIdx] && int.TryParse(skValStr, out int newSkVal))
                { member[skValIdx] = newSkVal.ToString(); ApplyChanges(); }
                if (GUILayout.Button("MAX"))
                { member[skValIdx] = "100"; ApplyChanges(); }
                GUILayout.EndHorizontal();
            }
            // Hobby
            DrawHobbyEditor(member);
            // Luck
            GUILayout.BeginHorizontal();
            GUILayout.Label("Luck:", GUILayout.Width(60));
            string luckStr = GUILayout.TextField(ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_LUCK), GUILayout.Width(40));
            if (luckStr != ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_LUCK) && int.TryParse(luckStr, out int newLuck))
            { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_LUCK, newLuck.ToString()); ApplyChanges(); }
            if (GUILayout.Button("Max"))
            { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_LUCK, "100"); ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawHobbyEditor(List<string> member)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Hobby:", GUILayout.Width(60));
            string hobbyStr = ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_HOBBY);
            int.TryParse(hobbyStr, out int hobby);
            string hobbyLabel = ClanMemberData.HobbyOptions.ContainsKey(hobby) ? ClanMemberData.HobbyOptions[hobby] : "?";
            GUILayout.Label(hobbyLabel, GUILayout.Width(80));
            for (int i = 0; i < 5; i++)
            {
                var opt = ClanMemberData.HobbyOptions.ElementAt(i);
                if (GUILayout.Button(opt.Value))
                { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_HOBBY, opt.Key.ToString()); ApplyChanges(); }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(60));
            for (int i = 5; i < ClanMemberData.HobbyOptions.Count; i++)
            {
                var opt = ClanMemberData.HobbyOptions.ElementAt(i);
                if (GUILayout.Button(opt.Value))
                { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_HOBBY, opt.Key.ToString()); ApplyChanges(); }
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawPersonalityEditor(List<string> member)
        {
            GUILayout.Label("Personality", GUI.skin.label);
            int idx = ClanMemberData.IDX_PERSONALITY;
            if (idx >= member.Count) return;
            int.TryParse(member[idx], out int currentPers);
            string currentLabel = ClanMemberData.PersonalityOptions.ContainsKey(currentPers) ? ClanMemberData.PersonalityOptions[currentPers] : "?";
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Current: {currentLabel}", GUILayout.Width(120));
            for (int i = 0; i < 8; i++)
            {
                var opt = ClanMemberData.PersonalityOptions.ElementAt(i);
                if (GUILayout.Button(opt.Value))
                { member[idx] = opt.Key.ToString(); ApplyChanges(); }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(120));
            for (int i = 8; i < ClanMemberData.PersonalityOptions.Count; i++)
            {
                var opt = ClanMemberData.PersonalityOptions.ElementAt(i);
                if (GUILayout.Button(opt.Value))
                { member[idx] = opt.Key.ToString(); ApplyChanges(); }
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawStatGroup(List<string> member, string title, List<int> indices, int maxValue, bool handleRenownAsInt = false)
        {
            GUILayout.Label($"--- {title} ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            foreach (int idx in indices)
            {
                if (idx >= member.Count) continue;
                string label = ClanMemberData.MainStats.ContainsKey(idx) ? ClanMemberData.MainStats[idx] : $"Attr {idx}";
                string valStr = member[idx];

                if (float.TryParse(valStr, out float fv))
                    valStr = ((int)fv).ToString();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"{label}:", GUILayout.Width(90));
                string newVal = GUILayout.TextField(valStr, GUILayout.Width(50));
                if (newVal != valStr)
                {
                    if (int.TryParse(newVal, out int iVal))
                        member[idx] = iVal.ToString();
                    else
                        member[idx] = newVal;
                    ApplyChanges();
                }
                if (int.TryParse(valStr, out int cur))
                {
                    if (GUILayout.Button("-")) { member[idx] = Mathf.Max(0, cur - 1).ToString(); ApplyChanges(); }
                    if (GUILayout.Button("+")) { member[idx] = (cur + 1).ToString(); ApplyChanges(); }
                    if (GUILayout.Button($"MAX({maxValue})")) { member[idx] = maxValue.ToString(); ApplyChanges(); }
                }
                GUILayout.EndHorizontal();
            }
        }

        // ===================== NEW SECTIONS =====================
        private static void DrawStatusEditor(List<string> member)
        {
            GUILayout.Label("--- Status ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            int idx = ClanMemberData.IDX_STATUS;
            int.TryParse(member[idx], out int curr);
            string currLabel = ClanMemberData.StatusOptions.ContainsKey(curr) ? ClanMemberData.StatusOptions[curr] : "?";
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Current: {currLabel}", GUILayout.Width(130));
            if (GUILayout.Button("Available")) { member[idx] = "0"; ApplyChanges(); }
            if (GUILayout.Button("Official")) { member[idx] = "16"; ApplyChanges(); }
            if (GUILayout.Button("Travelling")) { member[idx] = "11"; ApplyChanges(); }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Custom ID:", GUILayout.Width(80));
            string val = GUILayout.TextField(member[idx], GUILayout.Width(40));
            if (val != member[idx] && int.TryParse(val, out int newVal)) { member[idx] = newVal.ToString(); ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawMarriageEditor(List<string> member)
        {
            GUILayout.Label("--- Marriage ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            int idx = ClanMemberData.IDX_MARRIAGE;
            int.TryParse(member[idx], out int curr);
            string currLabel = ClanMemberData.MarriageOptions.ContainsKey(curr) ? ClanMemberData.MarriageOptions[curr] : "?";
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Current: {currLabel}", GUILayout.Width(120));
            foreach (var opt in ClanMemberData.MarriageOptions)
                if (GUILayout.Button(opt.Value)) { member[idx] = opt.Key.ToString(); ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawPregnancyEditor(List<string> member, int idx)
        {
            GUILayout.Label("--- Pregnancy ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            string val = member[idx];
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Month: {val}", GUILayout.Width(100));
            if (GUILayout.Button("Not Pregnant")) { member[idx] = "-1"; ApplyChanges(); }
            if (GUILayout.Button("Pregnant (9mo)")) { member[idx] = "9"; ApplyChanges(); }
            if (GUILayout.Button("Give Birth (0)")) { member[idx] = "0"; ApplyChanges(); }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Custom:", GUILayout.Width(60));
            string newVal = GUILayout.TextField(val, GUILayout.Width(40));
            if (newVal != val && int.TryParse(newVal, out int nv)) { member[idx] = nv.ToString(); ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawScholarshipEditor(List<string> member)
        {
            GUILayout.Label("--- Scholarship ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            int idx = ClanMemberData.IDX_SCHOLARSHIP;
            int.TryParse(member[idx], out int curr);
            string currLabel = ClanMemberData.ScholarshipTitles.ContainsKey(curr) ? ClanMemberData.ScholarshipTitles[curr] : "?";
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Title: {currLabel}", GUILayout.Width(130));
            if (GUILayout.Button("Zhuangyuan")) { member[idx] = "9"; ApplyChanges(); }
            if (GUILayout.Button("Jinshi")) { member[idx] = "6"; ApplyChanges(); }
            if (GUILayout.Button("None")) { member[idx] = "0"; ApplyChanges(); }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Custom ID:", GUILayout.Width(80));
            string newVal = GUILayout.TextField(member[idx], GUILayout.Width(40));
            if (newVal != member[idx] && int.TryParse(newVal, out int nv)) { member[idx] = nv.ToString(); ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawFiefEditor(List<string> member)
        {
            GUILayout.Label("--- Fief Title ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            int idx = ClanMemberData.IDX_FIEF_TITLE;
            string raw = member[idx];
            var parts = raw.Split('|');
            string level = parts.Length > 0 ? parts[0] : "0";
            string prov = parts.Length > 1 ? parts[1] : "0";
            int.TryParse(level, out int lvl);
            string lvlLabel = ClanMemberData.FiefLevels.ContainsKey(lvl) ? ClanMemberData.FiefLevels[lvl] : "?";
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Lvl: {lvlLabel}, Prov: {prov}", GUILayout.Width(200));
            foreach (var opt in ClanMemberData.FiefLevels)
                if (GUILayout.Button(opt.Value)) { member[idx] = opt.Key + "|" + prov; ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawTraitsEditor(List<string> member)
        {
            GUILayout.Label("--- Traits ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            int idx = ClanMemberData.IDX_TRAITS;
            string traits = member[idx];
            GUILayout.Label($"Current: {(traits == "null" ? "None" : traits)}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Prodigy")) { member[idx] = "4@-1"; ApplyChanges(); }
            if (GUILayout.Button("Noble")) { member[idx] = "5@-1"; ApplyChanges(); }
            if (GUILayout.Button("Tireless")) { member[idx] = "18@-1"; ApplyChanges(); }
            if (GUILayout.Button("Remove All")) { member[idx] = "null"; ApplyChanges(); }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Edit:", GUILayout.Width(40));
            string newTraits = GUILayout.TextField(traits, GUILayout.Width(200));
            if (newTraits != traits) { member[idx] = newTraits; ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawClanDutyEditor(List<string> member)
        {
            GUILayout.Label("--- Clan Duty ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            int idx = ClanMemberData.IDX_CLAN_DUTY;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Duty:", GUILayout.Width(50));
            string val = GUILayout.TextField(member[idx], GUILayout.Width(200));
            if (val != member[idx]) { member[idx] = val; ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawStudySchoolEditor(List<string> member)
        {
            GUILayout.Label("--- Study School ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            int idx = ClanMemberData.IDX_STUDY_SCHOOL;
            int.TryParse(member[idx], out int curr);
            string currLabel = ClanMemberData.StudySchools.ContainsKey(curr) ? ClanMemberData.StudySchools[curr] : "?";
            GUILayout.BeginHorizontal();
            GUILayout.Label($"School: {currLabel}", GUILayout.Width(120));
            foreach (var opt in ClanMemberData.StudySchools)
                if (GUILayout.Button(opt.Value)) { member[idx] = opt.Key.ToString(); ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        // --- EXTRA INTERNAL DATA SECTIONS ---
        private static void DrawInternalDataSection(string label, List<string> member, int idx)
        {
            if (idx >= member.Count) return;
            GUILayout.Label($"--- {label} ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(100));
            string val = GUILayout.TextField(member[idx], GUILayout.Width(300));
            if (val != member[idx]) { member[idx] = val; ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawIntFieldSection(string label, List<string> member, int idx)
        {
            if (idx >= member.Count) return;
            GUILayout.Label($"--- {label} ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(100));
            string val = GUILayout.TextField(member[idx], GUILayout.Width(60));
            if (val != member[idx] && int.TryParse(val, out int iVal)) { member[idx] = iVal.ToString(); ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawLongTextField(string label, List<string> member, int idx)
        {
            if (idx >= member.Count) return;
            GUILayout.Label($"--- {label} ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(100));
            string val = GUILayout.TextField(member[idx], GUILayout.Width(300));
            if (val != member[idx]) { member[idx] = val; ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        // ===================== RANK EDITOR =====================
        private static void DrawRankEditor(List<string> member)
        {
            GUILayout.Label("--- Official Rank & Office ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            int rankIdx = RankManager.FindRankIndex(member);
            if (rankIdx >= 0)
            {
                string rankStr = member[rankIdx];
                var parts = rankStr.Split('@');
                GUILayout.BeginHorizontal();
                GUILayout.Label("Rank:"); string r = GUILayout.TextField(parts.Length > 0 ? parts[0] : "0", GUILayout.Width(30));
                GUILayout.Label("Mil:"); string mil = GUILayout.TextField(parts.Length > 1 ? parts[1] : "0", GUILayout.Width(30));
                GUILayout.Label("Pol:"); string pol = GUILayout.TextField(parts.Length > 2 ? parts[2] : "0", GUILayout.Width(30));
                GUILayout.EndHorizontal();
                string newRank = $"{r}@{mil}@{pol}";
                if (newRank != rankStr) { member[rankIdx] = newRank; ApplyChanges(); }
                GUILayout.Label("Presets:", GUI.skin.label);
                foreach (var category in RankManager.Presets)
                {
                    GUILayout.Label(category.CategoryName, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                    DrawButtonFlow(category.Presets, preset =>
                    {
                        if (GUILayout.Button(preset.Label, GUILayout.MaxWidth(180)))
                        {
                            member[rankIdx] = preset.Code;
                            ApplyChanges();
                        }
                    });
                    GUILayout.Space(4);
                }
            }
            else GUILayout.Label("No rank field found.");
        }

        private static void DrawButtonFlow<T>(List<T> items, Action<T> drawItem)
        {
            int perRow = 4;
            for (int i = 0; i < items.Count; i += perRow)
            {
                GUILayout.BeginHorizontal();
                for (int j = i; j < i + perRow && j < items.Count; j++)
                    drawItem(items[j]);
                GUILayout.EndHorizontal();
            }
        }

        // ===================== CHEAT METHODS =====================
        private static void MaxAllStats(List<string> member, int maxVal)
        {
            foreach (int idx in ClanMemberData.MainStats.Keys)
                if (idx < member.Count) member[idx] = maxVal.ToString();
            ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_LUCK, maxVal.ToString());
            ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_TALENT_VALUE, maxVal.ToString());
            if (ClanMemberData.IDX_PERSONALITY < member.Count) member[ClanMemberData.IDX_PERSONALITY] = "6";
            ApplyChanges();
        }

        private static void BoostAllStats(List<string> member, int amount)
        {
            foreach (int idx in ClanMemberData.MainStats.Keys)
            {
                if (idx >= member.Count) continue;
                int cur = 0;
                if (idx == ClanMemberData.IDX_RENOWN)
                {
                    if (float.TryParse(member[idx], out float f)) cur = (int)f;
                }
                else int.TryParse(member[idx], out cur);
                member[idx] = Mathf.Clamp(cur + amount, 0, 100).ToString();
            }
            ApplyChanges();
        }

        private static void ChangeAge(List<string> member, int delta)
        {
            int idx = ClanMemberData.IDX_AGE;
            if (idx < member.Count && int.TryParse(member[idx], out int a))
                member[idx] = Math.Max(0, a + delta).ToString();
            ApplyChanges();
        }

        private static void SetAge(List<string> member, int newAge)
        {
            int idx = ClanMemberData.IDX_AGE;
            if (idx < member.Count) member[idx] = newAge.ToString();
            ApplyChanges();
        }

        private static void ChangeStat(List<string> member, int idx, int delta, int maxValue, bool isIntRenown = false)
        {
            if (idx >= member.Count) return;
            int curValue = 0;
            string raw = member[idx];
            if (isIntRenown && idx == ClanMemberData.IDX_RENOWN)
            {
                if (float.TryParse(raw, out float f)) curValue = (int)f;
                else return;
            }
            else { if (!int.TryParse(raw, out curValue)) return; }
            member[idx] = Mathf.Clamp(curValue + delta, 0, maxValue).ToString();
            ApplyChanges();
        }

        private static void SetStat(List<string> member, int idx, int value, bool isIntRenown = false)
        {
            if (idx >= member.Count) return;
            member[idx] = value.ToString();
            ApplyChanges();
        }

        private static void DrawClanLeader(List<string> member)
        {
            int idx = ClanMemberData.IDX_CLAN_LEADER;
            if (idx >= member.Count) return;
            bool isClanLeader = member[idx] == "1";
            bool newVal = GUILayout.Toggle(isClanLeader, "Clan Leader (0 = member, 1 = leader)");
            if (newVal != isClanLeader)
            {
                member[idx] = newVal ? "1" : "0";
                ApplyChanges();
            }
        }

        private static void ApplyChanges()
        {
            ClanMemberData.SetMemberList(currentSubCategory, memberList);
        }
    }
}