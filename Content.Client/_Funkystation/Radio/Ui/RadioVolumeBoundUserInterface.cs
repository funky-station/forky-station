using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.Radio.Ui;

[UsedImplicitly]
public sealed class RadioVolumeBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private RadioVolumeMenu? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<RadioVolumeMenu>();

        if (EntMan.TryGetComponent(Owner, out RadioMicrophoneComponent? mic))
        {
            _menu.Update((Owner, mic));
        }
        if (EntMan.TryGetComponent(Owner, out RadioSpeakerComponent? speaker))
        {
            _menu.Update((Owner, speaker));
        }

        _menu.OnSliderChanged += value =>
        {
            SendMessage(new RadioVolumeSliderMessage(value));
        };
    }

    public void Update(Entity<RadioMicrophoneComponent> ent)
    {
        _menu?.Update(ent);
    }
    public void Update(Entity<RadioSpeakerComponent> ent)
    {
        _menu?.Update(ent);
    }
}
