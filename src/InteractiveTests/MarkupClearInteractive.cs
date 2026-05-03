using System;
using System.Threading.Tasks;
using Spectre.Console;

namespace InteractiveTests;

public static class MarkupClearInteractive
{
    public static async Task Run()
    {
        AnsiConsole.MarkupLine("[bold green]Démo interactive des nouvelles fonctionnalités markup[/]");
        AnsiConsole.MarkupLine("Cette démo montre les directives de nettoyage avec markup comme [yellow][wnext][clear:to eol][/], [yellow][wnext][clear:screen][/], [yellow][wnext][clear:to eos][/], [yellow][wnext][clear:#7][/], ainsi que le déplacement du curseur via [yellow][wnext][move:...][/]. Elle démontre aussi [yellow][wnext][wnext][/] pour écrire du markup comme texte brut.");
        AnsiConsole.MarkupLine("");

        AnsiConsole.Markup("Démarrage et chargement de la demo... ");
        await Task.Delay(700);
        AnsiConsole.MarkupLine("[green]Terminé[/]");

        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("[dim]Scénario 1 : Mettre à jour la ligne actuelle sur place.[/]");
        AnsiConsole.Markup("Progression : [yellow]10%[/]");
        await Task.Delay(1000);
        AnsiConsole.Markup("[clear:line]");
        AnsiConsole.MarkupLine("Progression : [green]100%[/]");

        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("Appuie sur [cyan]Entrée[/] pour effacer tout l'écran et continuer.");
        Console.ReadLine();
        AnsiConsole.Markup("[clear:screen]");
        AnsiConsole.MarkupLine("[bold blue]Écran effacé[/]");

        AnsiConsole.MarkupLine("Voici une nouvelle mise en page après l'effacement de l'écran.");
        AnsiConsole.MarkupLine("");

        AnsiConsole.MarkupLine("[dim]Scénario 2 : Déplacer le curseur à une ligne spécifique.[/]");
        AnsiConsole.MarkupLine("[bold]Appuie sur [cyan]Entrée[/] pour écraser la ligne 15.[/]");
        Console.ReadLine();

        AnsiConsole.Markup("[clear:#15]");
        AnsiConsole.MarkupLine("[green]Le curseur a été déplacé à la ligne 15 et cette ligne y a été écrite.[/]");
        AnsiConsole.MarkupLine("");

        AnsiConsole.MarkupLine("[dim]Scénario 2b : Déplacer le curseur avec des directives de déplacement explicites.[/]");
        AnsiConsole.MarkupLine("Appuie sur [cyan]Entrée[/] pour déplacer le curseur de 2 lignes vers le bas et 10 colonnes vers la droite.");
        Console.ReadLine();

        AnsiConsole.Markup("[move:down 2]");
        AnsiConsole.MarkupLine("[green]Descendu de 2 lignes.[/]");
        AnsiConsole.Markup("[move:right 10]");
        AnsiConsole.MarkupLine("[green]Déplacé de 10 colonnes vers la droite.[/]");
        AnsiConsole.MarkupLine("");

        AnsiConsole.MarkupLine("[dim]Scénario 2c : Ignorer une directive de markup avec [wnext][escnext].[/]");
        AnsiConsole.MarkupLine("Appuie sur [cyan]Entrée[/] pour ignorer la prochaine directive (rien ne devrait se produire).");
        Console.ReadLine();
        AnsiConsole.MarkupLine("[escnext][clear:line]cette ligne ne devrait pas s'afficher car elle sera effacée immédiatement");
        AnsiConsole.MarkupLine("[green]La directive de nettoyage a été ignorée, la ligne n'a pas été effacée.[/]");

        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("[dim]Scénario 2d : Écrire une directive de markup comme texte brut avec [wnext][wnext].[/]");
        AnsiConsole.MarkupLine("Appuie sur [cyan]Entrée[/] pour écrire la prochaine directive comme texte normal.");
        Console.ReadLine();
        AnsiConsole.MarkupLine("[wnext][clear:line]cette ligne ne devrait pas s'afficher car elle sera effacée immédiatement");
        AnsiConsole.MarkupLine("[green]Le markup a été affiché comme texte brut ci-dessus et n'a pas été interprété.[/]");

        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("[dim]Scénario 3 : Effacer du curseur jusqu'à la fin de l'écran.[/]");
        AnsiConsole.MarkupLine("Ajoute quelques lignes ci-dessous, puis appuie sur [cyan]Entrée[/] pour les effacer.");
        AnsiConsole.MarkupLine("Ligne un à effacer.");
        AnsiConsole.MarkupLine("Ligne deux à effacer.");
        AnsiConsole.MarkupLine("Ligne trois à effacer.");
        Console.ReadLine();

        await Task.Delay(800);
        AnsiConsole.Markup("[move:up 4][clear:to eos]");
        AnsiConsole.MarkupLine("[green]Tout ce qui se trouvait sous ce point a été effacé.[/]");
        AnsiConsole.MarkupLine("");

        AnsiConsole.MarkupLine("[bold green]Démo du markup terminée ![/]");
        AnsiConsole.MarkupLine("[bold green]Appuie sur [aqua]ENTRÉE[/] pour continuer ![/]");
        Console.ReadLine();
    }
}
