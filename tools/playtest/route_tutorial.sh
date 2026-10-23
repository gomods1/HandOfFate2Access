#!/bin/sh
# Replays a fresh profile from launch to the first combat of the tutorial encounter.
H="python $(dirname "$0")/hof2.py"
$H launch
$H keys enter
$H until "say.*(Femenino|Masculino)" 120
# The cards accept input only once the dealer stops talking: retry until the map is up.
for i in $(seq 1 40); do
  $H keys enter > /dev/null
  sleep 3
  if $H log 400 | grep -q "recon state=GameState_Map Primary=Map Secondary"; then break; fi
done
$H until "recon state=GameState_Map Primary=Map>Encounter" 400
$H until "say: Continuar" 20
$H keys enter
$H until "say.*escondidas|say.*Ac.rcate" 20
$H keys enter
$H until "gambit shuffle ended" 90
$H keys enter
$H until "gambit pick revealed" 20
for i in 1 2 3 4 5 6 7 8; do
  $H until "say: (Continuar|Salta)" 20 || true
  $H keys enter
done
$H until "Showcase" 60
