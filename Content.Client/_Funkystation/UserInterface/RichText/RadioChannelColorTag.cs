using System.Diagnostics.CodeAnalysis;
using Content.Client._Funkystation.Radio;
using Content.Shared.Radio;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._Funkystation.UserInterface.RichText;

[UsedImplicitly]
public sealed partial class RadioChannelColorTag : IMarkupTagHandler
{
    [Dependency] private RadioChannelColorManager _channelColors = null!;

    public static readonly Color DefaultColor = ColorTag.DefaultColor;
    public string Name => "radiochannel";

    /// <inheritdoc/>
    public void PushDrawContext(MarkupNode node, MarkupDrawingContext context)
    {
        if (!node.Value.TryGetString(out var channelId)
            || !_channelColors.TryGetRadioChannelColor(channelId, out var color))
        {
            context.Color.Push(DefaultColor);
            return;
        }

        context.Color.Push(color.Value);
    }

    /// <inheritdoc/>
    public void PopDrawContext(MarkupNode node, MarkupDrawingContext context)
    {
        context.Color.Pop();
    }
}
