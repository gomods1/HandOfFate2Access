# Hand of Fate 2 Access

Un mod qui rend Hand of Fate 2 jouable sans la vue. Tout ce que le jeu affiche est lu par votre lecteur d'écran, et les parties en temps réel (le combat, le pendule, la roue, le mélange des cartes de chance) deviennent jouables à l'oreille grâce à des signaux sonores ajoutés.

C'est une reconstruction du « Hand of Fate 2 Accessibility Mod » abandonné, fondée sur Hand of Fate Access, le mod du premier jeu.

## Prérequis

- Windows.
- Hand of Fate 2 (Steam).
- Un lecteur d'écran (NVDA, JAWS et autres). Sans lecteur, le mod utilise la voix intégrée de Windows.

## Installation

Le plus simple est l'installateur, `HandOfFate2AccessibilityInstaller.exe`, joint à chaque version : il trouve le jeu via Steam, installe ou met à jour le mod et peut le désinstaller. Il fonctionne avec un lecteur d'écran. Pour installer à la main :

1. Extrayez le zip de la version dans le dossier de Hand of Fate 2 (celui qui contient `Hand of Fate 2.exe` ; avec une installation Steam par défaut, `C:\Program Files (x86)\Steam\steamapps\common\Hand of Fate 2`). Acceptez la fusion des dossiers.
2. Lancez le jeu. Quelques secondes après le démarrage, vous entendez « Hand of Fate 2 Access version ... chargé ».

Le zip contient le chargeur de mods (BepInEx) déjà configuré.

## Fonctionnement

Hand of Fate 2 possède déjà une navigation complète au clavier et à la manette ; le mod la lit. Parcourez les menus, les cartes et les choix avec les commandes du jeu (flèches, Entrée ou Espace pour confirmer, Tab pour revenir, Échap pour le menu de pause) et chaque élément parle :

- Les cartes lisent leur nom, leur type, leurs statistiques et chaque trait avec son texte de règles. L'équipement qu'on vous propose est comparé à celui que vous portez.
- Les rencontres lisent leur titre, le texte de l'histoire, les résultats (« +7 nourriture ») et les options, y compris le coût ou la condition imprimés à côté d'une option et si elle est indisponible.
- Les répliques parlées du croupier et des personnages sont lues en entier dans votre langue, que l'option Sous-titres du jeu soit activée ou non.
- Les questions de la barre du bas (« Équiper la hache du guerrier ? Oui, Non ») sont lues quand elles apparaissent.
- Les objectifs du défi sont lus quand ils apparaissent, changent, sont accomplis ou échouent.
- La fin d'une partie est annoncée avec le verdict du jeu lui-même (mort par blessures ou par famine, abandon, défi échoué), puis les résultats.
- Les dialogues, le tutoriel des commandes de combat (avec la touche de chaque action), les présentations de créatures et la personnalisation du personnage (origines, sceaux, et une description de chaque coiffure et couleur de tenue) sont lus.
- Les variations de nourriture, d'or, de renommée et de vie sont annoncées au moment où elles se produisent.
- Le nom des écrans est prononcé quand vous y entrez (menu principal, carte, carte des défis, constructeur de deck, inventaire, pause, résultats). L'inventaire dit aussi ce que contient chaque emplacement, vides compris, car le curseur du jeu saute les emplacements vides.
- Le tableau des scores de fin de partie (Infini) est lu ligne par ligne avec le nom de ses colonnes.
- Boutiques : chaque article lit son prix (« 37 or ») et dit quand vous ne pouvez pas le payer ; les actions de confirmation qui font quelque chose de précis (« Acheter, Entrée ») sont annoncées avec leur touche.
- Les actions sur une touche de fonction (F1 Inventaire, F3 Carte, F3 Terminer sur l'écran du personnage, F3 Commencer dans le constructeur de deck) sont annoncées avec leur touche quand elles apparaissent. L'inventaire dit que Tab (la touche retour du jeu) le ferme.
- Dans une rencontre, les touches numériques 1 à 9 choisissent directement une option (une fonction du jeu).
- La carte des défis : chaque défi lit son nom et s'il est verrouillé (avec ce qui le déverrouille), terminé, ou s'il a gagné son jeton d'or ou d'argent ; Campagne, Aventure, Infini et Feu de camp disent lequel est affiché (Feu de camp contient l'écran de changement d'apparence). Ouvrir un défi lit son titre et sa description.
- Paramètres : chaque ligne lit son nom et sa valeur (listes, cases à cocher, curseurs, affectations de touches), et la nouvelle valeur est prononcée quand vous la changez.
- Les cartes de récompense retournées à la fin d'un défi sont lues au fur et à mesure qu'elles sont révélées.
- Le constructeur de deck : chaque deck dit son nom, son rôle et son remplissage (« Rencontres, 13 cartes sur 13 ») ; à l'intérieur, gauche et droite parcourent les cartes disponibles ou votre deck (bas et haut passent de l'un à l'autre) et chaque carte dit son nom, son genre et ses statistiques (Ctrl + Haut lit le reste) ; confirmer l'ajoute ou la retire et le nouveau compte est prononcé.
- Les questions de la barre du bas nomment leurs touches au clavier (« Partir, Tab ; Accepter le dé, F3 »). Les noms des touches sont prononcés dans votre langue.

## Touches ajoutées par le mod

- **F4** : l'aide de l'écran où vous êtes, d'abord les touches du jeu, puis celles du mod. Les flèches haut et bas la lisent entrée par entrée ; F4 ou Tab la ferme. (F1 est la touche d'inventaire du jeu.)
- **Ctrl + Haut / Ctrl + Bas** : lisent les détails de ce qui a le focus, ligne par ligne. Une carte est annoncée en bref (nom, genre, statistiques) ; ses traits et ses règles sont ici. Fonctionne partout, combat compris.
- **Ctrl + Gauche / Ctrl + Droite** : changent ce que lisent Ctrl + Haut et Bas : **Détails** (l'élément qui a le focus), **Événements** (tout ce qui a été annoncé jusqu'ici, le plus récent d'abord : texte des rencontres, questions, débuts de combat, alertes de vie, résultats) et **État** (vie, nourriture, or, renommée, poison et l'équipement de chaque emplacement). Les listes vides sont sautées.
- **H** (ou barre oblique, ou clic du stick droit) : votre vie, nourriture, or, renommée et poison.
- **O** : les objectifs du défi en cours.
- **Maj + flèches** (ou le stick droit) sur la carte : un curseur d'exploration qui parcourt tout le plateau visible carte par carte, en lisant chaque carte, si vous y êtes, si vous pouvez vous y rendre, si c'est la sortie et quelles directions en partent. Il ne fait que lire ; déplacez-vous avec les flèches et Entrée du jeu. Ctrl + Haut lit ensuite la carte explorée en entier.
- **Maj + point** sur la carte : le curseur d'exploration saute à la prochaine carte que vous n'avez pas encore visitée, la plus proche d'abord ; **Maj + virgule** revient à la précédente. Chaque lecture se termine par l'endroit où se trouve la carte par rapport à vous et sa place dans la liste (« non visitée 2 sur 5 ») ; après la dernière, on repart de la plus proche. Une boutique ou une autre carte où vous êtes déjà entré ne compte plus. Il ne fait que lire, comme Maj + flèches.
- **C** (ou clic du stick gauche) en combat : un bip venant de la direction de l'ennemi le plus proche. Une fois les ennemis éliminés, il pointe vers ce qu'il reste à rejoindre (la sortie, le cercle qui se referme d'un boss, l'or ou la nourriture tombés), ou dit « plus d'ennemis ».
- **Ennemis que vous pouvez achever** : un ennemi jeté à terre émet un sifflement montant depuis l'endroit où il gît, qui se répète plus vite à mesure que vous approchez, jusqu'à ce qu'il se relève. Allez-y et appuyez sur la touche d'achèvement du jeu. Quand l'ennemi le plus proche peut être achevé, **C** répond par ce sifflement au lieu du bip habituel. Dès que l'achèvement porterait (assez près, et dans la direction où vous poussez ou regardez), le sifflement prend un tintement clair : appuyez maintenant sur la touche d'achèvement, en poussant vers le son. **C** donne aussi le tintement quand l'ennemi le plus proche est à portée.

## Le combat à l'oreille

- Le combat commence par le nombre d'ennemis (« Combat, 3 ennemis ») et le compte est prononcé quand il change. Votre vie est annoncée quand elle passe sous la moitié, le quart et le dixième.
- **Tintement métallique** : une attaque que vous pouvez contrer. Appuyez sur défendre. Le jeu n'associe attaquer et défendre qu'à des boutons de la souris, le mod leur donne donc des touches : **V attaque et B défend** (sauf si vous attribuez les vôtres dans Paramètres, Commandes, Affecter une entrée). Les annonces nomment la touche du clavier, ou le bouton de la manette quand vous en utilisez une.
- **Craquement** : une attaque que vous ne pouvez pas bloquer. Esquivez (Espace).
- **Vent** : les murs. Le vent de chaque côté grandit quand vous approchez d'un mur de ce côté ; un choc signifie que vous êtes rentré dedans.
- Les projectiles, les dangers au sol, les rayons et les pièges ont chacun leur son, toujours placé là où se trouve le danger : éloignez-vous du son.
- Un carillon vous signale qu'une capacité d'arme (à gauche) ou d'artefact (à droite) est de nouveau prête.
- **Immobilisé** : quand un ennemi s'accroche à vous (nuées de gnomes, le piège d'un mage), vous entendez « Immobilisé » avec la touche à presser plusieurs fois jusqu'à « libre ». Les petits coups reçus entre-temps sont le piège lui-même.
- **Armure** : certains ennemis (la Terreur de Westwend, les ennemis corrompus) portent une armure que seul le **Choc** endommage (E au clavier par défaut, B à la manette ; le mod ajoute X s'il n'a pas de touche). Un bruit sourd métallique signifie que votre coup a ricoché ; la première fois sur chaque ennemi, vous entendez aussi « Blindé, utilisez Choc » avec la touche. Un craquement signifie que votre Choc l'a fissurée ; un fracas et « Armure brisée » signifient que vos coups portent de nouveau pleinement ; un scintillement montant signifie que l'armure se reforme (doux pendant qu'elle se répare, plus fort avec « Armure restaurée » quand une armure brisée revient). Chaque son vient de cet ennemi, et ils ont leur propre volume dans les réglages. La commande Étourdir du jeu s'appelle Choc partout dans le mod, comme sur les cartes du jeu.
- Si le combat est trop dur, le Mode apprenti du jeu (Paramètres, Général ; le jeu le propose aussi après quelques morts) automatise les ripostes et les mises à mort et augmente vos dégâts. Le mod fonctionne de la même façon dans les deux cas.
- Dans les combats qui se terminent à une sortie (salles à pièges), la sortie émet un bip depuis le prochain tournant du chemin praticable qui y mène ; marcher vers le bip contourne donc les murs jusqu'à la sortie ; l'or et la nourriture tombés émettent le même bip, plus léger.
- Un piège qui tire dans le couloir où vous vous tenez (flèches, fléchettes) joue le craquement : bougez.

Le menu de pause a une nouvelle option tout en bas, **Glossaire des sons** : parcourez-le avec les flèches pour entendre ce que signifie chaque son, Entrée le joue, Tab le ferme.

## Mini-jeux

- **Cartes de chance** : les cartes sont montrées, chacune avec sa propre tonalité et son résultat prononcé depuis sa position, puis la tonalité de chaque carte la suit pendant le mélange. Choisissez la position que vous avez suivie ; les cartes se lisent « Emplacement 1 », « Emplacement 2 »...
- **Pendule** : pendant qu'il balance, vous entendez le résultat sous le faisceau, qui se déplace de gauche à droite avec lui : plus aigu et plus brillant, c'est mieux. Appuyez sur confirmer sur la tonalité voulue ; le focus vous le rappelle.
- **Roue** : les cartes de la roue sont lues quand elle est disposée ; pendant qu'elle tourne, chaque segment passe avec un tic dans la voix de son résultat ; appuyez sur confirmer pour la ralentir. L'endroit où elle s'arrête est prononcé.
- **Dés** : appuyez sur confirmer pour lancer (sinon le jeu demande un glisser à la souris). La cible, le total et les dés de chaque lancer, et si la cible est atteinte, sont prononcés ; les dés que vous pouvez relancer lisent leur valeur et s'ils sont marqués, et marquer un dé est annoncé. F3 relance.

## Langues

Le mod suit la langue du jeu : les douze langues du jeu sont couvertes (tout ce que le jeu dit lui-même est toujours dans votre langue). Ce README existe dans chacune d'elles dans le dossier `docs`, en Markdown et en HTML (`docs/README.<langue>.md` et `docs/html/README.<langue>.html`).

## Pour les développeurs

Voir `CLAUDE.md`. `setup-bepinex.ps1` installe le chargeur, `build.ps1` compile et déploie, `test.ps1` lance les tests hors jeu.
