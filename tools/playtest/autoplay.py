"""Crude autoplay for exploration: drives the game forward for N steps so the mod's
speech across many screens can be reviewed afterwards from the log.

  python autoplay.py STEPS

Each step looks at the latest game state (from the mod's recon log lines) and acts:
in a fight it attacks (left mouse) and dodges now and then; anywhere else it presses
Enter (confirm the focused option, continue a page, move to the focused map card).
Stops early when the run ends (scoreboard / start screen) or the game window is gone.
"""
import re, sys, time, os
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import hof2

def state():
    lines = hof2.read_since(max(0, hof2.log_size() - 200000))
    for l in reversed(lines):
        m = re.search(r"recon state=(\S+) Primary=(\S*) Secondary=(\S*) Dialogue=(\S*)", l)
        if m:
            return m.groups()
    return ("none", "", "", "")

REMOTE = "--remote" in sys.argv
_last_spoken = None
_stale = 0

def act(key):
    """Press a key: through the window when it can take focus, else through the
    mod's playtest remote."""
    if REMOTE:
        try:
            hof2.focus(); hof2.tap(key); return
        except SystemExit:
            pass
        names = {"enter": "button:Select", "f3": "button:F3", "space": None}
        cmd = names.get(key)
        if cmd: hof2.cmd_remote([cmd])
        return
    hof2.focus()
    hof2.tap(key)

_visits = {}

def pick_map_card():
    """Look in each direction; take an unfinished card if there is one, else the exit
    (unless standing on it), else the least-visited neighbour, so the walk explores
    instead of circling finished cards."""
    options = []
    for d in ["Up", "Down", "Left", "Right"]:
        off = hof2.log_size()
        hof2.cmd_remote(["nav:" + d])
        time.sleep(0.4)
        lines = [l for l in hof2.read_since(off) if "] say" in l]
        if not lines: continue
        text = lines[-1].split("] say", 1)[1]
        name = text.split(",")[0]
        if "aqu" in text: continue
        score = _visits.get(name, 0)
        if "complet" not in text: score -= 100
        if "duendes" in text or "descuidero" in text: score -= 1000
        elif "salida" in text: score -= 50
        options.append((score, d, name))
    if options:
        options.sort()
        _, d, name = options[0]
        _visits[name] = _visits.get(name, 0) + 1
        hof2.cmd_remote(["nav:" + d])
        time.sleep(0.4)
    act("enter")
    time.sleep(3)

def pick_challenge():
    for _ in range(12):
        off = hof2.log_size()
        hof2.cmd_remote(["nav:Right"])
        time.sleep(0.5)
        lines = [l for l in hof2.read_since(off) if "] say" in l]
        text = lines[-1] if lines else ""
        if text and not any(w in text for w in ("ficha", "token", "bloque", "locked", "complet")):
            break
    hof2.cmd_remote(["button:Select"])
    time.sleep(6)

def main():
    sys.argv = [a for a in sys.argv if a != "--remote"]
    steps = int(sys.argv[1]) if len(sys.argv) > 1 else 50
    for i in range(steps):
        if not hof2.find_window():
            print("game window gone"); return
        st, primary, secondary, dialogue = state()
        if "StartScreen" in primary and not REMOTE:
            print("back at start screen"); return
        # The deck builder starts the challenge on F3; Enter only edits decks.
        if REMOTE and "Deckbuilder" in primary and not dialogue:
            # Fill the decks with the game's recommendation (F2, confirm), then start.
            hof2.cmd_remote(["button:F2", "wait:2", "button:Select", "wait:3", "button:F3"])
            time.sleep(5)
            continue
        # The challenge map: move along the challenge row to one not yet won.
        if REMOTE and primary == "ChallengeMap":
            pick_challenge()
            continue
        # The add-on panels on the challenge map close with Cancel.
        if REMOTE and "AddonContent" in primary:
            hof2.cmd_remote(["button:Cancel"])
            time.sleep(3)
            continue
        if st == "GameState_Level_Play" and not dialogue and REMOTE:
            time.sleep(4)
            hof2.cmd_remote(["kill"])
            time.sleep(3)
            continue
        if st == "GameState_Level_Play" and not dialogue:
            hof2.focus()
            for _ in range(8):
                hof2.mouse_button("lmb"); time.sleep(0.15)
            hof2.tap("space")
            continue
        # On the map, Enter moves to the focused card; walking back over finished cards
        # loops forever. Look around (remote only) for the exit or an unfinished card.
        if REMOTE and st == "GameState_Map" and primary == "Map":
            pick_map_card()
            continue
        # Character customisation finishes on F3, not Enter; Enter there only cycles
        # the carousel. Detect it by the slot prompt repeating.
        spoken = [l for l in hof2.read_since(max(0, hof2.log_size() - 20000)) if "] say" in l][-6:]
        # Choosing dice to reroll: marking toggles on Enter; F3 rerolls.
        if spoken and sum("marcado para volver" in l or "marked for reroll" in l for l in spoken) >= 2                 and ("dado " in spoken[-1] or "die " in spoken[-1]):
            act("f3")
            time.sleep(3)
            continue
        if sum("Elige una apariencia" in l or "Choose" in l for l in spoken) >= 3:
            act("f3")
            time.sleep(3)
            continue
        # Nothing new said for a few steps: the screen may want a hotkey (F3 continues
        # most of the game's own prompts).
        global _last_spoken, _stale
        cur = spoken[-1] if spoken else None
        _stale = _stale + 1 if cur == _last_spoken else 0
        _last_spoken = cur
        if REMOTE and _stale >= 4:
            act("f3")
            _stale = 0
            time.sleep(3)
            continue
        # Going round in circles (an inn's menu, a shop): choose a different option.
        recent = [l.split("] say", 1)[1] for l in hof2.read_since(max(0, hof2.log_size() - 40000)) if "] say" in l][-12:]
        if "Encounter" in primary and "Dice" not in primary and recent and max(recent.count(x) for x in recent) >= 3:
            # Walk the options: take one that moves on ("next level"), else the last
            # (usually "leave").
            texts = []
            for _ in range(6):
                off = hof2.log_size()
                hof2.cmd_remote(["nav:Down"])
                time.sleep(0.4)
                got = [l.split("] say", 1)[1] for l in hof2.read_since(off) if "] say" in l]
                texts.append(got[-1] if got else "")
            moves = [i for i, t in enumerate(texts) if "siguiente nivel" in t or "next level" in t]
            if not moves:
                moves = [i for i, t in enumerate(texts) if "sin explorar" in t or "unexplored" in t.lower()]
            if not moves:
                moves = [i for i, t in enumerate(texts) if any(w in t for w in ("Salir", "Desp", "Marcha", "Vete", "Abandona", "Leave", "Exit"))]
            if moves:
                for _ in range(len(texts) - 1 - moves[0]):
                    hof2.cmd_remote(["nav:Up"])
                    time.sleep(0.3)
            else:
                hof2.cmd_remote(["nav:Up"])
            act("enter")
            time.sleep(2.5)
            continue
        act("enter")
        time.sleep(2.5)
    print("steps done")

if __name__ == "__main__":
    main()
