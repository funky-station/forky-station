using System.Diagnostics.CodeAnalysis;
using Content.Client._Funkystation.UserInterface.Controls;
using JetBrains.Annotations;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Input;
using Robust.Shared.Utility;

namespace Content.Client._Funkystation.UserInterface.RichText;

/// <summary>
/// Functionally identical to <see cref="CommandLinkTag"/>, exists to use a unique type
/// for the purpose of chat stacking ghost follow buttons
/// </summary>
[UsedImplicitly]
public sealed partial class GhostFollowTag : IMarkupTagHandler
{
    [Dependency] private IClientConsoleHost _clientConsoleHost = default!;

    public string Name => "ghostfollow";

    /// <inheritdoc/>
    public bool TryCreateControl(MarkupNode node, [NotNullWhen(true)] out Control? control)
    {
        if (!node.Value.TryGetString(out var text)
            || !node.Attributes.TryGetValue("command", out var commandParameter)
            || !commandParameter.TryGetString(out var command))
        {
            control = null;
            return false;
        }

        var label = new GhostFollowLabel();
        label.Text = text;

        label.MouseFilter = Control.MouseFilterMode.Stop;
        label.FontColorOverride = Color.LightBlue;
        label.DefaultCursorShape = Control.CursorShape.Hand;

        label.OnMouseEntered += _ => label.FontColorOverride = Color.Blue;
        label.OnMouseExited += _ => label.FontColorOverride = Color.LightBlue;
        label.OnKeyBindDown += args => OnKeybindDown(args, command);

        if (node.Attributes.TryGetValue("title", out var titleArg))
            label.ToolTip = titleArg.StringValue;

        control = label;
        return true;
    }

    private void OnKeybindDown(GUIBoundKeyEventArgs args, string command)
    {
        if (args.Function != EngineKeyFunctions.UIClick)
            return;

        _clientConsoleHost.ExecuteCommand(command);
    }
}
