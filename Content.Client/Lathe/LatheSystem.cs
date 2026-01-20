// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Andreas Kämper
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-FileCopyrightText: 2022 Chris V
// SPDX-FileCopyrightText: 2022 ShadowCommander
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-License-Identifier: MIT

using Robust.Client.GameObjects;
using Content.Shared.Lathe;
using Content.Shared.Power;
using Content.Client.Power;
using Content.Shared.Research.Prototypes;

namespace Content.Client.Lathe;

public sealed class LatheSystem : SharedLatheSystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<LatheComponent, AppearanceChangeEvent>(OnAppearanceChange);
    }

    private void OnAppearanceChange(EntityUid uid, LatheComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        // Lathe specific stuff
        if (_appearance.TryGetData<bool>(uid, LatheVisuals.IsRunning, out var isRunning, args.Component))
        {
            if (_sprite.LayerMapTryGet((uid, args.Sprite), LatheVisualLayers.IsRunning, out var runningLayer, false) &&
                component.RunningState != null &&
                component.IdleState != null)
            {
                var state = isRunning ? component.RunningState : component.IdleState;
                _sprite.LayerSetRsiState((uid, args.Sprite), runningLayer, state);
            }
        }

        if (_appearance.TryGetData<bool>(uid, PowerDeviceVisuals.Powered, out var powered, args.Component) &&
            _sprite.LayerMapTryGet((uid, args.Sprite), PowerDeviceVisualLayers.Powered, out var powerLayer, false))
        {
            _sprite.LayerSetVisible((uid, args.Sprite), powerLayer, powered);

            if (component.UnlitIdleState != null &&
                component.UnlitRunningState != null)
            {
                var state = isRunning ? component.UnlitRunningState : component.UnlitIdleState;
                _sprite.LayerSetRsiState((uid, args.Sprite), powerLayer, state);
            }
        }
    }

    ///<remarks>
    /// Whether or not a recipe is available is not really visible to the client,
    /// so it just defaults to true.
    ///</remarks>
    protected override bool HasRecipe(EntityUid uid, LatheRecipePrototype recipe, LatheComponent component)
    {
        return true;
    }
}

public enum LatheVisualLayers : byte
{
    IsRunning
}
