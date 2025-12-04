# Pokemon Battle API Documentation

RESTful API for Pokemon Battle game built with C# .NET 9.0 and ASP.NET Core.

## Base URL
```
http://localhost:5000/api
```

## API Endpoints

### User Management

#### Create User
```http
POST /api/user
Content-Type: application/json

{
  "name": "Ash",
  "gender": "Male"
}
```
**Response:** 201 Created
```json
{
  "id": "uuid",
  "name": "Ash",
  "gender": "Male",
  "pokemon": []
}
```

#### Get All Users
```http
GET /api/user
```
**Response:** 200 OK
```json
[
  {
    "id": "uuid",
    "name": "Ash",
    "gender": "Male",
    "pokemon": [...]
  }
]
```

#### Get User by ID
```http
GET /api/user/{id}
```
**Response:** 200 OK | 404 Not Found

#### Update User
```http
PUT /api/user/{id}
Content-Type: application/json

{
  "name": "Ash Ketchum",
  "gender": "Male"
}
```
**Response:** 200 OK | 404 Not Found

#### Delete User
```http
DELETE /api/user/{id}
```
**Response:** 204 No Content | 404 Not Found

### Pokemon Management (User)

#### Add Pokemon to User
```http
POST /api/user/{id}/pokemon
Content-Type: application/json

{
  "pokemonType": "Pikachu",
  "name": "Sparky",
  "level": 25
}
```
**Response:** 200 OK

#### Remove Pokemon from User
```http
DELETE /api/user/{id}/pokemon/{pokemonIndex}
```
**Response:** 200 OK | 400 Bad Request

#### Get User's Pokemon
```http
GET /api/user/{id}/pokemon
```
**Response:** 200 OK
```json
[
  {
    "name": "Pikachu",
    "type": "Electric",
    "level": 25,
    "currentHitPoint": 100,
    "maxHitPoint": 100,
    "attack": 55,
    "defense": 40,
    "speed": 90
  }
]
```

#### Get Available Pokemon Types
```http
GET /api/user/pokemon/types
```
**Response:** 200 OK
```json
[
  "Pikachu",
  "Charmander",
  "Bulbasaur",
  "Squirtle",
  "Gengar",
  "Mewtwo",
  "Snorlax",
  "Aron"
]
```

### Pokemon Information

#### Get All Playable Pokemon
```http
GET /api/pokemon/playable
```
**Response:** 200 OK
```json
{
  "count": 8,
  "pokemon": [...]
}
```

#### Get Pokemon by Name
```http
GET /api/pokemon/playable/{name}
```
**Response:** 200 OK | 404 Not Found

#### Get Available Types
```http
GET /api/pokemon/types
```
**Response:** 200 OK

#### Filter Pokemon by Type
```http
GET /api/pokemon/filter/type/{type}
```
**Response:** 200 OK | 404 Not Found

#### Sort Pokemon by Stats
```http
GET /api/pokemon/stats/{stat}
```
**Parameters:** stat = `hp`, `attack`, `defense`, `speed`
**Response:** 200 OK

#### Get Pokemon Statistics
```http
GET /api/pokemon/statistics
```
**Response:** 200 OK

#### Check Pokemon Exists
```http
GET /api/pokemon/exists/{name}
```
**Response:** 200 OK
```json
{
  "name": "Pikachu",
  "exists": true
}
```

### Battle System

#### Start Battle
```http
POST /api/battle/start
Content-Type: application/json

{
  "player1Id": "uuid1",
  "player2Id": "uuid2"
}
```
**Response:** 200 OK
```json
{
  "battleId": "battle-uuid",
  "player1": {...},
  "player2": {...},
  "currentTurn": 1
}
```

#### Execute Attack
```http
POST /api/battle/{battleId}/attack
Content-Type: application/json

{
  "isSpecialAttack": false
}
```
**Response:** 200 OK

#### Use Special Attack
```http
POST /api/battle/{battleId}/special-attack
```
**Response:** 200 OK

#### Switch Pokemon
```http
POST /api/battle/{battleId}/switch
Content-Type: application/json

{
  "playerNumber": 1,
  "pokemonIndex": 2
}
```
**Response:** 200 OK

#### Get Battle State
```http
GET /api/battle/{battleId}
```
**Response:** 200 OK

## Validation Rules

### User Creation/Update
- **Name:** 3-20 characters, alphanumeric and spaces only
- **Gender:** Must be "Male", "Female", or "Unknown"
- **ID Format:** Must be valid GUID

### Pokemon
- **Type:** Must be from available Pokemon types
- **Level:** Positive integer
- **Index:** Valid array index (0-5)

## Error Responses

### 400 Bad Request
```json
{
  "error": "Validation error message"
}
```

### 404 Not Found
```json
{
  "error": "Resource not found message"
}
```

### 500 Internal Server Error
```json
{
  "error": "Operation failed",
  "message": "Detailed error message"
}
```

## Database

- **Type:** SQLite
- **Location:** `backend/PokemonBattle.db`
- **ORM:** Entity Framework Core
- **Auto-migration:** Database created on first run