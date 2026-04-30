using UnityEngine;
using ClanUI = HoLMod.MemberCheat.ClanMember.ClanMemberUI;
using SpouseUI = HoLMod.MemberCheat.Spouse.SpouseUI;
using RetainerUI = HoLMod.MemberCheat.Retainer.RetainerUI;
using ShiJiaUI = HoLMod.MemberCheat.ShiJia.ShiJiaUI;
using RoyalUI = HoLMod.MemberCheat.Royal.RoyalUI;
using FarmUI = HoLMod.MemberCheat.Farm.FarmUI;
using CourtUI = HoLMod.MemberCheat.Court.CourtUI;
using DecreeUI = HoLMod.MemberCheat.Decree.DecreeUI;
using CityUI = HoLMod.MemberCheat.City.CityUI;
using FiefUI = HoLMod.MemberCheat.Fief.FiefUI;
using MineUI = HoLMod.MemberCheat.Mine.MineUI;
using CampUI = HoLMod.MemberCheat.Camp.CampUI;
using EstateUI = HoLMod.MemberCheat.Estate.EstateUI;
using WarEventUI = HoLMod.MemberCheat.WarEvent.WarEventUI;
using HorseUI = HoLMod.MemberCheat.Horse.HorseUI;
using KingCityUI = HoLMod.MemberCheat.KingCity.KingCityUI;
using VassalPrinceUI = HoLMod.MemberCheat.VassalPrince.VassalPrinceUI;
using AttackPrefectureUI = HoLMod.MemberCheat.AttackPrefecture.AttackPrefectureUI;
using GameSettingsUI = HoLMod.MemberCheat.GameSettings.GameSettingsUI;

namespace HoLMod.MemberCheat
{
    public class MainUI : MonoBehaviour
    {
        private bool showPanel = false;
        private Rect windowRect;
        private int selectedTab = 0;
        private string[] tabNames = {
            "Clan", "Spouses", "Retainers", "Other Clans", "Royal",
            "Farms", "Court", "Decrees", "Cities", "Fiefs",
            "Mines", "Camps", "Estates", "War Events", "Horses",
            "Imp. Capital", "Vassal Princes", "Hostile Pref.", "Game"
        };

        private void Awake()
        {
            float w = 1050f;
            float h = 1000f;
            windowRect = new Rect((Screen.width - w) / 2f, (Screen.height - h) / 2f, w, h);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                showPanel = !showPanel;
                if (showPanel) ForceRefreshCurrentTab();
            }
        }

        private void ForceRefreshCurrentTab()
        {
            switch (selectedTab)
            {
                case 0: ClanUI.RequestRefresh(); break;
                case 1: SpouseUI.RequestRefresh(); break;
                case 2: RetainerUI.RequestRefresh(); break;
                case 3: ShiJiaUI.RequestRefresh(); break;
                case 4: RoyalUI.RequestRefresh(); break;
                case 5: FarmUI.RequestRefresh(); break;
                case 6: CourtUI.RequestRefresh(); break;
                case 7: DecreeUI.RequestRefresh(); break;
                case 8: CityUI.RequestRefresh(); break;
                case 9: FiefUI.RequestRefresh(); break;
                case 10: MineUI.RequestRefresh(); break;
                case 11: CampUI.RequestRefresh(); break;
                case 12: EstateUI.RequestRefresh(); break;
                case 13: WarEventUI.RequestRefresh(); break;
                case 14: HorseUI.RequestRefresh(); break;
                case 15: KingCityUI.RequestRefresh(); break;
                case 16: VassalPrinceUI.RequestRefresh(); break;
                case 17: AttackPrefectureUI.RequestRefresh(); break;
            }
        }

        private void OnGUI()
        {
            if (!showPanel) return;
            windowRect = GUI.Window(999, windowRect, DrawMainWindow, $"{ModInfo.Name} v{ModInfo.Version}");
        }

        private void DrawMainWindow(int id)
        {
            GUILayout.BeginVertical();

            // Two rows of 10 + 9 buttons
            int cols = 10;
            int rows = Mathf.CeilToInt(tabNames.Length / (float)cols);
            for (int r = 0; r < rows; r++)
            {
                GUILayout.BeginHorizontal();
                for (int c = 0; c < cols; c++)
                {
                    int index = r * cols + c;
                    if (index >= tabNames.Length) break;

                    GUI.enabled = (index != selectedTab);
                    if (GUILayout.Button(tabNames[index], GUILayout.Width(95)))
                    {
                        selectedTab = index;
                        ForceRefreshCurrentTab();
                    }
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);

            switch (selectedTab)
            {
                case 0: ClanUI.Draw(); break;
                case 1: SpouseUI.Draw(); break;
                case 2: RetainerUI.Draw(); break;
                case 3: ShiJiaUI.Draw(); break;
                case 4: RoyalUI.Draw(); break;
                case 5: FarmUI.Draw(); break;
                case 6: CourtUI.Draw(); break;
                case 7: DecreeUI.Draw(); break;
                case 8: CityUI.Draw(); break;
                case 9: FiefUI.Draw(); break;
                case 10: MineUI.Draw(); break;
                case 11: CampUI.Draw(); break;
                case 12: EstateUI.Draw(); break;
                case 13: WarEventUI.Draw(); break;
                case 14: HorseUI.Draw(); break;
                case 15: KingCityUI.Draw(); break;
                case 16: VassalPrinceUI.Draw(); break;
                case 17: AttackPrefectureUI.Draw(); break;
                case 18: GameSettingsUI.Draw(); break;
            }

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
    }
}