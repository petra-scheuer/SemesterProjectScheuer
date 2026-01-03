#!/usr/bin/env bash

BASE_URL="http://localhost:10001"
USER="testuser_$RANDOM"
PASS="1234"

echo "== Test /users/register =="
curl -X POST "$BASE_URL/users/register" \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"$USER\",\"password\":\"$PASS\"}" \
  -v

echo
echo "== Test /users/login =="
TOKEN=$(curl -s -X POST "$BASE_URL/users/login" \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"$USER\",\"password\":\"$PASS\"}")

echo "TOKEN=$TOKEN"

echo
echo "== Test /media (register media) =="
curl -X POST "$BASE_URL/media/register" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "title": "New test",
    "description": "Minimal curl test",
    "mediaType": "Movie",
    "releaseYear": 2024,
    "genres": ["Sci-Fi"],
    "ageRestriction": 16
  }' \
  -v
echo
echo "== Test /media/{id} (GET media by ID) =="

curl -s -X GET "$BASE_URL/media/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"MediaId": 1}' -v
  
echo "== Test /media/all (GET all media) =="
curl -s -X GET "$BASE_URL/media/all" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" 

echo
echo "== Filter: title=New (sollte mind. 1 Ergebnis enthalten) =="
curl -s -X GET "$BASE_URL/media/filter?title=New" \
  -H "Authorization: Bearer $TOKEN"

echo
echo "== Filter: ageRestriction=16 (sollte mind. 1 Ergebnis enthalten) =="
curl -s -X GET "$BASE_URL/media/filter?ageRestriction=16" \
  -H "Authorization: Bearer $TOKEN"

echo
echo "== Filter: releaseYear=2024 (sollte mind. 1 Ergebnis enthalten) =="
curl -s -X GET "$BASE_URL/media/filter?releaseYear=2024" \
  -H "Authorization: Bearer $TOKEN"

echo
echo "== Kombi-Filter: title=New & ageRestriction=16 & releaseYear=2024 (sollte 1 Ergebnis enthalten) =="
curl -s -X GET "$BASE_URL/media/filter?title=New&ageRestriction=16&releaseYear=2024" \
  -H "Authorization: Bearer $TOKEN"

echo
echo "== Kombi-Filter NEGATIV: title=New & ageRestriction=99 (sollte LEER sein) =="
curl -s -X GET "$BASE_URL/media/filter?title=New&ageRestriction=99" \
  -H "Authorization: Bearer $TOKEN"

echo "== Test /media (change media) =="
curl -X PUT "$BASE_URL/media/register" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "MediaId": 1,
    "title": "New Changed Title",
    "description": "Minimal curl test",
    "mediaType": "Movie",
    "releaseYear": 2024,
    "genres": ["Sci-Fi"],
    "ageRestriction": 16
  }' \
  -v
    
echo "== Test /media/{id} (DELETE media by ID) =="

curl -s -X DELETE "$BASE_URL/media/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"MediaId": 1}' -v
  