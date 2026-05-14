using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;
using ClanData = HoLMod.MemberCheat.ClanMember.ClanMemberData;
using RankMgr = HoLMod.MemberCheat.ClanMember.RankManager;

namespace HoLMod.MemberCheat.Royal
{
    public static class RoyalUI
    {
        private static List<List<string>> members;
        private static List<List<string>> spouses;
        private static int selectedIndex = -1;
        private static bool memberMode = true;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (members == null || spouses == null) return;

            UIHelpers.ActionButtons(
                ("Royal Members", () => { memberMode = true; selectedIndex = -1; }),
                ("Royal Spouses", () => { memberMode = false; selectedIndex = -1; })
            );

            var list = memberMode ? members : spouses;

            UIHelpers.SearchBar(ref searchText);

            var filtered = list.Select((m, i) => new { m, i })
                .Where(x => string.IsNullOrEmpty(searchText) || RoyalData.GetMemberName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(200));
            for (int j = 0; j < filtered.Count; j++)
            {
                var item = filtered[j];
                string mName = RoyalData.GetMemberName(item.m);
                int age = RoyalData.GetAge(item.m);
                if (GUILayout.Button($"{item.i}: {mName} (Age {age})")) selectedIndex = item.i;
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < list.Count)
            {
                var member = list[selectedIndex];
                DrawMemberEdit(member);
            }
        }

        private static void DrawMemberEdit(List<string> member)
        {
            string displayName = RoyalData.GetMemberName(member);
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
            string nameField = GUILayout.TextField(RoyalData.GetCompositeSub(member, RoyalData.SUB_NAME), GUILayout.Width(120));
            if (nameField != RoyalData.GetCompositeSub(member, RoyalData.SUB_NAME))
                RoyalData.SetCompositeSub(member, RoyalData.SUB_NAME, nameField);
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
                RoyalData.GetCompositeSub(member, RoyalData.SUB_GENDER) == "1" ? "Male" : "Female",
                ClanData.GenderOptions, key =>
                RoyalData.SetCompositeSub(member, RoyalData.SUB_GENDER, key.ToString()));

            int.TryParse(RoyalData.GetCompositeSub(member, RoyalData.SUB_TALENT_TYPE), out int talent);
            UIHelpers.DropdownButtons("Talent",
                ClanData.TalentTypeOptions.ContainsKey(talent) ? ClanData.TalentTypeOptions[talent] : "?",
                ClanData.TalentTypeOptions, key =>
                RoyalData.SetCompositeSub(member, RoyalData.SUB_TALENT_TYPE, key.ToString()),
                60, 70);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent Value:", GUILayout.Width(90));
            string tvStr = GUILayout.TextField(RoyalData.GetCompositeSub(member, RoyalData.SUB_TALENT_VALUE), GUILayout.Width(40));
            if (tvStr != RoyalData.GetCompositeSub(member, RoyalData.SUB_TALENT_VALUE))
                RoyalData.SetCompositeSub(member, RoyalData.SUB_TALENT_VALUE, tvStr);
            if (GUILayout.Button("MAX")) RoyalData.SetCompositeSub(member, RoyalData.SUB_TALENT_VALUE, "100");
            GUILayout.EndHorizontal();

            int.TryParse(RoyalData.GetCompositeSub(member, RoyalData.SUB_SKILL_TYPE), out int skill);
            UIHelpers.DropdownButtons("Skill",
                ClanData.SkillTypeOptions.ContainsKey(skill) ? ClanData.SkillTypeOptions[skill] : "?",
                ClanData.SkillTypeOptions, key =>
                RoyalData.SetCompositeSub(member, RoyalData.SUB_SKILL_TYPE, key.ToString()),
                60, 90);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Luck:", GUILayout.Width(60));
            string luckStr = GUILayout.TextField(RoyalData.GetCompositeSub(member, RoyalData.SUB_LUCK), GUILayout.Width(40));
            if (luckStr != RoyalData.GetCompositeSub(member, RoyalData.SUB_LUCK) && int.TryParse(luckStr, out int newLuck))
                RoyalData.SetCompositeSub(member, RoyalData.SUB_LUCK, newLuck.ToString());
            if (GUILayout.Button("100")) RoyalData.SetCompositeSub(member, RoyalData.SUB_LUCK, "100");
            GUILayout.EndHorizontal();

            int.TryParse(RoyalData.GetCompositeSub(member, RoyalData.SUB_PERSONALITY), out int currPers);
            UIHelpers.DropdownButtonsWrapped("Personality",
                ClanData.PersonalityOptions.ContainsKey(currPers) ? ClanData.PersonalityOptions[currPers] : "?",
                ClanData.PersonalityOptions, key =>
                RoyalData.SetCompositeSub(member, RoyalData.SUB_PERSONALITY, key.ToString()),
                8, 60);

            int.TryParse(RoyalData.GetCompositeSub(member, RoyalData.SUB_HOBBY), out int hobby);
            UIHelpers.DropdownButtonsWrapped("Hobby",
                ClanData.HobbyOptions.ContainsKey(hobby) ? ClanData.HobbyOptions[hobby] : "?",
                ClanData.HobbyOptions, key =>
                RoyalData.SetCompositeSub(member, RoyalData.SUB_HOBBY, key.ToString()),
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
            if (member.Count > 16)
                UIHelpers.FloatFieldWithButtons("Renown", member, 16, 100);
            if (member.Count > 19)
                UIHelpers.FloatFieldWithButtons("Health", member, 19, 100);
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
            if (member.Count > 16) member[16] = "100";
            if (member.Count > 19) member[19] = "100";
            RoyalData.SetCompositeSub(member, RoyalData.SUB_LUCK, "100");
            RoyalData.SetCompositeSub(member, RoyalData.SUB_TALENT_VALUE, "100");
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
            BoostIdx(16);
            BoostIdx(19);
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            members = RoyalData.GetMembers();
            spouses = RoyalData.GetSpouses();
            selectedIndex = -1;
        }
    }
}
