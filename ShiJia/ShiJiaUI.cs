using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;
using ClanData = HoLMod.MemberCheat.ClanMember.ClanMemberData;
using RankMgr = HoLMod.MemberCheat.ClanMember.RankManager;

namespace HoLMod.MemberCheat.ShiJia
{
    public static class ShiJiaUI
    {
        private static List<List<string>> clanList;
        private static int selectedClan = -1;
        private static List<List<string>> members;
        private static List<List<string>> spouses;
        private static int selectedMember = -1;
        private static bool memberMode = true;
        private static Vector2 scrollClan, scrollMem, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (clanList == null) return;

            GUILayout.Label($"Other Clans ({clanList.Count})", GUI.skin.box);
            scrollClan = GUILayout.BeginScrollView(scrollClan, GUILayout.Height(120));
            for (int i = 0; i < clanList.Count; i++)
            {
                if (clanList[i].Count < 2) continue;
                string name = clanList[i][1];
                string alive = clanList[i][0] == "0" ? "" : " [DEAD]";
                if (GUILayout.Button($"{i}: {name}{alive}")) { selectedClan = i; LoadClanMembers(); selectedMember = -1; }
            }
            GUILayout.EndScrollView();

            if (selectedClan >= 0 && selectedClan < clanList.Count)
            {
                GUILayout.Label($"Clan: {clanList[selectedClan][1]} (Level {clanList[selectedClan][2]})", GUI.skin.box);
                UIHelpers.ActionButtons(
                    ("Members", () => { memberMode = true; selectedMember = -1; }),
                    ("Spouses", () => { memberMode = false; selectedMember = -1; })
                );

                var list = memberMode ? members : spouses;
                if (list == null) list = new List<List<string>>();

                UIHelpers.SearchBar(ref searchText);

                var filtered = list.Select((m, i) => new { m, i })
                    .Where(x => string.IsNullOrEmpty(searchText) || ShiJiaData.GetMemberName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

                scrollMem = GUILayout.BeginScrollView(scrollMem, GUILayout.Height(150));
                for (int j = 0; j < filtered.Count; j++)
                {
                    var item = filtered[j];
                    string mName = ShiJiaData.GetMemberName(item.m);
                    int age = ShiJiaData.GetAge(item.m);
                    if (GUILayout.Button($"{item.i}: {mName} (Age {age})")) selectedMember = item.i;
                }
                GUILayout.EndScrollView();

                if (selectedMember >= 0 && selectedMember < list.Count)
                {
                    var member = list[selectedMember];
                    DrawMemberEdit(member, list);
                }
            }
        }

        private static void DrawMemberEdit(List<string> member, List<List<string>> parentList)
        {
            string displayName = ShiJiaData.GetMemberName(member);
            GUILayout.Label($"Editing: {displayName}", UIHelpers.BoldLabel);

            UIHelpers.ActionButtons(
                ("Max All (100)", () => MaxAll(member)),
                ("Boost +10", () => BoostAll(member))
            );

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            DrawNameAge(member);
            DrawCompositeEditor(member);
            DrawStats(member);
            DrawRankEditor(member);

            GUILayout.EndScrollView();
        }

        private static void DrawNameAge(List<string> member)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GUILayout.Width(50));
            string nameField = GUILayout.TextField(ShiJiaData.GetCompositeSub(member, ShiJiaData.SUB_NAME), GUILayout.Width(120));
            if (nameField != ShiJiaData.GetCompositeSub(member, ShiJiaData.SUB_NAME))
                ShiJiaData.SetCompositeSub(member, ShiJiaData.SUB_NAME, nameField);
            GUILayout.Label("Age:", GUILayout.Width(35));
            string ageStr = GUILayout.TextField(member[3], GUILayout.Width(40));
            if (ageStr != member[3] && int.TryParse(ageStr, out int na))
                member[3] = na.ToString();
            if (GUILayout.Button("-1")) { int.TryParse(member[3], out int a); member[3] = Math.Max(0, a - 1).ToString(); }
            if (GUILayout.Button("+1")) { int.TryParse(member[3], out int a); member[3] = (a + 1).ToString(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawCompositeEditor(List<string> member)
        {
            UIHelpers.Section("Basic Info");

            UIHelpers.DropdownButtons("Gender",
                ShiJiaData.GetCompositeSub(member, ShiJiaData.SUB_GENDER) == "1" ? "Male" : "Female",
                ClanData.GenderOptions, key =>
                ShiJiaData.SetCompositeSub(member, ShiJiaData.SUB_GENDER, key.ToString()));

            int.TryParse(ShiJiaData.GetCompositeSub(member, ShiJiaData.SUB_TALENT_TYPE), out int talent);
            UIHelpers.DropdownButtons("Talent",
                ClanData.TalentTypeOptions.ContainsKey(talent) ? ClanData.TalentTypeOptions[talent] : "?",
                ClanData.TalentTypeOptions, key =>
                ShiJiaData.SetCompositeSub(member, ShiJiaData.SUB_TALENT_TYPE, key.ToString()),
                60, 70);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent Value:", GUILayout.Width(90));
            string tvStr = GUILayout.TextField(ShiJiaData.GetCompositeSub(member, ShiJiaData.SUB_TALENT_VALUE), GUILayout.Width(40));
            if (tvStr != ShiJiaData.GetCompositeSub(member, ShiJiaData.SUB_TALENT_VALUE))
                ShiJiaData.SetCompositeSub(member, ShiJiaData.SUB_TALENT_VALUE, tvStr);
            if (GUILayout.Button("MAX")) ShiJiaData.SetCompositeSub(member, ShiJiaData.SUB_TALENT_VALUE, "100");
            GUILayout.EndHorizontal();

            int.TryParse(ShiJiaData.GetCompositeSub(member, ShiJiaData.SUB_SKILL_TYPE), out int skill);
            UIHelpers.DropdownButtons("Skill",
                ClanData.SkillTypeOptions.ContainsKey(skill) ? ClanData.SkillTypeOptions[skill] : "?",
                ClanData.SkillTypeOptions, key =>
                ShiJiaData.SetCompositeSub(member, ShiJiaData.SUB_SKILL_TYPE, key.ToString()),
                60, 90);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Luck:", GUILayout.Width(60));
            string luckStr = GUILayout.TextField(ShiJiaData.GetCompositeSub(member, ShiJiaData.SUB_LUCK), GUILayout.Width(40));
            if (luckStr != ShiJiaData.GetCompositeSub(member, ShiJiaData.SUB_LUCK) && int.TryParse(luckStr, out int newLuck))
                ShiJiaData.SetCompositeSub(member, ShiJiaData.SUB_LUCK, newLuck.ToString());
            if (GUILayout.Button("100")) ShiJiaData.SetCompositeSub(member, ShiJiaData.SUB_LUCK, "100");
            GUILayout.EndHorizontal();

            int.TryParse(ShiJiaData.GetCompositeSub(member, ShiJiaData.SUB_PERSONALITY), out int currPers);
            UIHelpers.DropdownButtonsWrapped("Personality",
                ClanData.PersonalityOptions.ContainsKey(currPers) ? ClanData.PersonalityOptions[currPers] : "?",
                ClanData.PersonalityOptions, key =>
                ShiJiaData.SetCompositeSub(member, ShiJiaData.SUB_PERSONALITY, key.ToString()),
                8, 60);

            int.TryParse(ShiJiaData.GetCompositeSub(member, ShiJiaData.SUB_HOBBY), out int hobby);
            UIHelpers.DropdownButtonsWrapped("Hobby",
                ClanData.HobbyOptions.ContainsKey(hobby) ? ClanData.HobbyOptions[hobby] : "?",
                ClanData.HobbyOptions, key =>
                ShiJiaData.SetCompositeSub(member, ShiJiaData.SUB_HOBBY, key.ToString()),
                5, 60);
        }

        private static void DrawStats(List<string> member)
        {
            UIHelpers.Section("Stats");

            string[] labels = { "Writing", "Might", "Business", "Arts" };
            for (int i = 4; i <= 7 && i < member.Count; i++)
                UIHelpers.FloatFieldWithButtons(labels[i - 4], member, i, 100);

            if (member.Count > 8)
                UIHelpers.FloatFieldWithButtons("Mood", member, 8, 100);
            if (member.Count > 17)
                UIHelpers.FloatFieldWithButtons("Renown", member, 17, 100);
            if (member.Count > 20)
                UIHelpers.FloatFieldWithButtons("Health", member, 20, 100);
        }

        private static void DrawRankEditor(List<string> member)
        {
            for (int i = 0; i < member.Count; i++)
            {
                if (member[i].Contains("@") && member[i].Count(c => c == '@') >= 2)
                {
                    UIHelpers.Section("Rank & Office");
                    string rankStr = member[i];
                    var parts = rankStr.Split('@');
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Rank:"); string r = GUILayout.TextField(parts.Length > 0 ? parts[0] : "0", GUILayout.Width(30));
                    GUILayout.Label("Mil:"); string mil = GUILayout.TextField(parts.Length > 1 ? parts[1] : "0", GUILayout.Width(30));
                    GUILayout.Label("Pol:"); string pol = GUILayout.TextField(parts.Length > 2 ? parts[2] : "0", GUILayout.Width(30));
                    GUILayout.EndHorizontal();
                    string newRank = $"{r}@{mil}@{pol}";
                    if (newRank != rankStr) member[i] = newRank;
                    GUILayout.Label("Presets:", GUI.skin.label);
                    foreach (var cat in RankMgr.Presets)
                    {
                        GUILayout.Label(cat.CategoryName, UIHelpers.SectionHeader);
                        for (int j = 0; j < cat.Presets.Count; j += 4)
                        {
                            GUILayout.BeginHorizontal();
                            for (int k = j; k < j + 4 && k < cat.Presets.Count; k++)
                            {
                                var preset = cat.Presets[k];
                                if (GUILayout.Button(preset.Label, GUILayout.MaxWidth(180)))
                                    member[i] = preset.Code;
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    return;
                }
            }
        }

        private static void MaxAll(List<string> member)
        {
            for (int i = 4; i <= 7 && i < member.Count; i++) member[i] = "100";
            if (member.Count > 8) member[8] = "100";
            if (member.Count > 17) member[17] = "100";
            if (member.Count > 20) member[20] = "100";
            ShiJiaData.SetCompositeSub(member, ShiJiaData.SUB_LUCK, "100");
            ShiJiaData.SetCompositeSub(member, ShiJiaData.SUB_TALENT_VALUE, "100");
        }

        private static void BoostAll(List<string> member)
        {
            void BoostIdx(int i)
            {
                if (i < member.Count)
                {
                    float.TryParse(member[i], out float f);
                    member[i] = Mathf.Clamp(Mathf.RoundToInt(f) + 10, 0, 100).ToString();
                }
            }
            for (int i = 4; i <= 7; i++) BoostIdx(i);
            BoostIdx(8);
            BoostIdx(17);
            BoostIdx(20);
        }

        private static void Refresh()
        {
            scrollClan = Vector2.zero;
            scrollMem = Vector2.zero;
            scrollEdit = Vector2.zero;
            clanList = ShiJiaData.GetClanList();
            selectedClan = -1;
            selectedMember = -1;
            members = null;
            spouses = null;
        }

        private static void LoadClanMembers()
        {
            members = ShiJiaData.GetMembers(selectedClan);
            spouses = ShiJiaData.GetSpouses(selectedClan);
        }
    }
}
