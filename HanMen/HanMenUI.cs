using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.HanMen
{
    public static class HanMenUI
    {
        private static List<List<string>> members;
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (members == null) return;

            GUILayout.Label($"Civilians (HanMen) ({members.Count})", GUI.skin.box);
            UIHelpers.SearchBar(ref searchText);

            var filtered = string.IsNullOrEmpty(searchText)
                ? members.Select((m, i) => new { m, i }).ToList()
                : members.Select((m, i) => new { m, i })
                    .Where(x => HanMenData.GetName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int j = 0; j < filtered.Count; j++)
            {
                var item = filtered[j];
                string name = HanMenData.GetName(item.m);
                int age = HanMenData.GetAge(item.m);
                if (GUILayout.Button($"{item.i}: {name} (Age {age})"))
                    selectedIndex = item.i;
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < members.Count)
            {
                var member = members[selectedIndex];
                DrawMemberEdit(member);
            }
        }

        private static void DrawMemberEdit(List<string> member)
        {
            string name = HanMenData.GetName(member);
            int age = HanMenData.GetAge(member);
            GUILayout.Label($"Editing: {name} (Age {age})", UIHelpers.BoldLabel);

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            UIHelpers.Section("Basic Info");
            DrawStatField(member, HanMenData.IDX_PERSON_KEY, "Person Key");
            DrawTextField(member, HanMenData.IDX_APPEARANCE, "Appearance");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GUILayout.Width(80));
            var parts = member.Count > HanMenData.IDX_COMPOSITE ? member[HanMenData.IDX_COMPOSITE].Split('|') : new string[0];
            string curName = parts.Length > 0 ? parts[0] : "";
            string newName = GUILayout.TextField(curName, GUILayout.Width(200));
            if (newName != curName)
            {
                parts[0] = newName;
                member[HanMenData.IDX_COMPOSITE] = string.Join("|", parts);
                HanMenData.SetMembers(members);
            }
            GUILayout.EndHorizontal();

            DrawStatField(member, HanMenData.IDX_AGE, "Age");

            UIHelpers.Section("Stats");
            DrawStatField(member, HanMenData.IDX_WRITING, "Writing");
            DrawStatField(member, HanMenData.IDX_MIGHT, "Might");
            DrawStatField(member, HanMenData.IDX_BUSINESS, "Business");
            DrawStatField(member, HanMenData.IDX_ARTS, "Arts");
            DrawStatField(member, HanMenData.IDX_MOOD, "Mood");
            DrawStatField(member, HanMenData.IDX_RENOWN, "Renown");
            DrawStatField(member, HanMenData.IDX_CHARISMA, "Charisma");
            DrawStatField(member, HanMenData.IDX_HEALTH, "Health");
            DrawStatField(member, HanMenData.IDX_CUNNING, "Cunning");

            UIHelpers.Section("Position & Progression");
            DrawTextField(member, HanMenData.IDX_EMPIRE_POSITION, "Empire Position");
            DrawTextField(member, HanMenData.IDX_SCHOLARSHIP, "Scholarship");
            DrawTextField(member, HanMenData.IDX_FIEF_TITLE, "Fief Title");
            DrawTextField(member, HanMenData.IDX_STATUS, "Status");
            DrawTextField(member, HanMenData.IDX_STAT_GAIN, "Stat Gain");
            DrawTextField(member, HanMenData.IDX_SCHOOL, "School");

            GUILayout.EndScrollView();
        }

        private static void DrawStatField(List<string> member, int idx, string label)
        {
            if (idx >= member.Count) return;
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(100));
            string val = GUILayout.TextField(UIHelpers.GetDisplayValue(member[idx]), GUILayout.Width(120));
            if (val != member[idx])
            {
                member[idx] = val;
                HanMenData.SetMembers(members);
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawTextField(List<string> member, int idx, string label)
        {
            if (idx >= member.Count) return;
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(100));
            string val = GUILayout.TextField(member[idx], GUILayout.Width(200));
            if (val != member[idx])
            {
                member[idx] = val;
                HanMenData.SetMembers(members);
            }
            GUILayout.EndHorizontal();
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            members = HanMenData.GetMembers();
            selectedIndex = -1;
        }
    }
}
