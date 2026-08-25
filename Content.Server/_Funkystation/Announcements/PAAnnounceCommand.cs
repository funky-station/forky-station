using Content.Server._MACRO.Announcements;
using Content.Server.Administration;
using Content.Server.Chat.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.Prototypes;

namespace Content.Server.Announcements;

/// <summary>
/// Command class with wrapper methods for making announcements using the PA system
/// so that I don't have to change the args for the announce command
/// </summary>
[AdminCommand(AdminFlags.Moderator)]
public sealed partial class PAAnnounceCommand : LocalizedEntityCommands
{
    [Dependency] private ChatSystem _chat = null!;
    [Dependency] private AnnouncerManager _announcer = null!;
    [Dependency] private IPrototypeManager _proto = null!;
    [Dependency] private IResourceManager _res = null!;

    public override string Command => "announce:pa"; // larping toolshed
    public override string Description => Loc.GetString("cmd-pa-announce-desc");
    public override string Help => Loc.GetString("cmd-announce-help", ("command", Command));

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        AnnounceCommand.OnExecute(shell, argStr, args, false,
            Loc, _announcer, _chat);
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        return AnnounceCommand.OnGetCompletion(shell, args,
            Loc, _proto, _res);
    }
}

