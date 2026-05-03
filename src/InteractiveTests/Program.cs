using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace InteractiveTests;

public static class Program
{
    public static async Task Main(string[] args)
    {
        
        // Check if the console can accept key strokes
        if (!AnsiConsole.Profile.Capabilities.Interactive)
        {
            AnsiConsole.MarkupLine("[red]L'environnement ne supporte pas les fonctionnalités interactives.[/]");
            return;
        }

        // Confirmation si on veut voir les prompts en exemples.
        if (!AskConfirmation())
        {
            return;
        }

        WriteDivider("Fonctionnalité 1 : Mode basique (comme l'original - rétrocompatible)");  //<-- 1ere Issue
        AnsiConsole.MarkupLine("[dim]Ceci démontre le comportement original de Show() avec la fonctionnalité [cyan]DefaultInput[/].[/]");
        AnsiConsole.WriteLine();
        var sport = AskSport();
        AnsiConsole.MarkupLine($"[green]Sport sélectionné :[/] {sport}");

        WriteDivider("Fonctionnalité 2: (Renderable) TextPrompt en rendu asynchrone avec hook");
        AnsiConsole.MarkupLine("[dim]Ceci démontre le nouveau mode TextPrompt.ShowAsRenderableAsync().[/]");
        AnsiConsole.WriteLine();
        var day = await AskDayAsRenderable();
        AnsiConsole.MarkupLine($"[green]Jour sélectionné :[/] {day}");

        WriteDivider("Fonctionnalité 3: (Renderable) SelectionPrompt en rendu asynchrone");
        AnsiConsole.MarkupLine("[dim]Ceci démontre un SelectionPrompt rendu à l'intérieur d'un panneau.[/]");
        AnsiConsole.WriteLine();
        var fruit = await AskSelectionAsRenderable();
        AnsiConsole.MarkupLine($"[green]Fruit sélectionné :[/] {fruit}");

        WriteDivider("Fonctionnalité 4: (Renderable) Multi-Selection Prompt en rendu asynchrone");
        AnsiConsole.MarkupLine("[dim]Ceci démontre un MultiSelectionPrompt rendu à l'intérieur d'un panneau.[/]");
        AnsiConsole.WriteLine();
        var colors = await AskMultiSelectionAsRenderable();
        AnsiConsole.MarkupLine($"[green]Sélection des couleurs:[/] {string.Join(", ", colors)}");

        WriteDivider("Fonctionnalité 5: (Renderable) Confirmation Prompt");
        AnsiConsole.MarkupLine("[dim]Ceci démontre un ConfirmationPrompt rendu à l'intérieur d'un panneau.[/]");
        AnsiConsole.WriteLine();
        var confirmed = await AskConfirmAsRenderable();
        AnsiConsole.MarkupLine($"[green]Confirmed:[/] {confirmed}");

        
        WriteDivider("Fonctionnalité 6: (PromptHistory) Historique des choix de Prompt précedents");
        AnsiConsole.MarkupLine("[dim]Ceci démontre la navigation dans l'historique des entrées utilisateurs avec les flèches haut/bas.[/]");
        AnsiConsole.WriteLine();
        HistoryInteractive.Run();

        WriteDivider("Résumé des résultats");
        AnsiConsole.Write(new Table()
            .AddColumns("[grey]Fonctionnalité[/]", "[grey]Résultat[/]")
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .AddRow(new Markup("[cyan]Mode bloquant (Fonctionnalité 1)[/]\n[green]Sport préféré[/]"), new Markup("sport"))
            .AddRow(new Markup("[cyan]Mode rendu (Fonctionnalité 2)[/]\n[green]Jour préféré[/]"), new Markup("day"))
            .AddRow(new Markup("[cyan]Sélection rendue (Fonctionnalité 3)[/]\n[green]Fruit sélectionné[/]"), new Markup("fruit"))
            .AddRow(new Markup("[cyan]Sélection multiple (Fonctionnalité 4)[/]\n[green]Couleurs sélectionnées[/]"), new Markup(string.Join(", ", "colors")))
            .AddRow(new Markup("[cyan]Confirmation rendue (Fonctionnalité 5)[/]\n[green]Confirmé[/]"), new Markup("confirmed.ToString()")));


        WriteDivider("Fonctionnalité 7: Méthodes de nettoyage/Actions de curseur avec le markup");
        AnsiConsole.MarkupLine("[dim]Ceci démontre comment nettoyer les lignes ou des zones avec du markup.[/]");
        AnsiConsole.WriteLine();
        await MarkupClearInteractive.Run();


        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[green]Tous les exemples devraient avoir fonctionné correctement ![/]");
    }
    private static void WriteDivider(string text)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule($"[yellow]{text}[/]").RuleStyle("grey").LeftJustified());
    }

    public static bool AskConfirmation()
    {
        if (!AnsiConsole.Confirm("Exécuter les exemples de prompts ?"))
        {
            AnsiConsole.MarkupLine("Ok... peut-être la prochaine fois!");
            return false;
        }

        return true;
    }


    /// <summary>
    /// Demonstrates Feature 1: DefaultInput - Blocking mode with default value injection.
    /// The default value is placed in the input buffer, allowing user to edit it.
    /// </summary>
    public static string AskSport()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Quel est ton [green]sport préféré[/] ?")
                .InvalidChoiceMessage("[red]Ce n'est pas un sport ![/]")
                .DefaultValue("Soccer")
                .AddChoice("Soccer")
                .AddChoice("Hockey")
                .AddChoice("Basketball")
                .ShowDefaultValue(true)
                .EditableDefaultValue(true)  // Issue 1: Injection du default value dans le input buffer
                .PromptStyle("cyan"));
    }

    /// <summary>
    /// Demonstrates Feature 2: Renderable IPrompt - Async renderable mode with live hook updates.
    /// The prompt is rendered with live updates as the user types, via the render hook.
    /// </summary>
    public static async Task<string> AskDayAsRenderable()
    {
        var prompt = new TextPrompt<string>("Quel [green]jour[/] te convient le plus ?")
            .InvalidChoiceMessage("[red]Ce n'est pas un jour ![/]")
            .DefaultValue("Dimanche")
            .AddChoice("Lundi")
            .AddChoice("Mardi")
            .AddChoice("Mercredi")
            .AddChoice("Jeudi")
            .AddChoice("Vendredi")
            .AddChoice("Samedi")
            .AddChoice("Dimanche")
            .ShowDefaultValue(false)
            //.EditableDefaultValue(true)  // supporte aussi le default input en renderable mode! (Wrong branch rn tho)
            .PromptStyle("magenta");

        var result = await prompt.ShowAsRenderableAsync(
            AnsiConsole.Console,
            new Panel(prompt)
                .Header("Example de prompt rendue avec une bordure et Titre", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Red),
            CancellationToken.None);
        //var result = await prompt.ShowAsRenderableAsync(AnsiConsole.Console, CancellationToken.None);

        return result;
    }

    public static async Task<string> AskSelectionAsRenderable()
    {
        var prompt = new SelectionPrompt<string>()
            .Title("Choisis un fruit")
            .AddChoices("Pomme", "Banane", "Cerise")
            .DefaultValue("Banane");

        return await prompt.ShowAsRenderableAsync(
            AnsiConsole.Console,
            renderable => new Panel(renderable)
                .Header("Sélectionne un fruit")
                .RoundedBorder()
                .BorderColor(Color.Green),
            CancellationToken.None);
    }

    public static async Task<IReadOnlyList<string>> AskMultiSelectionAsRenderable()
    {
        var prompt = new MultiSelectionPrompt<string>()
            .Title("Choisis les couleurs")
            .AddChoices("Rouge", "Vert", "Bleu", "Jaune", "Cyan", "Magenta");

        return await prompt.ShowAsRenderableAsync(
            AnsiConsole.Console,
            renderable => new Panel(renderable)
                .Header("Sélection multiple de couleurs")
                .RoundedBorder()
                .BorderColor(Color.Blue),
            CancellationToken.None);
    }

    public static async Task<bool> AskConfirmAsRenderable()
    {
        var prompt = new ConfirmationPrompt("Veux-tu continuer ?")
            .ShowChoices(true)
            .ShowDefaultValue(true);

        prompt.DefaultValue = true;

        return await prompt.ShowAsRenderableAsync(
            AnsiConsole.Console,
            CancellationToken.None);
    }
}