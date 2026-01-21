// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects;

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class MakeSentient : EntityEffectBase<MakeSentient>
{
    /// <summary>
    /// Description for the ghost role created by this effect.
    /// </summary>
    [DataField]
    public LocId RoleDescription = "ghost-role-information-cognizine-description";

    /// <summary>
    /// Whether we give the target the ability to speak coherently.
    /// </summary>
    [DataField]
    public bool AllowSpeech = true;

    public override string EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("entity-effect-guidebook-make-sentient", ("chance", Probability));
}
