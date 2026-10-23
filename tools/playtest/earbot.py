"""By-ear combat bot: fights using only what a blind player gets from the mod.

  python earbot.py [deaf] [--max SECONDS]

It never looks at the screen. It reads the mod's log as a player would hear it: a
"cue block" line is the clang (press defend), a "cue dodge" line is the crack (press
evade). Between cues it attacks (left click) toward whatever the game targets. With
"deaf" it ignores the cues and only attacks, which is the baseline: the difference in
hits taken between the two is what the cues buy.

Runs until the fight ends (the game leaves GameState_Level_Play) or --max seconds, and
prints a report: hits taken (cued vs uncued), cues heard, result.
"""
import os, re, sys, time
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import hof2

DEAF = "deaf" in sys.argv
MAX = 240.0
if "--max" in sys.argv:
    MAX = float(sys.argv[sys.argv.index("--max") + 1])

CUE = re.compile(r"\[HoF2Access\] cue attack_(block|dodge)")
HIT = re.compile(r"\[HoF2Access\] player hit: ([\d.]+) from (\S+) \((.*?)\); (.*)")
STATE = re.compile(r"recon state=(\S+)")
SAY = re.compile(r"\[HoF2Access\] say[:!] (.*)")

def fight(deaf=DEAF, max_seconds=MAX):
    """Fight the running battle; returns (cues, hits, state, lines)."""
    hof2.focus()
    off = hof2.log_size()
    t0 = time.time()
    hits, cues, lines = [], [], []
    # The fight may already be running: seed the state from the last recon line.
    state = "none"
    for l in reversed(hof2.read_since(max(0, off - 200000))):
        m = STATE.search(l)
        if m: state = m.group(1); break
    seen_play = state == "GameState_Level_Play"
    last_attack = 0.0
    while time.time() - t0 < max_seconds:
        new = hof2.read_since(off)
        off = hof2.log_size()
        for l in new:
            m = STATE.search(l)
            if m:
                state = m.group(1)
                if state == "GameState_Level_Play": seen_play = True
            m = CUE.search(l)
            if m:
                cues.append((time.time() - t0, m.group(1)))
                if not deaf:
                    # Defend on the mod's keyboard default (B), as a blind player would.
                    if m.group(1) == "block": hof2.tap("b", hold=0.08)
                    else: hof2.tap("space", hold=0.08)
            m = HIT.search(l)
            if m:
                hits.append((time.time() - t0, float(m.group(1)), m.group(2), m.group(3), m.group(4)))
            m = SAY.search(l)
            if m: lines.append(m.group(1))
        if seen_play and state != "GameState_Level_Play":
            break
        if time.time() - last_attack > 0.45:
            hof2.mouse_button("lmb", hold=0.05)
            last_attack = time.time()
        time.sleep(0.03)
    report(deaf, time.time() - t0, state, cues, hits, lines)
    return cues, hits, state, lines

def report(deaf, duration, state, cues, hits, lines):
    mode = "deaf" if deaf else "ear"
    print("mode=%s duration=%.0fs state=%s cues=%d hits=%d" % (mode, duration, state, len(cues), len(hits)))
    uncued = 0
    for t, amount, attack, attacker, cue in hits:
        tag = ""
        m = re.search(r"last cue ([\d.]+)s before", cue)
        if not m or float(m.group(1)) > 2.5:
            uncued += 1; tag = "  <-- uncued"
        print("  hit %5.1fs %5.1f %s (%s); %s%s" % (t, amount, attack, attacker, cue, tag))
    print("  uncued hits: %d of %d" % (uncued, len(hits)))
    for l in lines:
        if any(k in l for k in ("enem", "Fight", "health", "Health", "Vida", "enemig", "Combate")):
            print("  said:", l)

if __name__ == "__main__":
    fight()
