using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.City
{
    public static class CityUI
    {
        private static List<List<List<string>>> allCities;
        private static List<CityEntry> flatCities = new List<CityEntry>();
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        private class CityEntry
        {
            public int PrefectureIndex;
            public int CityIndex;
            public List<string> Data;
            public string DisplayName;
        }

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (allCities == null) return;

            GUILayout.Label($"Cities ({flatCities.Count})", GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = GUILayout.TextField(searchText, GUILayout.Width(120));
            if (GUILayout.Button("Clear")) searchText = "";
            GUILayout.EndHorizontal();

            var filtered = string.IsNullOrEmpty(searchText)
                ? flatCities
                : flatCities.Where(c => c.DisplayName.ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int i = 0; i < filtered.Count; i++)
            {
                var entry = filtered[i];
                if (GUILayout.Button($"[P{entry.PrefectureIndex}C{entry.CityIndex}] {entry.DisplayName}"))
                    selectedIndex = flatCities.IndexOf(entry);
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < flatCities.Count)
            {
                var city = flatCities[selectedIndex].Data;
                DrawCityEdit(city);
            }
        }

        private static void DrawCityEdit(List<string> city)
        {
            string name = CityData.GetCityName(city);
            GUILayout.Label($"City: {name}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            for (int i = 0; i < city.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Field {i}:", GUILayout.Width(80));
                string val = GUILayout.TextField(city[i], GUILayout.Width(200));
                if (val != city[i]) city[i] = val;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private static void Refresh()
        {
            allCities = CityData.GetCities();
            flatCities.Clear();
            for (int p = 0; p < allCities.Count; p++)
            {
                for (int c = 0; c < allCities[p].Count; c++)
                {
                    var city = allCities[p][c];
                    string name = CityData.GetCityName(city);
                    flatCities.Add(new CityEntry
                    {
                        PrefectureIndex = p,
                        CityIndex = c,
                        Data = city,
                        DisplayName = name
                    });
                }
            }
            selectedIndex = -1;
        }
    }
}