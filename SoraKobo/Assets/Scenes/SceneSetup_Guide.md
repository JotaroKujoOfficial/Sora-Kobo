# Scene Setup Guide — Sora Kobo

This file describes exactly how to set up Unity scenes from scratch.
Follow this after importing the project and installing Mirror.

---

## Scenes Required

| Scene       | Purpose                             |
|-------------|-------------------------------------|
| Bootstrap   | Persistent systems (never unloaded) |
| MainMenu    | Main menu + customization           |
| Game        | Multiplayer game world              |
| MapEditor   | Offline map editor                  |

---

## 1. Bootstrap Scene

Create an empty scene named **Bootstrap**.

### GameBootstrap GameObject
- Add empty GameObject: `GameBootstrap`
- Attach: `SoraKobo.Multiplayer.GameBootstrap`
- Check: `DontDestroyOnLoad` is called in Awake automatically

### SoraNetworkManager GameObject
- Add empty GameObject: `NetworkManager`
- Attach: `SoraKobo.Multiplayer.SoraNetworkManager`
- Attach: `kcp2k.KcpTransport`
- Set `Player Prefab` → drag the Player prefab (see below)
- Set `Max Players Per Server` = 10
- Set `Server Name` = "Sora Kobo Server"

### AudioManager GameObject
- Add empty GameObject: `AudioManager`
- Attach: `SoraKobo.Audio.AudioManager`
- Attach: `SoraKobo.Audio.ProceduralAudioGenerator`
- Add 2 AudioSource children (one for music, one for SFX)

### InteractionManager GameObject
- Add empty GameObject: `InteractionManager`
- Attach: `SoraKobo.Interaction.InteractionManager`

### UIManager GameObject
- Add empty GameObject: `UIManager`
- Attach: `SoraKobo.UI.UIManager`
- Assign all screen references

---

## 2. Game Scene

### Camera
- Main Camera
  - Attach: `SoraKobo.Camera.CameraFollow`
  - Attach: `SoraKobo.UI.ResolutionAdapter`
  - Set Projection: Orthographic, Size: 7

### BuildingSystem GameObject
- Add empty GameObject: `BuildingSystem`
- Attach: `SoraKobo.Building.BuildingSystem`
- Attach: `Mirror.NetworkIdentity`
- Set Block Prefab → Block prefab (see below)

### WorldGenerator GameObject
- Add empty GameObject: `WorldGenerator`
- Attach: `SoraKobo.Building.WorldGenerator`
- Attach: `Mirror.NetworkIdentity`

---

## 3. Player Prefab

Create a Player prefab at `Assets/Resources/Prefabs/Player.prefab`:

### Root GameObject: `Player`
- Add: `Mirror.NetworkIdentity` (check "Server Only" for position authority if desired)
- Add: `SoraKobo.Player.PlayerController`
- Add: `SoraKobo.Player.PlayerCustomization`
- Add: `SoraKobo.Multiplayer.PlayerSyncExtras`
- Add: `Rigidbody2D` (Gravity Scale: 3, Freeze Rotation Z: checked)
- Add: `BoxCollider2D` (size ~0.8 x 1.6)

### Child: `Sprite` (body renderer)
- Add: `SpriteRenderer` (assign colored square sprite)

### Child: `Hair` (hair renderer)
- Add: `SpriteRenderer`

### Child: `Outfit` (outfit renderer)
- Add: `SpriteRenderer`

### Child: `NameTag`
- Add: `SoraKobo.Player.PlayerNameTag`
- Add: `TextMeshPro` (set text = "Player")

### Child: `GroundCheck`
- Position: (0, -0.85, 0)
- Assign to `PlayerController.groundCheck`

---

## 4. Block Prefab

Create a Block prefab at `Assets/Resources/Prefabs/Block.prefab`:

- Root: `Block`
- Add: `SpriteRenderer` (assign a 16x16 white square sprite)
- Add: `BoxCollider2D`
- Add: `Mirror.NetworkIdentity`
- Add: `SoraKobo.Building.PlacedBlock`

---

## 5. Interactable Prefabs

### Chair Prefab
- SpriteRenderer + BoxCollider2D (trigger)
- NetworkIdentity
- ChairObject script
- Tag: "Interactable", Layer: "Interactable"

### Piano Prefab
- SpriteRenderer + BoxCollider2D (trigger)
- NetworkIdentity
- PianoObject script

### Door Prefab
- SpriteRenderer + BoxCollider2D (solid, trigger for detection)
- NetworkIdentity
- DoorObject script

### Swing Prefab
- SpriteRenderer (parent) + BoxCollider2D
- NetworkIdentity
- SwingObject script

---

## 6. Canvas (Game HUD)

Create a Canvas (Screen Space - Overlay) in the Game scene.

### Children:
1. **SafeArea Panel** — RectTransform anchored to fill safe area
2. **Joystick** — FloatingJoystick component, bottom-left
3. **Jump Button** — bottom-right, calls `HUDController.OnJumpButton()`
4. **Interact Button** — calls `HUDController.OnInteractButton()`
5. **Build Toggle** — calls `HUDController.OnBuildToggle()`
6. **Emote Button** — calls `EmoteWheelUI.ToggleWheel()`
7. **Build Panel** — contains block palette (hidden by default)
8. **Chat** — ChatUI component
9. **HUD Controller** — `SoraKobo.UI.HUDController` on Canvas root

---

## 7. Main Menu Canvas

Create separate Canvas in MainMenu scene (or show/hide via UIManager).

### Children:
1. **Title Text** — "Sora Kobo" (大きいフォント)
2. **Splash / Background** — animated gradient background
3. **Play Button** → `MainMenuUI.OnPlayButton()`
4. **Host Button** → `MainMenuUI.OnHostButton()`
5. **Customize Button** → `MainMenuUI.OnCustomizeButton()`
6. **Map Editor Button** → `MainMenuUI.OnMapEditorButton()`
7. **Settings Button** → `MainMenuUI.OnSettingsButton()`
8. **Quit Button** → `MainMenuUI.OnQuitButton()`
9. **Join Panel** — IP input, port input, connect button
10. **Host Panel** — port input, server name input, host button

---

## 8. Map Editor Canvas

1. **Toolbar** — Save, Load, Clear, Back, Erase, Build buttons
2. **Layer Selector** — 3 buttons (BG / Main / FG)
3. **Block Palette** — scrollable grid of block buttons
4. **Save Panel** — map name input + confirm
5. **Load Panel** — scrollable list of saved maps

---

## 9. Physics Layers

In **Edit → Project Settings → Physics 2D**:

| Layer         | Index | Description               |
|---------------|-------|---------------------------|
| Default       | 0     | Unused                    |
| Ground        | 6     | Placed blocks (solid)     |
| Player        | 7     | Player colliders          |
| Interactable  | 8     | Interactable objects      |
| Background    | 9     | Background blocks (no collision)|

**Collision Matrix:** Players collide with Ground. Players do NOT collide with Interactable (use triggers). Background layer collides with nothing.

---

## 10. Build Settings

File → Build Settings:
1. Switch Platform → Android
2. Add Scenes in Order:
   - Bootstrap (index 0)
   - MainMenu  (index 1)
   - Game      (index 2)
   - MapEditor (index 3)
3. Player Settings:
   - Company Name: SoraKoboStudio
   - Product Name: Sora Kobo
   - Bundle Identifier: com.sorakobostudio.sorakobo
   - Min API Level: Android 7.0 (API 24)
   - Target API Level: Android 13 (API 33)
   - Scripting Backend: IL2CPP
   - Target Architecture: ARM64
