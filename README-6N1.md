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
- **Description** : Refactoriser les classes de prompts pour qu'elles implémentent l'interface `IRenderable`. Cette transformation permet de manipuler les prompts comme des éléments graphiques : les insérer dans des mises en page (`Layouts`), les imbriquer dans d'autres `IRenderable`, etc. En pratique, cela les rend comparables à des widgets réutilisables.

- **Complexité** : Cette évolution nécessite une compréhension approfondie du pipeline de rendu de Spectre.Console. Il faut convertir des classes conçues pour un usage séquentiel et interactif en objets passifs capables de se rendre "en temps réel" au sein du moteur. Tout en faisant cela, il est impératif de ne pas rompre la compatibilité avec les prompts existants, ce qui est délicat étant donné l'architecture actuelle et les dépendances internes. Le produit final est une approche utilisant un hook pour

J'ai eu beaucoup de problèmes pendant le mois de développement de ce système. La majeure partie des erreurs concernait le rendu, le input handling qui ne faisait rien, ou l'ajout de nouvelles fonctionnalités dans le `TextPrompt.cs`. Un de ces bugs était un problème d'overdraw : à chaque fois que le renderable était "dirty", chaque input écrivait un autre prompt complet au-dessus du précédent, et parfois en empiétant sur lui-même. Le nouveau système de messages d'invalidité causait aussi un overdraw sur les bordures du panel/layout où se trouvait le prompt. Il a fallu plusieurs jours pour découvrir qu'il fallait modifier le `LiveRenderable.cs`, ce qui semble être davantage un "hack" qu'autre chose, car le problème semblait provenir de la manière de build/render le `IRenderable`. Un autre problème est que, dans les tests interactifs que j'ai créés, vu que la branche PromptHistory n'a jamais touché celle de Irenderable-Prompts, il est impossible de faire plusieurs actions telles que remonter l'historique, autre que simplement taper les lettres dans le mode renderable.

- **Travail réalisé** :
 - (État de l'issue à la remise d'avancement)
 	1. Pour l'instant, seule une des classes de prompts (`TextPrompt.cs`) a reçu les changements nécessaires pour devenir renderable, mais une fois celle-ci terminée, il sera simple de rendre toutes les autres en `IRenderable`. ~~Un des problèmes actuels est qu'il brise la rétrocompatibilité lors de la gestion des tests. Il faut donc une deuxième refactorisation de ce code pour qu'il en soit compatible.~~
 - [Commit 82253b7](https://github.com/MapelSiroup/spectre.console/commit/82253b7a1045e5fb667c9df3a8c232fb30315b6e)
 	1. `TextPrompt.cs` → Étend `IRenderable`, introduction de `Measure()`, `Render()` et `BuildPrompt()`.
 - (Après remise d'avancement)
 - [Commit 1ca65b7](https://github.com/MapelSiroup/spectre.console/commit/1ca65b730b4cb21fc8772b4ffe7f9d821908d70e)
	1. Création du `TextPromptRenderHook.cs` pour être un "Live-Renderable" (permet de rafraîchir et l'imbrication dans un autre renderable).
	2. Création de `ShowAsRenderableAsync()` dans `TextPrompt.cs`, qui permet l'affichage en mode `IRenderable` et l'utilisation d'un wrapper si l'on n'a pas de layout prédéfini pour y placer le prompt.
	3. Mise à jour du `TextPrompt.cs` depuis le `Upstream/Main`, qui a ajouté l'issue `#595` (première issue du projet), incluant l'ajout de l'autofill `EditableDefaultValue()`.
	4. `LiveRenderable.cs` → Correction d'un bug d'overdraw dans plusieurs cas lorsqu'un utilisateur met à jour le renderable. Le correctif cause l'échec d'un test unitaire dans une autre partie du projet à cause d'une séquence ASCII supplémentaire (mouvement de curseur additionnel pour enlever l'overdraw), fonctionnellement identique mais différent au niveau du diff détecté par le test.
 - [Commit 506e998](https://github.com/MapelSiroup/spectre.console/commit/506e99874e186d0912074016e22eb1b8808d691b)
	1. Deuxième correction du `LiveRenderable.cs`.
	2. Ajout du mode renderable au `ConfirmationPrompt.cs`, qui utilise techniquement un `TextPrompt` en arrière-plan (donc on passe simplement le wrapper en paramètre).
	3. Mise à jour du `ListPrompt.cs`, qui avait déjà son propre RenderHook, pour utiliser notre système de wrapper.
	4. Ajout de `ShowAsRenderableAsync()` pour `MultiSelectionPrompt.cs` et `SelectionPrompt.cs`.
	5. Mise à jour de `TextPrompt.cs` pour le nouveau handling de `_message`.
 - [Commit 3945794](https://github.com/MapelSiroup/spectre.console/commit/3945794eb5dd140c2deaa39d49dd07acca1eed58)
	1. Ajout de quelques tests dans `TextPromptTests.cs`, `MultiSelectionPromptTests.cs` et `SelectionPromptTests.cs` pour vérifier le système de rendu.




## Issue [#1570](https://github.com/spectreconsole/spectre.console/issues/1570) - `More Clearing Methods` (PR [#2114](https://github.com/spectreconsole/spectre.console/pull/2114))
- **Description** : Ajouter de nouveaux moyens pour "clear" la console, partiellement avec des lignes spécifiques ou une zone. L'issue de base explique que l'on peut le faire avec plusieurs méthodes, comme un enum, des classes statiques ou des tokens markup. Notamment, les tokens markup m'ont intéressé parce qu'ils sont très intuitifs à utiliser et que le système de tokenisation permet l'ajout de fonctionnalités ou d'alias facilement, permettant la modification et l'expansion du répertoire de tokens au fur et à mesure. J'ai choisi de créer non seulement des tokens pour "clear" l'écran, mais aussi pour positionner le curseur à une position relationnelle ou globale dans l'écran console. Cette méthode permet d'insérer des directives textuelles (`[clear:...]` et `[move:...]`) afin de piloter le rendu directement depuis des strings. 

- **Complexité** :
  - La complexité de ce système était moyenne. Le principal défi a été de modifier le parser markup existant sans casser le rendu des styles et des balises standards déjà en place.
  - Il a fallu modifier le markup pour gérer deux types de segments, dont un nouveau type de balise : les segments de texte stylés et les segments de contrôle ANSI (ce qui contrôle la console), sans affecter les tokens markup de style courant dans une même string.
  - Comprendre le paradigme de programmation existant des séquences ANSI pour exécuter des commandes du curseur console, ainsi que l'utilisation du markup dans le projet.

  J'ai eu quelques problèmes avec les balises auto-fermantes qui pouvaient ruiner le style ou effacer les mauvaises lignes. Les tokens/balises `[escnext]` et `[wnext]` ne se comptaient pas eux-mêmes pendant un bon moment, ce qui faisait en sorte que quelque chose comme `[wnext][wnext]` dans une string n'écrivait pas la deuxième balise en texte et faisait planter le programme. J'ai encore un problème que je n'ai jamais réglé : je n'ai pas de moyen de restaurer le curseur après un déplacement, ce qui fait que le texte peut être décalé une fois un mouvement effectué et faire de l'overdraw sur du texte existant. Il faut donc faire attention lors de son utilisation, mais j'aime quand même avoir la possibilité de placer le curseur n'importe où.

- **Travail réalisé** :
  - [Commit 8fd361d](https://github.com/MapelSiroup/spectre.console/commit/8fd361d0a86f1d9cb13694201385be46fe94768c)
  	1. Implémentation des tokens markup pour l'effacement et le positionnement du curseur.
  	2. Gestion de l'interprétation des tokens en segments de contrôle ANSI dans `Paragraph.cs`.
  	3. Implémentation de `[clear:line]`, `[clear:to eol]`, `[clear:to eos]`, `[clear:screen]`, ainsi que `[clear:#row]`, qui parse le texte en code ANSI dans `AnsiMarkup.cs`.
  - [Commit 2dcbc9f](https://github.com/MapelSiroup/spectre.console/commit/2dcbc9f41be4ae7a5e5db8e66c9c31b613774d98)
  	1. Implémentation de `[move:up N]`, `[move:down N]`, `[move:forward N]`, `[move:backward N]`, `[move:left N]`, `[move:right N]`, `[move:to row;col]`, `[move:to N]`, `[move:home]`, `[move:origin]`.
    2. Ajout de tokens d'échappement et d'écriture brute : `[escnext]`, `[wnext]`. 
  -	[Commit 2f98419](https://github.com/MapelSiroup/spectre.console/commit/2f9841989740410d0cbb270851230355b77bee14)
  	1. Ajout des tests unitaires pour `[move:...]`, `[clear:...]`, `[escnext]`, `[wnext]`, couvrant les cas valides et invalides.


## Issue [#158](https://github.com/spectreconsole/spectre.console/issues/158) - `Up key listener for prompt history` (PR [#2113](https://github.com/spectreconsole/spectre.console/pull/2113))
- **Description** : Le but de cette issue est de créer un moyen pour les utilisateurs de retrouver le texte de leurs réponses précédentes avec les flèches haut et bas du clavier, permettant de retourner facilement en arrière. Cela est utile pour des applications qui nécessiteraient des `TextPrompt` pour contrôler des commandes répétitives ou des `ConfirmationPrompt` à la suite les uns des autres. Il n'y avait pas de suggestion dans l'issue concernant la méthode à utiliser pour rendre cela possible, mais simplement le résultat attendu lors de l'appui sur les flèches haut/bas. L'historique doit aussi être dans l'ordre dans lequel les réponses ont été soumises.

- **Complexité** : La complexité de ce système était moyenne. Il a fallu que je réfléchisse un peu avant de commencer afin de comprendre l'initialisation de l'objet "console" et comment celui-ci perdure dans le temps, afin de permettre que l'historique reste avec le programme et ne soit pas détruit lorsque l'on a terminé avec un prompt. J'ai fini par choisir d'avoir l'historique comme étant un objet, ce qui rend le système un peu plus modulaire qu'une simple liste de strings ou autre. Le fait que l'historique soit un objet permet aussi d'ajouter des fonctions, des règles ou des flags à celui-ci, tels que `Clear()` qui remet l'historique à zéro, ou `Add()` pour ajouter un élément dans l'historique.

- **Travail réalisé** :
  - [Commit 24da364](https://github.com/MapelSiroup/spectre.console/commit/24da364cbc808b11ca8fa5ce94834f8db1aeda91)
  	1. Création de la classe `PromptHistory.cs`, qui contient la liste des entrées de l'historique ainsi que les fonctions d'ajout d'entrée et de remise à zéro.
  	2. Ajout de `.History(TextPrompt obj, PromptHistory obj)` et `.DisableHistory(TextPrompt obj)` dans `TextPromptExtension.cs`, permettant respectivement de définir un historique spécifique et de désactiver la fonctionnalité d'historique manuellement.
  	3. Implémentation de l'ajout des réponses valides dans l'historique avant leur retour dans la fonction `ShowAsync()` de `TextPrompt.cs`.
  	4. Mise à jour du `ConfirmationPrompt.cs`, qui utilise internalement `TextPrompt<char>()` afin de pouvoir lui fournir un `PromptHistory` spécifique.
  	5. Gestion de l'interprétation des flèches haut et bas dans le input handling de `AnsiConsoleExtensions.Input.cs` afin de permettre la navigation dans l'historique dans l'ordre.
  	6. Ajout de tests unitaires dans `TextPromptTests.cs`, couvrant les cas valides et invalides.
  - [Commit 88f5b2b](https://github.com/MapelSiroup/spectre.console/commit/88f5b2ba76f45c1732feac83e6f2d02b3cbe697a)
  	1. Ajout de tests unitaires dans `ConfirmationPromptTests.cs`, couvrant les cas valides, invalides ainsi que la possibilité de lire l'historique d'un `TextPrompt` précédent.


# TODO avant la semaine 13
- [x] Ajouter au moins deux nouvelles issues à traiter.  [[#1570](https://github.com/spectreconsole/spectre.console/issues/1570)(PR: [#2114](https://github.com/spectreconsole/spectre.console/pull/2114)) et [#158](https://github.com/spectreconsole/spectre.console/issues/158)(PR: [#2113](https://github.com/spectreconsole/spectre.console/pull/2113))]
- [x]  Écrire au moins cinq tests unitaires supplémentaires couvrant les nouvelles fonctionnalités. 