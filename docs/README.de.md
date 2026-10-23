# Hand of Fate 2 Access

Ein Mod, der Hand of Fate 2 ohne Sehen spielbar macht. Alles, was das Spiel zeigt, wird über deinen Screenreader gesprochen, und die Echtzeitteile (Kampf, Pendel, Rad, das Mischen der Glückskarten) werden durch zusätzliche Klangsignale nach Gehör spielbar.

Es ist ein Neubau des aufgegebenen „Hand of Fate 2 Accessibility Mod“, aufbauend auf Hand of Fate Access, dem Mod für das erste Spiel.

## Voraussetzungen

- Windows.
- Hand of Fate 2 (Steam).
- Ein Screenreader (NVDA, JAWS und andere). Ohne Screenreader nutzt der Mod die in Windows eingebaute Sprachausgabe.

## Installation

Am einfachsten geht es mit dem Installationsprogramm `HandOfFate2AccessibilityInstaller.exe`, das jedem Release beiliegt: Es findet das Spiel über Steam, installiert oder aktualisiert den Mod und kann ihn deinstallieren. Es funktioniert mit dem Screenreader. Von Hand geht es so:

1. Entpacke die Release-Zip in den Ordner von Hand of Fate 2 (den mit `Hand of Fate 2.exe`; bei einer Standard-Steam-Installation `C:\Program Files (x86)\Steam\steamapps\common\Hand of Fate 2`). Erlaube das Zusammenführen der Ordner.
2. Starte das Spiel. Ein paar Sekunden nach dem Start hörst du „Hand of Fate 2 Access Version ... geladen“.

Die Zip enthält den bereits konfigurierten Mod-Loader (BepInEx).

## So funktioniert es

Hand of Fate 2 hat bereits eine vollständige Tastatur- und Controller-Navigation; der Mod liest sie vor. Bewege dich mit den Steuerungen des Spiels durch Menüs, Karten und Entscheidungen (Pfeiltasten, Eingabe oder Leertaste zum Bestätigen, Tab zum Zurückgehen, Escape für das Pausenmenü), und jedes Element spricht:

- Karten lesen Namen, Typ, Werte und jede Eigenschaft mit ihrem Regeltext vor. Angebotene Ausrüstung wird mit der verglichen, die du trägst.
- Begegnungen lesen Titel, Erzähltext, Ergebnisse („+7 Nahrung“) und Optionen vor, einschließlich der Kosten oder Bedingung neben einer Option und ob sie nicht verfügbar ist.
- Die gesprochenen Zeilen des Kartengebers und der Figuren werden vollständig in deiner Sprache vorgelesen, egal ob die Untertitel-Einstellung des Spiels an ist oder nicht.
- Fragen in der unteren Leiste („Kriegeraxt ausrüsten? Ja, Nein“) werden vorgelesen, sobald sie erscheinen.
- Die Ziele der Herausforderung werden vorgelesen, wenn sie erscheinen, sich ändern, erfüllt werden oder scheitern.
- Das Ende eines Durchlaufs wird mit dem Urteil des Spiels angesagt (Tod durch Wunden oder Hunger, Aufgabe, gescheiterte Herausforderung), danach die Ergebnisse.
- Dialoge, das Tutorial der Kampfsteuerung (mit der Taste jeder Aktion), Kreaturenvorstellungen und die Charaktergestaltung (Herkunft, Siegel und eine Beschreibung jeder Frisur und Kleidungsfarbe) werden vorgelesen.
- Änderungen an Nahrung, Gold, Ruhm und Leben werden angesagt, sobald sie passieren.
- Bildschirmnamen werden beim Betreten gesprochen (Hauptmenü, Karte, Herausforderungskarte, Deckbauer, Inventar, Pause, Ergebnisse). Das Inventar sagt außerdem, was in jedem Platz liegt, leere eingeschlossen, denn der Cursor des Spiels überspringt leere Plätze.
- Die Punktetabelle am Ende eines Durchlaufs (Endlos) wird Zeile für Zeile mit ihren Spaltennamen vorgelesen.
- Läden: Jeder Artikel liest seinen Preis („37 Gold“) und sagt, wenn du ihn dir nicht leisten kannst; Bestätigungsaktionen, die etwas Bestimmtes tun („Kaufen, Eingabe“), werden mit ihrer Taste angesagt.
- Aktionen auf einer Funktionstaste (F1 Inventar, F3 Karte, F3 Fertig auf dem Charakterbildschirm, F3 Start im Deckbauer) werden mit ihrer Taste angesagt, wenn sie erscheinen. Das Inventar sagt, dass Tab (die Zurück-Taste des Spiels) es schließt.
- In einer Begegnung wählen die Zifferntasten 1 bis 9 eine Option direkt (eine Spielfunktion).
- Die Herausforderungskarte: Jede Herausforderung liest ihren Namen und ob sie gesperrt ist (mit dem, was sie freischaltet), abgeschlossen ist oder ihren Gold- oder Silbermarker verdient hat; Kampagne, Abenteuer, Endlos und Lagerfeuer sagen, welcher Bereich gezeigt wird (Lagerfeuer enthält den Bildschirm zum Ändern des Aussehens). Das Öffnen einer Herausforderung liest Titel und Beschreibung.
- Einstellungen: Jede Zeile liest Namen und Wert (Listen, Kontrollkästchen, Regler, Tastenbelegungen), und der neue Wert wird beim Ändern gesprochen.
- Belohnungskarten, die am Ende einer Herausforderung umgedreht werden, werden beim Aufdecken vorgelesen.
- Der Deckbauer: Jedes Deck sagt seinen Namen, seinen Zweck und seinen Füllstand („Begegnungen, 13 von 13 Karten“); darin bewegen Links und Rechts durch die verfügbaren Karten oder dein Deck (Unten und Oben wechseln dazwischen), und jede Karte sagt Namen, Art und Werte (Strg + Oben liest den Rest); Bestätigen fügt sie hinzu oder entfernt sie, und die neue Anzahl wird gesprochen.
- Fragen in der unteren Leiste nennen auf der Tastatur ihre Tasten („Verlassen, Tab; Würfel annehmen, F3“). Tastennamen werden in deiner Sprache gesprochen.

## Tasten, die der Mod hinzufügt

- **F4**: Hilfe für den aktuellen Bildschirm, zuerst die Spieltasten dort, dann die des Mods. Pfeil hoch und runter lesen sie Eintrag für Eintrag; F4 oder Tab schließt sie. (F1 ist die Inventartaste des Spiels.)
- **Strg + Oben / Strg + Unten**: liest die Details dessen, was den Fokus hat, Zeile für Zeile. Eine Karte wird kurz angesagt (Name, Art, Werte); ihre Eigenschaften und Regeln stehen hier. Funktioniert überall, auch im Kampf.
- **Strg + Links / Strg + Rechts**: wechselt, was Strg + Oben und Unten lesen: **Details** (das fokussierte Element), **Ereignisse** (alles bisher Angesagte, Neuestes zuerst: Begegnungstext, Fragen, Kampfbeginn, Lebenswarnungen, Ergebnisse) und **Status** (Leben, Nahrung, Gold, Ruhm, Gift und die Ausrüstung in jedem Platz). Leere Listen werden übersprungen.
- **H** (oder Schrägstrich, oder Klick auf den rechten Stick): dein Leben, Nahrung, Gold, Ruhm und Gift.
- **O**: die Ziele der aktuellen Herausforderung.
- **Umschalt + Pfeiltasten** (oder der rechte Stick) auf der Karte: ein Erkundungscursor, der das ganze sichtbare Brett Karte für Karte abläuft und zu jeder Karte sagt, ob du dort stehst, ob du dorthin ziehen kannst, ob sie der Ausgang ist und welche Richtungen von ihr weiterführen. Er liest nur; ziehe mit den Pfeiltasten und Eingabe des Spiels. Strg + Oben liest danach die erkundete Karte vollständig.
- **Umschalt + Punkt** auf der Karte: Der Erkundungscursor springt zur nächsten Karte, die du noch nicht besucht hast, die nächstgelegene zuerst; **Umschalt + Komma** geht zur vorherigen zurück. Jede Ansage endet damit, wo die Karte von dir aus liegt, und mit ihrem Platz in der Liste („unbesucht 2 von 5“); nach der letzten beginnt es wieder bei der nächstgelegenen. Ein Laden oder eine andere Karte, die du schon betreten hast, zählt nicht mehr. Er liest nur, wie Umschalt + Pfeiltasten.
- **C** (oder Klick auf den linken Stick) im Kampf: ein Piepton aus der Richtung des nächsten Gegners. Sind die Gegner weg, zeigt er stattdessen auf das, was noch zu erreichen ist (den Ausgang, den sich schließenden Kreis eines Bosses, liegen gelassenes Gold oder Nahrung), oder sagt „keine Gegner mehr“.
- **Gegner, die du erledigen kannst**: Ein zu Boden geworfener Gegner spielt ein aufsteigendes Zischen von dort, wo er liegt, das sich schneller wiederholt, je näher du kommst, bis er aufsteht. Geh hin und drück die Gnadenstoß-Taste des Spiels. Kann der nächste Gegner erledigt werden, antwortet **C** mit diesem Zischen statt mit dem üblichen Piepton. Sobald der Gnadenstoß treffen würde (nah genug und in der Richtung, in die du drückst oder schaust), bekommt das Zischen ein helles Klingen dazu: Drück jetzt die Gnadenstoß-Taste, in Richtung des Klangs. Auch **C** antwortet mit dem Klingen, wenn der nächste Gegner in Reichweite ist.

## Kampf nach Gehör

- Der Kampf beginnt mit der Zahl der Gegner („Kampf, 3 Gegner“), und die Zahl wird gesprochen, wenn sie sich ändert. Dein Leben wird gesprochen, wenn es unter die Hälfte, ein Viertel und ein Zehntel fällt.
- **Klirren**: ein Angriff, den du kontern kannst. Drücke Verteidigen. Das Spiel legt Angreifen und Verteidigen nur auf Maustasten, darum gibt der Mod ihnen Tasten: **V greift an, B verteidigt** (es sei denn, du belegst eigene in Einstellungen, Steuerung, Eingabe belegen). Ansagen nennen die Tastaturtaste oder den Controller-Knopf, wenn ein Pad in Gebrauch ist.
- **Knacken**: ein Angriff, den du nicht blocken kannst. Ausweichen (Leertaste).
- **Wind**: Wände. Der Wind auf jeder Seite wird stärker, je näher du einer Wand auf dieser Seite kommst; ein Rumpler heißt, du bist in eine gelaufen.
- Geschosse, Bodengefahren, Strahlen und Fallen haben jeweils ihren eigenen Klang, immer dort platziert, wo die Gefahr ist: Geh vom Klang weg.
- Ein Glockenton sagt dir, dass eine Waffenfähigkeit (links) oder Artefaktfähigkeit (rechts) wieder bereit ist.
- **Gefangen**: Wenn sich ein Gegner an dich klammert (Gnomschwärme, die Fessel eines Magiers), hörst du „Gefangen“ mit der Taste, die du wiederholt drücken musst, bis „frei“ kommt. Die kleinen Treffer währenddessen sind die Fessel selbst.
- **Rüstung**: Manche Gegner (der Schrecken von Westwend, verderbte Feinde) tragen eine Rüstung, die nur der **Wuchtschlag** beschädigt (standardmäßig E auf der Tastatur, B am Controller; hat er keine Taste, ergänzt die Mod X). Ein dumpfes Klonk heißt, dein Treffer ist abgeprallt; beim ersten Mal pro Gegner hörst du außerdem „Gepanzert, Wuchtschlag benutzen“ mit der Taste. Ein Knirschen heißt, dein Wuchtschlag hat sie angeknackst; ein Krachen und „Rüstung gebrochen“ heißen, deine Treffer wirken wieder voll; ein aufsteigender Schimmer heißt, die Rüstung wächst nach (leise, während sie heilt, lauter mit „Rüstung wiederhergestellt“, wenn gebrochene Rüstung zurückkommt). Jeder Klang kommt von diesem Gegner und hat in den Einstellungen eine eigene Lautstärke. Die Spielsteuerung Betäuben heißt in der ganzen Mod Wuchtschlag, wie auf den Karten des Spiels.
- Ist der Kampf zu schwer, automatisiert der Lehrlingsmodus des Spiels (Einstellungen, Allgemein; das Spiel bietet ihn auch nach ein paar Toden an) Konter und Todesstöße und erhöht deinen Schaden. Der Mod funktioniert in beiden Fällen gleich.
- In Kämpfen, die an einem Ausgang enden (Fallenräume), piept der Ausgang von der nächsten Biegung des begehbaren Weges dorthin, sodass dich der Weg zum Piepton um die Wände herum zum Ausgang führt; liegen gelassenes Gold und Nahrung piepen genauso, mit einem leichteren Klang.
- Eine Falle, die in die Bahn schießt, in der du stehst (Pfeile, Bolzen), spielt das Knacken: Beweg dich.

Das Pausenmenü hat ganz unten eine neue Option, **Klangglossar**: Geh es mit den Pfeiltasten durch, um zu hören, was jeder Klang bedeutet; Eingabe spielt ihn ab, Tab schließt es.

## Minispiele

- **Glückskarten**: Die Karten werden gezeigt, jede mit eigenem Ton und ihrem Ergebnis aus ihrer Position gesprochen; dann folgt der Ton jeder Karte ihr durch das Mischen. Wähle die Position, die du verfolgt hast; die Karten werden als „Platz 1“, „Platz 2“ ... vorgelesen.
- **Pendel**: Während es schwingt, hörst du das Ergebnis unter dem Strahl, das sich mit ihm nach links und rechts bewegt: höher und heller ist besser. Drücke Bestätigen beim gewünschten Ton; der Fokus erinnert dich daran.
- **Rad**: Die Karten auf dem Rad werden beim Auslegen vorgelesen; während es dreht, tickt jedes Segment in der Stimme seines Ergebnisses vorbei; drücke Bestätigen, um es zu bremsen. Wo es landet, wird gesprochen.
- **Würfel**: Drücke Bestätigen zum Werfen (sonst verlangt das Spiel ein Ziehen mit der Maus). Ziel, Summe und Würfel jedes Wurfs und ob du das Ziel erreicht hast, werden gesprochen; Würfel, die du neu werfen kannst, lesen ihren Wert und ob sie markiert sind, und das Markieren wird angesagt. F3 wirft neu.

## Sprachen

Der Mod folgt der Sprache des Spiels: Alle zwölf Sprachen des Spiels sind abgedeckt (alles, was das Spiel selbst sagt, ist immer in deiner Sprache). Diese README gibt es in jeder von ihnen im Ordner `docs`, als Markdown und als HTML (`docs/README.<Sprache>.md` und `docs/html/README.<Sprache>.html`).

## Für Entwickler

Siehe `CLAUDE.md`. `setup-bepinex.ps1` installiert den Loader, `build.ps1` baut und deployt, `test.ps1` führt die Offline-Tests aus.
