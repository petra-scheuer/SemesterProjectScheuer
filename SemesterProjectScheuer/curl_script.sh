#!/usr/bin/env bash

BASE_URL="http://localhost:10001"
USER="testuser_$RANDOM"
PASS="1234"

echo "== Test /users/register =="
curl POST "$BASE_URL/users/register" \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"$USER\",\"password\":\"$PASS\"}" \
  -v

echo
echo "== Test /users/login =="
curl POST "$BASE_URL/users/login" \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"$USER\",\"password\":\"$PASS\"}" \
  -v