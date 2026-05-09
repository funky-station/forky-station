using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry.Reagent;

/// <summary>
/// The route through which a reagent was administered into a body.
/// Used to differentiate metabolic effects based on how a chemical entered the body.
/// </summary>
[Serializable, NetSerializable]
public enum ReagentAdministrationRoute : byte
{
    /// <summary>
    /// No specific route tracked. Uses the reagent's default metabolisms.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Reagent was ingested by eating or drinking.
    /// </summary>
    Ingestion = 1,

    /// <summary>
    /// Reagent was injected via syringe, hypospray, or similar device.
    /// </summary>
    Injection = 2,

    /// <summary>
    /// Reagent was applied topically via direct contact, puddles, or similar.
    /// </summary>
    Topical = 3,

    /// <summary>
    /// Reagent was inhaled via smoke or vapor.
    /// </summary>
    Inhalation = 4,
}
