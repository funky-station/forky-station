using Content.Client.CartridgeLoader;
using Content.Shared.CartridgeLoader;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.CartridgeLoader;

[UsedImplicitly]
public sealed class SingleCartridgeLoaderBoundUserInterface(EntityUid owner, Enum uiKey)
    : CartridgeLoaderBoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private SingleCartridgeLoaderWindow? _window;

    protected override void Open()
    {
        base.Open();

        EnsureWindow();
    }

    private SingleCartridgeLoaderWindow EnsureWindow()
    {
        return _window ??= this.CreateWindow<SingleCartridgeLoaderWindow>();
    }

    protected override void AttachCartridgeUI(Control cartridgeUIFragment, string? title)
    {
        var window = EnsureWindow();
        window.Content.AddChild(cartridgeUIFragment);

        if (title != null)
            window.Title = title;
    }

    protected override void DetachCartridgeUI(Control cartridgeUIFragment)
    {
        _window?.Content.RemoveChild(cartridgeUIFragment);
    }

    protected override void UpdateAvailablePrograms(List<(EntityUid, CartridgeComponent)> programs)
    {

    }
}
