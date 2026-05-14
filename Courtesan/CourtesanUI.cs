using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;
using HoLMod.MemberCheat.ClanMember;

namespace HoLMod.MemberCheat.Courtesan
{
    public static class CourtesanUI
    {
        private static List<List<List<string>>> allCities;
        private static List<CourtesanEntry> flatList = new List<CourtesanEntry>();
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        private class CourtesanEntry
        {
            public int CityIndex;
            public int MemberIndex;
            public List<string> Data;
            public string DisplayName;
        }

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (allCities == null) return;

            GUILayout.Label($"Courtesans ({flatList.Count})", GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = GUILayout.TextField(searchText, GUILayout.Width(120));
            if (GUILayout.Button("Clear")) searchText = "";
            GUILayout.EndHorizontal();

            var filtered = string.IsNullOrEmpty(searchText)
                ? flatList
                : flatList.Where(e => e.DisplayName.ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int i = 0; i < filtered.Count; i++)
            {
                var entry = filtered[i];
                if (GUILayout.Button($"[City{entry.CityIndex} #{entry.MemberIndex}] {entry.DisplayName}"))
                    selectedIndex = flatList.IndexOf(entry);
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < flatList.Count)
            {
                var member = flatList[selectedIndex].Data;
                DrawMemberEdit(member);
            }
        }

        private static void DrawMemberEdit(List<string> member)
        {
            string name = ClanMemberData.GetMemberName(member);
            int age = ClanMemberData.GetAge(member);
            GUILayout.Label($"Editing: {name} (Age {age})", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            DrawNameAge(member);
            DrawCompositeEditor(member);
            DrawPersonalityEditor(member);
            
            DrawStatGroup(member, "Stats", new List<int> { 3, 4, 5, 6, 7, 8 }, 100);
            DrawStatGroup(member, "Courtesan Stats", new List<int> { 11, 13, 14, 15, 16 }, 100);

            // Pregnancy index 17
            if (member.Count > 17)
            {
                GUILayout.Label("--- Pregnancy ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Month: {member[17]}", GUILayout.Width(100));
                if (GUILayout.Button("Not Pregnant")) { member[17] = "-1"; ApplyChanges(); }
                if (GUILayout.Button("Pregnant (9mo)")) { member[17] = "9"; ApplyChanges(); }
                if (GUILayout.Button("Give Birth (0)")) { member[17] = "0"; ApplyChanges(); }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private static void DrawNameAge(List<string> member)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GUILayout.Width(50));
            string name = GUILayout.TextField(ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_NAME), GUILayout.Width(120));
            if (name != ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_NAME))
            { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_NAME, name); ApplyChanges(); }
            GUILayout.Label("Age:", GUILayout.Width(35));
            string ageStr = GUILayout.TextField(member.Count > ClanMemberData.IDX_AGE ? member[ClanMemberData.IDX_AGE] : "0", GUILayout.Width(40));
            if (int.TryParse(ageStr, out int na) && (member.Count <= ClanMemberData.IDX_AGE || ageStr != member[ClanMemberData.IDX_AGE]))
            { 
                while(member.Count <= ClanMemberData.IDX_AGE) member.Add("0");
                member[ClanMemberData.IDX_AGE] = na.ToString(); 
                ApplyChanges(); 
            }
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
            string genderStr = ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_GENDER);
            int.TryParse(genderStr, out int gender);
            GUILayout.Label(ClanMemberData.GenderOptions.ContainsKey(gender) ? ClanMemberData.GenderOptions[gender] : "?", GUILayout.Width(60));
            if (GUILayout.Button("Male")) { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_GENDER, "1"); ApplyChanges(); }
            if (GUILayout.Button("Female")) { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_GENDER, "0"); ApplyChanges(); }
            GUILayout.EndHorizontal();

            // Talent Type
            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent:", GUILayout.Width(60));
            string talentStr = ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_TALENT_TYPE);
            int.TryParse(talentStr, out int talent);
            GUILayout.Label(ClanMemberData.TalentTypeOptions.ContainsKey(talent) ? ClanMemberData.TalentTypeOptions[talent] : "?", GUILayout.Width(70));
            foreach (var opt in ClanMemberData.TalentTypeOptions)
                if (GUILayout.Button(opt.Value)) { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_TALENT_TYPE, opt.Key.ToString()); ApplyChanges(); }
            GUILayout.EndHorizontal();

            // Talent Value
            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent Val:", GUILayout.Width(90));
            string tvStr = GUILayout.TextField(ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_TALENT_VALUE), GUILayout.Width(40));
            if (tvStr != ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_TALENT_VALUE))
            { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_TALENT_VALUE, tvStr); ApplyChanges(); }
            if (GUILayout.Button("MAX")) { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_TALENT_VALUE, "100"); ApplyChanges(); }
            GUILayout.EndHorizontal();

            // Skill Type
            GUILayout.BeginHorizontal();
            GUILayout.Label("Skill:", GUILayout.Width(60));
            string skillStr = ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_SKILL_TYPE);
            int.TryParse(skillStr, out int skill);
            GUILayout.Label(ClanMemberData.SkillTypeOptions.ContainsKey(skill) ? ClanMemberData.SkillTypeOptions[skill] : "?", GUILayout.Width(90));
            foreach (var opt in ClanMemberData.SkillTypeOptions)
                if (GUILayout.Button(opt.Value)) { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_SKILL_TYPE, opt.Key.ToString()); ApplyChanges(); }
            GUILayout.EndHorizontal();

            // Hobby
            GUILayout.BeginHorizontal();
            GUILayout.Label("Hobby:", GUILayout.Width(60));
            string hobbyStr = ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_HOBBY);
            int.TryParse(hobbyStr, out int hobby);
            GUILayout.Label(ClanMemberData.HobbyOptions.ContainsKey(hobby) ? ClanMemberData.HobbyOptions[hobby] : "?", GUILayout.Width(80));
            foreach (var opt in ClanMemberData.HobbyOptions)
                if (GUILayout.Button(opt.Value)) { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_HOBBY, opt.Key.ToString()); ApplyChanges(); }
            GUILayout.EndHorizontal();

            // Luck
            GUILayout.BeginHorizontal();
            GUILayout.Label("Luck:", GUILayout.Width(60));
            string luckStr = GUILayout.TextField(ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_LUCK), GUILayout.Width(40));
            if (luckStr != ClanMemberData.GetCompositeSub(member, ClanMemberData.SUB_LUCK))
            { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_LUCK, luckStr); ApplyChanges(); }
            if (GUILayout.Button("MAX")) { ClanMemberData.SetCompositeSub(member, ClanMemberData.SUB_LUCK, "100"); ApplyChanges(); }
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
            foreach (var opt in ClanMemberData.PersonalityOptions)
                if (GUILayout.Button(opt.Value)) { member[idx] = opt.Key.ToString(); ApplyChanges(); }
            GUILayout.EndHorizontal();
        }

        private static void DrawStatGroup(List<string> member, string title, List<int> indices, int maxValue)
        {
            GUILayout.Label($"--- {title} ---", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            foreach (int idx in indices)
            {
                if (idx >= member.Count) continue;
                string label = ClanMemberData.MainStats.ContainsKey(idx) ? ClanMemberData.MainStats[idx] : $"Stat {idx}";
                string rawVal = member[idx];
                int curVal = 0;
                if (float.TryParse(rawVal, out float fv)) curVal = Mathf.RoundToInt(fv);
                else int.TryParse(rawVal, out curVal);

                GUILayout.BeginHorizontal();
                GUILayout.Label($"{label}:", GUILayout.Width(90));
                string newValStr = GUILayout.TextField(curVal.ToString(), GUILayout.Width(50));
                if (int.TryParse(newValStr, out int nv) && nv != curVal)
                {
                    member[idx] = nv.ToString();
                    ApplyChanges();
                }
                if (GUILayout.Button("-")) { member[idx] = Mathf.Max(0, curVal - 1).ToString(); ApplyChanges(); }
                if (GUILayout.Button("+")) { member[idx] = (curVal + 1).ToString(); ApplyChanges(); }
                if (GUILayout.Button("MAX")) { member[idx] = maxValue.ToString(); ApplyChanges(); }
                GUILayout.EndHorizontal();
            }
        }

        private static void ChangeAge(List<string> member, int delta)
        {
            int idx = ClanMemberData.IDX_AGE;
            if (idx < member.Count && int.TryParse(member[idx], out int a))
                member[idx] = Math.Max(0, a + delta).ToString();
            ApplyChanges();
        }

        private static void ApplyChanges()
        {
            // Courtesans are in cities, need to push back to allCities
            // Since we are editing a List<string> reference from allCities, 
            // we just need to call the data provider to save.
            CourtesanData.SetCourtesans(allCities);
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            allCities = CourtesanData.GetCourtesans();
            flatList.Clear();
            for (int c = 0; c < allCities.Count; c++)
            {
                for (int m = 0; m < allCities[c].Count; m++)
                {
                    var member = allCities[c][m];
                    flatList.Add(new CourtesanEntry
                    {
                        CityIndex = c,
                        MemberIndex = m,
                        Data = member,
                        DisplayName = $"{ClanMemberData.GetMemberName(member)} (Age {ClanMemberData.GetAge(member)})"
                    });
                }
            }
            selectedIndex = -1;
        }
    }
}
