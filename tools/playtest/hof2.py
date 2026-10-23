"""Playtest harness for Hand of Fate 2: eyes (window capture), hands (scancode
SendInput), ears (the mod's log). Usage:

  python hof2.py launch            start the game (kills a running one first)
  python hof2.py kill
  python hof2.py deploy [nolaunch] kill, build.ps1, relaunch
  python hof2.py shot [label]      capture the game window -> shots/NNNN_label.png
  python hof2.py keys K1 K2 ...    tap keys (names below); "K*3" repeats, "wait:0.5" sleeps,
                                   "ctrl+up" chords; prints new spoken lines afterwards
  python hof2.py say [N]           last N spoken lines (default 20)
  python hof2.py log [N] [grep]    last N mod log lines, optionally filtered
  python hof2.py mark              print the current log size (for 'since')
  python hof2.py since OFFSET      log lines written after OFFSET
  python hof2.py remote CMD...     queue commands for the mod's playtest remote (works
                                   without window focus): "nav:Down", "button:Select",
                                   "button:F3", "kill", "mod:status", "wait:1.5"; prints
                                   new spoken lines afterwards. "remote off" disables it.
"""
import ctypes, ctypes.wintypes as wt, os, subprocess, sys, time

GAME = r"S:\steam\steamapps\common\Hand of Fate 2"
EXE = os.path.join(GAME, "Hand of Fate 2.exe")
LOG = os.path.join(os.environ["USERPROFILE"], "AppData", "LocalLow", "Defiant Development", "Hand of Fate 2", "output_log.txt")
HERE = os.path.dirname(os.path.abspath(__file__))
SHOTS = os.path.join(HERE, "shots")
TITLE = "Hand of Fate 2"

user32 = ctypes.WinDLL("user32", use_last_error=True)
try:
    ctypes.windll.shcore.SetProcessDpiAwareness(2)
except Exception:
    user32.SetProcessDPIAware()

SC = {  # set-1 scancodes; 'e' = extended
    "esc": 0x01, "1": 0x02, "2": 0x03, "3": 0x04, "4": 0x05, "5": 0x06, "6": 0x07, "7": 0x08,
    "8": 0x09, "9": 0x0A, "0": 0x0B, "minus": 0x0C, "equals": 0x0D, "backspace": 0x0E, "tab": 0x0F,
    "q": 0x10, "w": 0x11, "e": 0x12, "r": 0x13, "t": 0x14, "y": 0x15, "u": 0x16, "i": 0x17, "o": 0x18,
    "p": 0x19, "enter": 0x1C, "ctrl": 0x1D, "a": 0x1E, "s": 0x1F, "d": 0x20, "f": 0x21, "g": 0x22,
    "h": 0x23, "j": 0x24, "k": 0x25, "l": 0x26, "semicolon": 0x27, "quote": 0x28, "shift": 0x2A,
    "z": 0x2C, "x": 0x2D, "c": 0x2E, "v": 0x2F, "b": 0x30, "n": 0x31, "m": 0x32, "comma": 0x33,
    "period": 0x34, "slash": 0x35, "alt": 0x38, "space": 0x39, "f1": 0x3B, "f2": 0x3C, "f3": 0x3D,
    "f4": 0x3E, "f5": 0x3F, "f6": 0x40, "f7": 0x41, "f8": 0x42, "f9": 0x43, "f10": 0x44, "f11": 0x57, "f12": 0x58,
    "home": (0x47, True), "up": (0x48, True), "pgup": (0x49, True), "left": (0x4B, True),
    "right": (0x4D, True), "end": (0x4F, True), "down": (0x50, True), "pgdn": (0x51, True),
    "insert": (0x52, True), "delete": (0x53, True), "rctrl": (0x1D, True),
}

class KEYBDINPUT(ctypes.Structure):
    _fields_ = [("wVk", wt.WORD), ("wScan", wt.WORD), ("dwFlags", wt.DWORD), ("time", wt.DWORD), ("dwExtraInfo", ctypes.c_size_t)]
class MOUSEINPUT(ctypes.Structure):
    _fields_ = [("dx", wt.LONG), ("dy", wt.LONG), ("mouseData", wt.DWORD), ("dwFlags", wt.DWORD), ("time", wt.DWORD), ("dwExtraInfo", ctypes.c_size_t)]
class _U(ctypes.Union):
    _fields_ = [("ki", KEYBDINPUT), ("mi", MOUSEINPUT)]
class INPUT(ctypes.Structure):
    _fields_ = [("type", wt.DWORD), ("u", _U)]

def _send_scan(code, ext, up):
    flags = 0x0008 | (0x0001 if ext else 0) | (0x0002 if up else 0)
    inp = INPUT(type=1, u=_U(ki=KEYBDINPUT(0, code, flags, 0, 0)))
    user32.SendInput(1, ctypes.byref(inp), ctypes.sizeof(INPUT))

def _sc(name):
    v = SC[name.lower()]
    return (v, False) if isinstance(v, int) else v

def find_window():
    found = []
    @ctypes.WINFUNCTYPE(wt.BOOL, wt.HWND, wt.LPARAM)
    def cb(h, _):
        n = user32.GetWindowTextLengthW(h)
        if n:
            buf = ctypes.create_unicode_buffer(n + 1)
            user32.GetWindowTextW(h, buf, n + 1)
            if buf.value == TITLE and user32.IsWindowVisible(h):
                found.append(h)
        return True
    user32.EnumWindows(cb, 0)
    return found[0] if found else None

def focus():
    h = find_window()
    if not h:
        sys.exit("game window not found")
    if user32.GetForegroundWindow() != h:
        # Alt tap lets SetForegroundWindow succeed from a background process.
        _send_scan(0x38, False, False); _send_scan(0x38, False, True)
        user32.ShowWindow(h, 9)
        user32.SetForegroundWindow(h)
        time.sleep(0.3)
    kernel32 = ctypes.WinDLL("kernel32")
    for _ in range(5):
        if user32.GetForegroundWindow() == h: break
        # Attach to the foreground window's input queue so Windows' foreground lock
        # lets this process hand the focus to the game.
        fg = user32.GetForegroundWindow()
        fg_thread = user32.GetWindowThreadProcessId(fg, None)
        me = kernel32.GetCurrentThreadId()
        user32.AttachThreadInput(me, fg_thread, True)
        _send_scan(0x38, False, False); _send_scan(0x38, False, True)
        user32.ShowWindow(h, 9)
        user32.BringWindowToTop(h)
        user32.SetForegroundWindow(h)
        user32.AttachThreadInput(me, fg_thread, False)
        time.sleep(0.3)
    if user32.GetForegroundWindow() != h:
        sys.exit("could not bring game window to front")
    return h

def tap(chord, hold=0.15):
    parts = [_sc(p) for p in chord.split("+")]
    for c, e in parts:
        _send_scan(c, e, False); time.sleep(0.02)
    time.sleep(hold)
    for c, e in reversed(parts):
        _send_scan(c, e, True); time.sleep(0.02)

def log_size():
    return os.path.getsize(LOG) if os.path.exists(LOG) else 0

def read_since(off):
    if not os.path.exists(LOG):
        return []
    with open(LOG, "rb") as f:
        f.seek(off)
        b = f.read()
        try: return b.decode("utf-8").splitlines()
        except UnicodeDecodeError: return b.decode("cp1252", "replace").splitlines()

def spoken(lines):
    out = []
    for l in lines:
        i = l.find("[HoF2Access] say")
        if i >= 0:
            out.append(l[i + len("[HoF2Access] "):])
    return out

def mouse_button(which, hold=0.08):
    down, up = {"lmb": (0x0002, 0x0004), "rmb": (0x0008, 0x0010), "mmb": (0x0020, 0x0040)}[which]
    for f in (down, up):
        inp = INPUT(type=0, u=_U(mi=MOUSEINPUT(0, 0, 0, f, 0, 0)))
        user32.SendInput(1, ctypes.byref(inp), ctypes.sizeof(INPUT))
        time.sleep(hold)

def cmd_keys(args):
    focus()
    off = log_size()
    for a in args:
        if a.split("*")[0] in ("lmb", "rmb", "mmb"):
            name, n = (a.split("*") + ["1"])[:2]
            for _ in range(int(n)):
                mouse_button(name); time.sleep(0.2)
            continue
        if a.startswith("wait:"):
            time.sleep(float(a[5:])); continue
        n = 1
        if "*" in a:
            a, n = a.split("*"); n = int(n)
        for _ in range(n):
            tap(a); time.sleep(0.25)
    time.sleep(0.6)
    for l in spoken(read_since(off)):
        print(l)

def cmd_shot(label="shot"):
    import mss, mss.tools
    h = focus()
    time.sleep(0.2)
    r = wt.RECT(); user32.GetClientRect(h, ctypes.byref(r))
    pt = wt.POINT(0, 0); user32.ClientToScreen(h, ctypes.byref(pt))
    os.makedirs(SHOTS, exist_ok=True)
    n = len([f for f in os.listdir(SHOTS) if f.endswith(".png")])
    path = os.path.join(SHOTS, "%04d_%s.png" % (n, label))
    with mss.MSS() as m:
        img = m.grab({"left": pt.x, "top": pt.y, "width": r.right, "height": r.bottom})
        from PIL import Image
        im = Image.frombytes("RGB", img.size, img.rgb)
        if im.width > 1600:
            im = im.resize((1600, int(im.height * 1600 / im.width)))
        im.save(path)
    print(path)

def cmd_launch():
    # The game itself intermittently hangs on its first loading screen (seen with and
    # without mods), so a launch that does not reach the start menu is retried.
    for attempt in range(4):
        cmd_kill(quiet=True)
        os.startfile("steam://rungameid/456670")
        t0 = time.time()
        while time.time() - t0 < 75:
            time.sleep(1)
            if find_window() and "StartScreen_Idle>StartScreen " in "".join(read_since(0)):
                time.sleep(2)
                print("start screen up"); return
        print("load hung (attempt %d); relaunching" % (attempt + 1))
    print("start screen not reached")

def cmd_kill(quiet=False):
    r = subprocess.run(["taskkill", "/F", "/IM", "Hand of Fate 2.exe"], capture_output=True, text=True)
    if not quiet:
        print(r.stdout.strip() or r.stderr.strip())
    time.sleep(1)

PLUGIN_DIR = os.path.join(GAME, "BepInEx", "plugins", "HandOfFate2Access")

def cmd_remote(items):
    """Drive the game through the mod's playtest remote (Diagnostics/RemoteControl):
    the game keeps running without focus, so this works when the window can't be
    brought to the front."""
    flag = os.path.join(PLUGIN_DIR, "playtest-remote.enabled")
    if items == ["off"]:
        if os.path.exists(flag): os.remove(flag)
        return
    if not os.path.exists(flag):
        open(flag, "w").close()
        time.sleep(0.5)
    off = log_size()
    path = os.path.join(PLUGIN_DIR, "playtest-remote.txt")
    for it in items:
        if it.startswith("wait:"):
            time.sleep(float(it[5:])); continue
        with open(path, "a", encoding="utf-8") as f:
            f.write(it.replace(":", " ", 1) + "\n")
        time.sleep(0.35)
    time.sleep(0.5)
    for l in spoken(read_since(off)): print(l)

def main():
    a = sys.argv[1:]
    if not a:
        print(__doc__); return
    c, rest = a[0], a[1:]
    if c == "launch": cmd_launch()
    elif c == "deploy":
        cmd_kill(quiet=True)
        root = os.path.dirname(os.path.dirname(HERE))
        r = subprocess.run(["powershell", "-ExecutionPolicy", "Bypass", "-File", os.path.join(root, "build.ps1")], capture_output=True, text=True)
        errs = [l for l in r.stdout.splitlines() if ": error " in l or "ERROR" in l or "FAILED" in l]
        uniq = []
        for l in errs:
            if l not in uniq: uniq.append(l)
        print(chr(10).join(uniq[:15]) if uniq else "build ok")
        if r.returncode != 0 or errs:
            sys.exit(1)
        if "nolaunch" not in rest: cmd_launch()
    elif c == "kill": cmd_kill()
    elif c == "shot": cmd_shot(*(rest or ["shot"]))
    elif c == "keys": cmd_keys(rest)
    elif c == "focus": focus()
    elif c == "click":
        # absolute screen pixel coordinates
        x, y = int(rest[0]), int(rest[1])
        user32.SetCursorPos(x, y); time.sleep(0.1)
        for f in (0x0002, 0x0004):
            inp = INPUT(type=0, u=_U(mi=MOUSEINPUT(0, 0, 0, f, 0, 0)))
            user32.SendInput(1, ctypes.byref(inp), ctypes.sizeof(INPUT)); time.sleep(0.05)
    elif c == "say":
        n = int(rest[0]) if rest else 20
        for l in spoken(read_since(0))[-n:]: print(l)
    elif c == "log":
        n = int(rest[0]) if rest else 40
        lines = read_since(0)
        if len(rest) > 1:
            lines = [l for l in lines if rest[1].lower() in l.lower()]
        for l in lines[-n:]: print(l)
    elif c == "until":
        # until REGEX [TIMEOUT]: wait for a new log line matching REGEX (from now)
        import re
        rx = re.compile(rest[0]); limit = float(rest[1]) if len(rest) > 1 else 60
        off = log_size(); t0 = time.time()
        # The game pauses while its window is in the background, so keep it in front
        # (only once past loading: stealing focus during the first load hangs it).
        if find_window() and "StartScreen_Idle>StartScreen " in "".join(read_since(0)):
            try: focus()
            except SystemExit: pass
        while time.time() - t0 < limit:
            time.sleep(0.5)
            for l in read_since(off):
                if rx.search(l):
                    print("matched:", l.strip()[:300]); return
        print("timeout waiting for", rest[0])
    elif c == "remote": cmd_remote(rest)
    elif c == "mark": print(log_size())
    elif c == "since":
        for l in read_since(int(rest[0])): print(l)
    else:
        print(__doc__)

if __name__ == "__main__":
    main()
