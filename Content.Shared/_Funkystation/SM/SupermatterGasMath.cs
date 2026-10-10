using System.Collections.Generic;
using Content.Shared.Atmos;

namespace Content.Shared._Funkystation.SM;

/// <summary>
/// Portable test helpers for gas mix contributions. Coefficients match the historical EE integration contract
/// (mole-weighted sums, not normalized fractions). Keep behavior stable when tuning gameplay gas YAML separately.
/// </summary>
public static class SupermatterGasMath
{
    private readonly record struct GasFact(float HeatPenalty, float PowerMixRatio);

    private static readonly Dictionary<Gas, GasFact> Table = new()
    {
        { Gas.Oxygen, new(1f, 1f) },
        { Gas.Nitrogen, new(-1.5f, -1f) },
        { Gas.CarbonDioxide, new(0.1f, 1f) },
        { Gas.Plasma, new(15f, 1f) },
        { Gas.Tritium, new(10f, 1f) },
        { Gas.WaterVapor, new(12f, 1f) },
        { Gas.Ammonia, new(1f, 1f) },
        { Gas.NitrousOxide, new(-5f, -1f) },
        { Gas.Frezon, new(-10f, -1f) },
        { Gas.BZ, new(5f, 1f) },
        { Gas.Healium, new(4f, 1f) },
        { Gas.Pluoxium, new(-2.5f, -1f) },
        { Gas.Nitrium, new(10f, 1f) },
        { Gas.Hydrogen, new(10f, 1f) },
        { Gas.HyperNoblium, new(-9f, -1f) },
        { Gas.ProtoNitrate, new(-4f, 1f) },
        { Gas.Zauker, new(4f, 2f) },
        { Gas.Halon, new(0.1f, 0.1f) },
        { Gas.Helium, new(0.1f, 0.1f) },
        { Gas.AntiNoblium, new(14f, 1f) },
    };

    private static float CalculateModifier(GasMixture mix, Func<GasFact, float> pick)
    {
        var acc = 0f;
        foreach (var gas in Enum.GetValues<Gas>())
        {
            var moles = mix.GetMoles(gas);
            if (moles <= 0)
                continue;
            if (!Table.TryGetValue(gas, out var fact))
                continue;
            acc += moles * pick(fact);
        }

        return acc;
    }

    public static float GetHeatPenalties(GasMixture mix) => CalculateModifier(mix, f => f.HeatPenalty);

    public static float GetPowerMixRatios(GasMixture mix) => CalculateModifier(mix, f => f.PowerMixRatio);
}
