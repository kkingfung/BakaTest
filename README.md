# BakaTest - Educational Quiz Battle Game

**BakaTest** is an educational quiz-based battle game built with Unity. Players answer questions from various subjects (Math, Science, English, History) to power their champions in strategic turn-based battles.

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Project Setup](#project-setup)
3. [Project Structure](#project-structure)
4. [Running the Game](#running-the-game)
5. [Unity Editor Tools](#unity-editor-tools)
6. [Architecture Overview](#architecture-overview)
7. [Troubleshooting](#troubleshooting)
8. [Documentation](#documentation)

---

## Prerequisites

### Required Software
- **Unity 6000.0.9f1** (Unity 6 - 2024 LTS)
  - Download from [Unity Hub](https://unity.com/download)
  - Ensure you select Unity version **6000.0.9f1** specifically
- **Git** (for cloning the repository)
- **Visual Studio 2022** or **JetBrains Rider** (recommended IDEs)

### Unity Modules Required
When installing Unity 6000.0.9f1, make sure to include:
- Windows Build Support (or your target platform)
- Visual Studio Community (if you don't have an IDE)

---

## Project Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd BakaTest
```

### 2. Open in Unity

1. Open **Unity Hub**
2. Click **"Open"** or **"Add"**
3. Navigate to the cloned repository folder
4. Select the **`BakaTest/BakaTest`** folder (the inner BakaTest folder)
5. Click **"Open"**

Unity will import the project and compile all scripts. This may take a few minutes on first import.

### 3. Verify Project Settings

After Unity opens:

1. Go to **Edit → Project Settings → Player**
2. Verify **Company Name** and **Product Name** are set correctly
3. Check **Scripting Backend** (recommended: IL2CPP for builds, Mono for development)
4. Verify **API Compatibility Level**: .NET Standard 2.1

### 4. Package Installation

The project uses Unity's Package Manager. Required packages should auto-install:
- **UI Toolkit** (built-in)
- **Input System** (com.unity.inputsystem)
- **TextMeshPro** (com.unity.textmeshpro)
- **2D Sprite** (com.unity.2d.sprite)
- **2D Animation** (com.unity.2d.animation)

If packages are missing:
1. Go to **Window → Package Manager**
2. Search for the missing package
3. Click **Install**

---

## Project Structure

```
BakaTest/
├── Assets/
│   ├── Resources/
│   │   └── Data/
│   │       ├── Champions/          # Champion ScriptableObjects
│   │       ├── Items/              # Item ScriptableObjects
│   │       ├── QuestionBanks/      # 16 question banks (4 subjects × 4 difficulties)
│   │       └── Localization/       # Language data (Japanese/English)
│   ├── Scenes/
│   │   ├── Startup.unity           # Game initialization scene (START HERE)
│   │   ├── MainMenu.unity
│   │   ├── TestSelection.unity
│   │   ├── TestTaking.unity
│   │   ├── PointAllocation.unity
│   │   ├── ChampionSelection.unity
│   │   ├── Battle.unity
│   │   └── Results.unity
│   ├── Scripts/
│   │   ├── Bootstrap/              # GameBootstrap (BakaTest.Bootstrap.asmdef)
│   │   ├── Core/                   # MVVM base classes (BakaTest.Core.asmdef)
│   │   ├── Data/                   # Data models (BakaTest.Data.asmdef)
│   │   ├── Services/               # Service layer (BakaTest.Services.asmdef)
│   │   ├── ViewModels/             # ViewModels (BakaTest.ViewModels.asmdef)
│   │   ├── Views/                  # Views (BakaTest.Views.asmdef)
│   │   ├── UI/                     # UI utilities (BakaTest.UI.asmdef)
│   │   └── Editor/                 # Editor scripts (BakaTest.Editor.asmdef)
│   └── UI/                         # UXML and USS files
├── ProjectSettings/
└── README.md (this file)
```

### Assembly Definitions

The project uses **8 assembly definitions** for modular compilation:

1. **BakaTest.Data** - Data models (no dependencies)
2. **BakaTest.Core** - MVVM framework, ServiceLocator
3. **BakaTest.Services** - Service implementations
4. **BakaTest.ViewModels** - UI ViewModels
5. **BakaTest.Views** - UI Views
6. **BakaTest.UI** - UI utilities (animations, loading screen)
7. **BakaTest.Bootstrap** - Game initialization
8. **BakaTest.Editor** - Unity Editor tools (Editor-only)

This provides:
- Fast incremental compilation
- Clear dependency boundaries
- Better code organization

---

## Running the Game

### First Time Setup

1. **Open the Startup Scene**
   - In Unity Editor, navigate to **Assets/Scenes/Startup.unity**
   - Double-click to open it
   - This is the entry point scene

2. **Press Play**
   - Click the **Play button** in Unity Editor
   - The game will initialize services and load the Main Menu

### Game Flow

```
Startup → MainMenu → TestSelection → TestTaking → PointAllocation 
                                                       ↓
                      Results ← Battle ← ChampionSelection
```

1. **Startup**: Initializes ServiceLocator and registers all services
2. **MainMenu**: Start game, access settings, shop, inventory
3. **TestSelection**: Choose subject and difficulty
4. **TestTaking**: Answer quiz questions
5. **PointAllocation**: Allocate earned points to champion stats
6. **ChampionSelection**: Select your champion for battle
7. **Battle**: Turn-based battle using allocated stats
8. **Results**: View battle results and rewards

### Testing Specific Features

To test individual scenes:
1. Open the desired scene (e.g., `Battle.unity`)
2. Make sure **Startup.unity** is included in **Build Settings** (File → Build Settings)
3. Services may not be initialized if you skip Startup scene

---

## Unity Editor Tools

The project includes comprehensive Editor tools under the **Tools** menu:

### Data Generators

**Tools → BakaTest → Generate Champion Data**
- Creates champion ScriptableObjects with predefined stats
- Automatically sets up subject affinities (Math→Attack, Science→Defense, etc.)

**Tools → BakaTest → Generate Item Data**
- Creates item ScriptableObjects (HP Potion, Revive, Strength Boost, etc.)
- Sets up item effects and costs

**Tools → BakaTest → Generate Question Banks**
- Creates 16 question bank files (4 subjects × 4 difficulties)
- Each bank contains 25 unique questions
- Automatically organized by subject and difficulty

**Tools → BakaTest → Generate Localization Data**
- Sets up Japanese and English localization files
- Creates translation entries for UI text

### Scene Setup Utilities

**Tools → BakaTest → Setup → Setup All Scenes**
- Automatically sets up all scenes with proper UI Document references
- Assigns UXML/USS files to UI Documents
- Configures View components

**Tools → BakaTest → Setup → Setup [SceneName]**
- Individual scene setup utilities:
  - Setup Main Menu
  - Setup Battle Scene
  - Setup Test Taking Scene
  - Setup Point Allocation Scene
  - Setup Champion Selection Scene
  - Setup Champion Shop Scene
  - Setup Inventory Scene
  - Setup Test Results Scene
  - Setup Settings Scene

### Debug Tools

**Tools → BakaTest → Debug → Force Recompile**
- Forces Unity to recompile all scripts
- Useful for resolving compilation issues

**Tools → BakaTest → Debug → Print Service Status**
- Logs the status of all registered services
- Helps debug ServiceLocator issues

---

## Architecture Overview

### MVVM Pattern

The project uses **Model-View-ViewModel (MVVM)** architecture:

- **Model**: Data classes and Services (business logic)
- **View**: UI Toolkit Views (inherits from `ViewBase<TViewModel>`)
- **ViewModel**: UI logic (inherits from `ViewModelBase`)

**Example:**
```csharp
// ViewModel
public class MainMenuViewModel : ViewModelBase
{
    private readonly ISceneManagementService _sceneService;
    
    public ICommand StartGameCommand { get; }
    
    public MainMenuViewModel(ISceneManagementService sceneService)
    {
        _sceneService = sceneService;
        StartGameCommand = new RelayCommand(ExecuteStartGame);
    }
    
    private void ExecuteStartGame() => _sceneService.LoadTestSelection();
}

// View
public class MainMenuView : ViewBase<MainMenuViewModel>
{
    protected override void Awake()
    {
        base.Awake();
        var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
        SetViewModel(new MainMenuViewModel(sceneService));
    }
    
    protected override void BindViewModel(MainMenuViewModel viewModel)
    {
        base.BindViewModel(viewModel);
        _startButton.clicked += () => viewModel.StartGameCommand.Execute(null);
    }
}
```

### ServiceLocator Pattern

Services are registered in **GameBootstrap.cs** on game start:

```csharp
ServiceLocator.Instance.Register<IQuestionBankService>(new QuestionBankService());
ServiceLocator.Instance.Register<ITestService>(new TestService());
ServiceLocator.Instance.Register<IBattleService>(new BattleService());
// ... etc
```

**Accessing services:**
```csharp
var testService = ServiceLocator.Instance.Get<ITestService>();
```

### UI Toolkit

All UI is built with **Unity's UI Toolkit**:
- **UXML** files define structure (Assets/UI/)
- **USS** files define styling (Assets/UI/)
- **C# Views** handle logic and data binding

---

## Troubleshooting

### Common Issues

#### 1. "Assembly definition file not found" errors

**Solution:**
- Ensure all 8 .asmdef files exist in their respective folders
- Delete the `Library` folder and let Unity reimport
- Go to **Assets → Reimport All**

#### 2. "ServiceLocator has not been initialized" error

**Cause:** You're testing a scene without starting from Startup.unity

**Solution:**
- Always start from **Startup.unity** scene
- Or manually initialize services in your test scene

#### 3. Missing UI references (null UIDocument)

**Solution:**
- Use the Scene Setup Utilities: **Tools → BakaTest → Setup → Setup [Scene Name]**
- Or manually assign UIDocument component in Inspector

#### 4. Question banks are empty or have duplicate questions

**Solution:**
- Question banks have been fully populated with 25 unique questions each
- If you see issues, regenerate: **Tools → BakaTest → Generate Question Banks**

#### 5. Compilation errors after pulling latest changes

**Solution:**
1. Close Unity
2. Delete the `Library` folder
3. Reopen Unity and let it reimport everything
4. If issues persist: **Tools → BakaTest → Debug → Force Recompile**

#### 6. Items or Champions not appearing in game

**Cause:** ScriptableObjects may not be generated

**Solution:**
- Generate champions: **Tools → BakaTest → Generate Champion Data**
- Generate items: **Tools → BakaTest → Generate Item Data**
- Check `Assets/Resources/Data/Champions/` and `Assets/Resources/Data/Items/`

---

## Documentation

Additional documentation files:

- **CLAUDE.md** - Development guide and coding conventions (Japanese)
- **IMPLEMENTATION_STATUS.md** - Current implementation status
- **ITEM_SYSTEM_IMPLEMENTATION.md** - Item system documentation
- **QUESTION_BANK_SYSTEM.md** - Question bank structure and usage
- **LOCALIZATION_GUIDE.md** - Multi-language system guide
- **TROUBLESHOOTING.md** - Extended troubleshooting guide
- **ASSET_CREATION_GUIDE.md** - Guide for creating game assets
- **PROJECT_STATUS.md** - Project completion status

---

## Game Features

### Subjects & Difficulties

**4 Subjects:**
- Math (数学)
- Science (理科)
- English (英語)
- History (歴史)

**4 Difficulty Levels:**
- Elementary (小学校) - 15 questions
- MiddleSchool (中学校) - 20 questions
- HighSchool (高校) - 25 questions
- University (大学) - 30 questions

### Subject Affinities

Each subject boosts specific stats:
- **Math** → Attack
- **Science** → Defense
- **English** → Speed
- **History** → HP

### Champion Elements

Champions have elemental affinities:
- Fire (炎)
- Water (水)
- Earth (地)
- Wind (風)
- Light (光)
- Dark (闇)

Element advantages provide damage multipliers in battle.

### Battle System

- Turn-based combat
- Item usage (HP Potion, Revive, stat boosts)
- Status effects (Attack Boost, Defense Boost, Speed Boost)
- AI opponent with strategic behavior

---

## Build Instructions

### Creating a Build

1. **Go to File → Build Settings**
2. **Add Scenes** in this order:
   - Startup
   - MainMenu
   - TestSelection
   - TestTaking
   - PointAllocation
   - ChampionSelection
   - Battle
   - Results
   - Settings
   - ChampionShop
   - Inventory
   - TestResults

3. **Select Platform** (Windows, Mac, Linux, etc.)
4. **Click "Build"** and choose output folder
5. **Run the executable** from the build folder

### Build Settings Recommendations

- **Development Build**: Enable for testing
- **Script Debugging**: Enable for debugging
- **Compression Method**: LZ4 (faster) or Default (smaller)
- **Scripting Backend**: IL2CPP (faster runtime) or Mono (faster builds)

---

## Contributing

When contributing to this project:

1. Follow the **MVVM pattern** for UI code
2. Use **ServiceLocator** for dependency injection
3. Write **Japanese comments** in code (see CLAUDE.md)
4. Create/update assembly definitions when adding new folders
5. Use Editor tools to regenerate data when needed
6. Test from **Startup.unity** scene
7. Update documentation when adding new features

---

## License

[Specify your license here]

---

## Credits

- **Unity Version**: 6000.0.9f1
- **UI Framework**: Unity UI Toolkit
- **Architecture**: MVVM with ServiceLocator

---

## Contact

[Add contact information or links here]
