using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.ZhuangTou
{
    public static class ZhuangTouUI
    {
        private static List<List<List<List<string>>>> allManagers;
        private static List<ManagerEntry> flatManagers = new List<ManagerEntry>();
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        private class ManagerEntry
        {
            public int FiefIndex;
            public int FarmIndex;
            public int ManagerIndex;
            public List<string> Data;
            public string DisplayName;
        }

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (allManagers == null) return;

            GUILayout.Label($"Farm Managers ({flatManagers.Count})", GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = GUILayout.TextField(searchText, GUILayout.Width(120));
            if (GUILayout.Button("Clear")) searchText = "";
            GUILayout.EndHorizontal();

            var filtered = string.IsNullOrEmpty(searchText)
                ? flatManagers
                : flatManagers.Where(e => e.DisplayName.ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int i = 0; i < filtered.Count; i++)
            {
                var entry = filtered[i];
                if (GUILayout.Button($"[F{entry.FiefIndex} Farm{entry.FarmIndex} #{entry.ManagerIndex}] {entry.DisplayName}"))
                    selectedIndex = flatManagers.IndexOf(entry);
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < flatManagers.Count)
            {
                var manager = flatManagers[selectedIndex].Data;
                DrawManagerEdit(manager);
            }
        }

        private static void DrawManagerEdit(List<string> manager)
        {
            string name = ZhuangTouData.GetManagerName(manager);
            int age = ZhuangTouData.GetAge(manager);
            GUILayout.Label($"Manager: {name} (Age {age})", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            for (int i = 0; i < manager.Count; i++)
            {
                string val = manager[i];
                string intVal = val;
                if (float.TryParse(val, out float fv))
                    intVal = Mathf.RoundToInt(fv).ToString();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Field {i}:", GUILayout.Width(80));
                string newVal = GUILayout.TextField(intVal, GUILayout.Width(120));
                if (newVal != intVal) manager[i] = newVal;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            allManagers = ZhuangTouData.GetManagers();
            flatManagers.Clear();
            for (int fief = 0; fief < allManagers.Count; fief++)
            {
                for (int farm = 0; farm < allManagers[fief].Count; farm++)
                {
                    for (int mgr = 0; mgr < allManagers[fief][farm].Count; mgr++)
                    {
                        var manager = allManagers[fief][farm][mgr];
                        string name = ZhuangTouData.GetManagerName(manager);
                        int age = ZhuangTouData.GetAge(manager);
                        flatManagers.Add(new ManagerEntry
                        {
                            FiefIndex = fief,
                            FarmIndex = farm,
                            ManagerIndex = mgr,
                            Data = manager,
                            DisplayName = $"{name} (Age {age})"
                        });
                    }
                }
            }
            selectedIndex = -1;
        }
    }
}