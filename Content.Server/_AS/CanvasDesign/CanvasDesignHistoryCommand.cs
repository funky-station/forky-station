using Content.Server.Administration;
using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server._AS.CanvasDesign;

[AdminCommand(AdminFlags.Logs)]
public sealed partial class CanvasDesignHistoryCommand : IConsoleCommand
{
    [Dependency] private EuiManager _eui = null!;
    [Dependency] private IEntityManager _entities = null!;

    public string Command => "canvas_history";
    public string Description => "Opens retained canvas history, optionally selecting a preview code.";
    public string Help => "canvas_history [id]";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } player)
        {
            shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
            return;
        }

        if (args.Length > 1)
        {
            shell.WriteError(Help);
            return;
        }

        var system = _entities.System<CanvasDesignSystem>();
        int? selected = null;
        if (args.Length == 1)
        {
            if (!int.TryParse(args[0], out var id) || id <= 0)
            {
                shell.WriteError(Help);
                return;
            }

            selected = id;
            if (!system.TryGetPreview(selected.Value, out _))
            {
                shell.WriteError($"Canvas preview {selected} was not found or has expired.");
                return;
            }
        }

        _eui.OpenEui(new CanvasDesignHistoryEui(null, system, selected), player);
    }
}
