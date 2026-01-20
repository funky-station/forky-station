// SPDX-FileCopyrightText: 2025 BarryNorfolk
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.ParticleAccelerator;

[Serializable, NetSerializable]
public enum AcceleratorParts : byte
{
    EndCap,
    FuelChamber,
    PowerBox,
    PortEmitter,
    ForeEmitter,
    StarboardEmitter
};


