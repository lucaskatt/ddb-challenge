# ddb-challenge

This project implements a simple web api using ASP.net Core that handles managing the health of a DnD character.

## Running
Run the service in docker using
```
docker-compose up app
```

## Testing
Run unit tests in docker using
```
docker-compose run test
```

## Endpoints

#### Create a character
```
POST http://localhost:8080/api/characters
```
body
```
{
  "name": "Briv",
  "level": 5,
  "classes": [
    {
    "name":"fighter",
    "hitDiceValue":10,
    "classLevel":3
    },
    {
      "name":"wizard",
      "hitdicevalue":6,
      "classLevel":2
    }
  ],
  "stats":{
    "strength":15,
    "dexterity":12,
    "constitution":14,
    "intelligence":13,
    "wisdom":10,
    "charisma":8
  },
  "items":[
    {
      "name":"Ioun Stone of Fortitude",
      "modifier":{
        "affectedObject":"stats",
        "affectedValue":"constitution",
        "value":2
      }
    }
  ],
  "defenses":[
    {
      "type":"fire",
      "defense":"immunity"
    },
    {
      "type":"slashing",
      "defense":"resistance"
    }
  ]
}
```

#### Get a character
```
GET http://localhost:8080/api/characters/{id}
```

#### Attack a character
```
POST http://localhost:8080/api/characters/{id}/attack
```
body
```
{
  "damage": 6,
  "type": "slashing"
}
```

#### Heal a character
```
POST http://localhost:8080/api/characters/{id}/heal
```
body
```
{
  "hp": 3
}
```

#### Add temporary hp to a character
```
POST http://localhost:8080/api/characters/{id}/temphp
```
body
```
{
  "hp": 5
}
```