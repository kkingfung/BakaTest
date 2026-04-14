# Available Slash Commands

## Quick Reference

| Command | Purpose | Category |
|---------|---------|----------|
| `/create-viewmodel` | Generate new ViewModel class | MVVM |
| `/create-view` | Generate new View class | MVVM |
| `/create-service` | Generate service interface + implementation | Architecture |
| `/create-champion` | Generate champion ScriptableObject | Game System |
| `/create-test-system` | Generate test system components | Game System |
| `/review-mvvm` | Review MVVM implementation | Code Quality |

## Usage Examples

### Create a complete MVVM feature
```
1. /create-viewmodel
   → Enter "TestSelection"
   → Specify needed services

2. /create-view
   → Enter "TestSelection"
   → Specify UI elements needed

3. /review-mvvm
   → Review both files
   → Apply suggestions
```

### Create a new service
```
/create-service
→ Enter service name (e.g., "BattleManagement")
→ Specify service responsibilities
```

### Create game data
```
/create-champion
→ Enter champion name and details

/create-test-system
→ Select which component to implement
```

## Tips

- Use Tab to autocomplete command names
- Commands follow project coding conventions from CLAUDE.md
- All generated code includes Japanese comments
- Commands are interactive - Claude will ask for details

---

For detailed documentation, see `.claude/README.md`
