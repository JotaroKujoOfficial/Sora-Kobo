# 🌸 Sora Kobo — 空の工房
### Уютная 2D мультиплеерная песочница для Android

---

## Содержимое проекта

```
SoraKobo/
├── Assets/
│   ├── Scripts/
│   │   ├── Player/          # Контроллер игрока, кастомизация, теги
│   │   ├── Building/        # Строительная система, блоки, карта-сериализатор
│   │   ├── Interaction/     # Интерактивные объекты (стул, дверь, качели...)
│   │   ├── Interactables/   # Конкретные интерактивные объекты (знаки)
│   │   ├── Multiplayer/     # Mirror NetworkManager, синхронизация
│   │   ├── MapEditor/       # Редактор карт (оффлайн)
│   │   ├── UI/              # HUD, главное меню, кастомизация, пианино
│   │   ├── Camera/          # Следование камеры, адаптация разрешений
│   │   ├── Audio/           # AudioManager, процедурная генерация звуков
│   │   └── Data/            # Структуры данных (блоки, карты, игрок)
│   └── Scenes/
│       └── SceneSetup_Guide.md  # Полный гайд по настройке сцен
├── Packages/
│   └── manifest.json        # Зависимости (включая Mirror через OpenUPM)
├── ProjectSettings/
│   └── ProjectSettings.asset # Настройки Android
└── README.md                # Этот файл
```

---

## Шаг 1 — Требования

| Компонент | Версия |
|-----------|--------|
| Unity | **2022.3 LTS** (любая подверсия) |
| Android Build Support | из Unity Hub |
| Mirror Networking | последняя через OpenUPM |

---

## Шаг 2 — Установка Unity

1. Откройте **Unity Hub** → вкладка "Installs"
2. Установите **Unity 2022.3 LTS** с модулями:
   - ✅ Android Build Support
   - ✅ Android SDK & NDK Tools
   - ✅ OpenJDK

---

## Шаг 3 — Открыть проект

1. В Unity Hub → "Add" → выберите папку `SoraKobo/`
2. Unity откроет проект (первый импорт занимает 3–5 мин)

---

## Шаг 4 — Установить Mirror Networking

Mirror — библиотека для мультиплеера. Установите через Window → Package Manager:

**Способ 1 (OpenUPM — рекомендуется):**
```
// Файл Packages/manifest.json уже настроен.
// Просто дождитесь, пока Unity установит зависимости.
```

**Способ 2 (Unity Package Manager → Add from URL):**
```
https://github.com/MirrorNetworking/Mirror.git
```

**Способ 3 (Asset Store):**
- Window → Asset Store → поиск "Mirror" → Import

---

## Шаг 5 — Создание сцен

Следуйте файлу `Assets/Scenes/SceneSetup_Guide.md`.

**Краткий порядок:**
1. Создайте сцены: Bootstrap, MainMenu, Game, MapEditor
2. В Bootstrap добавьте: `GameBootstrap`, `SoraNetworkManager`, `AudioManager`
3. В Game добавьте: `BuildingSystem`, `WorldGenerator`, Camera с `CameraFollow`
4. Создайте префаб Player (NetworkIdentity + PlayerController + Rigidbody2D)
5. Создайте префаб Block (SpriteRenderer + BoxCollider2D + PlacedBlock)
6. Настройте Canvas с HUD кнопками

---

## Шаг 6 — Настройка TextMeshPro

При первом запуске Unity спросит "Import TMP Essentials" — нажмите **Import**.

---

## Шаг 7 — Сборка APK

1. **File → Build Settings**
2. Выберите платформу **Android** → Switch Platform
3. Добавьте сцены в нужном порядке (Bootstrap=0, MainMenu=1, Game=2, MapEditor=3)
4. Нажмите **Player Settings**:
   - Product Name: `Sora Kobo`
   - Bundle Identifier: `com.sorakobostudio.sorakobo`
   - Min API Level: `API 24`
   - Scripting Backend: `IL2CPP`
   - Target Architecture: `ARM64`
5. Нажмите **Build** → выберите папку → дождитесь `SoraKobo.apk`

---

## Шаг 8 — Запуск мультиплеера

### Хост (создатель сервера):
1. Откройте игру на устройстве/PC → "Host Game"
2. Укажите порт (по умолчанию: **7777**)
3. Нажмите "Start Host" — вы играете как сервер+клиент

### Клиент (подключение):
1. Откройте игру → "Join Game"
2. Введите IP-адрес хоста и порт **7777**
3. Нажмите "Connect"

**Узнать IP-адрес хоста:**
- Android: Настройки → О телефоне → Статус → IP-адрес
- PC: `ipconfig` (Windows) или `ifconfig` (Mac/Linux)

**Требования к сети:**
- Все устройства должны быть в одной Wi-Fi сети (LAN)
- Или хост настраивает Port Forwarding на роутере для игры через интернет

---

## Архитектура

```
[Клиент 1] ──┐
[Клиент 2] ──┼──► [Mirror KCP сервер] ──► Авторитетный игровой мир
[Клиент 3] ──┘         ↑
                   (хост = сервер + клиент)
```

- **Движок сети:** Mirror + KCP Transport (UDP, низкая задержка)
- **Вместимость:** до 10 игроков одновременно
- **Синхронизация:** SyncVar для состояний, Command/ClientRpc для действий
- **Блоки:** NetworkServer.Spawn — мгновенная репликация для всех

---

## Игровые механики

### 🧱 Строительство
- Кнопка "Build" активирует режим строительства
- Выберите блок из палитры
- Нажмите на мир → блок размещается на сервере → синхронизируется всем
- Кнопка "Erase" — удаление блоков

### 🎭 Кастомизация персонажа
- Главное меню → "Customize"
- Выбор причёски, одежды, аксессуаров
- Цвет через Hue-слайдеры
- Сохраняется в PlayerPrefs

### 🪑 Интерактивные объекты
| Объект | Действие |
|--------|----------|
| Стул | Сесть / встать |
| Пианино | Открыть клавиатуру, играть ноты |
| Дверь | Открыть / закрыть |
| Качели | Раскачаться |
| Еда | Съесть (объект исчезает) |
| Транспорт | Сесть за руль, ехать |

### 🗺️ Редактор карт
- Главное меню → "Map Editor"
- Рисуй блоки на сетке
- Сохраняй/загружай как JSON-файлы
- Загруженная карта применяется в мультиплеере при хостинге

### 💬 Чат
- Кнопка чата → ввод текста → Send
- Синхронизируется через сервер на всех клиентов

### 😊 Эмоции
- Кнопка Emote → колесо эмоций (Wave, Dance, Sit, Jump)

---

## Файлы карт

Карты сохраняются в формате JSON:

**Android:** `/sdcard/Android/data/com.sorakobostudio.sorakobo/files/Maps/`

**Пример карты:**
```json
{
  "mapName": "MyWorld",
  "authorName": "Player",
  "width": 100,
  "height": 50,
  "saveDate": "2025-01-01 12:00",
  "blocks": [
    {"blockId": "grass", "x": 0, "y": 5, "layer": 1},
    {"blockId": "dirt",  "x": 0, "y": 4, "layer": 1}
  ]
}
```

---

## Типы блоков

| ID | Цвет | Описание |
|----|------|----------|
| grass | 🟩 | Трава (поверхность) |
| dirt | 🟫 | Земля |
| stone | ⬛ | Камень |
| wood | 🟤 | Дерево (бревно) |
| planks | 🟫 | Доски |
| sand | 🟡 | Песок |
| water | 🔵 | Вода |
| snow | ⬜ | Снег |
| brick | 🔴 | Кирпич |
| glass | 🔷 | Стекло |
| leaf | 🍃 | Листья |
| cloud | ☁️ | Облако (фон) |
| flower | 🌸 | Цветок (декор) |
| sky | 💙 | Небо (фон) |

---

## Слои (Layers)

| Layer | Индекс | Описание |
|-------|--------|----------|
| Background | 0 | Декоративный фон, не блокирует |
| Main | 1 | Основной слой со столкновениями |
| Foreground | 2 | Декор поверх персонажа |

---

## Производительность

| Параметр | Значение |
|----------|----------|
| Target FPS | 60 |
| Max Players | 10 |
| Network Send Rate | 20 Hz |
| Physics | 2D Rigidbody |
| Audio | Процедурная + WAV |
| Min Android API | 24 |

---

## Структура скриптов

```
SoraKobo.Player
  ├── PlayerController        — движение, прыжок, взаимодействие
  ├── PlayerCustomization     — внешний вид (sync по сети)
  ├── PlayerNameTag           — тег над головой
  └── PlayerNameTag           — билборд

SoraKobo.Building
  ├── BuildingSystem          — размещение/удаление блоков (networked)
  ├── PlacedBlock             — компонент одного блока
  ├── BlockPalette            — UI-палитра блоков
  ├── BuildTouchInput         — тач-ввод для строительства
  ├── MapSerializer           — JSON сохранение/загрузка
  ├── FurniturePlacer         — размещение мебели-prefabs
  └── WorldGenerator          — процедурный мир при запуске

SoraKobo.Interaction
  ├── InteractableObject      — базовый класс
  ├── InteractionManager      — реестр интерактивных объектов
  ├── ChairObject             — стул
  ├── DoorObject              — дверь
  ├── PianoObject             — пианино
  ├── SwingObject             — качели
  ├── FoodObject              — еда
  ├── VehicleObject           — транспорт
  └── MusicInstrument         — музыкальный инструмент

SoraKobo.Multiplayer
  ├── SoraNetworkManager      — хост/клиент, коллбэки
  ├── GameBootstrap           — точка входа игры
  ├── PlayerSyncExtras        — дополнительная сетевая синхронизация
  └── ServerBrowser           — браузер серверов (прямое IP)

SoraKobo.UI
  ├── UIManager               — переключение экранов
  ├── MainMenuUI              — главное меню
  ├── HUDController           — игровой HUD
  ├── CharacterCustomizationUI— кастомизация
  ├── MapEditorUI             — редактор карт
  ├── ChatUI                  — чат
  ├── PianoUI                 — клавиатура пианино
  ├── EmoteWheelUI            — колесо эмоций
  ├── FloatingJoystick        — виртуальный джойстик
  ├── ResolutionAdapter       — адаптация под разрешение
  ├── LoadingScreen           — экран загрузки
  ├── SplashScreen            — сплеш-экран
  ├── BackgroundAnimator      — анимация фона
  ├── StarfieldEffect         — частицы на фоне
  ├── MobileButtonAnimator    — анимация кнопок
  ├── SettingsUI              — настройки
  └── FurniturePaletteUI      — палитра мебели

SoraKobo.MapEditor
  └── MapEditorController     — полная логика редактора карт

SoraKobo.Camera
  └── CameraFollow            — следование за игроком

SoraKobo.Audio
  ├── AudioManager            — музыка + SFX
  └── ProceduralAudioGenerator— генерация звуков без WAV файлов

SoraKobo.Data
  ├── BlockData               — структуры данных блоков
  ├── MapSaveData             — структура сохранения карты
  └── PlayerSaveData          — структура сохранения игрока
```
