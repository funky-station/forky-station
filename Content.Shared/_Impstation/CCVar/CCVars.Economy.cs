using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    ///     If command can cashout the stations scrip for spesos
    /// </summary>
    public static readonly CVarDef<bool> ScripStationCashout =
        CVarDef.Create("economy.scrip_station_cashout", true, CVar.REPLICATED);

    /// <summary>
    ///     Sales tax taken on POS purchases, as a percentage (4.95 = 4.95%). Goes to the station scrip account.
    /// </summary>
    public static readonly CVarDef<float> PosTax =
        CVarDef.Create("economy.pos_tax", 4.95f, CVar.REPLICATED);
}
