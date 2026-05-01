using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;
using ClanData = HoLMod.MemberCheat.ClanMember.ClanMemberData;

namespace HoLMod.MemberCheat.Spouse
{
    public static class SpouseUI
    {
        private static int selectedIndex = -1;
        private static List<List<string>> list;
        private static string[] allNames;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(60));
            string newSearch = GUILayout.TextField(searchText, GUILayout.Width(150));
            if (newSearch != searchText) searchText = newSearch;
            if (GUILayout.Button("Refresh")) Refresh();
            GUILayout.EndHorizontal();

            string[] displayNames = allNames;
            if (!string.IsNullOrEmpty(searchText))
                displayNames = allNames.Where(n => n.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();

            GUILayout.Label($"Spouses: {list?.Count ?? 0}", GUI.skin.box);
            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(120));
            if (displayNames.Length > 0)
            {
                int displaySel = Mathf.Clamp(displayNames.ToList().IndexOf(GetCurrentMemberName()), 0, displayNames.Length - 1);
                int newSel = GUILayout.SelectionGrid(displaySel, displayNames, 1);
                if (newSel != displaySel && newSel >= 0 && newSel < displayNames.Length)
                {
                    string selectedName = displayNames[newSel];
                    int realIdx = Array.IndexOf(allNames, selectedName);
                    if (realIdx >= 0) selectedIndex = realIdx;
                }
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && list != null && selectedIndex < list.Count)
            {
                var member = list[selectedIndex];
                string displayName = SpouseData.GetName(member);
                GUILayout.Label($"Editing: {displayName}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Max All (100)")) MaxAll(member);
                if (GUILayout.Button("Boost +10")) BoostAll(member);
                GUILayout.EndHorizontal();

                scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(600));
                DrawNameAge(member);
                DrawCompositeEditor(member);
                DrawStats(member);
                DrawSpecialFields(member);

                // --- NEW SECTIONS ---
                if (member.Count > SpouseData.IDX_STATUS)
                    DrawStatusEditor(member);
                if (member.Count > SpouseData.IDX_PREGNANCY)
                    DrawPregnancyEditor(member);
                // Traits editor removed due to missing IDX_TRAITS in SpouseData

                // --- Extra missing fields ---
                DrawIntFieldSection("Status Duration", member, SpouseData.IDX_STATUS_DURATION);
                DrawInternalDataSection("Equipment", member, SpouseData.IDX_EQUIPMENT);
                DrawInternalDataSection("Recent Events", member, SpouseData.IDX_RECENT_EVENTS);
                DrawInternalDataSection("Official Pos", member, SpouseData.IDX_OFFICIAL_POS);
                DrawIntFieldSection("Unknown (27)", member, SpouseData.IDX_UNK_27);

                DrawDanger();
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Select a spouse to edit.");
            }
        }

        private static string GetCurrentMemberName()
        {
            if (selectedIndex >= 0 && selectedIndex < list?.Count)
                return $"{selectedIndex}. {SpouseData.GetName(list[selectedIndex])} (Age {SpouseData.GetAge(list[selectedIndex])})";
            return "";
        }

        // ===================== DRAW SECTIONS =====================
        private static void DrawNameAge(List<string> member)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GUILayout.Width(50));
            string nameField = GUILayout.TextField(SpouseData.GetCompositeSub(member, SpouseData.SUB_NAME), GUILayout.Width(120));
            if (nameField != SpouseData.GetCompositeSub(member, SpouseData.SUB_NAME))
            { SpouseData.SetCompositeSub(member, SpouseData.SUB_NAME, nameField); Apply(); }
            GUILayout.Label("Age:", GUILayout.Width(35));
            string ageStr = GUILayout.TextField(member[SpouseData.IDX_AGE], GUILayout.Width(40));
            if (ageStr != member[SpouseData.IDX_AGE] && int.TryParse(ageStr, out int na)) { member[SpouseData.IDX_AGE] = na.ToString(); Apply(); }
            if (GUILayout.Button("-1")) ChangeAge(member, -1);
            if (GUILayout.Button("+1")) ChangeAge(member, +1);
            GUILayout.EndHorizontal();
        }

        private static void DrawCompositeEditor(List<string> member)
        {
            GUILayout.Label("Basic Info", GUI.skin.label);

            // Gender
            GUILayout.BeginHorizontal();
            GUILayout.Label("Gender:", GUILayout.Width(60));
            string genderStr = SpouseData.GetCompositeSub(member, SpouseData.SUB_GENDER);
            int.TryParse(genderStr, out int gender);
            string genderLabel = gender == 0 ? "Female" : (gender == 1 ? "Male" : "?");
            GUILayout.Label(genderLabel, GUILayout.Width(60));
            if (GUILayout.Button("Male")) { SpouseData.SetCompositeSub(member, SpouseData.SUB_GENDER, "1"); Apply(); }
            if (GUILayout.Button("Female")) { SpouseData.SetCompositeSub(member, SpouseData.SUB_GENDER, "0"); Apply(); }
            GUILayout.EndHorizontal();

            // Talent Type & Value
            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent:", GUILayout.Width(60));
            string talentStr = SpouseData.GetCompositeSub(member, SpouseData.SUB_TALENT_TYPE);
            int.TryParse(talentStr, out int talent);
            string talentLabel = ClanData.TalentTypeOptions.ContainsKey(talent) ? ClanData.TalentTypeOptions[talent] : "?";
            GUILayout.Label(talentLabel, GUILayout.Width(70));
            foreach (var opt in ClanData.TalentTypeOptions)
                if (GUILayout.Button(opt.Value)) { SpouseData.SetCompositeSub(member, SpouseData.SUB_TALENT_TYPE, opt.Key.ToString()); Apply(); }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent Value:", GUILayout.Width(90));
            string tvStr = GUILayout.TextField(SpouseData.GetCompositeSub(member, SpouseData.SUB_TALENT_VALUE), GUILayout.Width(40));
            if (tvStr != SpouseData.GetCompositeSub(member, SpouseData.SUB_TALENT_VALUE)) { SpouseData.SetCompositeSub(member, SpouseData.SUB_TALENT_VALUE, tvStr); Apply(); }
            if (GUILayout.Button("MAX")) { SpouseData.SetCompositeSub(member, SpouseData.SUB_TALENT_VALUE, "100"); Apply(); }
            GUILayout.EndHorizontal();

            // Skill Type + Skill Points
            GUILayout.BeginHorizontal();
            GUILayout.Label("Skill:", GUILayout.Width(60));
            string skillStr = SpouseData.GetCompositeSub(member, SpouseData.SUB_SKILL_TYPE);
            int.TryParse(skillStr, out int skill);
            string skillLabel = ClanData.SkillTypeOptions.ContainsKey(skill) ? ClanData.SkillTypeOptions[skill] : "?";
            GUILayout.Label(skillLabel, GUILayout.Width(90));
            foreach (var opt in ClanData.SkillTypeOptions)
                if (GUILayout.Button(opt.Value)) { SpouseData.SetCompositeSub(member, SpouseData.SUB_SKILL_TYPE, opt.Key.ToString()); Apply(); }
            GUILayout.EndHorizontal();

            DrawIntStat(member, SpouseData.IDX_SKILL_POINTS, "Skill Points", 100);

            // Luck
            GUILayout.BeginHorizontal();
            GUILayout.Label("Luck:", GUILayout.Width(60));
            string luckStr = GUILayout.TextField(SpouseData.GetCompositeSub(member, SpouseData.SUB_LUCK), GUILayout.Width(40));
            if (luckStr != SpouseData.GetCompositeSub(member, SpouseData.SUB_LUCK) && int.TryParse(luckStr, out int newLuck)) { SpouseData.SetCompositeSub(member, SpouseData.SUB_LUCK, newLuck.ToString()); Apply(); }
            if (GUILayout.Button("100")) { SpouseData.SetCompositeSub(member, SpouseData.SUB_LUCK, "100"); Apply(); }
            GUILayout.EndHorizontal();

            // Personality
            GUILayout.Label("Personality:", GUI.skin.label);
            int.TryParse(SpouseData.GetCompositeSub(member, SpouseData.SUB_PERSONALITY), out int currPers);
            string persLabel = ClanData.PersonalityOptions.ContainsKey(currPers) ? ClanData.PersonalityOptions[currPers] : "?";
            GUILayout.Label($"Current: {persLabel}");
            for (int i = 0; i < 8; i++)
            {
                var opt = ClanData.PersonalityOptions.ElementAt(i);
                if (GUILayout.Button(opt.Value)) { SpouseData.SetCompositeSub(member, SpouseData.SUB_PERSONALITY, opt.Key.ToString()); Apply(); }
            }
            GUILayout.BeginHorizontal();
            for (int i = 8; i < ClanData.PersonalityOptions.Count; i++)
            {
                var opt = ClanData.PersonalityOptions.ElementAt(i);
                if (GUILayout.Button(opt.Value)) { SpouseData.SetCompositeSub(member, SpouseData.SUB_PERSONALITY, opt.Key.ToString()); Apply(); }
            }
            GUILayout.EndHorizontal();

            // Hobby
            GUILayout.BeginHorizontal();
            GUILayout.Label("Hobby:", GUILayout.Width(60));
            string hobbyStr = SpouseData.GetCompositeSub(member, SpouseData.SUB_HOBBY);
            int.TryParse(hobbyStr, out int hobby);
            string hobbyLabel = ClanData.HobbyOptions.ContainsKey(hobby) ? ClanData.HobbyOptions[hobby] : "?";
            GUILayout.Label(hobbyLabel, GUILayout.Width(80));
            foreach (var opt in ClanData.HobbyOptions.Take(5))
                if (GUILayout.Button(opt.Value)) { SpouseData.SetCompositeSub(member, SpouseData.SUB_HOBBY, opt.Key.ToString()); Apply(); }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(60));
            foreach (var opt in ClanData.HobbyOptions.Skip(5))
                if (GUILayout.Button(opt.Value)) { SpouseData.SetCompositeSub(member, SpouseData.SUB_HOBBY, opt.Key.ToString()); Apply(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawStats(List<string> member)
        {
            GUILayout.Label("--- Stats ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            DrawIntStatFromFloat(member, SpouseData.IDX_WRITING, "Writing", 100);
            DrawIntStatFromFloat(member, SpouseData.IDX_MIGHT, "Might", 100);
            DrawIntStatFromFloat(member, SpouseData.IDX_BUSINESS, "Business", 100);
            DrawIntStatFromFloat(member, SpouseData.IDX_ARTS, "Arts", 100);
            DrawIntStatFromFloat(member, SpouseData.IDX_MOOD, "Mood", 100);
            DrawIntStatFromFloat(member, SpouseData.IDX_RENOWN, "Renown", 100);
            DrawIntStatFromFloat(member, SpouseData.IDX_CHARISMA, "Charisma", 100);
            DrawIntStatFromFloat(member, SpouseData.IDX_HEALTH, "Health", 100);
            DrawIntStatFromFloat(member, SpouseData.IDX_CUNNING, "Cunning", 100);
            DrawIntStatFromFloat(member, SpouseData.IDX_STAMINA, "Stamina", 100);
        }

        private static void DrawSpecialFields(List<string> member)
        {
            GUILayout.Label("--- Special ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });

            // Marital Harmony
            if (member.Count > SpouseData.IDX_MARITAL_HARMONY)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Harmony:", GUILayout.Width(80));
                string harmony = SpouseData.GetMaritalHarmony(member);
                string newHarmony = GUILayout.TextField(harmony, GUILayout.Width(60));
                if (newHarmony != harmony) { SpouseData.SetMaritalHarmony(member, newHarmony); Apply(); }
                GUILayout.EndHorizontal();
            }

            // Pregnancy Probability
            if (member.Count > SpouseData.IDX_PREGNANCY_PROB)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Preg. Prob:", GUILayout.Width(80));
                string probStr = GUILayout.TextField(member[SpouseData.IDX_PREGNANCY_PROB], GUILayout.Width(40));
                if (probStr != member[SpouseData.IDX_PREGNANCY_PROB] && int.TryParse(probStr, out int prob)) { member[SpouseData.IDX_PREGNANCY_PROB] = prob.ToString(); Apply(); }
                GUILayout.EndHorizontal();
            }
        }

        // --- NEW SECTIONS ---
        private static void DrawStatusEditor(List<string> member)
        {
            GUILayout.Label("--- Status ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            int idx = SpouseData.IDX_STATUS;
            int.TryParse(member[idx], out int curr);
            string currLabel = ClanData.StatusOptions.ContainsKey(curr) ? ClanData.StatusOptions[curr] : "?";
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Current: {currLabel}", GUILayout.Width(130));
            if (GUILayout.Button("Available")) { member[idx] = "0"; Apply(); }
            if (GUILayout.Button("Official")) { member[idx] = "16"; Apply(); }
            if (GUILayout.Button("Travelling")) { member[idx] = "11"; Apply(); }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Custom ID:", GUILayout.Width(80));
            string val = GUILayout.TextField(member[idx], GUILayout.Width(40));
            if (val != member[idx] && int.TryParse(val, out int newVal)) { member[idx] = newVal.ToString(); Apply(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawPregnancyEditor(List<string> member)
        {
            GUILayout.Label("--- Pregnancy ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            string val = member[SpouseData.IDX_PREGNANCY];
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Month: {val}", GUILayout.Width(100));
            if (GUILayout.Button("Not Pregnant")) { member[SpouseData.IDX_PREGNANCY] = "-1"; Apply(); }
            if (GUILayout.Button("Pregnant (9mo)")) { member[SpouseData.IDX_PREGNANCY] = "9"; Apply(); }
            if (GUILayout.Button("Give Birth (0)")) { member[SpouseData.IDX_PREGNANCY] = "0"; Apply(); }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Custom:", GUILayout.Width(60));
            string newVal = GUILayout.TextField(val, GUILayout.Width(40));
            if (newVal != val && int.TryParse(newVal, out int nv)) { member[SpouseData.IDX_PREGNANCY] = nv.ToString(); Apply(); }
            GUILayout.EndHorizontal();
        }

        // --- Extra missing fields ---
        private static void DrawInternalDataSection(string label, List<string> member, int idx)
        {
            if (idx >= member.Count) return;
            GUILayout.Label($"--- {label} ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(120));
            string val = GUILayout.TextField(member[idx], GUILayout.Width(300));
            if (val != member[idx]) { member[idx] = val; Apply(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawIntFieldSection(string label, List<string> member, int idx)
        {
            if (idx >= member.Count) return;
            GUILayout.Label($"--- {label} ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(120));
            string val = GUILayout.TextField(member[idx], GUILayout.Width(60));
            if (val != member[idx] && int.TryParse(val, out int iVal)) { member[idx] = iVal.ToString(); Apply(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawDanger()
        {
            if (GUILayout.Button("Exile Spouse"))
            {
                list.RemoveAt(selectedIndex);
                Apply();
                selectedIndex = -1;
                Refresh();
            }
        }

        // ===================== HELPERS =====================
        private static void DrawIntStat(List<string> member, int idx, string label, int maxVal)
        {
            if (idx >= member.Count) return;
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(90));
            string val = GUILayout.TextField(member[idx], GUILayout.Width(60));
            if (val != member[idx] && int.TryParse(val, out int iVal)) { member[idx] = iVal.ToString(); Apply(); }
            if (int.TryParse(member[idx], out int cur))
            {
                if (GUILayout.Button("-")) { member[idx] = Mathf.Max(0, cur - 1).ToString(); Apply(); }
                if (GUILayout.Button("+")) { member[idx] = (cur + 1).ToString(); Apply(); }
                if (GUILayout.Button($"MAX({maxVal})")) { member[idx] = maxVal.ToString(); Apply(); }
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawIntStatFromFloat(List<string> member, int idx, string label, int maxVal)
        {
            if (idx >= member.Count) return;
            float.TryParse(member[idx], out float curFloat);
            int displayVal = Mathf.RoundToInt(curFloat);
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(90));
            string newVal = GUILayout.TextField(displayVal.ToString(), GUILayout.Width(60));
            if (int.TryParse(newVal, out int newInt) && newInt != displayVal)
            {
                member[idx] = newInt.ToString();
                Apply();
            }
            float.TryParse(member[idx], out curFloat);
            int cur = Mathf.RoundToInt(curFloat);
            if (GUILayout.Button("-")) { member[idx] = Mathf.Max(0, cur - 1).ToString(); Apply(); }
            if (GUILayout.Button("+")) { member[idx] = (cur + 1).ToString(); Apply(); }
            if (GUILayout.Button($"MAX({maxVal})")) { member[idx] = maxVal.ToString(); Apply(); }
            GUILayout.EndHorizontal();
        }

        private static void ChangeAge(List<string> member, int delta)
        {
            if (member.Count > SpouseData.IDX_AGE && int.TryParse(member[SpouseData.IDX_AGE], out int a))
            { member[SpouseData.IDX_AGE] = Mathf.Max(0, a + delta).ToString(); Apply(); }
        }

        private static void MaxAll(List<string> member)
        {
            foreach (int idx in new[] { SpouseData.IDX_WRITING, SpouseData.IDX_MIGHT, SpouseData.IDX_BUSINESS, SpouseData.IDX_ARTS,
                SpouseData.IDX_MOOD, SpouseData.IDX_RENOWN, SpouseData.IDX_CHARISMA, SpouseData.IDX_HEALTH,
                SpouseData.IDX_CUNNING, SpouseData.IDX_STAMINA, SpouseData.IDX_SKILL_POINTS })
                if (idx < member.Count) member[idx] = "100";
            SpouseData.SetCompositeSub(member, SpouseData.SUB_LUCK, "100");
            SpouseData.SetCompositeSub(member, SpouseData.SUB_TALENT_VALUE, "100");
            Apply();
        }

        private static void BoostAll(List<string> member)
        {
            foreach (int idx in new[] { SpouseData.IDX_WRITING, SpouseData.IDX_MIGHT, SpouseData.IDX_BUSINESS, SpouseData.IDX_ARTS,
                SpouseData.IDX_MOOD, SpouseData.IDX_RENOWN, SpouseData.IDX_CHARISMA, SpouseData.IDX_HEALTH,
                SpouseData.IDX_CUNNING })
                if (idx < member.Count)
                {
                    float cur = 0f;
                    float.TryParse(member[idx], out cur);
                    int newVal = Mathf.Clamp(Mathf.RoundToInt(cur) + 10, 0, 100);
                    member[idx] = newVal.ToString();
                }
            Apply();
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            list = SpouseData.GetList();
            allNames = new string[list?.Count ?? 0];
            for (int i = 0; i < allNames.Length; i++)
                allNames[i] = $"{i}. {SpouseData.GetName(list[i])} (Age {SpouseData.GetAge(list[i])})";
            selectedIndex = Mathf.Clamp(selectedIndex, -1, (list?.Count ?? 1) - 1);
        }

        private static void Apply()
        {
            SpouseData.SetList(list);
            Refresh();
        }
    }
}