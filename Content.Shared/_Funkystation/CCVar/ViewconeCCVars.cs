using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

/// <summary>
/// CVars used to make the ES viewcone features toggleable.
/// </summary>
[CVarDefs]
public sealed class ViewconeCCVars : CVars
{
    // toggle for the whole viewcone system.
    public static readonly CVarDef<bool> ViewconeEnabled =
        CVarDef.Create("funkystation.viewcone_enabled", true, CVar.SERVER | CVar.REPLICATED);

    // base viewcone angle in degrees, before modifiers
    public static readonly CVarDef<float> ViewconeBaseAngle =
        CVarDef.Create("funkystation.viewcone_base_angle", 210f, CVar.SERVER | CVar.REPLICATED);

    // whether moving opposite your facing direction forces walk
    public static readonly CVarDef<bool> ForceWalkBackwards =
        CVarDef.Create("funkystation.force_walk_backwards", true, CVar.SERVER | CVar.REPLICATED);

    // whether mobs always rotate to face the mouse cursor
    public static readonly CVarDef<bool> AlwaysFaceCursor =
        CVarDef.Create("funkystation.always_face_cursor", false, CVar.SERVER | CVar.REPLICATED);

    // disables the grainy static-y noise on the viewcone occlusion
    public static readonly CVarDef<bool> DisableViewconeGrain =
        CVarDef.Create("funkystation.disable_viewcone_grain", false, CVar.CLIENTONLY | CVar.ARCHIVE);

    // 0-1, how much darker the viewcone occlusion overlay gets
    public static readonly CVarDef<float> ViewconeOcclusionOpacity =
        CVarDef.Create("funkystation.viewcone_occlusion_opacity", 0f, CVar.CLIENTONLY | CVar.ARCHIVE);
}
