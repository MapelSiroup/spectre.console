using Spectre.Console;

namespace InteractiveTests;

public static class HistoryInteractive
{
    public static async Task Run()
    {
        AnsiConsole.MarkupLine("[bold green]Test de l'historique interactif des prompts[/]");
        AnsiConsole.MarkupLine("Ceci démontre la navigation dans l'historique des prompts avec les flèches haut/bas.");
        AnsiConsole.MarkupLine("Utilisez sur [yellow]flèche haut[/] pour parcourir les entrées précédentes, [yellow]flèche bas[/] pour revenir en arrière.");
        AnsiConsole.MarkupLine("Appuyez sur Enter pour accepter, ou tapez du nouveau texte.");
        AnsiConsole.MarkupLine("L'historique est partagé entre tous les prompts utilisant la même instance de [cyan]PromptHistory[/] mais pour cette démo elle sera réinitialisée.[/]");

        var history = PromptHistory.Default;
        history.Clear(); // Start fresh pour la demo

        // First prompt
        var name = AnsiConsole.Prompt(
            new TextPrompt<string>("Quel est ton nom?")
                .History(history));

        // Second prompt
        var favoriteColor = AnsiConsole.Prompt(
            new TextPrompt<string>("Quelle est ta couleur préférée?")
                .History(history));

        // third prompt
        var motdepasse = AnsiConsole.Prompt(
            new TextPrompt<string>("Quel est ton mot de passe?")
                .Secret()
                .History(history));

        // Fourth prompt
        var city = new TextPrompt<string>("Dans quelle ville vis-tu?")
                .DefaultValue("Drummondville")
                .ShowDefaultValue(true)
                .EditableDefaultValue(true)
                .History(history);

        var cityResult = await city.ShowAsRenderableAsync(
            AnsiConsole.Console,
            new Panel(city)
                .Header("Dans quelle ville vis-tu?", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Red),
            CancellationToken.None);

        // Confirmation prompt
        var confirm = AnsiConsole.Prompt(
            new ConfirmationPrompt("Voulez-vous continuer?")
                .History(history));

        AnsiConsole.MarkupLine($"[green]Bonjour, {name}! Ta couleur préférée est {favoriteColor} et tu vis à {cityResult}.[/]");
        AnsiConsole.MarkupLine($"[green]Tu as choisi de {(confirm ? "continuer" : "arrêter")}.[/]");

        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("[dim]Entries de l'historique:[/]");
        foreach (var entry in history.Entries)
        {
            AnsiConsole.MarkupLine($"[dim]- {entry}[/]");
        }

        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("[yellow]Essayez de l'exécuter à nouveau et utilisez les flèches haut/bas pour naviguer dans l'historique![/]");
    }
}