using Content.Client._Funkystation.Documents.UI;
using Content.Shared._Funkystation.Documents;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.Documents;

[UsedImplicitly]
public sealed class DocumentPrinterBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private DocumentPrinterMenu? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<DocumentPrinterMenu>();
        _menu.OnPrintRequested += docId => SendMessage(new DocumentPrinterPrintMessage(docId));
        _menu.OnCopyRequested += () => SendMessage(new DocumentPrinterCopyMessage());
        _menu.OnEjectRequested += () => SendMessage(new DocumentPrinterEjectMessage());
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not DocumentPrinterBoundUserInterfaceState msg)
            return;

        _menu?.UpdateState(msg);
    }
}
