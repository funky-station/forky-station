// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Robust.Server.GameObjects;
using Robust.Shared.Random;

namespace Content.Server.RandomAppearance;

public sealed class RandomAppearanceSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RandomAppearanceComponent, ComponentInit>(OnComponentInit);
    }

    private void OnComponentInit(EntityUid uid, RandomAppearanceComponent component, ComponentInit args)
    {
        if (TryComp(uid, out AppearanceComponent? appearance) && component.EnumKey != null)
        {
            _appearance.SetData(uid, component.EnumKey, _random.Pick(component.SpriteStates), appearance);
        }
    }
}
