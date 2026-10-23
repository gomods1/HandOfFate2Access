"""Walk the game forward, printing everything the mod says, and fight by ear.

  python walk.py STEPS [deaf]

Each step: in a fight, hand over to earbot; otherwise press Enter (continue a page,
take the focused option, move to the focused map card, pick the focused slot). Prints
every spoken line and screen transition as it goes, and stops at screens that want a
human (dice, pendulum, wheel, shop, deck builder, challenge map, results).
"""
import os, re, sys, time
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import hof2, earbot

STOP_AT = ("Pendulum", "Wheel", "Deckbuilder", "ChallengeMap", "StartScreen")

def state():
    for l in reversed(hof2.read_since(max(0, hof2.log_size() - 200000))):
        m = re.search(r"recon state=(\S+) Primary=(\S*) Secondary=(\S*) Dialogue=(\S*)", l)
        if m: return m.groups()
    return ("none", "", "", "")

_visits = {}

def spoken_after(off):
    out = []
    for l in hof2.read_since(off):
        i = l.find("[HoF2Access] say")
        if i >= 0: out.append(l[i + 17:])
    return out

def pick_map_card():
    """Arrows move the game's selection from the selected card, not from the player, so
    step the selection around and take the first reachable card that is not finished
    (or the exit); after a round with nothing better, take any reachable card."""
    fallback = None
    for d in ("up", "right", "down", "left", "up", "left", "down", "right"):
        off = hof2.log_size()
        hof2.focus(); hof2.tap(d)
        time.sleep(0.5)
        lines = spoken_after(off)
        if not lines: continue
        text = lines[-1]
        if "reachable" not in text and "alcanzable" not in text: continue
        if ("completed" not in text and "completada" not in text) or "exit" in text or "salida" in text:
            print("  -> %s: %s" % (d, text))
            hof2.tap("enter"); time.sleep(3); return
        fallback = (d, text)
    if fallback:
        print("  -> (finished) %s" % fallback[1])
    hof2.tap("enter"); time.sleep(3)

def main():
    steps = int(sys.argv[1]) if len(sys.argv) > 1 else 30
    deaf = "deaf" in sys.argv
    for i in range(steps):
        st, primary, secondary, dialogue = state()
        if st == "GameState_Level_Play" and not dialogue:
            if "--nofight" in sys.argv:
                print("stop: fight started"); return
            print("-- fight --")
            earbot.fight(deaf=deaf, max_seconds=240)
            continue
        if primary.endswith("Shop"):
            # A shop: leave it (Tab) rather than spend everything.
            hof2.focus(); hof2.tap("tab"); time.sleep(2)
            continue
        if any(k in primary for k in STOP_AT):
            print("stop: screen %s" % primary); return
        if st == "GameState_Map" and primary == "Map":
            pick_map_card()
            continue
        # Going round in circles (a shop, a repeatable stall): the same line said three
        # times lately means the first option loops; take the last option instead.
        recent = [l for l in spoken_after(max(0, hof2.log_size() - 30000))[-14:] if len(l) > 24]
        if "Encounter" in primary and recent and max(recent.count(x) for x in recent) >= 3:
            hof2.focus()
            texts = []
            cursor = -1
            for _ in range(6):
                o = hof2.log_size(); hof2.tap("down"); time.sleep(0.4)
                got = spoken_after(o)
                texts.append(got[-1] if got else "")
                cursor = len(texts) - 1
                if len(texts) > 1 and texts[-1] == texts[0]:
                    texts.pop(); cursor = 0; break
            leave = [i for i, t in enumerate(texts) if any(w in t for w in ("Break camp", "Leave", "Depart", "Exit", "Move on", "Continue", "Go ", "Finish", "Levantar", "Marchar", "Salir", "Irse", "Continuar", "Terminar", "Dejar"))]
            target = leave[0] if leave else len(texts) - 1
            for _ in range(abs(target - cursor)):
                hof2.tap("down" if target > cursor else "up"); time.sleep(0.3)
            print("  (loop: taking option %r)" % (texts[target] if texts else "?"))
            hof2.tap("enter"); time.sleep(3)
            continue
        off = hof2.log_size()
        # Character customisation finishes on F3 (Enter only opens a slot's carousel);
        # so does the dice reroll once a die is marked (Enter toggles the mark).
        recent = spoken_after(max(0, off - 4000))[-2:]
        reroll = "Dice" in primary and any("marked for reroll" in l or "marcado para volver" in l for l in recent)
        hof2.focus(); hof2.tap("f3" if "Customisation" in primary or reroll else "enter")
        time.sleep(3)
        for l in hof2.read_since(off):
            i1 = l.find("[HoF2Access] ")
            if i1 < 0: continue
            t = l[i1 + 13:]
            if t.startswith("say") or t.startswith("recon") or "failed" in t or "Warn" in l:
                print("  " + t[:300])
    print("steps done")

if __name__ == "__main__":
    main()
