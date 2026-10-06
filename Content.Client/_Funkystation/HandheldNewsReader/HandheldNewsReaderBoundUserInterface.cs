using Content.Shared._Funkystation.HandheldNewsReader;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.HandheldNewsReader;

[UsedImplicitly]
public sealed class HandheldNewsReaderBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private HandheldNewsReaderWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<HandheldNewsReaderWindow>();
        _window.OnNextButtonPressed += () => SendMessage(new HandheldNewsReaderUiMessage(HandheldNewsReaderUiAction.Next));
        _window.OnPrevButtonPressed += () => SendMessage(new HandheldNewsReaderUiMessage(HandheldNewsReaderUiAction.Prev));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not HandheldNewsReaderBoundUserInterfaceState newsState || _window == null)
            return;

        if (newsState.Article is { } article)
            _window.UpdateState(article, newsState.TargetNum, newsState.TotalNum);
        else
            _window.UpdateEmptyState();
    }
}
