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

