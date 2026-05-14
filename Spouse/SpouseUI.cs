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

            UIHelpers.SearchBar(ref searchText);

            string[] displayNames = allNames;
            if (!string.IsNullOrEmpty(searchText))
                displayNames = allNames.Where(n => n.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();

            GUILayout.Label($"Spouses: {list?.Count ?? 0}", GUI.skin.box);
            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(120));
            if (displayNames.Length > 0)
            {
                int displaySel = Mathf.Clamp(Array.IndexOf(displayNames, GetCurrentMemberName()), 0, displayNames.Length - 1);
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
                GUILayout.Label($"Editing: {displayName}", UIHelpers.BoldLabel);

                UIHelpers.ActionButtons(
                    ("Max All (100)", () => MaxAll(member)),
                    ("Boost +10", () => BoostAll(member))
                );

                scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(600));
                DrawNameAge(member);
                DrawCompositeEditor(member);
                DrawStats(member);
                DrawSpecialFields(member);

                if (member.Count > SpouseData.IDX_STATUS)
                    DrawStatusEditor(member);
                if (member.Count > SpouseData.IDX_PREGNANCY)
                    DrawPregnancyEditor(member);

                DrawExtraFields(member);

                UIHelpers.DangerButton("Exile Spouse", () =>
                {
                    list.RemoveAt(selectedIndex);
                    Apply();
                    selectedIndex = -1;
                    Refresh();
                });
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
            UIHelpers.Section("Basic Info");

            UIHelpers.DropdownButtons("Gender",
                SpouseData.GetCompositeSub(member, SpouseData.SUB_GENDER) == "1" ? "Male" : "Female",
                ClanData.GenderOptions, key =>
                { SpouseData.SetCompositeSub(member, SpouseData.SUB_GENDER, key.ToString()); Apply(); });

            int.TryParse(SpouseData.GetCompositeSub(member, SpouseData.SUB_TALENT_TYPE), out int talent);
            UIHelpers.DropdownButtons("Talent",
                ClanData.TalentTypeOptions.ContainsKey(talent) ? ClanData.TalentTypeOptions[talent] : "?",
                ClanData.TalentTypeOptions, key =>
                { SpouseData.SetCompositeSub(member, SpouseData.SUB_TALENT_TYPE, key.ToString()); Apply(); },
                60, 70);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent Value:", GUILayout.Width(90));
            string tvStr = GUILayout.TextField(SpouseData.GetCompositeSub(member, SpouseData.SUB_TALENT_VALUE), GUILayout.Width(40));
            if (tvStr != SpouseData.GetCompositeSub(member, SpouseData.SUB_TALENT_VALUE)) { SpouseData.SetCompositeSub(member, SpouseData.SUB_TALENT_VALUE, tvStr); Apply(); }
            if (GUILayout.Button("MAX")) { SpouseData.SetCompositeSub(member, SpouseData.SUB_TALENT_VALUE, "100"); Apply(); }
            GUILayout.EndHorizontal();

            int.TryParse(SpouseData.GetCompositeSub(member, SpouseData.SUB_SKILL_TYPE), out int skill);
            UIHelpers.DropdownButtons("Skill",
                ClanData.SkillTypeOptions.ContainsKey(skill) ? ClanData.SkillTypeOptions[skill] : "?",
                ClanData.SkillTypeOptions, key =>
                { SpouseData.SetCompositeSub(member, SpouseData.SUB_SKILL_TYPE, key.ToString()); Apply(); },
                60, 90);

            UIHelpers.IntFieldWithButtons("Skill Points", member, SpouseData.IDX_SKILL_POINTS, 100, Apply);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Luck:", GUILayout.Width(60));
            string luckStr = GUILayout.TextField(SpouseData.GetCompositeSub(member, SpouseData.SUB_LUCK), GUILayout.Width(40));
            if (luckStr != SpouseData.GetCompositeSub(member, SpouseData.SUB_LUCK) && int.TryParse(luckStr, out int newLuck)) { SpouseData.SetCompositeSub(member, SpouseData.SUB_LUCK, newLuck.ToString()); Apply(); }
            if (GUILayout.Button("100")) { SpouseData.SetCompositeSub(member, SpouseData.SUB_LUCK, "100"); Apply(); }
            GUILayout.EndHorizontal();

            int.TryParse(SpouseData.GetCompositeSub(member, SpouseData.SUB_PERSONALITY), out int currPers);
            UIHelpers.DropdownButtonsWrapped("Personality",
                ClanData.PersonalityOptions.ContainsKey(currPers) ? ClanData.PersonalityOptions[currPers] : "?",
                ClanData.PersonalityOptions, key =>
                { SpouseData.SetCompositeSub(member, SpouseData.SUB_PERSONALITY, key.ToString()); Apply(); },
                8, 60);

            int.TryParse(SpouseData.GetCompositeSub(member, SpouseData.SUB_HOBBY), out int hobby);
            UIHelpers.DropdownButtonsWrapped("Hobby",
                ClanData.HobbyOptions.ContainsKey(hobby) ? ClanData.HobbyOptions[hobby] : "?",
                ClanData.HobbyOptions, key =>
                { SpouseData.SetCompositeSub(member, SpouseData.SUB_HOBBY, key.ToString()); Apply(); },
                5, 60);
        }

        private static void DrawStats(List<string> member)
        {
            UIHelpers.Section("Stats");
            UIHelpers.FloatFieldWithButtons("Writing", member, SpouseData.IDX_WRITING, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Might", member, SpouseData.IDX_MIGHT, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Business", member, SpouseData.IDX_BUSINESS, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Arts", member, SpouseData.IDX_ARTS, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Mood", member, SpouseData.IDX_MOOD, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Renown", member, SpouseData.IDX_RENOWN, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Charisma", member, SpouseData.IDX_CHARISMA, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Health", member, SpouseData.IDX_HEALTH, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Cunning", member, SpouseData.IDX_CUNNING, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Stamina", member, SpouseData.IDX_STAMINA, 100, Apply);
        }

        private static void DrawSpecialFields(List<string> member)
        {
            UIHelpers.Section("Special");

            if (member.Count > SpouseData.IDX_MARITAL_HARMONY)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Harmony:", GUILayout.Width(80));
                string harmony = SpouseData.GetMaritalHarmony(member);
                string newHarmony = GUILayout.TextField(harmony, GUILayout.Width(60));
                if (newHarmony != harmony) { SpouseData.SetMaritalHarmony(member, newHarmony); Apply(); }
                GUILayout.EndHorizontal();
            }

            if (member.Count > SpouseData.IDX_PREGNANCY_PROB)
                UIHelpers.IntFieldWithButtons("Preg. Prob", member, SpouseData.IDX_PREGNANCY_PROB, 100, Apply);

            if (member.Count > SpouseData.IDX_CLAN_DUTY)
                UIHelpers.TextField("Clan Duty", member, SpouseData.IDX_CLAN_DUTY, 80, 200, Apply);
        }

        private static void DrawStatusEditor(List<string> member)
        {
            UIHelpers.Section("Status");
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
            UIHelpers.Section("Pregnancy");
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

        private static void DrawExtraFields(List<string> member)
        {
            UIHelpers.Section("Extra Data");
            UIHelpers.IntField("Status Duration", member, SpouseData.IDX_STATUS_DURATION, 60, Apply);
            UIHelpers.TextField("Equipment", member, SpouseData.IDX_EQUIPMENT, 120, 300, Apply);
            UIHelpers.TextField("Recent Events", member, SpouseData.IDX_RECENT_EVENTS, 120, 300, Apply);
            UIHelpers.TextField("Children IDs", member, SpouseData.IDX_CHILD_IDS, 120, 300, Apply);
            UIHelpers.TextField("Appearance", member, SpouseData.IDX_APPEARANCE, 120, 300, Apply);
            UIHelpers.TextField("Housing", member, SpouseData.IDX_HOUSING, 120, 300, Apply);
            UIHelpers.TextField("Official Position", member, SpouseData.IDX_OFFICIAL_POS, 120, 300, Apply);
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
                    float.TryParse(member[idx], out float cur);
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
