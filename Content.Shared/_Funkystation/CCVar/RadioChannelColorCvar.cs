using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class RadioChannelColorCvar
{
    /// <summary>
    /// Specifies the ProtoId of the radio channel color preset/theme the player is using.
    /// </summary>
    public static readonly CVarDef<string> ChannelColorPreset =
        CVarDef.Create("funkystation.radio.channel_color_preset", "FunkyChannelColors", CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Allows the server to force clients to use the radio channel colors specified in the radio channel's prototype rather than any color preset.
    /// Also hides the option to change the color preset from the player settings (assuming they haven't opened the settings menu already).
    /// </summary>
    public static readonly CVarDef<bool> EnforceProtoDefaultColors =
        CVarDef.Create("funkystation.radio.enforce_default_channel_colors", false, CVar.SERVER | CVar.REPLICATED);
}
