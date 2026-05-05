## Spectre.Console - Introduction au projet
Spectre.Console est une librairie .NET permettant de construire des interfaces en ligne de commande riches et interactives. Elle fournit des composants pour les couleurs, les tableaux, les invites utilisateur, les animations, etc.

# Installation
## Prérequis
- Un IDE capable d'exécuter une solution C# (Visual Studio, Rider, VS Code…)
- Git installé sur votre machine
- .NET SDK 10 [Installation](https://learn.microsoft.com/dotnet/core/install/windows?WT.mc_id=dotnet-35129-website#install-with-windows-package-manager-winget) 

## Étapes d'installation
1. Cloner le dépôt Git dans un dossier local:
	```powershell
	git clone https://github.com/MapelSiroup/spectre.console.git
	```
2. Se positionner à la racine du dossier puis basculer sur la branche de démonstration :
	```powershell
	git switch readme-avec-examples
	```
	Cette branche contient tous les changements présentés dans ce README ainsi que le fichier de démonstration.
3. Il se peut bien que vous aurez des "erreurs", si c'est le cas il est nécessaire de lancer une compilation initiale (bouton marteau/build dans IntelliJ) : afin de restaurer les paquets et s'assurer que tout est correctement configuré.

# Utilisation et Demo
## Démonstration
1. Ouvrez le fichier `src/InteractiveTests/Program.cs` dans votre IDE.
2. Exécutez ce projet (`dotnet run` ou via l'IDE) pour voir une démonstration des fonctionnalités issues des travaux décrits plus bas.

## Utiliser la librairie
Pour intégrer Spectre.Console dans vos propres projets :
1. Créez un projet console .NET standard.
2. Ajouter le repo dans votre projet, une fois cela fait, des que vous tentez d'invoquer des function spectre.console votre IDE devrais vous inviter a rendre les projets necessaires en references.
2. Si vous voulez la version officielle, Ajoutez le package NuGet `Spectre.Console` :
	```powershell
	dotnet add package Spectre.Console
	```
3. Vous devrez surement faire un build initial du projet Spectre.Console puisque certain fichier ne se font generer que par un build initial du projet (sinon vous aurez des erreurs de Couleurs & JSON dans le projet).
4. Écrivez votre code en vous référant à la [documentation officielle](https://spectreconsole.net/console) qui contient de nombreux exemples et explications.

Le [projet de démonstration](https://github.com/MapelSiroup/spectre.console/tree/readme-avec-examples/src/InteractiveTests) contient un programme de demonstration sur certaines fonctionnalités développées pour les issues présentées. Simplement executer le Main() du `program.cs` dans ce projet avec une console ou terminal de votre choix.

# Descriptions des issues
## Issue [#595](https://github.com/spectreconsole/spectre.console/issues/595) – `Text prompt with editable default value` (PR [#2016](https://github.com/spectreconsole/spectre.console/pull/2016))
- **Description** : Ajout d'une fonctionnalité permettant au concepteur de l'interface d'injecter du texte par défaut dans le tampon de saisie d'un prompt. Avant ce correctif, la valeur par défaut n'était que présentée avant les deux‑points (`:`) et n'était pas réellement insérée dans le buffer : l'utilisateur devait la retaper pour la modifier. Cette nouvelle option fait gagner du temps et rend l'expérience plus fluide. Elle corrige aussi un bug : la valeur par défaut était considérée comme valide quel que soit l'entrée, même si celle‑ci était mal formatée.

- **Travail réalisé** : La plupart du temps (une semaine) a été consacré à comprendre le fonctionnement interne du "input buffer" géré dans `AnsiConsoleExtensions.Input.cs`. Modifier ce composant a pris du temps, car il n'était pas documenté et les abstractions n'étaient pas évidentes.

- **Compatibilité** : La fonctionnalité ne casse pas la rétro‑compatibilité. L'implémentation évite les refactorisations profondes du code existant, respectant ainsi une exigence forte du projet.



## Issue [#1281](https://github.com/spectreconsole/spectre.console/issues/1281) – `Make prompts IRenderable` (PR [#2112](https://github.com/spectreconsole/spectre.console/pull/2112))
- **Description** : Refactoriser les classes de prompts pour qu'elles implémentent l'interface `IRenderable`. Cette transformation permet de manipuler les prompts comme des éléments graphiques : les insérer dans des mises en page (`Layouts`), les imbriquer dans d'autres `IRenderable`, etc. En pratique, cela les rendrait comparables à des widgets réutilisables.

- **Complexité** : Cette évolution nécessite une compréhension approfondie du pipeline de rendu de Spectre.Console. Il faut convertir des classes conçues pour un usage séquentiel et interactif en objets passif capables de se rendre "en temps réel" au sein du moteur. Tout en faisant cela, il est impératif de ne pas rompre la compatibilité avec les prompts existants, ce qui est délicat étant donné l'architecture actuelle et les dépendances internes. Le produit final est une approche utilisant un Hook pour

J'ai eu beaucoup de probleme pendant le mois de developement de ce systeme. La majeure partie des erreurs etais le rendu, le input handling qui fait rien ou l'ajout de nouvelle fonctionallitees dans le `TextPrompt.cs`. Un de ces bugs etais un bug de OverDraw a chaque fois que le renderable etais "dirty", donc chaque input ecrivait un autre prompt entier au dessus du precedent et parfois empietant sur le meme, le nouveau systeme de message d'invaliditee causait aussi un overdraw sur les bordure du panel/layout ou se trouvait le prompt, il fallut plusieurs jour pour decouvrir que il fallait changer le `LiveRenderable.cs` ce qui semble etre un "hack" plus que d'autre chose car le probleme semblait provenir du moyen de Build/Render le IRenderable. Un autre issue est que dans les tests interactifs que j'ai cree, vu que la branche PromptHistory n'a jamais toucher celle du Irenderable-Prompts, il est impossible de faire plusieurs action tel que remonter l'historique, hors de simplement taper les lettres dans le mode Renderable.


- **Travail réalisé** :
 - (Etat de l'issue a la Remise d'avancement)
 	1. Pour l'instant, seul une des classe prompts (TextPrompt.cs) à reçu les changements necessaire pour se rendre renderable, mais une fois celle-ci terminer il sera simple de rendre tout les autres en `IRenderable`. ~~Un des problemes actuel est qu'il brise la rétro-compatibilité lors de la gestion des tests. il faut donc une deuxieme refactorisation de ce code pour qu'ìl en soit compatible.~~
 - [Commit 82253b7](https://github.com/MapelSiroup/spectre.console/commit/82253b7a1045e5fb667c9df3a8c232fb30315b6e)
 	1. `TextPrompt.cs` -> Extends IRenderable, Introduction de Measure(), Render() et BuildPrompt().
 - (Apres Remise Avancement)
 - [Commit 1ca65b7](https://github.com/MapelSiroup/spectre.console/commit/1ca65b730b4cb21fc8772b4ffe7f9d821908d70e)
	1. Creation du `TextPromptRenderHook.cs` pour etre un "Live-Renderable" (permet de rafraichir et l'imbrication dans un autre Renderable).
	2. Creation de `ShowAsRenderableAsync()` dans `TextPrompt.cs` qui permet l'affichage en mode IRenderable et l'utilistation d'un wrapper si l'on a pas de Layout predefini pour placer le prompt dedans.
	3. Mise-A-Jour du `TextPrompt.cs` depuis le `Upstream/Main` qui a Ajouter l'issue `#595` (premiere issue du projet). Donc ajout de l'autofill `EditableDefaultValue()`.
	4. `LiveRenderable.cs` -> Fix un bug de `OverDraw` dans plusieurs cas lorsqu'un utilisateur update le renderable, le fix cause un des Test Unitaire d'un autre partie du projet a echouer a cause de la sequence Ascii de trop (movement de curseur supplementaire pour enlever le overdraw) fonctionellement le meme mais different au DIFF vu par le test.
 - [Commit 506e998](https://github.com/MapelSiroup/spectre.console/commit/506e99874e186d0912074016e22eb1b8808d691b)
	1. Deuxieme fix du `LiveRenderable.cs`
	2. Ajout du mode Renderable au `ConfirmationPrompt.cs` qui techniquement utilise simplement un TextPrompt en arriere plan (alors on passe juste le wrapper en parametre)
	3. Mise-a-jour du `ListPrompt.cs` qui avait deja son propre RenderHook pour utiliser notre systeme de wrapper.
	4. Ajout de `ShowAsRenderableAsync()` pour `MultiSelectionPrompt.cs` et `SelectionPrompt.cs`.
	5. Mise-a-jour de `TextPrompt.cs` pour le nouveau `_message` Handling.
 - [Commit 3945794](https://github.com/MapelSiroup/spectre.console/commit/3945794eb5dd140c2deaa39d49dd07acca1eed58)
	1. Ajout de quelques tests dans `TextPromptTests.cs`, `MultiSelectionPromptTests.cs` et `SelectionPromptTests.cs` pour verifier le systeme de Rendu.




## Issue [#1570](https://github.com/spectreconsole/spectre.console/issues/1570) - `More Clearing Methods` (PR [#2114](https://github.com/spectreconsole/spectre.console/pull/2114))
- **Description** : Ajouter des nouveau moyen pour "clear" la console, partiellement avec des ligne specifique ou une zone. L'issue de base explique que l'on peut le faire avec plusieurs methodes, comme un enum, des classes statiques ou token markup; notamment les token markup m'on interesser parce que il est tres intuitif pour les utiliser et le systeme de tokenisation permet l'ajout de fonctionalitee ou d'aliases facilement, permettant la modification et l'expansion du repertoire de token au fur et a mesure. J'ai choisi de creer pas juste des token pour "clear" l'ecran mais aussi pour positioner le curseur dans une position relationelle ou globale dans l'ecran console. Cette methode permet d'inserer des directives textuelles (`[clear:...]` et `[move:...]`) afin de piloter le rendu directement depuis des strings.

- **Complexité** :
  - La complexite de ce systeme etais moyenne, Le challenge principal a été de modifier le parser markup existant sans casser le rendu des styles et des balises standards existantes.
  - Il a fallu changer le markup pour gérer deux types de segments, l'un des deux etant une nouvelle type de balise : les segments de texte stylés et les segments de contrôle ANSI (ce qui controle la console), sans affecter les tokens markup de style courant dans une meme string.
  - Comprendre le paradigme de programmation existant des sequences ANSI pour executer des commande du curseur console et de l'utilisation du Markup dans le projet console.
  
  J'ai eu quelque probleme avec les balise auto-fermante qui pouvait ruiner le style ou effacer les mauvaise lignes. Les token/balises `[escnext]` et `[wnext]` ne se comptait pas eu meme pendant un bon bout de temps ce qui faisait en sorte que quelque chose comme `[wnext][wnext]` dans une string n'ecrivait pas la deuxieme balise en texte et plantait le programme. J'ai encore un probleme que je n'ai jamais regler c'est le fait que je n'ai pas de moyen pour restorer le curseur apres un mouvement, cela fait que le texte peut etre decaler une fois un mouvement fait et "overdraw" sur du texte existant. Il faut donc faire attention quand l'on l'utilise mais j'aime quand meme avoir la possibilitee de placer le curseur peut importe ou. 

- **Travail réalisé** :
  - [Commit 8fd361d](https://github.com/MapelSiroup/spectre.console/commit/8fd361d0a86f1d9cb13694201385be46fe94768c)
  	1. Implémentation des tokens markup pour l'effacement et le positionnement du curseur.
  	2. Gestion de l'interpretation des tokens en segments de contrôle ANSI `Paragraph.cs`.
  	3. Implémentation de `[clear:line]`, `[clear:to eol]`, `[clear:to eos]`, `[clear:screen]`, ainsi que `[clear:#row]` qui parse le textuel en code ANSI dans `AnsiMarkup.cs`.
  - [Commit 2dcbc9f](https://github.com/MapelSiroup/spectre.console/commit/2dcbc9f41be4ae7a5e5db8e66c9c31b613774d98)
  	1. Implémentation de `[move:up N]`, `[move:down N]`, `[move:forward N]`, `[move:backward N]`, `[move:left N]`, `[move:right N]`, `[move:to row;col]`, `[move:to N]`, `[move:home]`, `[move:origin]`.
    2. Ajout de token d'echapement et d'ecriture brut, `[escnext]`, `[wnext]`. 
  -	[Commit 2f98419](https://github.com/MapelSiroup/spectre.console/commit/2f9841989740410d0cbb270851230355b77bee14)
  	1. Ajout des tests unitaire pour `[move:...]`, `[clear:...]`, `[escnext]`, `[wnext]` couvrant les cas valides et invalides.


## Issue [#158](https://github.com/spectreconsole/spectre.console/issues/158) - `Up key listener for prompt history` (PR [#2113](https://github.com/spectreconsole/spectre.console/pull/2113))
- **Description** : Le but de cette issue est de creer un moyen pour les utilisateurs de retrouver le texte de leurs reponses precedentes avec les fleches haut et bas du clavier, permettant de retourner facilement en arriere, utile pour des application qui necessiterais des textprompt pour controller des commandes repetitives ou des confirmationprompt a la suite des uns des autres. Il n'y avait pas de suggestion dans lissue pour la methode a utiliser pour rendre cela possible mais simplement le resultat attendue d'appuyer sur les fleches haut/bas. L'historique doit aussi etre dans l'ordre que les reponses on ete soumises.

- **Complexite** : La complexite de ce systeme etais moyenne, il fallut que je reflechisse un peu avant de commencer pour comprendre l'initialisation de l'object "console" et comment elle perdure dans le temps, ceci pour permettre que l'historique reste avec le programme et ne ce fasse pas detruire lorsque on a fini avec un prompt. J'ai fini par choisir d'avoir l'historique comme etant un object, cela rend le systeme un peu plus modulaire que d'une simple liste de string ou peut importe. L'historique etant un object permet aussi d'ajouter des fonctions et des regles ou flags a l'historique tel que clear() qui peut remettre a zero l'historique, ou add() pour ajouter un element dans l'historique.

- **Travail réalisé** :
  - [Commit 24da364](https://github.com/MapelSiroup/spectre.console/commit/24da364cbc808b11ca8fa5ce94834f8db1aeda91)
  	1. Creation de la classe `PromptHistory.cs`, qui contient la liste des entrees de l'historique et la fonction d'ajout d'entree et de remise a zero.
  	2. Ajout de `.History(TextPrompt obj, PromptHistory obj)` et `.DisableHistory(TextPrompt obj)` dans `TextPromptExtension.cs` qui permet de mettre un historique specifique et de desactiver la fonction historique manuellement, respectivement.
  	3. Implémentation de l'ajout de reponse valide dans l'historique avant leur retour dans la fonction `ShowAsync()` de `TextPrompt.cs`.
  	4. Mise a jour du `ConfirmationPrompt.cs` qui utilise internalement TextPrompt<char>() pour pouvoir lui fournir un PromptHistory specifique.
  	5. Gestion de l'interpretation des fleches Haut et Bas dans le inputhandling de `AnsiConsoleExtensions.Input.cs` pour permettre de naviguer l'historique dans l'ordre.
  	6. Ajout des tests unitaire pour `TextPromptTests.cs` couvrant les cas valides et invalides.
  - [Commit 88f5b2b](https://github.com/MapelSiroup/spectre.console/commit/88f5b2ba76f45c1732feac83e6f2d02b3cbe697a)
  	1. Ajout des tests unitaire pour `ConfirmationPromptTests.cs` couvrant les cas valides, invalides et la possibilite de lire l'historique d'un TextPrompt precedent.


# TODO avant la semaine 13
- [x] Ajouter au moins deux nouvelles issues à traiter.  [[#1570](https://github.com/spectreconsole/spectre.console/issues/1570)(PR: [#2114](https://github.com/spectreconsole/spectre.console/pull/2114)) et [#158](https://github.com/spectreconsole/spectre.console/issues/158)(PR: [#2113](https://github.com/spectreconsole/spectre.console/pull/2113))]
- [x]  Écrire au moins cinq tests unitaires supplémentaires couvrant les nouvelles fonctionnalités. 