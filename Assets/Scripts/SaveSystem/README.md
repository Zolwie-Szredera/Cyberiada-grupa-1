# 📦 System Zapisu Gry - Dokumentacja

## 🎯 Szybki Przegląd

System zapisu **automatycznie** zapisuje stan gry w momencie dotknięcia checkpointa przez gracza. Obsługuje wiele checkpointów na mapie i przechowuje wszystkie istotne dane.

---

## 📋 Dane Zapisywane

- ✅ **Scena** - nazwa aktualnej mapy
- ✅ **Checkpoint** - nazwa ostatnio dotknętego checkpointa
- ✅ **Pozycja gracza** - X, Y, Z
- ✅ **Zdrowie gracza** - aktualne HP/krew
- ✅ **Akcesoria wyposażone** - 4 sloty
- ✅ **Akcesoria w ekwipunku** - 16 slotów
- ✅ **Czas zapisu** - data i godzina
- ✅ **Czas gry** - ilość sekund rozgrywki

---

## 🚀 Szybki Start (15 minut)

### KROK 1: Main Menu Scene (5 min)

#### 1.1 Utwórz SaveManager
```
Hierarchia → Right-click → Create Empty
├─ Name: "SaveManager"
└─ Add Component → SaveManager.cs
```

#### 1.2 Utwórz GameStateManager
```
Hierarchia → Right-click → Create Empty
├─ Name: "GameStateManager"
└─ Add Component → GameStateManager.cs
```

#### 1.3 Skonfiguruj Button "New Game"
```
1. Hierarchia → Canvas → Button "New Game"
2. Inspector → Button (Script) → On Click ()
3. [+] → Drag "GameStateManager" GameObject
4. Dropdown → Select: GameStateManager → StartNewGame(string)
5. Wpisz parametr: "Level1Courtyard" (zmień na swoją first level)
```

#### 1.4 Skonfiguruj Button "Continue"
```
1. Hierarchia → Canvas → Button "Continue"
2. WAŻNE: Zaznacz checkbox "Interactable" → FALSE na start
3. Inspector → Button (Script) → On Click ()
4. [+] → Drag "GameStateManager" GameObject
5. Dropdown → Select: GameStateManager → ContinueGame()
6. (Bez parametrów!)
```

**Result powinien wyglądać tak:**
```
Button "New Game":
  On Click() → GameStateManager.StartNewGame("Level1Courtyard")

Button "Continue":
  On Click() → GameStateManager.ContinueGame()
  Interactable: FALSE (domyślnie)
```

---

### KROK 2: Game Scenes (5 min)

Dla **każdej** sceny z graczem (Level1Courtyard, levelTutorial, itd.):

#### 2.1 Dodaj SaveSystemLoader
```
1. Otwórz scenę (np. Level1Courtyard.unity)
2. Hierarchia → Right-click → Create Empty
3. Rename → "SaveSystemLoader"
4. Add Component → SaveSystemLoader.cs
5. Ustaw jako pierwszy w hierarchii (drag na górę)
```

#### 2.2 Sprawdzenia
```
Player powinien mieć:
├─ Tag: "Player" ✓
└─ Komponenty:
   ├─ PlayerController.cs ✓
   ├─ PlayerHealth.cs ✓
   ├─ CheckpointSystem.cs ✓
   ├─ AccessoriesManager.cs ✓
   └─ PlayerStats.cs ✓

Checkpoints powinny mieć:
├─ Component: Checkpoint.cs ✓
├─ Collider2D z IsTriger = true ✓
└─ Light2D (do efektu) ✓
```

---

### KROK 3: Resources Folder (3 min)

#### 3.1 Utwórz strukturę
```
Assets/
├─ Resources/
│  └─ Accessories/
│     ├─ (Przenieś WSZYSTKIE akcesoria tutaj)
│     └─ ...
```

#### 3.2 Przenieś akcesoria
```
1. Assets/ScriptableObjects/Accessories/ (lub gdzie są)
2. Cut (Ctrl+X) wszystkie akcesoria
3. Paste (Ctrl+V) do Assets/Resources/Accessories/
```

⚠️ **WAŻNE**: Akcesoria MUSZĄ być w `Resources/Accessories/` aby się załadowały!

---

## 🔧 Jak Działa System

### Zapis Gry (Automatycznie)

```
Gracz dotyka Checkpointa
        ↓
Checkpoint.cs → OnTriggerEnter2D()
        ↓
CheckpointSystem.currentCheckpoint = ten checkpoint
        ↓
SaveManager.Instance.SaveGame()
        ↓
SaveData → JSON → gamesave.json
        ↓
✓ "[SaveManager] Game saved successfully"
```

### Ładowanie Gry (Automatycznie)

```
Gracz kliknie "New Game" lub "Continue"
        ↓
SceneManager.LoadScene(sceneName)
        ↓
SaveSystemLoader.Start()
        ↓
Załaduj SaveData z JSON
        ↓
Przywróć: pozycję, zdrowie, akcesoria, checkpoint
        ↓
✓ "[SaveSystemLoader] Game state loaded"
```

---

## 💻 Użycie w Kodzie

### Ręczny Zapis
```csharp
if (SaveManager.Instance != null)
{
    SaveManager.Instance.SaveGame();
}
```

### Ładowanie
```csharp
SaveData saveData = SaveManager.Instance.LoadGame();
if (saveData != null)
{
    Debug.Log("Ostatnia scena: " + saveData.sceneName);
}
```

### Sprawdzenie czy Zapis Istnieje
```csharp
if (SaveManager.Instance.SaveFileExists())
{
    // Włącz przycisk "Continue"
}
else
{
    // Wyłącz przycisk "Continue"
}
```

### Nowa Gra (Delete Save)
```csharp
SaveManager.Instance.DeleteSaveFile();
SaveManager.Instance.ResetPlayTime();
SceneManager.LoadScene("Level1Courtyard");
```

---

## 📁 Struktura Plików

```
Assets/Scripts/SaveSystem/
├─ SaveData.cs              # [System.Serializable] - struktura danych
├─ SaveManager.cs           # Singleton - zarządza IO (read/write)
├─ SaveSystemLoader.cs      # Ładuje state na scenie
├─ GameStateManager.cs      # Controller menu głównego
├─ SaveSystemUI.cs          # Helper dla UI (opcjonalny)
├─ SaveSystemDebug.cs       # Debug panel (opcjonalny)
├─ PauseMenuSaveIntegration.cs  # Integracja z pause menu
└─ README.md                # Ta dokumentacja
```

---

## 🧪 Testowanie

### Test 1: Nowa Gra
```
✓ Otwórz MainMenu
✓ Kliknij "New Game"
✓ Powinna załadować Level1Courtyard
✓ SaveSystemLoader powinno zalogować "Game state loaded"
```

### Test 2: Checkpoint & Save
```
✓ Osiągnij checkpoint w grze
✓ Console: "[SaveManager] Game saved successfully"
✓ Gracz otrzyma full HP
✓ Plik powinien być zapisany w Application.persistentDataPath/gamesave.json
```

### Test 3: Kontynuacja Gry
```
✓ Wróć do MainMenu (Alt+Left lub przycisk)
✓ Przycisk "Continue" powinien być enabled
✓ Kliknij "Continue"
✓ Gra załaduje się z ostatniego checkpointa
✓ Pozycja, zdrowie i akcesoria takie jak były
```

### Test 4: Nowa Gra (Delete Save)
```
✓ Masz aktywny zapis
✓ Kliknij "New Game"
✓ Stary zapis powinien być usunięty
✓ Przycisk "Continue" powinien być disabled
```

---

## 📂 Lokalizacja Pliku Zapisu

**Scieżka:** `Application.persistentDataPath/gamesave.json`

**Na Windows:**
```
C:\Users\[username]\AppData\LocalLow\[CompanyName]\[ProductName]\gamesave.json
```

Możesz zobaczyć ścieżkę w Console podczas grania:
```
[SaveManager] Save path: C:\Users\...\AppData\LocalLow\...\gamesave.json
```

---

## 📊 Przykład JSON Zapisu

```json
{
  "sceneName": "Level1Courtyard",
  "checkpointName": "Checkpoint_002",
  "playerPosition": {
    "x": 12.5,
    "y": 5.0,
    "z": 0.0
  },
  "playerHealth": 100,
  "equippedAccessoryNames": [
    "Charm_Speed",
    "Charm_Health",
    "",
    ""
  ],
  "inventoryAccessoryNames": [
    "Charm_Damage",
    "Charm_Shield",
    ...
  ],
  "saveTime": "2026-04-27 14:30:45",
  "playTime": 3600
}
```

---

## 🎯 Multi-Checkpoint System

System **wspiera wiele checkpointów** na jednej mapie! Oto jak to działa:

```
Mapa Level1 → wiele Game Objects Checkpoint
├─ Checkpoint_001
├─ Checkpoint_002
└─ Checkpoint_003

Gracz dotyka Checkpoint_002
  → SaveManager.Instance.SaveGame()
  → checkpointName = "Checkpoint_002"
  
Gracz dotyka Checkpoint_003
  → SaveManager.Instance.SaveGame()
  → checkpointName = "Checkpoint_003"
  
Gracz zawsze wraca do OSTATNIO dotknętego checkpointa!
```

---

## 🐛 Typowe Błędy i Rozwiązania

| Błąd | Przyczyna | Rozwiązanie |
|------|-----------|------------|
| "SaveManager.Instance is null" | SaveManager nie na MainMenu | Dodaj SaveManager GameObject z componentem |
| Gra się nie zapisuje | SaveManager.Instance = null lub Player nie ma tagu | Sprawdzić Console, dodać tag "Player" |
| Akcesoria się nie ładują | Akcesoria nie w Resources/Accessories | Przenieść do Assets/Resources/Accessories/ |
| Przycisk "Continue" nie działa | GameStateManager.ContinueGame() nie przypisany | Skonfigurować przycisk w Inspector |
| "Checkpoint not found" | Checkpoint ma inną nazwę | Sprawdzić checkpointName w JSON i w scenie |
| SaveSystemLoader nie załaduje | SaveSystemLoader brakuje na scenie | Dodać SaveSystemLoader na każdą game scene |

---

## ✅ Checklist Przed Launchiem

- [ ] SaveManager na MainMenu scene
- [ ] GameStateManager na MainMenu scene
- [ ] Button "New Game" → GameStateManager.StartNewGame()
- [ ] Button "Continue" → GameStateManager.ContinueGame()
- [ ] SaveSystemLoader na **każdej** game scene
- [ ] Assets/Resources/Accessories/ folder istnieje
- [ ] Wszystkie akcesoria w Resources/Accessories/
- [ ] Player ma tag "Player"
- [ ] Checkpoints mają Collider2D z IsTriger = true
- [ ] Test New Game - OK ✓
- [ ] Test Checkpoint Save - OK ✓
- [ ] Test Continue - OK ✓
- [ ] Test akcesoria załadowują się - OK ✓
- [ ] Console bez ERROR logów - OK ✓

---

## 🎨 Bonus: SaveSystemUI (Opcjonalnie)

Jeśli chcesz wyświetlać informacje o ostatnim zapisie:

```csharp
// W Canvas:
1. Add Component → SaveSystemUI.cs
2. Inspector → Assign:
   - Continue Button Text: Text komponentu przycisku
   - Continue Button: Przycisk "Continue"
   - Save Info Text: Text ze info o zapisie
```

Automatycznie wyświetli:
- ✓ Ostatnią scenę
- ✓ Czas zapisu
- ✓ Czas gry
- ✓ Enable/Disable Continue Button

---

## 🔍 Debug Mode

Dodaj `SaveSystemDebug.cs` do Canvas dla testowania:

```
Canvas → [New GameObject]
  └─ SaveSystemDebug.cs
     └─ Assign Text field do debugText
     
W Console zobaczysz:
✓ [SaveManager] Game saved successfully
✓ [SaveSystemLoader] Game state loaded successfully
✓ Save path: C:\Users\...\AppData\LocalLow\...\gamesave.json
```

---

## 🎬 Sekwencja Zdarzeń

### Nowa Gra
```
MainMenu
   ↓ (Kliknij "New Game")
GameStateManager.StartNewGame("Level1Courtyard")
   ├─ SaveManager.Instance.DeleteSaveFile()
   ├─ SaveManager.Instance.ResetPlayTime()
   └─ SceneManager.LoadScene("Level1Courtyard")
        ↓
   SaveSystemLoader.Start() (LoadGame() zwraca null)
        ↓
   Gracz spawns at START
```

### Wznowienie Gry
```
MainMenu
   ↓ (Kliknij "Continue")
GameStateManager.ContinueGame()
   └─ SaveManager.Instance.LoadGame()
        ├─ Load SaveData from JSON
        └─ SceneManager.LoadScene(saveData.sceneName)
             ↓
        SaveSystemLoader.Start()
             ├─ Restore Position
             ├─ Restore Health
             ├─ Restore Checkpoint
             └─ Load Accessories
                  ↓
             Gracz pojawia się przy CHECKPOINT
```

---

## 📞 Troubleshooting

### Gra się nie zapisuje?
1. Sprawdź czy SaveManager.Instance != null
2. Sprawdzić czy Player ma tag "Player"
3. Sprawdzić czy Checkpoint ma Collider2D z IsTriger = true
4. Otwórz Console (Ctrl+Shift+C) i sprawdź błędy

### Akcesoria się nie ładują?
1. Sprawdzić czy akcesoria są w `Assets/Resources/Accessories/`
2. Sprawdzić czy nazwy akcesoriów w JSON zgadzają się dokładnie
3. Jeśli przeniósłeś akcesoria, Unity może potrzebować reload (Ctrl+R)
4. Sprawdzić Console dla błędów `Resources.Load()`

### Continue Button nie włącza się?
1. Sprawdzić czy gamesave.json istnieje w Application.persistentDataPath
2. Sprawdzić czy SaveSystemUI lub MainMenuUI ma logikę Enable/Disable
3. Kliknąć checkpoint raz aby wygenerować zapis

---

## 📖 Dodatkowe Zasoby

- **SaveData.cs** - struktura danych JSON
- **SaveManager.cs** - główny manager (Singleton)
- **SaveSystemLoader.cs** - ładowanie stanu na scenie
- **GameStateManager.cs** - kontroler menu głównego
- **Checkpoint.cs** (w MapScripts/) - trigger zapisu
