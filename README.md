# Member Cheat — House of Legacy Save Editor

A comprehensive save‑editor mod for **House of Legacy** (Steam ID 2503770).  
Open a GUI with **F8** and modify nearly every piece of game data in real time.

## ✨ Features

- **19 fully editable tabs** covering all character types, resources, and systems
- **Real‑time editing** — changes are applied immediately to the game state
- **Search bars** on every list for quick member/item lookup
- **Integer display** for all stats (floats are automatically converted)
- **Preset system** for talents, skills, personalities, hobbies, ranks, and more
- **Quick actions** (“Max All 100” and “Boost +10”) in every character editor
- **Family & finances** sub‑tabs with coin/gold manipulation
- **Safe reflection** — reads and writes static fields of the `Mainload` class
- **Modular code** — each tab has its own Data/UI pair for easy expansion

## 📦 Installation

1. Install **BepInEx 5** for House of Legacy.
2. Download the latest release of `HoLMod.MemberCheat.dll`.
3. Place the DLL inside `BepInEx/plugins/`.
4. Launch the game. Press `F8` to open the cheat window.

## 🎮 Usage

| Key | Action |
|-----|--------|
| `F8` | Toggle cheat window |

The window has a tab bar at the top. Click any tab to switch the editor.  
Most lists have a **search bar** and a **Refresh** button.

### Tabs Overview

| Tab | Edits |
|-----|-------|
| **Clan** | Current/Branch family members, Family Data (name, level, renown, warehouse), Treasure (coins & gold bars with quick add/subtract buttons), Finances (income & spending) |
| **Spouses** | All player spouses — stats, talent, skill, personality, hobby, marital harmony, pregnancy, status, traits, exile |
| **Retainers** | Retainers — stats, talent, skill, personality, salary, pregnancy, status, dismiss |
| **Other Clans** | Browse other clans → members & spouses with full composite editor + rank presets |
| **Royal** | Royal family members & spouses — full editor with rank presets |
| **Farms** | Player‑owned farms — size, fertility, population, production, workers |
| **Court** | Royal court ministers (Guan_JingCheng) — all fields editable |
| **Decrees** | Imperial decrees (ZhengLing_Now) — all fields editable |
| **Cities** | All cities (CityData_now) — browse by prefecture/city, all fields editable |
| **Fiefs** | Fief ownership data (Fengdi_now) — all fields editable |
| **Mines** | Mines (Kuang_now) — all fields editable |
| **Camps** | Military camps (JunYing_now) — all fields editable |
| **Estates** | Estates (Fudi_now) — all fields editable |
| **War Events** | Ongoing military conflicts (WarEvent_Now) |
| **Horses** | Player’s owned horses (Horse_Have) |
| **Imp. Capital** | Imperial capital data (KingCityData_now) |
| **Vassal Princes** | Vassal prince data (WangGData_now) |
| **Hostile Pref.** | Hostile prefectures (CityID_CanAttack) |
| **Game** | In‑game date (year/month/day) |

*Each tab auto‑refreshes when opened or when switching tabs.*

### Character Editors

All character editors (Clan Member, Spouse, Retainer, Other Clan members/spouses, Royal members/spouses) include:

- **Name & Age** with quick age‑change buttons
- **Composite fields** (talent type/value, gender, skill type/value, luck, personality, hobby) with full selection buttons
- **Stats** (Writing, Might, Business, Arts, Mood, Renown, Health, Charisma, Cunning, Stamina) displayed as integers
- **Special fields** (Clan Leader, Status, Marriage, Pregnancy, Scholarship, Fief Title, Traits, Clan Duty, Study School)
- **Rank editor** with categorized rank presets (None/Special, Official No Position, Military, Government, Censors Regional, Royal)
- **Quick actions** at the top: “Max All (100)” and “Boost +10”

## 🛠 Building from Source

1. Clone the repository.
2. Open a terminal in the project folder.
3. Run:  
   ```
   dotnet build
   ```
4. The DLL will be in `bin/Debug/net35/` (or your target framework).  
   Copy it to `BepInEx/plugins/`.

### Dependencies

- BepInEx 5
- Harmony (via BepInEx)
- Unity 2020.1.14 references (provided by the game)
- YuanAPI (the game’s modding API)

## 📁 Project Structure

```markdown
HoLMod.MemberCheat/
├── Plugin.cs                     Entry point (BepInEx plugin)
├── MainUI.cs                     19‑tab window manager
├── ModInfo.cs                    Version & name constants
├── ClanMember/                   Current & Branch clan members
├── Spouse/                       Player’s spouses
├── Retainer/                     Retainers
├── ShiJia/                       Other clans
├── Royal/                        Royal family
├── Farm/                         Farms
├── Court/                        Court ministers
├── Decree/                       Imperial decrees
├── City/                         Cities
├── Fief/                         Fiefs
├── Mine/                         Mines
├── Camp/                         Military camps
├── Estate/                       Estates
├── WarEvent/                     War events
├── Horse/                        Horses
├── KingCity/                     Imperial capital
├── VassalPrince/                 Vassal princes
├── AttackPrefecture/             Hostile prefectures
├── GameSettings/                 Date editor
└── README.md                     This file
```

Each sub‑folder contains a `*Data.cs` (data access via reflection) and a `*UI.cs` (Unity OnGUI editor).

## 📝 License

This mod is provided as open‑source for the benefit of the House of Legacy community.  
Feel free to fork, modify, and distribute.

## 🙏 Credits

- Built with BepInEx, Harmony, and the YuanAPI framework.
