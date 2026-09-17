using Content.Shared._Funkystation.CCVar;
using Robust.Shared.Configuration;

namespace Content.Client.Outline;

public static class OutlineColor
{
    /// <summary>
    /// Gets the client's custom interaction outline colors if the client has enabled them via <see cref="CCVars.UseCustomInteractionOutlineColors"/>.
    /// </summary>
    /// <param name="inRange">Whether the thing we're getting the outline for is in-range or out-of-range.</param>
    /// <param name="outlineColor">The retrieved color. If the client is not using custom colors or
    /// the cvar was set to an invalid color, this will default to the cvar's default color setting.</param>
    /// <returns>True if the client is using custom colors and the cvar was set to a valid hex color.</returns>
    public static bool TryGetCustomOutlineColor(bool inRange, out Color outlineColor, IConfigurationManager? configManager = null, ISawmill? sawmill = null)
    {
        configManager ??= IoCManager.Resolve<IConfigurationManager>();
        sawmill ??= IoCManager.Resolve<ILogManager>().GetSawmill("outline_color");

        var colorCvar = inRange
        ? InteractionOutlineCVars.ValidInteractionOutlineColor
        : InteractionOutlineCVars.InvalidInteractionOutlineColor;

        var color = configManager.GetCVar(colorCvar);

        if (!configManager.GetCVar(CCVars.UseCustomInteractionOutlineColors))
        {
            if (!Color.TryFromHex(colorCvar.DefaultValue, out outlineColor))
                sawmill.Error($"{colorCvar.Name} has an invalid default value (\"{colorCvar.DefaultValue}\", expected a valid hex color).");
            return false;
        }

        if (!Color.TryFromHex(color, out outlineColor))
        {
            sawmill.Warning($"{colorCvar.Name} is set to an invalid color (\"{color}\", expected a valid hex color).");
            if (!Color.TryFromHex(colorCvar.DefaultValue, out outlineColor))
                sawmill.Error($"{colorCvar.Name} has an invalid default value (\"{colorCvar.DefaultValue}\", expected a valid hex color).");
            return false;
        }
        return true;
    }
}
