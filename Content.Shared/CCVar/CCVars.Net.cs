// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 Quantum-cross
// SPDX-FileCopyrightText: 2024 Simon
// SPDX-License-Identifier: MIT

using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    public static readonly CVarDef<float> NetAtmosDebugOverlayTickRate =
        CVarDef.Create("net.atmosdbgoverlaytickrate", 3.0f);

    public static readonly CVarDef<float> NetGasOverlayTickRate =
        CVarDef.Create("net.gasoverlaytickrate", 3.0f);

    public static readonly CVarDef<int> GasOverlayThresholds =
        CVarDef.Create("net.gasoverlaythresholds", 20);
}
