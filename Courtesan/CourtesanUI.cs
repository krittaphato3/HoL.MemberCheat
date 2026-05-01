using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

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
            string name = CourtesanData.GetName(member);
            int age = CourtesanData.GetAge(member);
            GUILayout.Label($"Editing: {name} (Age {age})", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            // Show all fields as editable text
            for (int i = 0; i < member.Count; i++)
            {
                // Attempt to convert float stats to integer
                string val = member[i];
                string intVal = val;
                if (float.TryParse(val, out float fv))
                    intVal = Mathf.RoundToInt(fv).ToString();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Field {i}:", GUILayout.Width(80));
                string newVal = GUILayout.TextField(intVal, GUILayout.Width(120));
                if (newVal != intVal)
                {
                    // If the field was originally float, store as integer
                    member[i] = newVal;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
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
                    string name = CourtesanData.GetName(member);
                    int age = CourtesanData.GetAge(member);
                    flatList.Add(new CourtesanEntry
                    {
                        CityIndex = c,
                        MemberIndex = m,
                        Data = member,
                        DisplayName = $"{name} (Age {age})"
                    });
                }
            }
            selectedIndex = -1;
        }
    }
}