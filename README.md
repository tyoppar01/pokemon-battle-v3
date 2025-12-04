# Pokemon Battle V3 🎮

Terminal-style Pokemon battle game with C# backend and JavaScript frontend.

## 📁 Folder Structure

```
pokemon-battle-v3/
├── backend/          # C# .NET 9.0 API (SQLite database)
│   ├── Controllers/  # API endpoints
│   ├── Models/       # Pokemon & Character classes
│   ├── Services/     # Business logic
│   └── Data/         # Database context
├── frontend/         # HTML/CSS/JS interface
│   ├── js/          # JavaScript modules
│   ├── styles/      # Terminal-style CSS
│   └── index.html   # Main UI
└── main.sh          # Startup script
```

## 🚀 Quick Start

Run the startup script:
```bash
chmod +x main.sh
./main.sh
```

Access the game at **http://localhost:8000**

## 🎯 How to Play

### Create a Trainer
1. Select "Create New Trainer" from main menu
2. Enter name and gender
3. Choose up to 6 Pokemon from 8 available types
4. Click "Create Trainer" to save

### Start a Battle
1. Select "Start Battle" from main menu
2. Choose two different trainers from dropdowns
3. Review their Pokemon teams in preview
4. Click "Start Battle"
5. Choose "Fight" → Select attack (Normal/Special)
6. Battle continues until all trainer's Pokemon faint

### Manage Trainers
- **View All Trainers**: See all created trainers and their teams
- **Delete Trainer**: Click ✕ button on trainer card
- **View Pokemon**: Browse all 8 playable Pokemon types

## 🔊 Features
- Retro terminal-style UI
- Pokemon sound effects on all interactions
- Turn-based battle system with type effectiveness
- SQLite persistent storage
- RESTful API architecture
