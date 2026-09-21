using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    ///     Intensity multiplier for the pain flash overlay. 0 fully disables the overlay, 1 is full strength
    /// </summary>
    public static readonly CVarDef<float> FunkyPainFlashIntensity =
        CVarDef.Create("funkystation.pain_flash_intensity", 1f, CVar.CLIENTONLY | CVar.ARCHIVE);
}
