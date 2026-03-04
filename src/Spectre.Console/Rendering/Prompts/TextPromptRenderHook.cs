namespace Spectre.Console.Rendering.Prompts;

/// <summary>
/// Render hook for TextPrompt interactive rendering.
/// Manages the live, updating display of the input field.
/// </summary>
internal sealed class TextPromptRenderHook<T> : IRenderHook
{
    private readonly IAnsiConsole _console;
    private readonly Func<IRenderable> _builder;
    private readonly LiveRenderable _live;
    private bool _dirty;

    public TextPromptRenderHook(IAnsiConsole console, Func<IRenderable> builder)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _builder = builder ?? throw new ArgumentNullException(nameof(builder));

        _live = new LiveRenderable(console);
        _dirty = true;
    }

    /// <summary>
    /// Marks the renderable as dirty, triggering a rebuild on next pipeline pass.
    /// </summary>
    public void Refresh()
    {
        _dirty = true;
        _console.Write(ControlCode.Empty);  // Trigger the pipeline
    }

    /// <summary>
    /// Clears the live renderable and restores cursor position.
    /// </summary>
    public void Clear()
    {
        _console.Write(_live.RestoreCursor());
    }

    /// <summary>
    /// IRenderHook: intercepts the render pipeline.
    /// </summary>
    public IEnumerable<IRenderable> Process(RenderOptions options, IEnumerable<IRenderable> renderables)
    {
        // Rebuild the input field renderable if state changed
        if (!_live.HasRenderable || _dirty)
        {
            _live.SetRenderable(_builder());
            _dirty = false;
        }

        // Emit the live renderable and any other renderables in the pipeline
        yield return _live.PositionCursor(options);

        foreach (var renderable in renderables)
        {
            yield return renderable;
        }

        yield return _live;
    }
}