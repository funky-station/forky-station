using Content.Client.Gameplay;
using Content.Client.Lobby;
using Content.Shared._Funkystation.CCVar;
using Robust.Client.Console;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Configuration;

namespace Content.Client._Funkystation.ContentWarning;

public sealed partial class ContentWarningUIController : UIController, IOnStateEntered<LobbyState>, IOnStateEntered<GameplayState>
{
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private IClientConsoleHost _consoleHost = default!;

    private ContentWarningPopup? _window;

    private void AttemptOpenContentWarningPopup()
    {
        if (!_cfg.GetCVar(ContentWarningCVars.Display) || _cfg.GetCVar(ContentWarningCVars.Acknowledged))
            return;

        OpenContentWarningPopup();
    }

    public void OnStateEntered(LobbyState _)
    {
        AttemptOpenContentWarningPopup();
    }

    public void OnStateEntered(GameplayState _)
    {
        AttemptOpenContentWarningPopup();
    }

    private void OpenContentWarningPopup()
    {
        if (_window != null)
            return;

        _window = new ContentWarningPopup();
        _window.OpenCentered();
        _window.OnContentWarningReject += () =>
        {
            _window.Close();
            _window = null;

            if (_cfg.GetCVar(ContentWarningCVars.KickOnIgnore))
                _consoleHost.ExecuteCommand("quit");
        };
        _window.OnContentWarningAccept += () =>
        {
            _window.Close();
            _window = null;
            _cfg.SetCVar(ContentWarningCVars.Acknowledged, true);
            _cfg.SaveToFile();
        };
    }
}
