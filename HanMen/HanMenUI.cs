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

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = GUILayout.TextField(searchText, GUILayout.Width(120));
            if (GUILayout.Button("Clear")) searchText = "";
            GUILayout.EndHorizontal();

            var filtered = string.IsNullOrEmpty(searchText)
                ? members.Select((m, i) => new { m, i }).ToList()
                : members.Select((m, i) => new { m, i }).Where(x => HanMenData.GetName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

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
            GUILayout.Label($"Editing: {name} (Age {age})", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            for (int i = 0; i < member.Count; i++)
            {
                string val = member[i];
                string intVal = val;
                if (float.TryParse(val, out float fv))
                    intVal = Mathf.RoundToInt(fv).ToString();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Field {i}:", GUILayout.Width(80));
                string newVal = GUILayout.TextField(intVal, GUILayout.Width(120));
                if (newVal != intVal) member[i] = newVal;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
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