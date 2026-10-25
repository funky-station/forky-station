using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._Funkystation.CCVar;
using Content.Shared._Funkystation.Radio;
using Content.Shared.Radio;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;

namespace Content.Client._Funkystation.Radio;

/// <summary>
/// Manages getting the currently selected channel color for <see cref="UserInterface.RichText.RadioChannelColorTag"/>
/// </summary>
public sealed partial class RadioChannelColorManager : IPostInjectInit
{
    [Dependency] private IPrototypeManager _prototypeManager = null!;
    [Dependency] private IConfigurationManager _cfg = null!;
    [Dependency] private ILogManager _logManager = null!;

    private static readonly ProtoId<RadioChannelColorsPrototype> DefaultChannelColors = "DefaultChannelColors";
    private static readonly CVarDef<string> ColorPresetCvar = RadioChannelColorCvar.ChannelColorPreset;
    private static readonly CVarDef<bool> EnforceProtoDefaultsCvar = RadioChannelColorCvar.EnforceProtoDefaultColors;

    private ProtoId<RadioChannelColorsPrototype> _currentPreset;
    private bool _enforceProtoDefaults;

    private ISawmill _sawmill = null!;

    /// <summary>
    /// Gets the channel color based on the color preset specified by <see cref="RadioChannelColorCvar.ChannelColorPreset"/>
    /// or by the channel's default color if unspecified in the chosen preset.
    /// </summary>
    /// <param name="channel">ProtoId for the radio channel we want the chosen color for.</param>
    /// <param name="color"></param>
    /// <returns>True when a color was successfully found, either in the chosen color preset or the default specified in the channel's prototype,
    /// or false when the provided channel couldn't be found.</returns>
    public bool TryGetRadioChannelColor(ProtoId<RadioChannelPrototype> channel, [NotNullWhen(true)] out Color? color)
    {
        if (!_enforceProtoDefaults)
        {
            if (!_prototypeManager.TryIndex(_currentPreset, out var channelColors))
            {
                _sawmill.Warning("No such radio channel color preset {currentPreset} exists. Resetting to default ({DefaultChannelColors}).", _currentPreset, DefaultChannelColors);
                _cfg.SetCVar(ColorPresetCvar, DefaultChannelColors);
            }
            else if (channelColors.Colors.TryGetValue(channel, out var channelColor))
            {
                color = channelColor;
                return true;
            }
        }

        if (_prototypeManager.TryIndex(channel, out var channelProto))
        {
            color = channelProto.Color;
            return true;
        }

        _sawmill.Warning("Tried to get the color of an unknown channel {channel}.", channel);
        color = null;
        return false;
    }

    public void PostInject()
    {
        _sawmill = _logManager.GetSawmill("radio_channel_color_manager");
        _cfg.OnValueChanged(ColorPresetCvar, value => _currentPreset = value, true);
        _cfg.OnValueChanged(EnforceProtoDefaultsCvar, value => _enforceProtoDefaults = value, true);
    }
}
