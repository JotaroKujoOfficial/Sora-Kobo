# Инструкция по сборке APK — Sora Kobo

## Что нужно установить

1. **Unity Hub** → https://unity.com/download
2. **Unity 2022.3 LTS** через Unity Hub с модулями:
   - Android Build Support
   - Android SDK & NDK Tools  
   - OpenJDK
3. **Mirror Networking** (устанавливается через Package Manager в Unity)

---

## Шаги сборки

### 1. Открыть проект в Unity

```
Unity Hub → Projects → Add → выбрать папку SoraKobo/
```

Подождите, пока Unity импортирует все ассеты (~3-5 минут).

### 2. Импорт TextMeshPro

Если появится диалог "TMP Essentials" → **Import TMP Essentials**.

### 3. Установить Mirror Networking

```
Window → Package Manager → (+) → Add package from git URL:
https://github.com/MirrorNetworking/Mirror.git#v89.0.0
```

Или через OpenUPM (уже в manifest.json):
```
Window → Package Manager → подождите установки из OpenUPM
```

### 4. Создать сцены

Следуйте `Assets/Scenes/SceneSetup_Guide.md` для настройки 4 сцен.

**Быстрый старт (минимальный):**

Для теста можно создать одну сцену "Game" со всеми объектами:
- GameObject "NetworkManager" + `SoraNetworkManager` + `KcpTransport`
- GameObject "BuildingSystem" + `BuildingSystem` + `NetworkIdentity`
- GameObject "AudioManager" + `AudioManager` + `ProceduralAudioGenerator`
- Main Camera + `CameraFollow` + `ResolutionAdapter`
- Canvas + `UIManager` + `HUDController`
- Создать Player prefab + Block prefab

### 5. Переключиться на Android

```
File → Build Settings → Android → Switch Platform
```

### 6. Настройить Player Settings

```
File → Build Settings → Player Settings
```

| Настройка | Значение |
|-----------|----------|
| Product Name | Sora Kobo |
| Bundle Identifier | com.sorakobostudio.sorakobo |
| Version | 1.0 |
| Min API Level | Android 7.0 (API level 24) |
| Target API Level | Android 13 (API level 33) |
| Scripting Backend | IL2CPP |
| Target Architectures | ARM64 |
| Default Orientation | Portrait |
| Internet Access | Required |

### 7. Добавить сцены в Build

```
File → Build Settings → Add Open Scenes
```

Порядок сцен:
- 0: Bootstrap
- 1: MainMenu
- 2: Game  
- 3: MapEditor

### 8. Собрать APK

```
File → Build Settings → Build
```

Выберите папку и имя файла: `SoraKobo.apk`

Время сборки: ~5-15 минут (зависит от ПК).

---

## Установка на устройство

### Способ 1: USB
```bash
adb install SoraKobo.apk
```

### Способ 2: Файл
Скопируйте APK на устройство → откройте файловый менеджер → установите
(нужно разрешить установку из неизвестных источников в настройках Android)

### Способ 3: Build And Run
В Build Settings нажмите **Build And Run** — Unity установит APK напрямую на подключённое устройство.

---

## Устранение проблем

**Ошибка "Mirror not found":**
→ Установите Mirror через Package Manager (см. шаг 3)

**Ошибка "TextMeshPro":**
→ Window → TextMeshPro → Import TMP Essentials

**Ошибка "IL2CPP not found":**
→ Unity Hub → Editor → Add modules → NDK/IL2CPP

**Ошибка "Android SDK not found":**
→ Edit → Preferences → External Tools → Android SDK путь

**"NetworkIdentity not found on prefab":**
→ Убедитесь что Player и Block prefabs имеют компонент NetworkIdentity

---

## Как тестировать мультиплеер

### На одном ПК (два редактора):
Используйте ParrelSync или запустите Build + Play In Editor одновременно.

### На двух Android-устройствах:
1. Установите APK на оба
2. Подключите к одному Wi-Fi
3. На устройстве А: "Host Game" → запомните IP (Настройки → О телефоне)
4. На устройстве Б: "Join Game" → введите IP устройства А → Connect

---

## Сетевые требования

| Протокол | Порт | Описание |
|----------|------|----------|
| UDP | 7777 | Mirror KCP Transport (основной) |

Для игры через интернет: настройте Port Forwarding на роутере хоста: UDP 7777 → локальный IP хоста.
