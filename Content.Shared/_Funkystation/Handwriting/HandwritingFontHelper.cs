namespace Content.Shared._Funkystation.Handwriting;

public static class HandwritingFontHelper
{
    // if no handwriting component, defaults to this, which is just the same as casual
    private const string DefaultFontId = "HandwritingCasual";
    private const int DefaultFontSize = 20;

    /// <summary>
    /// wraps text in a [hwfont] tag
    /// </summary>
    public static string WrapIfHandwritten(IEntityManager entMan, EntityUid writer, string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var fontId = DefaultFontId;
        var fontSize = DefaultFontSize;

        if (entMan.TryGetComponent<HandwritingFontComponent>(writer, out var font))
        {
            fontId = font.FontId;
            fontSize = font.FontSize;
        }

        var id = fontId.Replace("\"", "");

        return $"[hwfont=\"{id}\" size=\"{fontSize}\"]{text}[/hwfont]";
    }
}
