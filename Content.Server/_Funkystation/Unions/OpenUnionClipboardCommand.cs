using System.Linq;
using Content.Server.Administration;
using Content.Shared._Funkystation.Traits.Unions;
using Content.Shared._Funkystation.Unions;
using Content.Shared.Administration;
using Robust.Server.GameObjects;
using Robust.Shared.Console;

namespace Content.Server._Funkystation.Unions;

[AdminCommand(AdminFlags.Debug)]
public sealed partial class OpenUnionClipboardCommand : IConsoleCommand
{
    [Dependency] private IEntityManager _entManager = default!;

    public string Command => "openunionclipboard";

    public string Description => "test command for unions.";

    public string Help => "openunionclipboard";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player?.AttachedEntity is not { } target)
        {
            shell.WriteLine("spawn yourself in");
            return;
        }

        var unionSelector = _entManager.System<UnionSelectorSystem>();
        if (!unionSelector.TryGetUnionForMember(target, out var union) || union == null)
        {
            shell.WriteLine("you are not in a union");
            return;
        }

        var clipboard = _entManager.EnsureComponent<UnionClipboardComponent>(target);
        clipboard.GroupingId = union.GroupingIds.FirstOrDefault() ?? string.Empty;

        var ui = _entManager.System<UserInterfaceSystem>();
        ui.SetUi(target, UnionClipboardUiKey.Key, new InterfaceData("UnionClipboardBoundUserInterface"));
        ui.OpenUi(target, UnionClipboardUiKey.Key, shell.Player);
    }
}
