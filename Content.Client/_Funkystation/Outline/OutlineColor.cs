using Content.Shared._Funkystation.CCVar;
using Robust.Shared.Configuration;

namespace Content.Client.Outline;

public static class OutlineColor
{
    /// <summary>
    /// Gets the relevant interaction outline color based on the client's outline color CVars.
    /// </summary>
    /// <param name="inRange">Whether the thing we're getting the outline for is in-range or out-of-range.</param>
    /// <returns>True if the cvar was set to a valid hex color.</returns>
    public static bool TryGetOutlineColor(bool inRange, out Color outlineColor, IConfigurationManager? configManager = null, ISawmill? sawmill = null)
    {
        configManager ??= IoCManager.Resolve<IConfigurationManager>();
        sawmill ??= IoCManager.Resolve<ILogManager>().GetSawmill("outlinecolor");

        var colorCvar = inRange
        ? InteractionOutlineCVars.ValidInteractionOutlineColor
        : InteractionOutlineCVars.InvalidInteractionOutlineColor;

        var color = configManager.GetCVar(colorCvar);

        if (!Color.TryFromHex(color, out outlineColor))
        {
            sawmill.Warning($"{colorCvar.Name} is set to an invalid color ({color}).");
            if (!Color.TryFromHex(colorCvar.DefaultValue, out outlineColor))
                sawmill.Error($"{colorCvar.Name} has an invalid default value ({colorCvar.DefaultValue}).");
            return false;
        }
        return true;
    }
}
