using Spectre.Console;

namespace InteractiveTests;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // Check if we can accept key strokes
        if (!AnsiConsole.Profile.Capabilities.Interactive)
        {
            AnsiConsole.MarkupLine("[red]Environment does not support interaction.[/]");
            return;
        }

        // Confirmation
        if (!AskConfirmation())
        {
            return;
        }

        WriteDivider("Feature 1: Blocking Mode (Original - Backward Compatible)");
        AnsiConsole.MarkupLine("[dim]This demonstrates the original Show() behavior with [cyan]DefaultInput[/] feature.[/]");
        AnsiConsole.WriteLine();
        var sport = AskSport();
        AnsiConsole.MarkupLine($"[green]Selected sport:[/] {sport}");

        WriteDivider("Feature 2: Renderable Mode (Live Interactive)");
        AnsiConsole.MarkupLine("[dim]This demonstrates the new ShowAsRenderableAsync() with hook-based live updates.[/]");
        AnsiConsole.WriteLine();
        var day = await AskDayAsRenderable();
        AnsiConsole.MarkupLine($"[green]Selected day:[/] {day}");

        // Summary
        WriteDivider("Results Summary");
        AnsiConsole.Write(new Table()
            .AddColumns("[grey]Feature[/]", "[grey]Result[/]")
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .AddRow("[cyan]Blocking Mode (Feature 1)[/]\n[green]Favorite sport[/]", sport)
            .AddRow("[cyan]Renderable Mode (Feature 2)[/]\n[green]Favorite day[/]", day));

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[green]✓ Both features should have worked correctly![/]");
    }

    private static void WriteDivider(string text)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule($"[yellow]{text}[/]").RuleStyle("grey").LeftJustified());
    }

    public static bool AskConfirmation()
    {
        if (!AnsiConsole.Confirm("Run prompt examples?"))
        {
            AnsiConsole.MarkupLine("Ok... :(");
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
            new TextPrompt<string>("What's your [green]favorite sport[/]?")
                .InvalidChoiceMessage("[red]That's not a sport![/]")
                .DefaultValue("Soccer")
                .AddChoice("Soccer")
                .AddChoice("Hockey")
                .AddChoice("Basketball")
                .ShowDefaultValue(true)
                .DefaultInput(true)  // ← Feature 1: Inject default into input buffer
                .PromptStyle("cyan"));
    }

    /// <summary>
    /// Demonstrates Feature 2: Renderable IPrompt - Async renderable mode with live hook updates.
    /// The prompt is rendered with live updates as the user types, via the render hook.
    /// </summary>
    public static async Task<string> AskDayAsRenderable()
    {
        var prompt = new TextPrompt<string>("Which [green]day[/] fits best?")
            .InvalidChoiceMessage("[red]That's not a day![/]")
            .DefaultValue("Sunday")
            .AddChoice("Monday")
            .AddChoice("Tuesday")
            .AddChoice("Wednesday")
            .AddChoice("Thursday")
            .AddChoice("Friday")
            .AddChoice("Saturday")
            .AddChoice("Sunday")
            .ShowDefaultValue(true)
            .DefaultInput(true)  // ← Also supports default input in renderable mode!
            .PromptStyle("magenta");

        // ShowAsRenderableAsync handles:
        // 1. Rendering the prompt with live hook updates
        // 2. Capturing user input asynchronously
        // 3. Updating the _currentInput state as user types
        // 4. Validating and returning the final result
        var result = await prompt.ShowAsRenderableAsync(AnsiConsole.Console, CancellationToken.None);

        return result;
    }
}