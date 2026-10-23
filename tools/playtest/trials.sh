#!/bin/sh
# trials.sh MODE N: launch the game N times and report whether each reached the
# start screen. MODE is vanilla (BepInEx proxy disabled) or modded.
G="/s/steam/steamapps/common/Hand of Fate 2"
H="python $(dirname "$0")/hof2.py"
[ "$1" = vanilla ] && mv "$G/winhttp.dll" "$G/winhttp.dll.off"
for i in $(seq 1 $2); do
  $H kill >/dev/null 2>&1
  python -c "import os; os.startfile('steam://rungameid/456670')"
  python - <<PY
import os,time
p=os.path.expandvars(r'%USERPROFILE%\AppData\LocalLow\Defiant Development\Hand of Fate 2\output_log.txt')
time.sleep(5); t0=time.time(); ok=False
while time.time()-t0<100:
    time.sleep(2)
    try: s=open(p,'rb').read()
    except: continue
    if b'menu:StartScreen_Idle' in s: ok=True; break
print('$1 run $i', 'ok' if ok else 'HANG', round(time.time()-t0))
PY
done
$H kill >/dev/null 2>&1
[ "$1" = vanilla ] && mv "$G/winhttp.dll.off" "$G/winhttp.dll"
