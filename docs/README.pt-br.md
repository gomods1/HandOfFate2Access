# Hand of Fate 2 Access

Um mod que torna Hand of Fate 2 jogável sem enxergar. Tudo o que o jogo mostra é falado pelo seu leitor de tela, e as partes em tempo real (o combate, o pêndulo, a roda, o embaralhar das cartas de sorte) ficam jogáveis de ouvido graças a sinais sonoros adicionados.

É uma reconstrução do abandonado "Hand of Fate 2 Accessibility Mod", baseada no Hand of Fate Access, o mod do primeiro jogo.

## Requisitos

- Windows.
- Hand of Fate 2 (Steam).
- Um leitor de tela (NVDA, JAWS e outros). Sem ele, o mod usa a voz embutida do Windows.

## Instalação

O jeito mais fácil é o instalador, `HandOfFate2AccessibilityInstaller.exe`, anexado a cada versão: ele encontra o jogo pela Steam, instala ou atualiza o mod e pode desinstalá-lo. Funciona com leitor de tela. Para instalar à mão:

1. Extraia o zip da versão na pasta do Hand of Fate 2 (a que contém `Hand of Fate 2.exe`; numa instalação padrão da Steam, `C:\Program Files (x86)\Steam\steamapps\common\Hand of Fate 2`). Permita mesclar as pastas.
2. Inicie o jogo. Alguns segundos após a abertura você ouve "Hand of Fate 2 Access versão ... carregado".

O zip contém o carregador de mods (BepInEx) já configurado.

## Como funciona

Hand of Fate 2 já tem navegação completa por teclado e controle; o mod a lê. Percorra menus, cartas e escolhas com os controles do próprio jogo (setas, Enter ou Espaço para confirmar, Tab para voltar, Esc para o menu de pausa) e cada item fala:

- As cartas leem nome, tipo, atributos e cada traço com seu texto de regras. O equipamento oferecido é comparado com o que você está usando.
- Os encontros leem título, texto da história, resultados ("+7 comida") e opções, incluindo o custo ou requisito impresso ao lado de uma opção e se ela está indisponível.
- As falas dubladas do carteador e dos personagens são lidas por inteiro no seu idioma, esteja a opção Legendas do jogo ligada ou não.
- As perguntas da barra inferior ("Equipar o Machado do guerreiro? Sim, Não") são lidas quando aparecem.
- Os objetivos do desafio são lidos quando aparecem, mudam, são concluídos ou falham.
- O fim de uma partida é anunciado com o veredito do próprio jogo (morte por ferimentos ou fome, desistência, desafio falhado), depois os resultados.
- Diálogos, o tutorial de controles de combate (com a tecla de cada ação), as apresentações de criaturas e a personalização do personagem (origens, sigilos e uma descrição de cada penteado e cor de roupa) são lidos.
- Mudanças de comida, ouro, fama e vida são anunciadas quando acontecem.
- Os nomes das telas são falados ao entrar nelas (menu principal, mapa, mapa de desafios, construtor de baralhos, inventário, pausa, resultados). O inventário também diz o que há em cada espaço, inclusive os vazios, já que o cursor do jogo pula os espaços vazios.
- A tabela de pontuação do fim da partida (Infinito) é lida linha por linha com os nomes das colunas.
- Lojas: cada item lê seu preço ("37 ouro") e avisa quando você não pode pagar; ações de confirmação que fazem algo específico ("Comprar, Enter") são anunciadas com sua tecla.
- Ações em uma tecla de função (F1 Inventário, F3 Mapa, F3 Concluir na tela do personagem, F3 Iniciar no construtor de baralhos) são anunciadas com sua tecla quando aparecem. O inventário diz que Tab (a tecla de voltar do jogo) o fecha.
- Em um encontro, as teclas numéricas de 1 a 9 escolhem uma opção diretamente (um recurso do jogo).
- O mapa de desafios: cada desafio lê seu nome e se está bloqueado (com o que o desbloqueia), concluído, ou se ganhou a ficha de ouro ou de prata; Campanha, Aventura, Infinito e Fogueira dizem qual está sendo mostrado (Fogueira contém a tela de mudar aparência). Abrir um desafio lê seu título e descrição.
- Configurações: cada linha lê seu nome e valor (listas, caixas de seleção, controles deslizantes, atalhos de teclas), e o novo valor é falado conforme você o altera.
- As cartas de recompensa viradas no fim de um desafio são lidas à medida que são reveladas.
- O construtor de baralhos: cada baralho diz seu nome, para que serve e quão cheio está ("Encontros, 13 de 13 cartas"); dentro, esquerda e direita percorrem as cartas disponíveis ou o seu baralho (baixo e cima alternam entre eles) e cada carta diz nome, tipo e atributos (Ctrl + Cima lê o resto); confirmar a adiciona ou remove e a nova contagem é falada.
- As perguntas da barra inferior nomeiam suas teclas no teclado ("Sair, Tab; Aceitar dado, F3"). Os nomes das teclas são falados no seu idioma.

## Teclas que o mod adiciona

- **F4**: ajuda para a tela em que você está, primeiro as teclas do jogo ali, depois as do mod. As setas para cima e para baixo a leem uma entrada por vez; F4 ou Tab a fecha. (F1 é a tecla de inventário do próprio jogo.)
- **Ctrl + Cima / Ctrl + Baixo**: leem os detalhes do que está em foco, uma linha por vez. Uma carta é anunciada de forma curta (nome, tipo, atributos); seus traços e regras ficam aqui. Funciona em todo lugar, inclusive no combate.
- **Ctrl + Esquerda / Ctrl + Direita**: alternam o que Ctrl + Cima e Baixo leem: **Detalhes** (o item em foco), **Eventos** (tudo o que foi anunciado até agora, do mais recente: texto dos encontros, perguntas, inícios de combate, avisos de vida, resultados) e **Status** (vida, comida, ouro, fama, veneno e o que está equipado em cada espaço). Listas vazias são puladas.
- **H** (ou barra, ou clique do analógico direito): sua vida, comida, ouro, fama e veneno.
- **O**: os objetivos do desafio atual.
- **Shift + setas** (ou o analógico direito) no mapa: um cursor de exploração que percorre todo o tabuleiro visível carta por carta, lendo cada carta, se você está nela, se pode ir até ela, se é a saída e quais direções continuam a partir dela. Ele só lê; mova-se com as setas e o Enter do próprio jogo. Ctrl + Cima então lê a carta explorada por inteiro.
- **Shift + ponto** no mapa: o cursor de exploração pula para a próxima carta que você ainda não visitou, a mais próxima primeiro; **Shift + vírgula** volta para a anterior. Cada leitura termina com onde a carta fica em relação a você e o lugar dela na lista ("não visitada 2 de 5"); depois da última, recomeça pela mais próxima. Uma loja ou outra carta em que você já entrou deixa de contar. Ele só lê, como Shift + setas.
- **C** (ou clique do analógico esquerdo) no combate: um bipe vindo da direção do inimigo mais próximo. Quando os inimigos acabam, ele aponta para o que resta alcançar (a saída, o círculo que se fecha de um chefe, ouro ou comida caídos), ou diz "nenhum inimigo restante".
- **Inimigos que você pode finalizar**: um inimigo derrubado toca um zunido ascendente de onde está caído, que se repete mais rápido conforme você se aproxima, até ele se levantar. Vá até ele e pressione a tecla de finalizar do jogo. Quando o inimigo mais próximo pode ser finalizado, **C** responde com esse zunido em vez do bipe de sempre. Assim que a finalização acertaria (perto o bastante e na direção em que você empurra ou olha), o zunido ganha um tilintar brilhante: pressione já a tecla de finalizar, empurrando na direção do som. **C** também dá o tilintar quando o inimigo mais próximo está ao alcance.

## Combate de ouvido

- A luta começa com o número de inimigos ("Combate, 3 inimigos") e a contagem é falada quando muda. Sua vida é falada quando cai abaixo da metade, de um quarto e de um décimo.
- **Clangor**: um ataque que você pode contra-atacar. Pressione defender. O jogo atribui atacar e defender apenas a botões do mouse, então o mod lhes dá teclas: **V ataca e B defende** (a menos que você atribua as suas em Configurações, Controles, Atribuir entrada). Os anúncios nomeiam a tecla do teclado, ou o botão do controle quando um está em uso.
- **Estalo**: um ataque que você não pode bloquear. Esquive (Espaço).
- **Vento**: paredes. O vento de cada lado cresce conforme você se aproxima de uma parede daquele lado; um baque significa que você bateu nela.
- Projéteis, perigos no chão, raios e armadilhas têm cada um seu próprio som, sempre posicionado onde está o perigo: afaste-se do som.
- Um sininho avisa que uma habilidade de arma (esquerda) ou de artefato (direita) está pronta de novo.
- **Preso**: quando um inimigo se agarra a você (enxames de gnomos, a armadilha de um mago) você ouve "Preso" com a tecla a pressionar repetidamente até ouvir "livre". Os pequenos golpes que você leva nesse meio-tempo são a própria armadilha.
- **Armadura**: alguns inimigos (o Terror de Westwend, inimigos corrompidos) usam uma armadura que só a **Investida** danifica (E no teclado por padrão, B no controle; o mod adiciona X se ela não tiver tecla). Um baque metálico surdo significa que seu golpe ricocheteou; na primeira vez em cada inimigo você também ouve "Blindado, use Investida" com a tecla. Um estalo significa que sua Investida rachou a armadura; um estrondo e "Armadura quebrada" significam que seus golpes voltam a causar dano total; um brilho ascendente significa que a armadura está se regenerando (baixo enquanto se conserta, mais alto com "Armadura restaurada" quando uma armadura quebrada volta). Cada som vem daquele inimigo, e eles têm volume próprio nas configurações. O controle Atordoar do jogo se chama Investida em todo o mod, como nas cartas do jogo.
- Se o combate estiver difícil demais, o Modo aprendiz do jogo (Configurações, Geral; o jogo também o oferece após algumas mortes) automatiza ripostes e finalizações e aumenta seu dano. O mod funciona do mesmo jeito nos dois casos.
- Em lutas que terminam numa saída (salas de armadilhas), a saída emite um bipe a partir da próxima curva do caminho transitável até ela, de modo que andar em direção ao bipe leva você ao redor das paredes até a saída; ouro e comida caídos emitem o mesmo bipe, mais leve.
- Uma armadilha que dispara pela pista em que você está (flechas, dardos) toca o estalo: saia dali.

O menu de pausa tem uma nova opção no fim, **Glossário de sons**: percorra-o com as setas para ouvir o que cada som significa, Enter o reproduz, Tab o fecha.

## Minijogos

- **Cartas de sorte**: as cartas são mostradas, cada uma com seu próprio tom e seu resultado falado a partir da sua posição, depois o tom de cada carta a segue durante o embaralhar. Escolha a posição que você acompanhou; as cartas são lidas como "Posição 1", "Posição 2"...
- **Pêndulo**: enquanto balança, você ouve o resultado sob o feixe, movendo-se para a esquerda e para a direita com ele: mais agudo e brilhante é melhor. Pressione confirmar no tom que quiser; o foco lembra você disso.
- **Roda**: as cartas da roda são lidas quando ela é montada; enquanto gira, cada segmento passa com um tique na voz do seu resultado; pressione confirmar para desacelerá-la. Onde ela para é falado.
- **Dados**: pressione confirmar para lançar (senão o jogo pede um arrastar do mouse). O alvo, o total e os dados de cada lançamento, e se você atingiu o alvo, são falados; dados que você pode relançar leem seu valor e se estão marcados, e marcar um é anunciado. F3 relança.

## Idiomas

O mod segue o idioma do jogo: todos os doze idiomas do jogo estão cobertos (tudo o que o próprio jogo diz está sempre no seu idioma). Este README existe em cada um deles na pasta `docs`, em Markdown e em HTML (`docs/README.<idioma>.md` e `docs/html/README.<idioma>.html`).

## Para desenvolvedores

Veja `CLAUDE.md`. `setup-bepinex.ps1` instala o carregador, `build.ps1` compila e implanta, `test.ps1` executa os testes offline.
