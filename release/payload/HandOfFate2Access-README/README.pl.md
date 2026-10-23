# Hand of Fate 2 Access

Mod, który sprawia, że w Hand of Fate 2 można grać bez wzroku. Wszystko, co pokazuje gra, jest wypowiadane przez twój czytnik ekranu, a części czasu rzeczywistego (walka, wahadło, koło, tasowanie kart losu) stają się grywalne ze słuchu dzięki dodanym sygnałom dźwiękowym.

To odbudowa porzuconego „Hand of Fate 2 Accessibility Mod”, oparta na Hand of Fate Access, modzie do pierwszej gry.

## Wymagania

- Windows.
- Hand of Fate 2 (Steam).
- Czytnik ekranu (NVDA, JAWS i inne). Bez niego mod używa wbudowanej mowy systemu Windows.

## Instalacja

Najprościej użyć instalatora `HandOfFate2AccessibilityInstaller.exe`, dołączonego do każdego wydania: znajduje grę przez Steam, instaluje lub aktualizuje moda i potrafi go odinstalować. Działa z czytnikiem ekranu. Aby zainstalować ręcznie:

1. Rozpakuj archiwum zip wydania do folderu Hand of Fate 2 (tego z plikiem `Hand of Fate 2.exe`; przy domyślnej instalacji Steam to `C:\Program Files (x86)\Steam\steamapps\common\Hand of Fate 2`). Zezwól na scalenie folderów.
2. Uruchom grę. Kilka sekund po starcie usłyszysz „Hand of Fate 2 Access wersja ... wczytany”.

Archiwum zawiera już skonfigurowany program ładujący mody (BepInEx).

## Jak to działa

Hand of Fate 2 ma już pełną nawigację klawiaturą i padem; mod ją odczytuje. Poruszaj się po menu, kartach i wyborach sterowaniem gry (strzałki, Enter lub spacja, by potwierdzić, Tab, by wrócić, Escape dla menu pauzy), a każdy element mówi:

- Karty czytają nazwę, typ, statystyki i każdą cechę z jej tekstem zasad. Oferowany ekwipunek jest porównywany z tym, który masz na sobie.
- Spotkania czytają tytuł, tekst fabuły, wyniki („+7 jedzenia”) i opcje, w tym koszt lub wymaganie wypisane obok opcji oraz to, czy jest niedostępna.
- Kwestie mówione rozdającego i postaci są czytane w całości w twoim języku, niezależnie od tego, czy w grze włączono napisy.
- Pytania na dolnym pasku („Założyć Topór wojownika? Tak, Nie”) są czytane, gdy się pojawiają.
- Cele wyzwania są czytane, gdy się pojawiają, zmieniają, zostają wykonane lub zawodzą.
- Koniec rozgrywki jest ogłaszany werdyktem samej gry (śmierć od ran lub z głodu, poddanie się, nieudane wyzwanie), a potem wyniki.
- Czytane są dialogi, samouczek sterowania w walce (z klawiszem każdej akcji), prezentacje stworzeń i personalizacja postaci (pochodzenie, pieczęcie oraz opis każdej fryzury i koloru stroju).
- Zmiany jedzenia, złota, sławy i zdrowia są ogłaszane na bieżąco.
- Nazwy ekranów są wypowiadane przy wejściu (menu główne, mapa, mapa wyzwań, kreator talii, ekwipunek, pauza, wyniki). Ekwipunek mówi też, co jest w każdym gnieździe, łącznie z pustymi, bo kursor gry pomija puste gniazda.
- Tabela wyników na koniec rozgrywki (Tryb nieskończony) jest czytana wiersz po wierszu z nazwami kolumn.
- Sklepy: każdy towar czyta cenę („37 złota”) i mówi, gdy cię na niego nie stać; akcje potwierdzenia robiące coś konkretnego („Kup, Enter”) są ogłaszane z klawiszem.
- Akcje na klawiszach funkcyjnych (F1 Ekwipunek, F3 Mapa, F3 Zakończ na ekranie postaci, F3 Start w kreatorze talii) są ogłaszane z klawiszem, gdy się pojawiają. Ekwipunek mówi, że Tab (klawisz powrotu w grze) go zamyka.
- W spotkaniu klawisze numeryczne od 1 do 9 wybierają opcję bezpośrednio (funkcja gry).
- Mapa wyzwań: każde wyzwanie czyta nazwę i to, czy jest zablokowane (z warunkiem odblokowania), ukończone, czy zdobyło złoty lub srebrny żeton; Kampania, Przygoda, Tryb nieskończony i Ognisko mówią, która sekcja jest pokazana (Ognisko zawiera ekran zmiany wyglądu). Otwarcie wyzwania czyta jego tytuł i opis.
- Ustawienia: każdy wiersz czyta nazwę i wartość (listy, pola wyboru, suwaki, przypisania klawiszy), a nowa wartość jest wypowiadana przy zmianie.
- Karty nagród odwracane na końcu wyzwania są czytane w miarę odkrywania.
- Kreator talii: każda talia mówi nazwę, przeznaczenie i zapełnienie („Spotkania, 13 z 13 kart”); w środku lewo i prawo przechodzą po dostępnych kartach lub twojej talii (dół i góra przełączają między nimi), a każda karta mówi nazwę, rodzaj i statystyki (Ctrl + Góra czyta resztę); potwierdzenie dodaje ją lub usuwa, a nowa liczba jest wypowiadana.
- Pytania na dolnym pasku podają swoje klawisze na klawiaturze („Odejdź, Tab; Przyjmij kość, F3”). Nazwy klawiszy są wypowiadane w twoim języku.

## Klawisze dodane przez mod

- **F4**: pomoc dla bieżącego ekranu, najpierw klawisze gry, potem klawisze moda. Strzałki w górę i w dół czytają ją pozycja po pozycji; F4 lub Tab ją zamyka. (F1 to klawisz ekwipunku samej gry.)
- **Ctrl + Góra / Ctrl + Dół**: czytają szczegóły tego, co ma fokus, wiersz po wierszu. Karta jest ogłaszana krótko (nazwa, rodzaj, statystyki); jej cechy i zasady są tutaj. Działa wszędzie, także w walce.
- **Ctrl + Lewo / Ctrl + Prawo**: przełączają, co czytają Ctrl + Góra i Dół: **Szczegóły** (element z fokusem), **Zdarzenia** (wszystko ogłoszone do tej pory, najnowsze najpierw: tekst spotkań, pytania, początki walk, ostrzeżenia o zdrowiu, wyniki) i **Stan** (zdrowie, jedzenie, złoto, sława, trucizna i to, co masz założone w każdym gnieździe). Puste listy są pomijane.
- **H** (lub ukośnik, lub wciśnięcie prawej gałki): twoje zdrowie, jedzenie, złoto, sława i trucizna.
- **O**: cele bieżącego wyzwania.
- **Shift + strzałki** (lub prawa gałka) na mapie: kursor przeglądu, który przechodzi całą widoczną planszę karta po karcie, czytając każdą kartę, czy na niej stoisz, czy możesz na nią przejść, czy jest wyjściem i w które strony prowadzą z niej ścieżki. Tylko czyta; poruszaj się strzałkami i Enterem gry. Ctrl + Góra czyta potem przejrzaną kartę w całości.
- **Shift + kropka** na mapie: kursor przeglądu przeskakuje do następnej karty, której jeszcze nie odwiedzono, od najbliższej; **Shift + przecinek** wraca do poprzedniej. Każdy odczyt kończy się tym, gdzie karta leży względem ciebie, i jej miejscem na liście („nieodwiedzona 2 z 5”); po ostatniej zaczyna znowu od najbliższej. Sklep lub inna karta, na którą już wszedłeś, przestaje się liczyć. Tylko czyta, jak Shift + strzałki.
- **C** (lub wciśnięcie lewej gałki) w walce: sygnał z kierunku najbliższego wroga. Gdy wrogów już nie ma, wskazuje to, do czego jeszcze trzeba dojść (wyjście, zaciskający się krąg bossa, upuszczone złoto lub jedzenie), albo mówi „nie ma już wrogów”.
- **Wrogowie, których możesz dobić**: powalony wróg wydaje narastający świst z miejsca, gdzie leży, powtarzany tym szybciej, im bliżej jesteś, dopóki nie wstanie. Podejdź i naciśnij klawisz dobijania z gry. Gdy najbliższego wroga można dobić, **C** odpowiada tym świstem zamiast zwykłego sygnału. Gdy dobicie by trafiło (dość blisko i w kierunku, w który się kierujesz lub patrzysz), do świstu dochodzi jasny dzwonek: naciśnij teraz klawisz dobijania, kierując się w stronę dźwięku. **C** też daje dzwonek, gdy najbliższy wróg jest w zasięgu.

## Walka ze słuchu

- Walka zaczyna się liczbą wrogów („Walka, 3 wrogów”), a liczba jest wypowiadana, gdy się zmienia. Twoje zdrowie jest wypowiadane, gdy spada poniżej połowy, ćwierci i jednej dziesiątej.
- **Szczęk**: atak, który możesz skontrować. Naciśnij obronę. Gra przypisuje atak i obronę tylko do przycisków myszy, więc mod daje im klawisze: **V atakuje, B broni** (chyba że przypiszesz własne w Ustawieniach, Sterowanie, Przypisz wejście). Komunikaty podają klawisz klawiatury lub przycisk pada, gdy go używasz.
- **Trzask**: atak, którego nie da się zablokować. Unikaj (spacja).
- **Wiatr**: ściany. Wiatr z każdej strony narasta, gdy zbliżasz się do ściany z tej strony; uderzenie oznacza, że w nią weszłeś.
- Pociski, zagrożenia na ziemi, promienie i pułapki mają własne dźwięki, zawsze umieszczone tam, gdzie jest zagrożenie: odsuń się od dźwięku.
- Dzwonek informuje, że umiejętność broni (lewo) lub artefaktu (prawo) jest znów gotowa.
- **Schwytany**: gdy wróg się ciebie uczepi (roje gnomów, pułapka maga), słyszysz „Schwytany” z klawiszem, który trzeba naciskać wielokrotnie, aż usłyszysz „wolny”. Drobne obrażenia w tym czasie to sama pułapka.
- **Pancerz**: niektórzy wrogowie (Postrach Westwend, spaczeni przeciwnicy) noszą pancerz, który rani tylko **Grzmotnięcie** (domyślnie E na klawiaturze, B na kontrolerze; gdy nie ma klawisza, mod dodaje X). Głuchy metaliczny stuk oznacza, że twój cios się odbił; za pierwszym razem przy każdym wrogu słyszysz też „Opancerzony, użyj Grzmotnięcia” z klawiszem. Chrzęst oznacza, że Grzmotnięcie go nadkruszyło; trzask i „Pancerz rozbity” oznaczają, że twoje ciosy znów zadają pełne obrażenia; narastający połysk oznacza, że pancerz odrasta (cicho, gdy się naprawia, głośniej z „Pancerz odnowiony”, gdy rozbity pancerz wraca). Każdy dźwięk dochodzi od tego wroga, a w ustawieniach mają własną głośność. Sterowanie Ogłuszenie z gry nazywa się w całym modzie Grzmotnięciem, jak na kartach gry.
- Jeśli walka jest zbyt trudna, Tryb ucznia w grze (Ustawienia, Ogólne; gra proponuje go też po kilku śmierciach) automatyzuje riposty i dobicia oraz zwiększa twoje obrażenia. Mod działa tak samo w obu przypadkach.
- W walkach kończących się przy wyjściu (sale z pułapkami) wyjście sygnalizuje od następnego zakrętu przechodniej ścieżki do niego, więc idąc w stronę sygnału, omijasz ściany aż do wyjścia; upuszczone złoto i jedzenie sygnalizują tak samo, lżejszym dźwiękiem.
- Pułapka strzelająca wzdłuż pasa, na którym stoisz (strzały, lotki), gra trzask: rusz się.

Menu pauzy ma na dole nową opcję, **Słownik dźwięków**: przejdź go strzałkami, by usłyszeć, co znaczy każdy dźwięk; Enter go odtwarza, Tab zamyka.

## Minigry

- **Karty losu**: karty są pokazywane, każda z własnym tonem i wynikiem wypowiedzianym z jej pozycji, a potem ton każdej karty podąża za nią podczas tasowania. Wybierz pozycję, którą śledziłeś; karty czytają się jako „Miejsce 1”, „Miejsce 2”...
- **Wahadło**: gdy się kołysze, słyszysz wynik pod promieniem, poruszający się z nim w lewo i w prawo: wyżej i jaśniej znaczy lepiej. Naciśnij potwierdzenie na tonie, który chcesz; fokus o tym przypomina.
- **Koło**: karty na kole są czytane przy rozkładaniu; gdy się kręci, każdy segment przelatuje z tyknięciem w głosie swojego wyniku; naciśnij potwierdzenie, by je spowolnić. Gdzie się zatrzyma, jest wypowiadane.
- **Kości**: naciśnij potwierdzenie, by rzucić (inaczej gra wymaga przeciągnięcia myszą). Wypowiadane są cel, suma i kości każdego rzutu oraz to, czy osiągnięto cel; kości do przerzutu czytają wartość i czy są zaznaczone, a zaznaczenie jest ogłaszane. F3 przerzuca.

## Języki

Mod podąża za językiem gry: obsłużonych jest wszystkie dwanaście języków gry (wszystko, co mówi sama gra, jest zawsze w twoim języku). Ten README istnieje w każdym z nich w folderze `docs`, jako Markdown i HTML (`docs/README.<język>.md` i `docs/html/README.<język>.html`).

## Dla programistów

Zobacz `CLAUDE.md`. `setup-bepinex.ps1` instaluje program ładujący, `build.ps1` buduje i wdraża, `test.ps1` uruchamia testy offline.
