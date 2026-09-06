using System.Linq;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Body.Systems;
using Content.Shared._Funkystation.Medical;
using Content.Shared.Atmos.Components;
using Content.Shared.Body.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Inventory;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Utility;

namespace Content.Server._Funkystation.Medical;

public sealed class BedInternalsSystem : EntitySystem
{
    [Dependency] private readonly InternalsSystem _internals = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly GasTankSystem _gasTank = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<BedInternalsComponent, GetVerbsEvent<InteractionVerb>>(OnGetVerbs);
        SubscribeLocalEvent<BedInternalsComponent, EntInsertedIntoContainerMessage>(OnTankInserted);
        SubscribeLocalEvent<BedInternalsComponent, EntRemovedFromContainerMessage>(OnTankRemoved);
        SubscribeLocalEvent<BedInternalsComponent, StrappedEvent>(OnStrapped);
        SubscribeLocalEvent<BedInternalsComponent, UnstrappedEvent>(OnUnstrapped);
    }

    private void OnGetVerbs(EntityUid uid, BedInternalsComponent comp, GetVerbsEvent<InteractionVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        // Only show the internals toggle if someone is buckled into the bed.
        if (!TryComp<StrapComponent>(uid, out var strap) || strap.BuckledEntities.Count == 0)
            return;

        var verb = new InteractionVerb
        {
            Text = comp.Enabled ? "Disable Internals" : "Enable Internals",
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/dot.svg.192dpi.png")),
            Act = () => ToggleBedInternals(uid, comp)
        };

        args.Verbs.Add(verb);
    }

    private void OnTankInserted(EntityUid uid, BedInternalsComponent comp, EntInsertedIntoContainerMessage args)
    {
        if (args.Container.ID != comp.GasSlot)
            return;

        comp.CachedTank = args.Entity;

        UpdateTankVisual(uid, args.Entity);

        if (comp.Enabled && TryComp<StrapComponent>(uid, out var strap))
        {
            foreach (var patient in strap.BuckledEntities)
            {
                if (HasComp<InternalsComponent>(patient))
                    ApplyInternalsToPatient(uid, comp, patient);
            }
        }
    }

    private void OnTankRemoved(EntityUid uid, BedInternalsComponent comp, EntRemovedFromContainerMessage args)
    {
        if (args.Container.ID != comp.GasSlot)
            return;

        comp.CachedTank = null;

        UpdateTankVisual(uid, null);

        if (comp.Enabled)
        {
            comp.Enabled = false;
            if (TryComp<StrapComponent>(uid, out var strap))
            {
                foreach (var patient in strap.BuckledEntities.ToArray())
                {
                    RemoveInternalsFromPatient(uid, comp, patient);
                }
            }
        }
    }

    private void UpdateTankVisual(EntityUid bed, EntityUid? tank)
    {
        if (tank != null && TryComp<BedTankVisualComponent>(tank.Value, out var visual))
        {
            _appearance.SetData(bed, BedInternalsVisuals.TankVisual, visual.Visual);
        }
        else
        {
            _appearance.SetData(bed, BedInternalsVisuals.TankVisual, BedTankVisual.None);
        }
    }

    private void OnStrapped(EntityUid uid, BedInternalsComponent comp, ref StrappedEvent args)
    {
        if (!comp.Enabled || comp.CachedTank == null)
            return;

        var patient = args.Buckle.Owner;

        if (!HasComp<InternalsComponent>(patient))
            return;

        ApplyInternalsToPatient(uid, comp, patient);
    }

    private void OnUnstrapped(EntityUid uid, BedInternalsComponent comp, ref UnstrappedEvent args)
    {
        var patient = args.Buckle.Owner;

        RemoveInternalsFromPatient(uid, comp, patient);

        // If the last patient has left the bed, reset internals to disabled.
        if (TryComp<StrapComponent>(uid, out var strap) && strap.BuckledEntities.Count == 0)
        {
            comp.Enabled = false;
        }
    }

    private void ToggleBedInternals(EntityUid uid, BedInternalsComponent comp)
    {
        if (!TryComp<StrapComponent>(uid, out var strap))
            return;

        // Don't allow enabling if there are no buckled patients.
        if (!comp.Enabled && strap.BuckledEntities.Count == 0)
            return;

        comp.Enabled = !comp.Enabled;

        if (comp.Enabled)
        {
            // Play a short internals enable sound only when there are buckled patients and a tank inserted.
            if (strap.BuckledEntities.Count > 0 && comp.CachedTank != null)
            {
                _audio.PlayPvs(new SoundPathSpecifier("/Audio/Effects/internals.ogg"), uid);
            }

            foreach (var patient in strap.BuckledEntities)
            {
                if (HasComp<InternalsComponent>(patient))
                    ApplyInternalsToPatient(uid, comp, patient);
            }
        }
        else
        {
            foreach (var patient in strap.BuckledEntities.ToArray())
            {
                RemoveInternalsFromPatient(uid, comp, patient);
            }
        }
    }

    private void ApplyInternalsToPatient(EntityUid bedUid, BedInternalsComponent comp, EntityUid patient)
    {
        if (!TryComp<InternalsComponent>(patient, out var internals))
            return;

        if (!comp.OriginalInternalsState.ContainsKey(patient))
        {
            comp.OriginalInternalsState[patient] = _internals.AreInternalsWorking(internals);
            comp.OriginalGasTank[patient] = internals.GasTankEntity;
        }

        if (_inventory.TryGetSlotEntity(patient, "mask", out var existing))
        {
            if (existing != null && TryComp<BreathToolComponent>(existing, out _))
            {
                _internals.ConnectBreathTool((patient, internals), existing.Value);

                ConnectBedTankToPatient(comp, patient, internals);
                return;
            }

            if (existing != null)
            {
                if (_inventory.TryUnequip(patient, "mask", out var removed, silent: true, force: true))
                {
                    if (removed != null)
                        comp.StoredMasks[patient] = removed.Value;
                }
                else
                {
                    return;
                }
            }
        }

        EntityUid maskEnt;
        if (comp.TempMasks.TryGetValue(patient, out var temp) && temp != EntityUid.Invalid)
        {
            maskEnt = temp;
        }
        else
        {
            maskEnt = EntityManager.Spawn(comp.MaskPrototype);
            Transform(maskEnt).Coordinates = Transform(bedUid).Coordinates;
            comp.TempMasks[patient] = maskEnt;
        }

        var equipped = _inventory.TryEquip(patient, maskEnt, "mask", silent: true, force: true);
        if (!equipped)
            return;

        _internals.ConnectBreathTool((patient, internals), maskEnt);

        ConnectBedTankToPatient(comp, patient, internals);
    }

    private void ConnectBedTankToPatient(BedInternalsComponent comp, EntityUid patient, InternalsComponent internals)
    {
        if (comp.CachedTank == null || !TryComp<GasTankComponent>(comp.CachedTank.Value, out var tankComp))
            return;

        if (internals.GasTankEntity != null && internals.GasTankEntity != comp.CachedTank)
            _internals.DisconnectTank((patient, internals), forced: true);

        // Set the tank's User so the parent-check in GasTankSystem doesn't disconnect it.
        tankComp.User = patient;
        tankComp.CheckUser = false;

        if (_internals.TryConnectTank((patient, internals), comp.CachedTank.Value))
        {
            _gasTank.UpdateUserInterface((comp.CachedTank.Value, tankComp));
            comp.BedProvidedTank[patient] = true;
        }
    }

    /// <summary>
    /// Reconnect a patient to their pre-buckle tank cleanly. Goes through
    /// <see cref="GasTankSystem.ConnectToInternals"/> so the tank's User, action
    /// toggle, alert, and UI all sync — funky did this via ToggleInternals(force:true),
    /// but that method is protected on forky's SharedInternalsSystem, so we drive
    /// it from the gas tank side instead.
    /// </summary>
    private void RestoreOriginalTank(EntityUid patient, EntityUid tankEntity)
    {
        if (!TryComp<GasTankComponent>(tankEntity, out var tankComp))
            return;

        // If the tank still thinks it belongs to someone, tear that down first —
        // ConnectToInternals refuses to run on an already-connected tank.
        if (tankComp.User != null)
            _gasTank.DisconnectFromInternals((tankEntity, tankComp), forced: true);

        _gasTank.ConnectToInternals((tankEntity, tankComp), user: patient);
    }

    private void RemoveInternalsFromPatient(EntityUid bedUid, BedInternalsComponent comp, EntityUid patient)
    {
        if (!TryComp<InternalsComponent>(patient, out var internals))
            return;

        if (!comp.OriginalInternalsState.ContainsKey(patient))
            return;

        var hadInternalsEnabled = comp.OriginalInternalsState.GetValueOrDefault(patient, false);
        var originalTank = comp.OriginalGasTank.GetValueOrDefault(patient, null);
        var bedProvidedTank = comp.BedProvidedTank.GetValueOrDefault(patient, false);

        if (bedProvidedTank)
        {
            // Case B: bed swapped patient onto its own tank. Disconnect it, then
            // restore the patient's original tank (if any).
            if (comp.CachedTank != null && TryComp<GasTankComponent>(comp.CachedTank.Value, out var bedTank))
                _gasTank.DisconnectFromInternals((comp.CachedTank.Value, bedTank), forced: true);
            else if (internals.GasTankEntity != null)
                _internals.DisconnectTank((patient, internals), forced: true);

            if (hadInternalsEnabled && originalTank != null && Exists(originalTank))
                RestoreOriginalTank(patient, originalTank.Value);
        }
        else if (hadInternalsEnabled)
        {
            // Case A: bed didn't touch the tank, but patient had internals on when
            // they buckled. Their own setup may have decayed mid-buckle (tank
            // emptied, tank yanked, etc.). If internals aren't currently working,
            // try to reconnect their original tank.
            if (!_internals.AreInternalsWorking(internals)
                && originalTank != null
                && Exists(originalTank))
            {
                RestoreOriginalTank(patient, originalTank.Value);
            }
        }
        else
        {
            // Case C: no bed tank, no prior internals. Defensive: if internals
            // somehow ended up connected, clear them.
            if (internals.GasTankEntity != null)
                _internals.DisconnectTank((patient, internals), forced: true);
        }

        if (comp.TempMasks.TryGetValue(patient, out var tempMask) && tempMask != EntityUid.Invalid)
        {
            _inventory.TryUnequip(patient, "mask", out _, silent: true, force: true);

            if (EntityManager.EntityExists(tempMask))
            {
                _internals.DisconnectBreathTool((patient, internals), tempMask);
                EntityManager.DeleteEntity(tempMask);
            }

            comp.TempMasks.Remove(patient);
        }

        if (comp.StoredMasks.TryGetValue(patient, out var stored) && stored != EntityUid.Invalid)
        {
            _inventory.TryEquip(patient, stored, "mask", silent: true, force: true);
            comp.StoredMasks.Remove(patient);
        }

        comp.OriginalInternalsState.Remove(patient);
        comp.OriginalGasTank.Remove(patient);
        comp.BedProvidedTank.Remove(patient);
    }
}
