## Spectre.Console - Introduction au projet
Spectre.Console est une librairie .NET permettant de construire des interfaces en ligne de commande riches et interactives. Elle fournit des composants pour les couleurs, les tableaux, les invites utilisateur, les animations, etc.

# Installation
## Prérequis
- Un IDE capable d'exécuter une solution C# (Visual Studio, Rider, VS Code…)
- Git installé sur votre machine
- .NET SDK 10 

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
3. Il se peut bien que vous aurez des "erreurs", si c'est le cas il est nécessaire de lancer une compilation initiale (bouton marteau/build dans IntelliJ) :

	afin de restaurer les paquets et s'assurer que tout est correctement configuré.

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
3. Écrivez votre code en vous référant à la [documentation officielle](https://spectreconsole.net/console) qui contient de nombreux exemples et explications.

Le fichier de démonstration mentionné ci-dessus illustre comment exploiter certaines fonctionnalités développées pour les issues présentées.

# Descriptions des issues
## Issue #595 – `Text prompt with editable default value` (PR [#2016](https://github.com/spectreconsole/spectre.console/pull/2016))
- **Description** : Ajout d'une fonctionnalité permettant au concepteur de l'interface d'injecter du texte par défaut dans le tampon de saisie d'un prompt. Avant ce correctif, la valeur par défaut n'était que présentée avant les deux‑points (`:`) et n'était pas réellement insérée dans le buffer : l'utilisateur devait la retaper pour la modifier. Cette nouvelle option fait gagner du temps et rend l'expérience plus fluide. Elle corrige aussi un bug : la valeur par défaut était considérée comme valide quel que soit l'entrée, même si celle‑ci était mal formatée.
- **Travail réalisé** : La plupart du temps (une semaine) a été consacré à comprendre le fonctionnement interne du "input buffer" géré dans `AnsiConsoleExtensions.Input.cs`. Modifier ce composant a pris du temps, car il n'était pas documenté et les abstractions n'étaient pas évidentes.
- **Compatibilité** : La fonctionnalité ne casse pas la rétro‑compatibilité. L'implémentation évite les refactorisations profondes du code existant, respectant ainsi une exigence forte du projet.

## Issue #1281 – `Make prompts IRenderable` (pas de PR pour l'instant)
- **Description** : Refactoriser les classes de prompts pour qu'elles implémentent l'interface `IRenderable`. Cette transformation permettrait de manipuler les prompts comme des éléments graphiques : les insérer dans des mises en page (`Layouts`), les imbriquer dans d'autres `IRenderable`, etc. En pratique, cela les rendrait comparables à des widgets réutilisables.
- **Complexité** : Cette évolution nécessite une compréhension approfondie du pipeline de rendu de Spectre.Console. Il faut convertir des classes conçues pour un usage séquentiel et interactif en objets passif capables de se rendre "en temps réel" au sein du moteur. Tout en faisant cela, il est impératif de ne pas rompre la compatibilité avec les prompts existants, ce qui est délicat étant donné l'architecture actuelle et les dépendances internes.
- **Travail réalisé** : Pour l'instant, seul une des classe prompts (TextPrompt.cs) à reçu les changements necessaire pour se rendre renderable, mais une fois celle-ci terminer il sera simple de rendre tout les autres en `IRenderable`. Un des problemes actuel est qu'il brise la rétro-compatibilité lors de la gestion des tests. il faut donc une deuxieme refactorisation de ce code pour qu'ìl en soit compatible.


# TODO avant la semaine 13
- Ajouter au moins deux nouvelles issues à traiter.
- Écrire au moins cinq tests unitaires supplémentaires couvrant les nouvelles fonctionnalités.