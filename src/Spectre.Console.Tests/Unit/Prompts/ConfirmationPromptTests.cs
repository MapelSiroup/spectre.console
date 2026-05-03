namespace Spectre.Console.Tests.Unit;

[ExpectationPath("Prompts/Confirmation")]
public sealed class ConfirmationPromptTests
{
    [Fact]
    public void Should_Return_True_When_User_Answers_Yes()
    {
        // Given
        var console = new TestConsole();
        console.Input.PushTextWithEnter("y");

        // When
        var result = console.Prompt(new ConfirmationPrompt("Continue?"));

        // Then
        result.ShouldBe(true);
    }

    [Fact]
    public void Should_Return_False_When_User_Answers_No()
    {
        // Given
        var console = new TestConsole();
        console.Input.PushTextWithEnter("n");

        // When
        var result = console.Prompt(new ConfirmationPrompt("Continue?"));

        // Then
        result.ShouldBe(false);
    }

    [Fact]
    public void Should_Add_Confirmation_Input_To_History()
    {
        // Given
        var history = new PromptHistory();
        var console = new TestConsole();
        console.Input.PushTextWithEnter("y");

        // When
        var result = console.Prompt(new ConfirmationPrompt("Continue?") { History = history });

        // Then
        result.ShouldBe(true);
        history.Entries.ShouldBe(new[] { "y" });
    }

    [Fact]
    public void Should_Share_History_Between_TextPrompt_And_ConfirmationPrompt()
    {
        // Given
        var sharedHistory = new PromptHistory();
        var console = new TestConsole();

        // First, a text prompt
        console.Input.PushTextWithEnter("hello");
        console.Prompt(new TextPrompt<string>("Enter text:") { History = sharedHistory });

        // Then, a confirmation prompt
        console.Input.PushTextWithEnter("n");
        var confirmResult = console.Prompt(new ConfirmationPrompt("Continue?") { History = sharedHistory });

        // Then
        confirmResult.ShouldBe(false);
        sharedHistory.Entries.ShouldBe(new[] { "hello", "n" });
    }

    [Fact]
    public void Should_Not_Add_Invalid_Confirmation_Input_To_History()
    {
        // Given
        var history = new PromptHistory();
        var console = new TestConsole();
        console.Input.PushTextWithEnter("maybe");
        console.Input.PushTextWithEnter("y");

        // When
        var result = console.Prompt(new ConfirmationPrompt("Continue?") { History = history });

        // Then
        result.ShouldBe(true);
        history.Entries.ShouldBe(new[] { "y" }); // Only the valid "y" is stored
    }
}