#!/bin/sh
# From the tutorial's first showcase: dismiss it and the controls tutorial, fight by
# spamming attacks, then page through the encounter until the map is back.
H="python $(dirname "$0")/hof2.py"
$H keys enter
$H until "Dialogue_Controller" 30
sleep 2; $H keys enter
for i in $(seq 1 40); do
  $H keys lmb*10 > /dev/null
  if $H log 200 | grep -q "Level_Out"; then break; fi
done
$H until "say: Continuar" 60
for i in $(seq 1 12); do
  $H keys enter > /dev/null
  sleep 3
  if $H log 30 | grep -q "recon state=GameState_Map Primary=Map Secondary"; then break; fi
done
$H say 6
