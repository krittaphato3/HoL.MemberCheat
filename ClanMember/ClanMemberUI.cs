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

            if (showingFamily) { DrawFamilyData(); return; }
            if (showingFinances) { DrawFinances(); return; }

            UIHelpers.SearchBar(ref searchText);

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
                GUILayout.Label($"Editing: {displayName}", UIHelpers.BoldLabel);

                if (GUILayout.Button("Dump Raw Data to Log"))
                    YuanLogger.LogInfo($"Raw ({displayName}): {string.Join(" | ", member.Select((v, i) => $"[{i}]={v}"))}");

                UIHelpers.ActionButtons(
                    ("Max All (100)", () => MaxAllStats(member, 100)),
                    ("Boost +10", () => BoostAllStats(member, 10))
                );

                scrollEditor = GUILayout.BeginScrollView(scrollEditor, GUILayout.Height(580));
                DrawNameAge(member);
                DrawCompositeEditor(member);
                DrawPersonalityEditor(member);
                DrawStatGroup(member, "Core Talents", ClanMemberData.UpperStats, 100);
                DrawStatGroup(member, "Other Attributes", ClanMemberData.LowerStats, 100, true);
                DrawClanLeader(member);

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

                DrawExtraInternalFields(member);
                DrawRankEditor(member);
                GUILayout.EndScrollView();

                UIHelpers.DangerButton("Exile Member", () =>
                {
                    memberList.RemoveAt(selectedMemberIndex);
                    ApplyChanges();
                    selectedMemberIndex = -1;
                    Refresh();
                });
            }
            else
            {
                GUILayout.Label("Select a member above.");
            }
        }

        private static void DrawFamilyData()
        {
            scrollEditor = GUILayout.BeginScrollView(scrollEditor, GUILayout.Height(580));
            UIHelpers.Section("Family Data");
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
            UIHelpers.Section("Treasure");
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

        private static void DrawFinances()
        {
            scrollEditor = GUILayout.BeginScrollView(scrollEditor, GUILayout.Height(580));

            UIHelpers.Section("Income (ZhangMu)");
            var zhiZeData = ClanMemberData.GetZhiZeData_ZhangMu();
            if (zhiZeData != null)
            {
                string[] zhiZeLabels = { "Auto-Purchase", "Social", "Entertainment", "Trade" };
                for (int i = 0; i < zhiZeData.Count && i < zhiZeLabels.Length; i++)
                {
                    if (zhiZeLabels[i] == null) continue;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{zhiZeLabels[i]}:", GUILayout.Width(110));
                    string val = GUILayout.TextField(UIHelpers.GetDisplayValue(zhiZeData[i]), GUILayout.Width(200));
                    if (val != zhiZeData[i]) { zhiZeData[i] = val; ClanMemberData.SetZhiZeData_ZhangMu(zhiZeData); }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(10);
            UIHelpers.Section("Spending (PerData)");
            var perData = ClanMemberData.GetPerData();
            if (perData != null)
            {
                string[] perLabels = { "Vassal Tax", "Land Tax" };
                for (int i = 0; i < perData.Count && i < perLabels.Length; i++)
                {
                    if (perLabels[i] == null) continue;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{perLabels[i]}:", GUILayout.Width(110));
                    string val = GUILayout.TextField(UIHelpers.GetDisplayValue(perData[i]), GUILayout.Width(200));
                    if (val != perData[i]) { perData[i] = val; ClanMemberData.SetPerData(perData); }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
        }

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
            UIHelpers.Section("Basic Info");

            UIHelpers.DropdownButtons("Gender",
                ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_GENDER) == "1" ? "Male" : "Female",
                ClanMemberData.GenderOptions, key =>
                { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_GENDER, key.ToString()); ApplyChanges(); });

            int.TryParse(ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_TALENT_TYPE), out int talent);
            UIHelpers.DropdownButtons("Talent",
                ClanMemberData.TalentTypeOptions.ContainsKey(talent) ? ClanMemberData.TalentTypeOptions[talent] : "?",
                ClanMemberData.TalentTypeOptions, key =>
                { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_TALENT_TYPE, key.ToString()); ApplyChanges(); },
                60, 70);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent Value:", GUILayout.Width(90));
            string tvStr = GUILayout.TextField(ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_TALENT_VALUE), GUILayout.Width(40));
            if (tvStr != ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_TALENT_VALUE))
            { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_TALENT_VALUE, tvStr); ApplyChanges(); }
            if (GUILayout.Button("MAX"))
            { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_TALENT_VALUE, "100"); ApplyChanges(); }
            GUILayout.EndHorizontal();

            int.TryParse(ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_SKILL_TYPE), out int skill);
            UIHelpers.DropdownButtons("Skill",
                ClanMemberData.SkillTypeOptions.ContainsKey(skill) ? ClanMemberData.SkillTypeOptions[skill] : "?",
                ClanMemberData.SkillTypeOptions, key =>
                { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_SKILL_TYPE, key.ToString()); ApplyChanges(); },
                60, 90);

            if (ClanMemberData.IDX_SKILL_VALUE < member.Count)
                UIHelpers.IntFieldWithButtons("Skill Value", member, ClanMemberData.IDX_SKILL_VALUE, 100, ApplyChanges);

            int.TryParse(ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_HOBBY), out int hobby);
            UIHelpers.DropdownButtonsWrapped("Hobby",
                ClanMemberData.HobbyOptions.ContainsKey(hobby) ? ClanMemberData.HobbyOptions[hobby] : "?",
                ClanMemberData.HobbyOptions, key =>
                { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_HOBBY, key.ToString()); ApplyChanges(); },
                5, 60);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Luck:", GUILayout.Width(60));
            string luckStr = GUILayout.TextField(ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_LUCK), GUILayout.Width(40));
            if (luckStr != ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_LUCK) && int.TryParse(luckStr, out int newLuck))
            { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_LUCK, newLuck.ToString()); ApplyChanges(); }
            if (GUILayout.Button("Max"))
            { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_LUCK, "100"); ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawPersonalityEditor(List<string> member)
        {
            int idx = ClanMemberData.IDX_PERSONALITY;
            if (idx >= member.Count) return;
            int.TryParse(member[idx], out int currentPers);
            string currentLabel = ClanMemberData.PersonalityOptions.ContainsKey(currentPers) ? ClanMemberData.PersonalityOptions[currentPers] : "?";
            UIHelpers.DropdownButtonsWrapped("Personality", currentLabel,
                ClanMemberData.PersonalityOptions, key =>
                { member[idx] = key.ToString(); ApplyChanges(); }, 8, 60);
        }

        private static void DrawStatGroup(List<string> member, string title, List<int> indices, int maxValue, bool handleRenownAsInt = false)
        {
            UIHelpers.Section(title);
            foreach (int idx in indices)
            {
                if (idx >= member.Count) continue;
                string label = ClanMemberData.MainStats.ContainsKey(idx) ? ClanMemberData.MainStats[idx] : $"Attr {idx}";
                UIHelpers.FloatFieldWithButtons(label, member, idx, maxValue, ApplyChanges);
            }
        }

        private static void DrawStatusEditor(List<string> member)
        {
            UIHelpers.Section("Status");
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
            UIHelpers.Section("Marriage");
            int idx = ClanMemberData.IDX_MARRIAGE;
            int.TryParse(member[idx], out int curr);
            string currLabel = ClanMemberData.MarriageOptions.ContainsKey(curr) ? ClanMemberData.MarriageOptions[curr] : "?";
            UIHelpers.DropdownButtons("Status", currLabel,
                ClanMemberData.MarriageOptions, key =>
                { member[idx] = key.ToString(); ApplyChanges(); }, 60, 120);
        }

        private static void DrawPregnancyEditor(List<string> member, int idx)
        {
            UIHelpers.Section("Pregnancy");
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
            UIHelpers.Section("Scholarship");
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
            UIHelpers.Section("Fief Title");
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
            UIHelpers.Section("Traits");
            int idx = ClanMemberData.IDX_TRAITS;
            string traits = member[idx];
            GUILayout.Label($"Current: {(traits == "null" ? "None" : traits)}");
            UIHelpers.ActionButtons(
                ("Prodigy", () => { member[idx] = "4@-1"; ApplyChanges(); }),
                ("Noble", () => { member[idx] = "5@-1"; ApplyChanges(); }),
                ("Tireless", () => { member[idx] = "18@-1"; ApplyChanges(); }),
                ("Remove All", () => { member[idx] = "null"; ApplyChanges(); })
            );
            GUILayout.BeginHorizontal();
            GUILayout.Label("Edit:", GUILayout.Width(40));
            string newTraits = GUILayout.TextField(traits, GUILayout.Width(200));
            if (newTraits != traits) { member[idx] = newTraits; ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawClanDutyEditor(List<string> member)
        {
            UIHelpers.Section("Clan Duty");
            int idx = ClanMemberData.IDX_CLAN_DUTY;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Duty:", GUILayout.Width(50));
            string val = GUILayout.TextField(member[idx], GUILayout.Width(200));
            if (val != member[idx]) { member[idx] = val; ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawStudySchoolEditor(List<string> member)
        {
            UIHelpers.Section("Study School");
            int idx = ClanMemberData.IDX_STUDY_SCHOOL;
            int.TryParse(member[idx], out int curr);
            string currLabel = ClanMemberData.StudySchools.ContainsKey(curr) ? ClanMemberData.StudySchools[curr] : "?";
            UIHelpers.DropdownButtons("School", currLabel,
                ClanMemberData.StudySchools, key =>
                { member[idx] = key.ToString(); ApplyChanges(); }, 60, 120);
        }

        private static void DrawExtraInternalFields(List<string> member)
        {
            UIHelpers.Section("Extra Data");
            UIHelpers.TextField("Appearance", member, ClanMemberData.IDX_APPEARANCE, 120, 300, ApplyChanges);
            UIHelpers.TextField("Children IDs", member, ClanMemberData.IDX_CHILD_IDS, 120, 300, ApplyChanges);
            UIHelpers.TextField("Estate / School", member, ClanMemberData.IDX_ESTATE, 120, 300, ApplyChanges);
            UIHelpers.IntField("Status Duration", member, ClanMemberData.IDX_STATUS_DURATION, 60, ApplyChanges);
            UIHelpers.IntField("Book Progress", member, ClanMemberData.IDX_BOOK_PROGRESS, 60, ApplyChanges);
            UIHelpers.TextField("Recent Events", member, ClanMemberData.IDX_RECENT_EVENTS, 120, 300, ApplyChanges);
            UIHelpers.TextField("Basic Stat Gain", member, ClanMemberData.IDX_BASIC_STAT_GAIN, 120, 300, ApplyChanges);
            UIHelpers.TextField("School Values", member, ClanMemberData.IDX_SCHOOL_VALUES, 120, 300, ApplyChanges);
            UIHelpers.IntField("Preg. Cooldown", member, ClanMemberData.IDX_PREGNANCY_COOLDOWN, 60, ApplyChanges);
            UIHelpers.TextField("Biography", member, ClanMemberData.IDX_BIOGRAPHY, 100, 300, ApplyChanges);
        }

        private static void DrawRankEditor(List<string> member)
        {
            UIHelpers.Section("Official Rank & Office");
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
                    GUILayout.Label(category.CategoryName, UIHelpers.SectionHeader);
                    for (int j = 0; j < category.Presets.Count; j += 4)
                    {
                        GUILayout.BeginHorizontal();
                        for (int k = j; k < j + 4 && k < category.Presets.Count; k++)
                        {
                            var preset = category.Presets[k];
                            if (GUILayout.Button(preset.Label, GUILayout.MaxWidth(180)))
                            { member[rankIdx] = preset.Code; ApplyChanges(); }
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Space(4);
                }
            }
            else GUILayout.Label("No rank field found.");
        }

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
                { if (float.TryParse(member[idx], out float f)) cur = (int)f; }
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
