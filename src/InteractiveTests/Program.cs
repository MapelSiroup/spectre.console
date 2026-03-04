using Spectre.Console;

namespace InteractiveTests;

public static class Program
{
    public static void Main(string[] args)
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

        WriteDivider("Choices");
        var sport = AskSport();

        // Summary
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[yellow]Results[/]").RuleStyle("grey").LeftJustified());
        AnsiConsole.Write(new Table().AddColumns("[grey]Question[/]", "[grey]Answer[/]")
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .AddRow("[grey]Favorite sport[/]", sport));
    }

    private static void WriteDivider(string text)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule($"[yellow]{text}[/]").RuleStyle("grey").LeftJustified());
    }

    public static bool AskConfirmation()
    {
        if (!AnsiConsole.Confirm("Run prompt example?"))
        {
            AnsiConsole.MarkupLine("Ok... :(");
            return false;
        }

        return true;
    }

    public static string AskSport()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("What's your [green]favorite sport[/]?")
                .InvalidChoiceMessage("[red]That's not a sport![/]")
                .DefaultValue("Sport?")
                .AddChoice("Soccer")
                .AddChoice("Hockey")
                .AddChoice("Basketball")
                .ShowDefaultValue(false)
                .DefaultInput(true));
    }
}