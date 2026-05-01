using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.Farm
{
    public static class FarmUI
    {
        private static List<List<List<string>>> allFarms;
        private static List<FarmEntry> flatFarms = new List<FarmEntry>();
        private static int selectedIndex = -1;
        private static Vector2 scrollFarm, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        private class FarmEntry
        {
            public int RegionIndex;
            public int FarmIndex;
            public List<string> Data;
            public string DisplayName;
        }

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (allFarms == null) return;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = GUILayout.TextField(searchText, GUILayout.Width(120));
            if (GUILayout.Button("Clear")) searchText = "";
            GUILayout.EndHorizontal();

            var filtered = string.IsNullOrEmpty(searchText)
                ? flatFarms
                : flatFarms.Where(f => f.DisplayName.ToLower().Contains(searchText.ToLower())).ToList();

            scrollFarm = GUILayout.BeginScrollView(scrollFarm, GUILayout.Height(200));
            for (int i = 0; i < filtered.Count; i++)
            {
                var entry = filtered[i];
                if (GUILayout.Button($"[R{entry.RegionIndex} F{entry.FarmIndex}] {entry.DisplayName}"))
                    selectedIndex = flatFarms.IndexOf(entry);
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < flatFarms.Count)
            {
                var farm = flatFarms[selectedIndex].Data;
                DrawFarmEdit(farm);
            }
        }

        private static void DrawFarmEdit(List<string> farm)
        {
            string fName = FarmData.GetFarmName(farm);
            int size = FarmData.GetFarmSize(farm);
            GUILayout.Label($"Farm: {fName} ({size} acres)", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            string[] labels = {
                "Ownership", null, "Fertility", null, "Location",
                "Size (acres)", "Name", "Population", null, null,
                "Environment", "Security", "Convenience", null, "Production",
                null, "Manager Key", null, null, "Young Trees 2yr",
                null, "Young Trees 4yr", "Young Trees 5yr", null, "Workers Total",
                null, "Idle Workers"
            };

            for (int i = 0; i < farm.Count; i++)
            {
                string label = (i < labels.Length) ? labels[i] : null;
                if (label == null) continue;
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{label}:", GUILayout.Width(120));
                string val = GUILayout.TextField(farm[i], GUILayout.Width(120));
                if (val != farm[i]) farm[i] = val;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private static void Refresh()
        {
            scrollFarm = Vector2.zero;
            scrollEdit = Vector2.zero;
            allFarms = FarmData.GetFarmList();
            flatFarms.Clear();
            for (int r = 0; r < allFarms.Count; r++)
            {
                for (int f = 0; f < allFarms[r].Count; f++)
                {
                    var farm = allFarms[r][f];
                    if (FarmData.IsPlayerFarm(farm, r))
                    {
                        string name = FarmData.GetFarmName(farm);
                        int sz = FarmData.GetFarmSize(farm);
                        flatFarms.Add(new FarmEntry
                        {
                            RegionIndex = r,
                            FarmIndex = f,
                            Data = farm,
                            DisplayName = $"{name} ({sz} acres)"
                        });
                    }
                }
            }
            selectedIndex = -1;
        }
    }
}