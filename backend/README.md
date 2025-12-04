# Pokemon Battle (V2)

A C# console game featuring 1v1 Pokemon battles with trainers and inventory management.

## Features

- **Characters**: Trainers with Pokemon collections and inventories
- **Pokemon**: Battle-ready creatures with stats, types, and special attacks
- **Items**: Medicine for healing, Pokeballs for capturing, and Evolution stones
- **Battle System**: Turn-based combat with type advantages and catching mechanics

Built with object-oriented design using inheritance, polymorphism, and generic collections for flexible item management.

## How to Play

1. **Player 1 Setup**
   - Enter your trainer name
   - Choose your Pokemon team (minimum 1, maximum 6)

2. **Player 2 Setup**
   - Enter your trainer name
   - Choose your Pokemon team (minimum 1, maximum 6)

3. **Battle!**
   - Take turns choosing actions:
     - **FIGHT**: Use normal attack or special attack (2 skills available)
     - **POKEMON**: Switch to another alive Pokemon
   - Type advantages apply to special attacks (super effective, not very effective)
   - When a Pokemon faints, you must switch to another

4. **Victory**
   - Battle continues until one player has no Pokemon alive
   - The player with remaining Pokemon wins!