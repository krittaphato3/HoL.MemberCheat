using System.Collections.Generic;
using UnityEngine;

namespace HoLMod.MemberCheat.KingCity
{
    public static class KingCityUI
    {
        private static List<string> kingCity;
        private static Vector2 scroll;
        private static bool needsRefresh = true;

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { kingCity = KingCityData.GetKingCity(); needsRefresh = false; }
            if (kingCity == null) return;

            GUILayout.Label($"Imperial Capital ({kingCity.Count} fields)", GUI.skin.box);

            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(600));

            for (int i = 0; i < kingCity.Count; i++)
            {
                if (!KingCityData.FieldLabels.ContainsKey(i)) continue;
                string label = KingCityData.FieldLabels[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{label}:", GUILayout.Width(100));
                string val = GUILayout.TextField(UIHelpers.GetDisplayValue(kingCity[i]), GUILayout.Width(300));
                if (val != kingCity[i])
                {
                    kingCity[i] = val;
                    KingCityData.SetKingCity(kingCity);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }
}
