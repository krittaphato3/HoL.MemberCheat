using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;
using ClanData = HoLMod.MemberCheat.ClanMember.ClanMemberData;

namespace HoLMod.MemberCheat.Retainer
{
    public static class RetainerUI
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

            GUILayout.Label($"Retainers: {list?.Count ?? 0}", GUI.skin.box);
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
                string displayName = RetainerData.GetName(member);
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
                if (member.Count > RetainerData.IDX_STATUS)
                    DrawStatusEditor(member);
                if (member.Count > RetainerData.IDX_PREGNANCY)
                    DrawPregnancyEditor(member);

                DrawDanger();
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Select a retainer to edit.");
            }
        }

        private static string GetCurrentMemberName()
        {
            if (selectedIndex >= 0 && selectedIndex < list?.Count)
                return $"{selectedIndex}. {RetainerData.GetName(list[selectedIndex])} (Age {RetainerData.GetAge(list[selectedIndex])})";
            return "";
        }

        // ===================== DRAW SECTIONS =====================
        private static void DrawNameAge(List<string> member)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GUILayout.Width(50));
            string nameField = GUILayout.TextField(RetainerData.GetCompositeSub(member, RetainerData.SUB_NAME), GUILayout.Width(120));
            if (nameField != RetainerData.GetCompositeSub(member, RetainerData.SUB_NAME))
            { RetainerData.SetCompositeSub(member, RetainerData.SUB_NAME, nameField); Apply(); }
            GUILayout.Label("Age:", GUILayout.Width(35));
            string ageStr = GUILayout.TextField(member[RetainerData.IDX_AGE], GUILayout.Width(40));
            if (ageStr != member[RetainerData.IDX_AGE] && int.TryParse(ageStr, out int na)) { member[RetainerData.IDX_AGE] = na.ToString(); Apply(); }
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
            string genderStr = RetainerData.GetCompositeSub(member, RetainerData.SUB_GENDER);
            int.TryParse(genderStr, out int gender);
            string genderLabel = gender == 0 ? "Female" : (gender == 1 ? "Male" : "?");
            GUILayout.Label(genderLabel, GUILayout.Width(60));
            if (GUILayout.Button("Male")) { RetainerData.SetCompositeSub(member, RetainerData.SUB_GENDER, "1"); Apply(); }
            if (GUILayout.Button("Female")) { RetainerData.SetCompositeSub(member, RetainerData.SUB_GENDER, "0"); Apply(); }
            GUILayout.EndHorizontal();

            // Talent Type & Value
            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent:", GUILayout.Width(60));
            string talentStr = RetainerData.GetCompositeSub(member, RetainerData.SUB_TALENT_TYPE);
            int.TryParse(talentStr, out int talent);
            string talentLabel = ClanData.TalentTypeOptions.ContainsKey(talent) ? ClanData.TalentTypeOptions[talent] : "?";
            GUILayout.Label(talentLabel, GUILayout.Width(70));
            foreach (var opt in ClanData.TalentTypeOptions)
                if (GUILayout.Button(opt.Value)) { RetainerData.SetCompositeSub(member, RetainerData.SUB_TALENT_TYPE, opt.Key.ToString()); Apply(); }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent Value:", GUILayout.Width(90));
            string tvStr = GUILayout.TextField(RetainerData.GetCompositeSub(member, RetainerData.SUB_TALENT_VALUE), GUILayout.Width(40));
            if (tvStr != RetainerData.GetCompositeSub(member, RetainerData.SUB_TALENT_VALUE)) { RetainerData.SetCompositeSub(member, RetainerData.SUB_TALENT_VALUE, tvStr); Apply(); }
            if (GUILayout.Button("MAX")) { RetainerData.SetCompositeSub(member, RetainerData.SUB_TALENT_VALUE, "100"); Apply(); }
            GUILayout.EndHorizontal();

            // Skill Type + Skill Points
            GUILayout.BeginHorizontal();
            GUILayout.Label("Skill:", GUILayout.Width(60));
            string skillStr = RetainerData.GetCompositeSub(member, RetainerData.SUB_SKILL_TYPE);
            int.TryParse(skillStr, out int skill);
            string skillLabel = ClanData.SkillTypeOptions.ContainsKey(skill) ? ClanData.SkillTypeOptions[skill] : "?";
            GUILayout.Label(skillLabel, GUILayout.Width(90));
            foreach (var opt in ClanData.SkillTypeOptions)
                if (GUILayout.Button(opt.Value)) { RetainerData.SetCompositeSub(member, RetainerData.SUB_SKILL_TYPE, opt.Key.ToString()); Apply(); }
            GUILayout.EndHorizontal();

            DrawIntStat(member, RetainerData.IDX_SKILL_POINTS, "Skill Points", 100);

            // Luck
            GUILayout.BeginHorizontal();
            GUILayout.Label("Luck:", GUILayout.Width(60));
            string luckStr = GUILayout.TextField(RetainerData.GetCompositeSub(member, RetainerData.SUB_LUCK), GUILayout.Width(40));
            if (luckStr != RetainerData.GetCompositeSub(member, RetainerData.SUB_LUCK) && int.TryParse(luckStr, out int newLuck)) { RetainerData.SetCompositeSub(member, RetainerData.SUB_LUCK, newLuck.ToString()); Apply(); }
            if (GUILayout.Button("100")) { RetainerData.SetCompositeSub(member, RetainerData.SUB_LUCK, "100"); Apply(); }
            GUILayout.EndHorizontal();

            // Personality
            GUILayout.Label("Personality:", GUI.skin.label);
            int.TryParse(RetainerData.GetCompositeSub(member, RetainerData.SUB_PERSONALITY), out int currPers);
            string persLabel = ClanData.PersonalityOptions.ContainsKey(currPers) ? ClanData.PersonalityOptions[currPers] : "?";
            GUILayout.Label($"Current: {persLabel}");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 8; i++)
            {
                var opt = ClanData.PersonalityOptions.ElementAt(i);
                if (GUILayout.Button(opt.Value)) { RetainerData.SetCompositeSub(member, RetainerData.SUB_PERSONALITY, opt.Key.ToString()); Apply(); }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            for (int i = 8; i < ClanData.PersonalityOptions.Count; i++)
            {
                var opt = ClanData.PersonalityOptions.ElementAt(i);
                if (GUILayout.Button(opt.Value)) { RetainerData.SetCompositeSub(member, RetainerData.SUB_PERSONALITY, opt.Key.ToString()); Apply(); }
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawStats(List<string> member)
        {
            GUILayout.Label("--- Stats ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            DrawIntStatFromFloat(member, RetainerData.IDX_WRITING, "Writing", 100);
            DrawIntStatFromFloat(member, RetainerData.IDX_MIGHT, "Might", 100);
            DrawIntStatFromFloat(member, RetainerData.IDX_BUSINESS, "Business", 100);
            DrawIntStatFromFloat(member, RetainerData.IDX_ARTS, "Arts", 100);
            DrawIntStatFromFloat(member, RetainerData.IDX_MOOD, "Mood", 100);
            DrawIntStatFromFloat(member, RetainerData.IDX_RENOWN, "Renown", 100);
            DrawIntStatFromFloat(member, RetainerData.IDX_CHARISMA, "Charisma", 100);
            DrawIntStatFromFloat(member, RetainerData.IDX_HEALTH, "Health", 100);
            DrawIntStatFromFloat(member, RetainerData.IDX_CUNNING, "Cunning", 100);
            DrawIntStatFromFloat(member, RetainerData.IDX_STAMINA, "Stamina", 100);
        }

        private static void DrawSpecialFields(List<string> member)
        {
            GUILayout.Label("--- Special ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            if (member.Count > RetainerData.IDX_SALARY)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Salary:", GUILayout.Width(80));
                string salStr = GUILayout.TextField(member[RetainerData.IDX_SALARY], GUILayout.Width(80));
                if (salStr != member[RetainerData.IDX_SALARY] && int.TryParse(salStr, out int salVal)) { member[RetainerData.IDX_SALARY] = salVal.ToString(); Apply(); }
                GUILayout.EndHorizontal();
            }
        }

        // --- NEW SECTIONS ---
        private static void DrawStatusEditor(List<string> member)
        {
            GUILayout.Label("--- Status ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            int idx = RetainerData.IDX_STATUS;
            int.TryParse(member[idx], out int curr);
            string currLabel = ClanData.StatusOptions.ContainsKey(curr) ? ClanData.StatusOptions[curr] : "?";
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Current: {currLabel}", GUILayout.Width(130));
            if (GUILayout.Button("Available")) { member[idx] = "0"; Apply(); }
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
            string val = member[RetainerData.IDX_PREGNANCY];
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Month: {val}", GUILayout.Width(100));
            if (GUILayout.Button("Not Pregnant")) { member[RetainerData.IDX_PREGNANCY] = "-1"; Apply(); }
            if (GUILayout.Button("Pregnant (9mo)")) { member[RetainerData.IDX_PREGNANCY] = "9"; Apply(); }
            if (GUILayout.Button("Give Birth (0)")) { member[RetainerData.IDX_PREGNANCY] = "0"; Apply(); }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Custom:", GUILayout.Width(60));
            string newVal = GUILayout.TextField(val, GUILayout.Width(40));
            if (newVal != val && int.TryParse(newVal, out int nv)) { member[RetainerData.IDX_PREGNANCY] = nv.ToString(); Apply(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawDanger()
        {
            if (GUILayout.Button("Dismiss Retainer"))
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
            if (member.Count > RetainerData.IDX_AGE && int.TryParse(member[RetainerData.IDX_AGE], out int a))
            { member[RetainerData.IDX_AGE] = Mathf.Max(0, a + delta).ToString(); Apply(); }
        }

        private static void MaxAll(List<string> member)
        {
            foreach (int idx in new[] { RetainerData.IDX_WRITING, RetainerData.IDX_MIGHT, RetainerData.IDX_BUSINESS, RetainerData.IDX_ARTS,
                RetainerData.IDX_MOOD, RetainerData.IDX_RENOWN, RetainerData.IDX_CHARISMA, RetainerData.IDX_HEALTH,
                RetainerData.IDX_CUNNING, RetainerData.IDX_STAMINA, RetainerData.IDX_SKILL_POINTS })
                if (idx < member.Count) member[idx] = "100";
            RetainerData.SetCompositeSub(member, RetainerData.SUB_LUCK, "100");
            RetainerData.SetCompositeSub(member, RetainerData.SUB_TALENT_VALUE, "100");
            Apply();
        }

        private static void BoostAll(List<string> member)
        {
            foreach (int idx in new[] { RetainerData.IDX_WRITING, RetainerData.IDX_MIGHT, RetainerData.IDX_BUSINESS, RetainerData.IDX_ARTS,
                RetainerData.IDX_MOOD, RetainerData.IDX_RENOWN, RetainerData.IDX_CHARISMA, RetainerData.IDX_HEALTH,
                RetainerData.IDX_CUNNING })
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
            list = RetainerData.GetList();
            allNames = new string[list?.Count ?? 0];
            for (int i = 0; i < allNames.Length; i++)
                allNames[i] = $"{i}. {RetainerData.GetName(list[i])} (Age {RetainerData.GetAge(list[i])})";
            selectedIndex = Mathf.Clamp(selectedIndex, -1, (list?.Count ?? 1) - 1);
        }

        private static void Apply()
        {
            RetainerData.SetList(list);
            Refresh();
        }
    }
}