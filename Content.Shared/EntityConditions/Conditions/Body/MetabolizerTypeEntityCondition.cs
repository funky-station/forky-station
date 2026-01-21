// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-License-Identifier: MIT

using Content.Shared.Body.Prototypes;
using Content.Shared.Localizations;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityConditions.Conditions.Body;

/// <inheritdoc cref="EntityCondition"/>
public sealed partial class MetabolizerTypeCondition : EntityConditionBase<MetabolizerTypeCondition>
{
    [DataField(required: true)]
    public ProtoId<MetabolizerTypePrototype>[] Type = default!;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype)
    {
        var typeList = new List<string>();

        foreach (var type in Type)
        {
            if (!prototype.Resolve(type, out var proto))
                continue;

            typeList.Add(proto.LocalizedName);
        }

        var names = ContentLocalizationManager.FormatListToOr(typeList);

        return Loc.GetString("entity-condition-guidebook-organ-type",
            ("name", names),
            ("shouldhave", !Inverted));
    }
}
