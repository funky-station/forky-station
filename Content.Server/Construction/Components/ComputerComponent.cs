// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Construction.Components
{
    [RegisterComponent, ComponentProtoName("Computer")]
    public sealed partial class ComputerComponent : Component
    {
        [DataField("board", customTypeSerializer:typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string? BoardPrototype;
    }
}
