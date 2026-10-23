# Hand of Fate 2 Access

Un mod que hace que Hand of Fate 2 se pueda jugar sin ver. Todo lo que el juego muestra se lee a través de tu lector de pantalla, y las partes en tiempo real (el combate, el péndulo, la rueda, la mezcla de las cartas de azar) se pueden jugar de oído gracias a señales sonoras añadidas.

Es una reconstrucción del abandonado "Hand of Fate 2 Accessibility Mod", basada en Hand of Fate Access, el mod del primer juego.

## Requisitos

- Windows.
- Hand of Fate 2 (Steam).
- Un lector de pantalla (NVDA, JAWS y otros). Sin él, el mod usa la voz integrada de Windows.

## Instalación

Lo más fácil es el instalador, `HandOfFate2AccessibilityInstaller.exe`, adjunto a cada versión: encuentra el juego a través de Steam, instala o actualiza el mod y puede desinstalarlo. Funciona con lector de pantalla. Para instalarlo a mano:

1. Extrae el zip de la versión en la carpeta de Hand of Fate 2 (la que contiene `Hand of Fate 2.exe`; en una instalación de Steam normal, `C:\Program Files (x86)\Steam\steamapps\common\Hand of Fate 2`). Permite que se combinen las carpetas.
2. Inicia el juego. Unos segundos después del arranque oirás "Hand of Fate 2 Access versión ... cargado".

El zip contiene el cargador de mods (BepInEx) ya configurado.

## Cómo funciona

Hand of Fate 2 ya tiene navegación completa por teclado y mando; el mod la lee. Muévete por los menús, cartas y opciones con los controles propios del juego (flechas, Intro o Espacio para confirmar, Tabulador para volver, Escape para el menú de pausa) y cada elemento habla:

- Las cartas leen su nombre, tipo, estadísticas y cada rasgo con su texto de reglas. El equipo que te ofrecen se compara con el que llevas puesto.
- Los encuentros leen su título, el texto de la historia, los resultados ("+7 de comida") y las opciones, incluido el coste o requisito impreso junto a una opción y si no está disponible.
- Las frases habladas del repartidor y de los personajes se leen completas en tu idioma, esté o no activada la opción Subtítulos del juego.
- Las preguntas de la barra inferior ("¿Equipar el Hacha del guerrero? Sí, No") se leen cuando aparecen.
- Los objetivos del desafío se leen cuando aparecen, cambian, se completan o fallan.
- El final de una partida se anuncia con el veredicto del propio juego (muerte por heridas o por hambre, abandono, desafío fallido) y después los resultados.
- Se leen los diálogos, el tutorial de controles de combate (con la tecla de cada acción), las presentaciones de criaturas y la personalización del personaje (orígenes, sellos y una descripción de cada peinado y color de atuendo).
- Los cambios de comida, oro, fama y vida se anuncian cuando ocurren.
- Los nombres de las pantallas se dicen al entrar en ellas (menú principal, mapa, mapa de desafíos, constructor de mazos, inventario, pausa, resultados). El inventario también dice qué hay en cada ranura, incluidas las vacías, porque el cursor del juego se salta las ranuras vacías.
- La tabla de puntuación del final de la partida (Infinito) se lee fila a fila con los nombres de sus columnas.
- Tiendas: cada artículo lee su precio ("37 de oro") y dice cuándo no puedes pagarlo; las acciones de confirmación que hacen algo concreto ("Comprar, Intro") se anuncian con su tecla.
- Las acciones asignadas a una tecla de función (F1 Inventario, F3 Mapa, F3 Terminar en la pantalla del personaje, F3 Empezar en el constructor de mazos) se anuncian con su tecla cuando aparecen. El inventario dice que Tabulador (la tecla de volver del juego) lo cierra.
- En un encuentro, las teclas numéricas del 1 al 9 eligen una opción directamente (una función del juego).
- El mapa de desafíos: cada desafío lee su nombre y si está bloqueado (con lo que lo desbloquea), completado o si ha ganado su ficha de oro o de plata; Campaña, Aventura, Infinito y Hoguera dicen cuál se muestra (Hoguera contiene la pantalla de cambiar apariencia). Al abrir un desafío se leen su título y descripción.
- Ajustes: cada fila lee su nombre y su valor (listas, casillas, deslizadores, asignaciones de teclas), y el nuevo valor se dice al cambiarlo.
- Las cartas de recompensa que se dan la vuelta al final de un desafío se leen al revelarse.
- El constructor de mazos: cada mazo dice su nombre, para qué sirve y cuánto está lleno ("Encuentros, 13 de 13 cartas"); dentro, izquierda y derecha recorren las cartas disponibles o tu mazo (abajo y arriba cambian entre ambos) y cada carta dice su nombre, tipo y estadísticas (Ctrl + Arriba lee el resto); confirmar la añade o la quita y se dice el nuevo recuento.
- Las preguntas de la barra inferior nombran sus teclas en el teclado ("Marcharse, Tabulador; Aceptar dado, F3"). Los nombres de las teclas se dicen en tu idioma.

## Teclas que añade el mod

- **F4**: ayuda para la pantalla en la que estás, primero las teclas del juego en ella y luego las del mod. Las flechas arriba y abajo la leen entrada a entrada; F4 o Tabulador la cierran. (F1 es la tecla de inventario del propio juego.)
- **Ctrl + Arriba / Ctrl + Abajo**: leen los detalles de lo que tiene el foco, línea a línea. Una carta se anuncia en corto (nombre, tipo, estadísticas); sus rasgos y reglas están aquí. Funciona en todas partes, incluido el combate.
- **Ctrl + Izquierda / Ctrl + Derecha**: cambian lo que leen Ctrl + Arriba y Abajo: **Detalles** (lo que tiene el foco), **Eventos** (todo lo anunciado hasta ahora, lo más reciente primero: texto de encuentros, preguntas, inicios de combate, avisos de vida, resultados) y **Estado** (vida, comida, oro, fama, veneno y lo equipado en cada ranura). Las listas vacías se saltan.
- **H** (o barra, o pulsación del stick derecho): tu vida, comida, oro, fama y veneno.
- **O**: los objetivos del desafío actual.
- **Mayús + flechas** (o el stick derecho) en el mapa: un cursor de exploración que recorre todo el tablero visible carta a carta, leyendo cada carta, si estás en ella, si puedes moverte a ella, si es la salida y qué direcciones continúan desde ella. Solo lee; muévete con las flechas e Intro del propio juego. Ctrl + Arriba lee después la carta explorada completa.
- **Mayús + punto** en el mapa: el cursor de exploración salta a la siguiente carta que aún no has visitado, la más cercana primero; **Mayús + coma** vuelve a la anterior. Cada lectura termina con dónde está la carta respecto a ti y su lugar en la lista («sin visitar 2 de 5»); después de la última vuelve a empezar por la más cercana. Una tienda u otra carta en la que ya entraste deja de contar. Solo lee, como Mayús + flechas.
- **C** (o pulsación del stick izquierdo) en combate: un pitido desde la dirección del enemigo más cercano. Cuando no quedan enemigos, apunta a lo que queda por alcanzar (la salida, el círculo que se cierra de un jefe, oro o comida caídos) o dice "no quedan enemigos".
- **Enemigos que puedes rematar**: un enemigo derribado suena con un silbido ascendente desde donde está, que se repite más rápido cuanto más te acercas, hasta que se levanta. Ve hacia él y pulsa la tecla de remate del juego. Cuando al enemigo más cercano se le puede rematar, **C** responde con ese silbido en vez del pitido habitual. En cuanto el remate acertaría (lo bastante cerca y en la dirección hacia la que empujas o miras), el silbido lleva encima un tintineo brillante: pulsa ya la tecla de remate, empujando hacia el sonido. **C** también da el tintineo cuando el enemigo más cercano está a tu alcance.

## Combate de oído

- El combate empieza con el número de enemigos ("Combate, 3 enemigos") y el recuento se dice cuando cambia. Tu vida se dice cuando baja de la mitad, de un cuarto y de un décimo.
- **Tañido metálico**: un ataque que puedes contraatacar. Pulsa defender. El juego asigna atacar y defender solo a botones del ratón, así que el mod les da teclas: **V ataca y B defiende** (salvo que asignes las tuyas en Ajustes, Controles, Asignar entrada). Los anuncios nombran la tecla del teclado, o el botón del mando cuando usas uno.
- **Chasquido**: un ataque que no puedes bloquear. Esquiva (Espacio).
- **Viento**: paredes. El viento de cada lado crece al acercarte a una pared de ese lado; un golpe significa que has chocado con una.
- Proyectiles, peligros en el suelo, rayos y trampas tienen cada uno su propio sonido, siempre situado donde está el peligro: aléjate del sonido.
- Una campanilla te avisa de que una habilidad de arma (izquierda) o de artefacto (derecha) vuelve a estar lista.
- **Atrapado**: cuando un enemigo se te engancha (enjambres de gnomos, la trampa de un mago) oyes "Atrapado" con la tecla que debes pulsar repetidamente hasta oír "libre". Los pequeños golpes que recibes mientras tanto son la propia trampa.
- **Armadura**: algunos enemigos (el Terror de Westwend, enemigos corruptos) llevan una armadura que solo daña la **Embestida** (E en el teclado por defecto, B en el mando; el mod añade X si no tiene tecla). Un golpe sordo metálico significa que tu ataque rebotó; la primera vez con cada enemigo oyes además «Con armadura, usa Embestida» con la tecla. Un crujido significa que tu Embestida la agrietó; un estruendo y «Armadura rota» significan que tus golpes vuelven a hacer todo su daño; un brillo ascendente significa que la armadura se regenera (suave mientras se repara, más fuerte con «Armadura restaurada» cuando vuelve una armadura rota). Cada sonido sale de ese enemigo y tienen su propio volumen en los ajustes. El control Aturdir del juego se llama Embestida en todo el mod, como en las cartas del juego.
- Si el combate es demasiado difícil, el Modo aprendiz del juego (Ajustes, General; el juego también lo ofrece tras unas cuantas muertes) automatiza las réplicas y los remates y aumenta tu daño. El mod funciona igual en ambos casos.
- En los combates que terminan en una salida (salas de trampas), la salida emite un pitido desde el siguiente giro del camino transitable hacia ella, de modo que caminar hacia el pitido te lleva rodeando las paredes hasta la salida; el oro y la comida caídos pitan igual con un sonido más ligero.
- Una trampa que dispara por el carril en el que estás (flechas, dardos) hace sonar el chasquido: muévete.

El menú de pausa tiene una opción nueva al final, **Glosario de sonidos**: recórrelo con las flechas para oír qué significa cada sonido, Intro lo reproduce, Tabulador lo cierra.

## Minijuegos

- **Cartas de azar**: se muestran las cartas, cada una con su propio tono y su resultado dicho desde su posición, y luego el tono de cada carta la sigue durante la mezcla. Elige la posición que seguiste; las cartas se leen como "Posición 1", "Posición 2"...
- **Péndulo**: mientras oscila oyes el resultado que hay bajo el haz, moviéndose a izquierda y derecha con él: más agudo y brillante es mejor. Pulsa confirmar en el tono que quieras; al enfocarlo se te recuerda.
- **Rueda**: las cartas de la rueda se leen cuando se coloca; mientras gira, cada segmento pasa con un tic en la voz de su resultado; pulsa confirmar para frenarla. Se dice dónde se detiene.
- **Dados**: pulsa confirmar para lanzar (el juego, si no, pide arrastrar con el ratón). Se dicen el objetivo, el total y los dados de cada tirada y si alcanzaste el objetivo; los dados que puedes volver a tirar leen su valor y si están marcados, y marcar uno se anuncia. F3 vuelve a tirar.

## Idiomas

El mod sigue el idioma del juego: los doce idiomas del juego están cubiertos (todo lo que dice el propio juego está siempre en tu idioma). Este README existe en todos ellos en la carpeta `docs`, en Markdown y en HTML (`docs/README.<idioma>.md` y `docs/html/README.<idioma>.html`).

## Para desarrolladores

Consulta `CLAUDE.md`. `setup-bepinex.ps1` instala el cargador, `build.ps1` compila y despliega, `test.ps1` ejecuta las pruebas sin el juego.
