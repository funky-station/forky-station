using Content.Shared._Funkystation.Item.ItemToggle.Components;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.Random;

namespace Content.Shared._Funkystation.Item.ItemToggle;

/// <summary>
/// System for <see cref="ItemToggleRandomFailComponent"/>.
/// </summary>
public sealed partial class ItemToggleRandomFailSystem : EntitySystem
{
    [Dependency] private IRobustRandom _random = null!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ItemToggleRandomFailComponent, ItemToggleActivateAttemptEvent>(OnToggleOnAttempt);
    }

    private void OnToggleOnAttempt(EntityUid uid, ItemToggleRandomFailComponent component, ref ItemToggleActivateAttemptEvent args)
    {
        if (_random.NextFloat() < component.FailChance)
        {
            args.Cancelled = true;
            args.Popup = !string.IsNullOrWhiteSpace(component.PopupText)
                ? Loc.GetString(component.PopupText)
                : null;
        }
    }
}
