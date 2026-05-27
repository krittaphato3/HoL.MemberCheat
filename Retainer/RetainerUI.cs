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

            UIHelpers.SearchBar(ref searchText);

            string[] displayNames = allNames;
            if (!string.IsNullOrEmpty(searchText))
                displayNames = allNames.Where(n => n.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();

            GUILayout.Label($"Retainers: {list?.Count ?? 0}", GUI.skin.box);
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
                string displayName = RetainerData.GetName(member);
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

                if (member.Count > RetainerData.IDX_STATUS)
                    DrawStatusEditor(member);
                if (member.Count > RetainerData.IDX_PREGNANCY)
                    DrawPregnancyEditor(member);

                UIHelpers.DangerButton("Dismiss Retainer", () =>
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
                GUILayout.Label("Select a retainer to edit.");
            }
        }

        private static string GetCurrentMemberName()
        {
            if (selectedIndex >= 0 && selectedIndex < list?.Count)
                return $"{selectedIndex}. {RetainerData.GetName(list[selectedIndex])} (Age {RetainerData.GetAge(list[selectedIndex])})";
            return "";
        }

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
            UIHelpers.Section("Basic Info");

            UIHelpers.DropdownButtons("Gender",
                RetainerData.GetCompositeSub(member, RetainerData.SUB_GENDER) == "1" ? "Male" : "Female",
                ClanData.GenderOptions, key =>
                { RetainerData.SetCompositeSub(member, RetainerData.SUB_GENDER, key.ToString()); Apply(); });

            int.TryParse(RetainerData.GetCompositeSub(member, RetainerData.SUB_TALENT_TYPE), out int talent);
            UIHelpers.DropdownButtons("Talent",
                ClanData.TalentTypeOptions.ContainsKey(talent) ? ClanData.TalentTypeOptions[talent] : "?",
                ClanData.TalentTypeOptions, key =>
                { RetainerData.SetCompositeSub(member, RetainerData.SUB_TALENT_TYPE, key.ToString()); Apply(); },
                60, 70);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Talent Value:", GUILayout.Width(90));
            string tvStr = GUILayout.TextField(RetainerData.GetCompositeSub(member, RetainerData.SUB_TALENT_VALUE), GUILayout.Width(40));
            if (tvStr != RetainerData.GetCompositeSub(member, RetainerData.SUB_TALENT_VALUE)) { RetainerData.SetCompositeSub(member, RetainerData.SUB_TALENT_VALUE, tvStr); Apply(); }
            if (GUILayout.Button("MAX")) { RetainerData.SetCompositeSub(member, RetainerData.SUB_TALENT_VALUE, "100"); Apply(); }
            GUILayout.EndHorizontal();

            int.TryParse(RetainerData.GetCompositeSub(member, RetainerData.SUB_SKILL_TYPE), out int skill);
            UIHelpers.DropdownButtons("Skill",
                ClanData.SkillTypeOptions.ContainsKey(skill) ? ClanData.SkillTypeOptions[skill] : "?",
                ClanData.SkillTypeOptions, key =>
                { RetainerData.SetCompositeSub(member, RetainerData.SUB_SKILL_TYPE, key.ToString()); Apply(); },
                60, 90);

            UIHelpers.IntFieldWithButtons("Skill Points", member, RetainerData.IDX_SKILL_POINTS, 100, Apply);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Luck:", GUILayout.Width(60));
            string luckStr = GUILayout.TextField(RetainerData.GetCompositeSub(member, RetainerData.SUB_LUCK), GUILayout.Width(40));
            if (luckStr != RetainerData.GetCompositeSub(member, RetainerData.SUB_LUCK) && int.TryParse(luckStr, out int newLuck)) { RetainerData.SetCompositeSub(member, RetainerData.SUB_LUCK, newLuck.ToString()); Apply(); }
            if (GUILayout.Button("100")) { RetainerData.SetCompositeSub(member, RetainerData.SUB_LUCK, "100"); Apply(); }
            GUILayout.EndHorizontal();

            int.TryParse(RetainerData.GetCompositeSub(member, RetainerData.SUB_PERSONALITY), out int currPers);
            UIHelpers.DropdownButtonsWrapped("Personality",
                ClanData.PersonalityOptions.ContainsKey(currPers) ? ClanData.PersonalityOptions[currPers] : "?",
                ClanData.PersonalityOptions, key =>
                { RetainerData.SetCompositeSub(member, RetainerData.SUB_PERSONALITY, key.ToString()); Apply(); },
                8, 60);
        }

        private static void DrawStats(List<string> member)
        {
            UIHelpers.Section("Stats");
            UIHelpers.FloatFieldWithButtons("Writing", member, RetainerData.IDX_WRITING, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Might", member, RetainerData.IDX_MIGHT, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Business", member, RetainerData.IDX_BUSINESS, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Arts", member, RetainerData.IDX_ARTS, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Mood", member, RetainerData.IDX_MOOD, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Renown", member, RetainerData.IDX_RENOWN, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Charisma", member, RetainerData.IDX_CHARISMA, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Health", member, RetainerData.IDX_HEALTH, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Cunning", member, RetainerData.IDX_CUNNING, 100, Apply);
            UIHelpers.FloatFieldWithButtons("Stamina", member, RetainerData.IDX_STAMINA, 100, Apply);
        }

        private static void DrawSpecialFields(List<string> member)
        {
            UIHelpers.Section("Special");

            if (member.Count > RetainerData.IDX_SALARY)
                UIHelpers.IntFieldWithButtons("Salary", member, RetainerData.IDX_SALARY, 999999, Apply);

            if (member.Count > RetainerData.IDX_CLAN_DUTY)
                UIHelpers.TextField("Clan Duty", member, RetainerData.IDX_CLAN_DUTY, 80, 200, Apply);

            if (member.Count > RetainerData.IDX_STATUS_DURATION)
                UIHelpers.IntField("Status Duration", member, RetainerData.IDX_STATUS_DURATION, 60, Apply);
        }

        private static void DrawStatusEditor(List<string> member)
        {
            UIHelpers.Section("Status");
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
            UIHelpers.Section("Pregnancy");
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
