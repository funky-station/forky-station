// SPDX-FileCopyrightText: 2024 brainfood1183 <113240905+brainfood1183@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson <karakkaraz@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Damage.Systems;
using Content.Shared.Popups;
using Content.Shared.Fax.Components;

namespace Content.Shared.Fax.Systems;
/// <summary>
/// System for handling execution of a mob within fax when copy or send attempt is made.
/// </summary>
public sealed class FaxecuteSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    public void Faxecute(EntityUid uid, FaxMachineComponent component, DamageOnFaxecuteEvent? args = null)
    {
        var sendEntity = component.PaperSlot.Item;
        if (sendEntity == null)
            return;

        if (!TryComp<FaxecuteComponent>(uid, out var faxecute))
            return;

        var damageSpec = faxecute.Damage;
        _damageable.ChangeDamage(sendEntity.Value, damageSpec);
        _popupSystem.PopupEntity(Loc.GetString("fax-machine-popup-error", ("target", uid)), uid, PopupType.LargeCaution);
        return;

    }
}
