using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Rendering;

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

        WriteDivider("Fonctionnalité 1: TextPrompt dans un Layout structuré");
        AnsiConsole.MarkupLine("[dim]Ceci démontre un TextPrompt rendu à l'intérieur d'un Layout avec panneaux personnalisés.[/]");
        AnsiConsole.WriteLine();
        var name = await AskNameInLayout();
        AnsiConsole.MarkupLine($"[green]Nom saisi :[/] {name}");

        WriteDivider("Fonctionnalité 2: SelectionPrompt dans un Layout avec couleurs");
        AnsiConsole.MarkupLine("[dim]Ceci démontre un SelectionPrompt rendu dans un Layout avec panneaux colorés.[/]");
        AnsiConsole.WriteLine();
        var animal = await AskAnimalInLayout();
        AnsiConsole.MarkupLine($"[green]Animal sélectionné :[/] {animal}");

        WriteDivider("Fonctionnalité 3: Historique des choix de Prompt précedents");
        AnsiConsole.MarkupLine("[dim]Ceci démontre la navigation dans l'historique des entrées utilisateurs avec les flèches haut/bas.[/]");
        AnsiConsole.WriteLine();
        HistoryInteractive.Run();

        WriteDivider("Fonctionnalité 4: (Renderable) TextPrompt en rendu asynchrone avec Layout");
        AnsiConsole.MarkupLine("[dim]Ceci démontre le nouveau mode TextPrompt.ShowAsRenderableAsync() dans un Layout.[/]");
        AnsiConsole.WriteLine();
        var day = await AskDayAsRenderableInLayout();
        AnsiConsole.MarkupLine($"[green]Jour sélectionné :[/] {day}");

        WriteDivider("Fonctionnalité 5: (Renderable) SelectionPrompt en rendu asynchrone avec Layout");
        AnsiConsole.MarkupLine("[dim]Ceci démontre un SelectionPrompt rendu à l'intérieur d'un Layout.[/]");
        AnsiConsole.WriteLine();
        var fruit = await AskSelectionAsRenderableInLayout();
        AnsiConsole.MarkupLine($"[green]Fruit sélectionné :[/] {fruit}");

        WriteDivider("Fonctionnalité 6: (Renderable) Multi-Selection Prompt en rendu asynchrone avec Layout");
        AnsiConsole.MarkupLine("[dim]Ceci démontre un MultiSelectionPrompt rendu à l'intérieur d'un Layout.[/]");
        AnsiConsole.WriteLine();
        var colors = await AskMultiSelectionAsRenderableInLayout();
        AnsiConsole.MarkupLine($"[green]Sélection des couleurs:[/] {string.Join(", ", colors)}");
        
        WriteDivider("Fonctionnalité 7: Mode basique (comme l'original - rétrocompatible)");
        AnsiConsole.MarkupLine("[dim]Ceci démontre le comportement original de Show() avec la fonctionnalité [cyan]DefaultInput[/].[/]");
        AnsiConsole.WriteLine();
        var sport = AskSport();
        AnsiConsole.MarkupLine($"[green]Sport sélectionné :[/] {sport}");
        
        WriteDivider("Fonctionnalité 8: (Renderable) Confirmation Prompt avec Layout");
        AnsiConsole.MarkupLine("[dim]Ceci démontre un ConfirmationPrompt rendu à l'intérieur d'un Layout.[/]");        
        AnsiConsole.WriteLine();
        var confirmed = await AskConfirmAsRenderableInLayout();

        

        WriteDivider("Résumé des résultats");
        AnsiConsole.Write(new Table()
            .AddColumns("[grey]Fonctionnalité[/]", "[grey]Résultat[/]")
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .AddRow(new Markup("[cyan]TextPrompt dans Layout (Fonctionnalité 1)[/]\n[green]Nom saisi[/]"), new Markup(name))
            .AddRow(new Markup("[cyan]SelectionPrompt dans Layout (Fonctionnalité 2)[/]\n[green]Animal sélectionné[/]"), new Markup(animal))
            .AddRow(new Markup("[cyan]Historique (Fonctionnalité 3)[/]\n[green]Navigation[/]"), new Markup("Démontré"))
            .AddRow(new Markup("[cyan]TextPrompt rendu Layout (Fonctionnalité 4)[/]\n[green]Jour préféré[/]"), new Markup(day))
            .AddRow(new Markup("[cyan]Sélection rendue Layout (Fonctionnalité 5)[/]\n[green]Fruit sélectionné[/]"), new Markup(fruit))
            .AddRow(new Markup("[cyan]Sélection multiple Layout (Fonctionnalité 6)[/]\n[green]Couleurs sélectionnées[/]"), new Markup(string.Join(", ", colors)))
            .AddRow(new Markup("[cyan]Confirmation Layout (Fonctionnalité 7)[/]\n[green]Confirmé[/]"), new Markup(confirmed.ToString()))
            .AddRow(new Markup("[cyan]Mode bloquant (Fonctionnalité 8)[/]\n[green]Sport préféré[/]"), new Markup(sport)));

        WriteDivider("Fonctionnalité 9: Méthodes de nettoyage/Actions de curseur avec le markup");
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


    public static async Task<string> AskDayAsRenderableInLayout()
    {
        var prompt = new TextPrompt<string>("Quel [green]jour[/] te convient le plus ?")
            .InvalidChoiceMessage("[red]Ce n'est pas un jour ![/]")
            .DefaultValue("Dimanche")
            .ShowChoices(false)
            .AddChoice("Lundi")
            .AddChoice("Mardi")
            .AddChoice("Mercredi")
            .AddChoice("Jeudi")
            .AddChoice("Vendredi")
            .AddChoice("Samedi")
            .AddChoice("Dimanche")
            .ShowDefaultValue(true)
            .PromptStyle("magenta");

        var infoPanel = new Panel("[bold cyan]Choisissez votre jour préféré[/]\n\nUtilisez les flèches pour naviguer et Entrée pour sélectionner.")
            .Header("Informations", Justify.Center)
            .BorderColor(Color.Cyan)
            .RoundedBorder();

        var layout = new Layout()
            .SplitRows(
                new Layout("Info")
                    .Update(infoPanel)
                    .Ratio(1),
                new Layout("Prompt")
                    .Update(new Panel(prompt)
                        .Header("Sélection du jour", Justify.Left)
                        .BorderColor(Color.Magenta)
                        .BorderStyle(new Style().Background(Color.DarkMagenta))
                        .RoundedBorder())
                    .Ratio(2));

        return await prompt.ShowAsRenderableAsync(
            AnsiConsole.Console,
            layout,
            CancellationToken.None);
    }

    public static async Task<string> AskSelectionAsRenderableInLayout()
    {
        var prompt = new SelectionPrompt<string>()
            .Title("Choisis un fruit")
            .AddChoices("Pomme", "Banane", "Cerise")
            .DefaultValue("Banane");

        var infoPanel = new Panel("[bold green]Sélectionnez un fruit[/]\n\nUtilisez les flèches haut/bas pour naviguer.")
            .Header("Guide", Justify.Center)
            .BorderColor(Color.Green)
            .DoubleBorder();

        var layout = new Layout()
            .SplitColumns(
                new Layout("Info")
                    .Update(infoPanel)
                    .Ratio(1),
                new Layout("Prompt")
                    .Update(new Panel("Placeholder"))
                    .Ratio(1));

        Func<IRenderable, IRenderable> wrapper = renderable => {
            layout["Prompt"].Update(new Panel(renderable)
                .Header("Sélection de fruit")
                .RoundedBorder()
                .BorderColor(Color.Yellow));
            return layout;
        };

        return await prompt.ShowAsRenderableAsync(
            AnsiConsole.Console,
            wrapper,
            CancellationToken.None);
    }

    public static async Task<IReadOnlyList<string>> AskMultiSelectionAsRenderableInLayout()
    {
        var prompt = new MultiSelectionPrompt<string>()
            .Title("Choisis les couleurs")
            .AddChoices("Rouge", "Vert", "Bleu", "Jaune", "Cyan", "Magenta");

        var infoPanel = new Panel("[bold red]Sélection multiple[/]\n\nUtilisez Espace pour sélectionner/désélectionner, Entrée pour confirmer.")
            .Header("Instructions", Justify.Center)
            .BorderColor(Color.Red)
            .BorderStyle(new Style().Background(Color.DarkRed))
            .RoundedBorder();

        var layout = new Layout()
            .SplitRows(
                new Layout("Info")
                    .Update(infoPanel)
                    .Ratio(1),
                new Layout("Prompt")
                    .Update(new Panel("Placeholder"))
                    .Ratio(2));

        Func<IRenderable, IRenderable> wrapper = renderable => {
            layout["Prompt"].Update(new Panel(renderable)
                .Header("Sélection multiple de couleurs")
                .RoundedBorder()
                .BorderColor(Color.Blue));
            return layout;
        };

        return await prompt.ShowAsRenderableAsync(
            AnsiConsole.Console,
            wrapper,
            CancellationToken.None);
    }

    public static async Task<bool> AskConfirmAsRenderableInLayout()
    {
        var prompt = new ConfirmationPrompt("Veux-tu continuer ?")
            .ShowChoices(true)
            .ShowDefaultValue(true);

        prompt.DefaultValue = true;

        return await prompt.ShowAsRenderableAsync(
            AnsiConsole.Console,
            new Panel("[bold yellow]Confirmation[/]\n\nConfirmer la remise de résultats ? [cyan](Y/n)[/]")
                .Header("Réponse requise", Justify.Center)
                .BorderColor(Color.Yellow)
                .RoundedBorder(),
            CancellationToken.None);
    }

    public static async Task<string> AskNameInLayout()
    {
        var prompt = new TextPrompt<string>("Quel est votre [green]nom[/] ?")
            .PromptStyle("yellow");

        // Create a layout with header and prompt sections
        var headerPanel = new Panel("[bold blue]Bienvenue dans l'application interactive[/]")
            .Header("Titre de l'application", Justify.Center)
            .BorderColor(Color.Blue)
            .RoundedBorder();

        var layout = new Layout()
            .SplitRows(
                new Layout("Header")
                    .Update(headerPanel)
                    .Ratio(1),
                new Layout("Prompt")
                    .Update(new Panel(prompt)
                        .Header("Saisie du nom", Justify.Left)
                        .BorderColor(Color.Yellow)
                        .BorderStyle(new Style().Background(Color.Gray11))
                        .RoundedBorder())
                    .Ratio(2));

        return await prompt.ShowAsRenderableAsync(
            AnsiConsole.Console,
            layout,
            CancellationToken.None);
    }

    public static async Task<string> AskAnimalInLayout()
    {
        var prompt = new SelectionPrompt<string>()
            .Title("Choisissez un animal")
            .AddChoices("Chat", "Chien", "Oiseau", "Poisson", "Lapin")
            .DefaultValue("Chat");

        // Create a colorful layout
        var leftPanel = new Panel("[bold green]Section gauche[/]\n\nCeci est un exemple de layout avec des panneaux colorés.")
            .Header("Informations", Justify.Center)
            .BorderColor(Color.Green)
            .BorderStyle(new Style().Background(Color.DarkGreen))
            .DoubleBorder();

        var layout = new Layout()
            .SplitColumns(
                new Layout("Left")
                    .Update(leftPanel)
                    .Ratio(1),
                new Layout("Right")
                    .Update(new Panel("Placeholder"))
                    .Ratio(1));

        Func<IRenderable, IRenderable> wrapper = renderable => {
            layout["Right"].Update(new Panel(renderable)
                .Header("Sélection d'animal", Justify.Left)
                .BorderColor(Color.Magenta)
                .BorderStyle(new Style().Background(Color.DarkMagenta))
                .RoundedBorder());
            return layout;
        };

        return await prompt.ShowAsRenderableAsync(
            AnsiConsole.Console,
            wrapper,
            CancellationToken.None);
    }
}