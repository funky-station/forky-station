using Content.Shared._Funkystation.StationRecords;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.StationRecords;

public sealed class XoRecordsConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private XoRecordsConsoleWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<XoRecordsConsoleWindow>();
        _window.OnKeySelected += key => SendMessage(new XoSelectRecordMessage(key));
        _window.OnSubmitted += (id, fields) => SendMessage(new XoSubmitRecordMessage(id, fields));
        _window.OnVerifyPressed += () => SendMessage(new XoVerifyRecordMessage());
        _window.OnAddPressed += () => SendMessage(new XoCreateRecordMessage());
        _window.OnDeletePressed += key => SendMessage(new XoDeleteRecordMessage(key));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not XoRecordsConsoleState cast)
            return;

        _window?.UpdateState(cast);
    }
}
