# Hand of Fate 2 Access

Una mod che rende Hand of Fate 2 giocabile senza vedere. Tutto ciò che il gioco mostra viene letto dal tuo screen reader, e le parti in tempo reale (il combattimento, il pendolo, la ruota, il mescolamento delle carte della sorte) diventano giocabili a orecchio grazie a segnali sonori aggiunti.

È una ricostruzione dell'abbandonata "Hand of Fate 2 Accessibility Mod", basata su Hand of Fate Access, la mod del primo gioco.

## Requisiti

- Windows.
- Hand of Fate 2 (Steam).
- Uno screen reader (NVDA, JAWS e altri). Senza, la mod usa la voce integrata di Windows.

## Installazione

Il modo più semplice è l'installer, `HandOfFate2AccessibilityInstaller.exe`, allegato a ogni versione: trova il gioco tramite Steam, installa o aggiorna la mod e può disinstallarla. Funziona con lo screen reader. Per installare a mano:

1. Estrai lo zip della versione nella cartella di Hand of Fate 2 (quella che contiene `Hand of Fate 2.exe`; con un'installazione Steam predefinita, `C:\Program Files (x86)\Steam\steamapps\common\Hand of Fate 2`). Consenti l'unione delle cartelle.
2. Avvia il gioco. Pochi secondi dopo l'avvio senti "Hand of Fate 2 Access versione ... caricato".

Lo zip contiene il caricatore di mod (BepInEx) già configurato.

## Come funziona

Hand of Fate 2 ha già una navigazione completa da tastiera e controller; la mod la legge. Muoviti tra menu, carte e scelte con i comandi del gioco (frecce, Invio o Spazio per confermare, Tab per tornare indietro, Esc per il menu di pausa) e ogni elemento parla:

- Le carte leggono nome, tipo, statistiche e ogni tratto con il suo testo di regole. L'equipaggiamento che ti viene offerto è confrontato con quello che indossi.
- Gli incontri leggono titolo, testo della storia, risultati ("+7 cibo") e opzioni, compresi il costo o il requisito stampati accanto a un'opzione e se non è disponibile.
- Le battute recitate del mazziere e dei personaggi vengono lette per intero nella tua lingua, che l'impostazione Sottotitoli del gioco sia attiva o no.
- Le domande nella barra in basso ("Equipaggiare l'Ascia del guerriero? Sì, No") vengono lette quando compaiono.
- Gli obiettivi della sfida vengono letti quando compaiono, cambiano, vengono completati o falliscono.
- La fine di una partita viene annunciata con il verdetto del gioco stesso (morte per ferite o per fame, resa, sfida fallita), poi i risultati.
- Vengono letti i dialoghi, il tutorial dei comandi di combattimento (con il tasto di ogni azione), le presentazioni delle creature e la personalizzazione del personaggio (origini, sigilli e una descrizione di ogni acconciatura e colore dell'abito).
- Le variazioni di cibo, oro, fama e vita vengono annunciate quando avvengono.
- I nomi delle schermate vengono pronunciati quando vi entri (menu principale, mappa, mappa delle sfide, costruttore di mazzi, inventario, pausa, risultati). L'inventario dice anche cosa c'è in ogni slot, vuoti compresi, perché il cursore del gioco salta gli slot vuoti.
- La tabella dei punteggi di fine partita (Infinito) viene letta riga per riga con i nomi delle colonne.
- Negozi: ogni articolo legge il suo prezzo ("37 oro") e dice quando non puoi permettertelo; le azioni di conferma che fanno qualcosa di specifico ("Compra, Invio") vengono annunciate con il loro tasto.
- Le azioni su un tasto funzione (F1 Inventario, F3 Mappa, F3 Fine nella schermata del personaggio, F3 Inizia nel costruttore di mazzi) vengono annunciate con il loro tasto quando compaiono. L'inventario dice che Tab (il tasto indietro del gioco) lo chiude.
- In un incontro, i tasti numerici da 1 a 9 scelgono direttamente un'opzione (una funzione del gioco).
- La mappa delle sfide: ogni sfida legge il suo nome e se è bloccata (con ciò che la sblocca), completata, o se ha guadagnato il gettone d'oro o d'argento; Campagna, Avventura, Infinito e Falò dicono quale sezione è mostrata (Falò contiene la schermata per cambiare aspetto). Aprire una sfida ne legge titolo e descrizione.
- Impostazioni: ogni riga legge nome e valore (liste, caselle, cursori, associazioni di tasti), e il nuovo valore viene pronunciato mentre lo cambi.
- Le carte ricompensa girate alla fine di una sfida vengono lette man mano che vengono rivelate.
- Il costruttore di mazzi: ogni mazzo dice il suo nome, a cosa serve e quanto è pieno ("Incontri, 13 di 13 carte"); all'interno, sinistra e destra scorrono le carte disponibili o il tuo mazzo (giù e su passano dall'uno all'altro) e ogni carta dice nome, tipo e statistiche (Ctrl + Su legge il resto); confermare la aggiunge o la rimuove e viene pronunciato il nuovo conteggio.
- Le domande nella barra in basso nominano i loro tasti sulla tastiera ("Vai via, Tab; Accetta dado, F3"). I nomi dei tasti vengono pronunciati nella tua lingua.

## Tasti aggiunti dalla mod

- **F4**: aiuto per la schermata in cui ti trovi, prima i tasti del gioco, poi quelli della mod. Le frecce su e giù lo leggono una voce alla volta; F4 o Tab lo chiude. (F1 è il tasto inventario del gioco.)
- **Ctrl + Su / Ctrl + Giù**: leggono i dettagli di ciò che ha il focus, una riga alla volta. Una carta viene annunciata in breve (nome, tipo, statistiche); i suoi tratti e le regole sono qui. Funziona ovunque, combattimento compreso.
- **Ctrl + Sinistra / Ctrl + Destra**: cambiano ciò che Ctrl + Su e Giù leggono: **Dettagli** (l'elemento con il focus), **Eventi** (tutto ciò che è stato annunciato finora, dal più recente: testo degli incontri, domande, inizi di combattimento, avvisi di vita, risultati) e **Stato** (vita, cibo, oro, fama, veleno e cosa è equipaggiato in ogni slot). Le liste vuote vengono saltate.
- **H** (o barra, o clic dello stick destro): vita, cibo, oro, fama e veleno.
- **O**: gli obiettivi della sfida in corso.
- **Maiusc + frecce** (o lo stick destro) sulla mappa: un cursore di esplorazione che percorre tutto il tabellone visibile carta per carta, leggendo ogni carta, se ci sei sopra, se puoi raggiungerla, se è l'uscita e quali direzioni proseguono da lì. Legge soltanto; muoviti con le frecce e Invio del gioco. Ctrl + Su legge poi per intero la carta esplorata.
- **Maiusc + punto** sulla mappa: il cursore di esplorazione salta alla prossima carta che non hai ancora visitato, la più vicina per prima; **Maiusc + virgola** torna alla precedente. Ogni lettura finisce con dove si trova la carta rispetto a te e il suo posto nell'elenco («non visitata 2 di 5»); dopo l'ultima ricomincia dalla più vicina. Un negozio o un'altra carta in cui sei già entrato non conta più. Legge soltanto, come Maiusc + frecce.
- **C** (o clic dello stick sinistro) in combattimento: un bip dalla direzione del nemico più vicino. Quando i nemici sono finiti, indica invece ciò che resta da raggiungere (l'uscita, il cerchio che si chiude di un boss, oro o cibo caduti), oppure dice "nessun nemico rimasto".
- **Nemici che puoi finire**: un nemico atterrato emette un sibilo ascendente da dove giace, che si ripete più veloce man mano che ti avvicini, finché non si rialza. Raggiungilo e premi il tasto del gioco per finirlo. Quando il nemico più vicino si può finire, **C** risponde con quel sibilo invece del solito bip. Appena il colpo di grazia andrebbe a segno (abbastanza vicino e nella direzione in cui spingi o guardi), il sibilo ha sopra un rintocco squillante: premi ora il tasto per finirlo, spingendo verso il suono. Anche **C** dà il rintocco quando il nemico più vicino è a portata.

## Combattere a orecchio

- Il combattimento inizia con il numero di nemici ("Combattimento, 3 nemici") e il conteggio viene pronunciato quando cambia. La tua vita viene pronunciata quando scende sotto la metà, un quarto e un decimo.
- **Clangore**: un attacco che puoi contrastare. Premi difendi. Il gioco assegna attacco e difesa solo ai pulsanti del mouse, quindi la mod dà loro dei tasti: **V attacca e B difende** (a meno che tu non assegni i tuoi in Impostazioni, Controlli, Associa input). Gli annunci nominano il tasto della tastiera, o il pulsante del controller quando ne usi uno.
- **Schiocco**: un attacco che non puoi bloccare. Schiva (Spazio).
- **Vento**: i muri. Il vento su ogni lato cresce quando ti avvicini a un muro su quel lato; un urto significa che ci sei andato contro.
- Proiettili, pericoli a terra, raggi e trappole hanno ciascuno il proprio suono, sempre posizionato dove si trova il pericolo: allontanati dal suono.
- Un campanello ti avvisa che un'abilità dell'arma (sinistra) o dell'artefatto (destra) è di nuovo pronta.
- **Intrappolato**: quando un nemico ti si aggrappa (sciami di gnomi, il laccio di un mago) senti "Intrappolato" con il tasto da premere ripetutamente finché non senti "libero". I piccoli colpi che subisci nel frattempo sono la trappola stessa.
- **Armatura**: alcuni nemici (il Terrore di Westwend, i nemici corrotti) indossano un'armatura che solo lo **Sfondamento** danneggia (E sulla tastiera di default, B sul controller; la mod aggiunge X se non ha un tasto). Un tonfo metallico sordo significa che il tuo colpo è rimbalzato; la prima volta su ogni nemico senti anche «Corazzato, usa Sfondamento» con il tasto. Uno scricchiolio significa che il tuo Sfondamento l'ha incrinata; un fragore e «Armatura spezzata» significano che i tuoi colpi tornano a fare pieno danno; un luccichio ascendente significa che l'armatura si sta riformando (piano mentre si ripara, più forte con «Armatura ripristinata» quando un'armatura spezzata ritorna). Ogni suono viene da quel nemico e hanno un volume proprio nelle impostazioni. Il comando Stordire del gioco si chiama Sfondamento in tutta la mod, come sulle carte del gioco.
- Se il combattimento è troppo difficile, la Modalità apprendista del gioco (Impostazioni, Generale; il gioco la propone anche dopo alcune morti) automatizza contrattacchi e finisher e aumenta i tuoi danni. La mod funziona allo stesso modo in entrambi i casi.
- Nei combattimenti che finiscono a un'uscita (stanze di trappole), l'uscita emette un bip dalla prossima svolta del percorso praticabile verso di essa, così camminare verso il bip ti porta attorno ai muri fino all'uscita; oro e cibo caduti emettono lo stesso bip, più leggero.
- Una trappola che spara lungo la corsia in cui ti trovi (frecce, dardi) riproduce lo schiocco: spostati.

Il menu di pausa ha una nuova opzione in fondo, **Glossario dei suoni**: scorrilo con le frecce per sentire cosa significa ogni suono, Invio lo riproduce, Tab lo chiude.

## Minigiochi

- **Carte della sorte**: le carte vengono mostrate, ciascuna con il proprio tono e il suo esito pronunciato dalla sua posizione, poi il tono di ogni carta la segue durante il mescolamento. Scegli la posizione che hai seguito; le carte si leggono come "Posizione 1", "Posizione 2"...
- **Pendolo**: mentre oscilla senti l'esito sotto il raggio, che si sposta a sinistra e a destra con lui: più acuto e brillante è meglio. Premi conferma sul tono che vuoi; il focus te lo ricorda.
- **Ruota**: le carte sulla ruota vengono lette quando viene disposta; mentre gira, ogni segmento passa con un tic nella voce del suo esito; premi conferma per rallentarla. Dove si ferma viene pronunciato.
- **Dadi**: premi conferma per lanciare (altrimenti il gioco richiede un trascinamento con il mouse). Vengono pronunciati l'obiettivo, il totale e i dadi di ogni lancio e se hai raggiunto l'obiettivo; i dadi che puoi rilanciare leggono il loro valore e se sono contrassegnati, e contrassegnarne uno viene annunciato. F3 rilancia.

## Lingue

La mod segue la lingua del gioco: tutte e dodici le lingue del gioco sono coperte (tutto ciò che dice il gioco stesso è sempre nella tua lingua). Questo README esiste in ognuna di esse nella cartella `docs`, in Markdown e in HTML (`docs/README.<lingua>.md` e `docs/html/README.<lingua>.html`).

## Per gli sviluppatori

Vedi `CLAUDE.md`. `setup-bepinex.ps1` installa il caricatore, `build.ps1` compila e distribuisce, `test.ps1` esegue i test offline.
