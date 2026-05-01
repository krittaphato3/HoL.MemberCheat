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

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Royal Members")) { memberMode = true; selectedIndex = -1; }
            if (GUILayout.Button("Royal Spouses")) { memberMode = false; selectedIndex = -1; }
            GUILayout.EndHorizontal();

            var list = memberMode ? members : spouses;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = GUILayout.TextField(searchText, GUILayout.Width(120));
            if (GUILayout.Button("Clear")) searchText = "";
            GUILayout.EndHorizontal();

            var filtered = list.Select((m, i) => new { m, i }).Where(x => string.IsNullOrEmpty(searchText) || RoyalData.GetMemberName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

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
            GUILayout.Label($"Editing: {displayName}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Max All (100)")) MaxAll(member);
            if (GUILayout.Button("Boost +10")) BoostAll(member);
            GUILayout.EndHorizontal();

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            // Name & Age
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GUILayout.Width(50));
            string nameField = GUILayout.TextField(RoyalData.GetCompositeSub(member, RoyalData.SUB_NAME), GUILayout.Width(120));
            if (nameField != RoyalData.GetCompositeSub(member, RoyalData.SUB_NAME))
            {
                RoyalData.SetCompositeSub(member, RoyalData.SUB_NAME, nameField);
            }
            GUILayout.Label("Age:", GUILayout.Width(35));
            string ageStr = GUILayout.TextField(member[3], GUILayout.Width(40));
            if (ageStr != member[3] && int.TryParse(ageStr, out int na))
            {
                member[3] = na.ToString();
            }
            if (GUILayout.Button("-1")) { int.TryParse(member[3], out int a); member[3] = Math.Max(0, a - 1).ToString(); }
            if (GUILayout.Button("+1")) { int.TryParse(member[3], out int a); member[3] = (a + 1).ToString(); }
            GUILayout.EndHorizontal();

            // Composite editor
            DrawCompositeEditor(member);

            // Stats
            DrawStats(member);

            // Rank editor
            DrawRankEditor(member);

            GUILayout.EndScrollView();
        }

        private static void DrawCompositeEditor(List<string> member)
        {
            GUILayout.Label("Basic Info", GUI.skin.label);

            // Gender
            GUILayout.BeginHorizontal();
            GUILayout.Label("Gender:", GUILayout.Width(60));
            string genderStr = RoyalData.GetCompositeSub(member, RoyalData.SUB_GENDER);
            int.TryParse(genderStr, out int gender);
            string genderLabel = gender == 0 ? "Female" : (gender == 1 ? "Male" : "?");
            GUILayout.Label(genderLabel, GUILayout.Width(60));
            if (GUILayout.Button("Male")) RoyalData.SetCompositeSub(member, RoyalData.SUB_GENDER, "1");
            if (GUILayout.Button("Female")) RoyalData.SetCompositeSub(member, RoyalData.SUB_GENDER, "0");
            GUILayout.EndHorizontal();

            // Talent Type & Value
            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent:", GUILayout.Width(60));
            string talentStr = RoyalData.GetCompositeSub(member, RoyalData.SUB_TALENT_TYPE);
            int.TryParse(talentStr, out int talent);
            string talentLabel = ClanData.TalentTypeOptions.ContainsKey(talent) ? ClanData.TalentTypeOptions[talent] : "?";
            GUILayout.Label(talentLabel, GUILayout.Width(70));
            foreach (var opt in ClanData.TalentTypeOptions)
                if (GUILayout.Button(opt.Value)) RoyalData.SetCompositeSub(member, RoyalData.SUB_TALENT_TYPE, opt.Key.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent Value:", GUILayout.Width(90));
            string tvStr = GUILayout.TextField(RoyalData.GetCompositeSub(member, RoyalData.SUB_TALENT_VALUE), GUILayout.Width(40));
            if (tvStr != RoyalData.GetCompositeSub(member, RoyalData.SUB_TALENT_VALUE))
                RoyalData.SetCompositeSub(member, RoyalData.SUB_TALENT_VALUE, tvStr);
            if (GUILayout.Button("MAX")) RoyalData.SetCompositeSub(member, RoyalData.SUB_TALENT_VALUE, "100");
            GUILayout.EndHorizontal();

            // Skill Type
            GUILayout.BeginHorizontal();
            GUILayout.Label("Skill:", GUILayout.Width(60));
            string skillStr = RoyalData.GetCompositeSub(member, RoyalData.SUB_SKILL_TYPE);
            int.TryParse(skillStr, out int skill);
            string skillLabel = ClanData.SkillTypeOptions.ContainsKey(skill) ? ClanData.SkillTypeOptions[skill] : "?";
            GUILayout.Label(skillLabel, GUILayout.Width(90));
            foreach (var opt in ClanData.SkillTypeOptions)
                if (GUILayout.Button(opt.Value)) RoyalData.SetCompositeSub(member, RoyalData.SUB_SKILL_TYPE, opt.Key.ToString());
            GUILayout.EndHorizontal();

            // Luck
            GUILayout.BeginHorizontal();
            GUILayout.Label("Luck:", GUILayout.Width(60));
            string luckStr = GUILayout.TextField(RoyalData.GetCompositeSub(member, RoyalData.SUB_LUCK), GUILayout.Width(40));
            if (luckStr != RoyalData.GetCompositeSub(member, RoyalData.SUB_LUCK) && int.TryParse(luckStr, out int newLuck))
                RoyalData.SetCompositeSub(member, RoyalData.SUB_LUCK, newLuck.ToString());
            if (GUILayout.Button("100")) RoyalData.SetCompositeSub(member, RoyalData.SUB_LUCK, "100");
            GUILayout.EndHorizontal();

            // Personality
            GUILayout.Label("Personality:", GUI.skin.label);
            int.TryParse(RoyalData.GetCompositeSub(member, RoyalData.SUB_PERSONALITY), out int currPers);
            string persLabel = ClanData.PersonalityOptions.ContainsKey(currPers) ? ClanData.PersonalityOptions[currPers] : "?";
            GUILayout.Label($"Current: {persLabel}");
            for (int i = 0; i < 8; i++)
            {
                var opt = ClanData.PersonalityOptions.ElementAt(i);
                if (GUILayout.Button(opt.Value)) RoyalData.SetCompositeSub(member, RoyalData.SUB_PERSONALITY, opt.Key.ToString());
            }
            GUILayout.BeginHorizontal();
            for (int i = 8; i < ClanData.PersonalityOptions.Count; i++)
            {
                var opt = ClanData.PersonalityOptions.ElementAt(i);
                if (GUILayout.Button(opt.Value)) RoyalData.SetCompositeSub(member, RoyalData.SUB_PERSONALITY, opt.Key.ToString());
            }
            GUILayout.EndHorizontal();

            // Hobby
            GUILayout.BeginHorizontal();
            GUILayout.Label("Hobby:", GUILayout.Width(60));
            string hobbyStr = RoyalData.GetCompositeSub(member, RoyalData.SUB_HOBBY);
            int.TryParse(hobbyStr, out int hobby);
            string hobbyLabel = ClanData.HobbyOptions.ContainsKey(hobby) ? ClanData.HobbyOptions[hobby] : "?";
            GUILayout.Label(hobbyLabel, GUILayout.Width(80));
            foreach (var opt in ClanData.HobbyOptions.Take(5))
                if (GUILayout.Button(opt.Value)) RoyalData.SetCompositeSub(member, RoyalData.SUB_HOBBY, opt.Key.ToString());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(60));
            foreach (var opt in ClanData.HobbyOptions.Skip(5))
                if (GUILayout.Button(opt.Value)) RoyalData.SetCompositeSub(member, RoyalData.SUB_HOBBY, opt.Key.ToString());
            GUILayout.EndHorizontal();
        }

        private static void DrawStats(List<string> member)
        {
            GUILayout.Label("--- Stats ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });

            // Stats 4-7
            string[] labels = { "Writing", "Might", "Business", "Arts" };
            for (int i = 4; i <= 7 && i < member.Count; i++)
            {
                string valStr = member[i];
                float.TryParse(valStr, out float fv);
                int displayVal = Mathf.RoundToInt(fv);
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{labels[i - 4]}:", GUILayout.Width(90));
                string newVal = GUILayout.TextField(displayVal.ToString(), GUILayout.Width(60));
                if (int.TryParse(newVal, out int intVal) && intVal != displayVal)
                {
                    member[i] = intVal.ToString();
                }
                if (GUILayout.Button("-")) { member[i] = Math.Max(0, displayVal - 1).ToString(); }
                if (GUILayout.Button("+")) { member[i] = (displayVal + 1).ToString(); }
                if (GUILayout.Button("MAX 100")) { member[i] = "100"; }
                GUILayout.EndHorizontal();
            }

            // Mood (8)
            if (member.Count > 8)
            {
                string moodStr = member[8];
                float.TryParse(moodStr, out float moodFloat);
                int mood = Mathf.RoundToInt(moodFloat);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Mood:", GUILayout.Width(90));
                string moodNew = GUILayout.TextField(mood.ToString(), GUILayout.Width(60));
                if (int.TryParse(moodNew, out int mVal) && mVal != mood) member[8] = mVal.ToString();
                if (GUILayout.Button("-")) { member[8] = Math.Max(0, mood - 1).ToString(); }
                if (GUILayout.Button("+")) { member[8] = (mood + 1).ToString(); }
                if (GUILayout.Button("MAX 100")) { member[8] = "100"; }
                GUILayout.EndHorizontal();
            }

            // Renown (16)
            if (member.Count > 16)
            {
                string renStr = member[16];
                float.TryParse(renStr, out float renFloat);
                int renown = Mathf.RoundToInt(renFloat);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Renown:", GUILayout.Width(90));
                string renNew = GUILayout.TextField(renown.ToString(), GUILayout.Width(60));
                if (int.TryParse(renNew, out int rVal) && rVal != renown) member[16] = rVal.ToString();
                if (GUILayout.Button("-")) { member[16] = Math.Max(0, renown - 1).ToString(); }
                if (GUILayout.Button("+")) { member[16] = (renown + 1).ToString(); }
                if (GUILayout.Button("MAX 100")) { member[16] = "100"; }
                GUILayout.EndHorizontal();
            }

            // Health (19)
            if (member.Count > 19)
            {
                string hpStr = member[19];
                float.TryParse(hpStr, out float hpFloat);
                int health = Mathf.RoundToInt(hpFloat);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Health:", GUILayout.Width(90));
                string hpNew = GUILayout.TextField(health.ToString(), GUILayout.Width(60));
                if (int.TryParse(hpNew, out int hVal) && hVal != health) member[19] = hVal.ToString();
                if (GUILayout.Button("-")) { member[19] = Math.Max(0, health - 1).ToString(); }
                if (GUILayout.Button("+")) { member[19] = (health + 1).ToString(); }
                if (GUILayout.Button("MAX 100")) { member[19] = "100"; }
                GUILayout.EndHorizontal();
            }
        }

        private static void DrawRankEditor(List<string> member)
        {
            for (int i = 0; i < member.Count; i++)
            {
                if (member[i].Contains("@") && member[i].Count(c => c == '@') >= 2)
                {
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
                        GUILayout.Label(cat.CategoryName, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                        for (int j = 0; j < cat.Presets.Count; j += 4)
                        {
                            GUILayout.BeginHorizontal();
                            for (int k = j; k < j + 4 && k < cat.Presets.Count; k++)
                            {
                                var preset = cat.Presets[k];
                                if (GUILayout.Button(preset.Label, GUILayout.MaxWidth(180)))
                                {
                                    member[i] = preset.Code;
                                }
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
            for (int i = 4; i <= 7 && i < member.Count; i++)
            {
                float.TryParse(member[i], out float f);
                int v = Mathf.Clamp(Mathf.RoundToInt(f) + 10, 0, 100);
                member[i] = v.ToString();
            }
            if (member.Count > 8) { float.TryParse(member[8], out float f); member[8] = Mathf.Clamp(Mathf.RoundToInt(f) + 10, 0, 100).ToString(); }
            if (member.Count > 16) { float.TryParse(member[16], out float f); member[16] = Mathf.Clamp(Mathf.RoundToInt(f) + 10, 0, 100).ToString(); }
            if (member.Count > 19) { float.TryParse(member[19], out float f); member[19] = Mathf.Clamp(Mathf.RoundToInt(f) + 10, 0, 100).ToString(); }
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