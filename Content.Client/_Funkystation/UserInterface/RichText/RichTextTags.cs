using Content.Client._Funkystation.UserInterface.RichText;
using Robust.Client.UserInterface.RichText;

namespace Content.Client.UserInterface.RichText;

/// <summary>
/// <see cref="Robust.Client.UserInterface.RichTextEntry.DefaultTags"/> is inside an internal access class that lives elsewhere :(
/// so we need to make a copy of it to do anything with it
/// if I were to upstream this to wizden, I would probably make that field accessible in a robust toolbox PR and get rid of this file
/// </summary>
public static class RichTextTags
{
    /// Mirrored from <see cref="Robust.Client.UserInterface.RichTextEntry"/>
    public static readonly Type[] DefaultTags =
    [
        typeof(BoldItalicTag),
        typeof(BoldTag),
        typeof(BulletTag),
        typeof(ColorTag),
        typeof(HeadingTag),
        typeof(ItalicTag)
    ];

    public static readonly Type[] DefaultWithRadioChannelColorTag =
    [
        typeof(BoldItalicTag),
        typeof(BoldTag),
        typeof(BulletTag),
        typeof(ColorTag),
        typeof(HeadingTag),
        typeof(ItalicTag),
        typeof(RadioChannelColorTag)
    ];
}
