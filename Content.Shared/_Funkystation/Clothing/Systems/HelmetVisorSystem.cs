using Content.Shared.Actions;
using Content.Shared._Funkystation.Clothing.Components;
using Content.Shared.Clothing;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Item;
using Robust.Shared.Audio.Systems;

namespace Content.Shared._Funkystation.Clothing.Systems;

public sealed partial class HelmetVisorSystem : EntitySystem
{
    [Dependency] private SharedActionsSystem _actions = null!;
    [Dependency] private SharedItemSystem _item = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private SharedAppearanceSystem _appearance = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HelmetVisorComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<HelmetVisorComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<HelmetVisorComponent, GetItemActionsEvent>(OnGetActions);
        SubscribeLocalEvent<HelmetVisorComponent, ToggleHelmetVisorEvent>(OnToggle);
        SubscribeLocalEvent<HelmetVisorComponent, GetEquipmentVisualsEvent>(OnGetVisuals, after: [typeof(ClothingSystem)]);
    }

    private void OnMapInit(Entity<HelmetVisorComponent> ent, ref MapInitEvent args)
    {
        _actions.AddAction(ent.Owner, ref ent.Comp.ActionEntity, ent.Comp.Action);
        UpdateAppearance(ent);

        if (!ent.Comp.IsActive)
            EntityManager.AddComponents(ent.Owner, ent.Comp.Components);
    }

    private void OnShutdown(Entity<HelmetVisorComponent> ent, ref ComponentShutdown args)
    {
        if (ent.Comp.ActionEntity != null)
            _actions.RemoveAction(ent.Owner, ent.Comp.ActionEntity);
    }

    private void OnGetActions(Entity<HelmetVisorComponent> ent, ref GetItemActionsEvent args)
    {
        if (ent.Comp.ActionEntity != null)
            args.AddAction(ent.Comp.ActionEntity.Value);
    }

    private void OnToggle(Entity<HelmetVisorComponent> ent, ref ToggleHelmetVisorEvent args)
    {
        args.Handled = true;

        ent.Comp.IsActive = !ent.Comp.IsActive;
        Dirty(ent);

        _audio.PlayPredicted(ent.Comp.ToggleSound, ent, args.Performer);

        UpdateAppearance(ent);
        _item.VisualsChanged(ent.Owner);

        if (!ent.Comp.IsActive)
            EntityManager.AddComponents(ent.Owner, ent.Comp.Components);
        else
            EntityManager.RemoveComponents(ent.Owner, ent.Comp.Components);
    }

    private void OnGetVisuals(Entity<HelmetVisorComponent> ent, ref GetEquipmentVisualsEvent args)
    {
        var state = ent.Comp.IsActive ? ent.Comp.StateUp : ent.Comp.StateDown;

        if (string.IsNullOrEmpty(state))
            return;

        if (!ent.Comp.IsActive)
        {
            var baseState = "equipped-" + args.Slot;

            foreach (var (key, layerData) in args.Layers)
            {
                if (key.StartsWith(args.Slot) && !string.IsNullOrEmpty(layerData.State))
                {
                    if (layerData.State.StartsWith(baseState))
                    {
                        var suffix = layerData.State.Substring(baseState.Length);

                        if (suffix.Contains("-unshaded"))
                            continue;

                        state += suffix;
                        break;
                    }
                }
            }
        }

        var layer = new PrototypeLayerData()
        {
            State = state,
            Visible = true
        };

        var insertIndex = args.Layers.Count;
        for (var i = 0; i < args.Layers.Count; i++)
        {
            if (args.Layers[i].Item1 == "light")
            {
                insertIndex = i;
                break;
            }
        }

        args.Layers.Insert(insertIndex, (ent.Comp.VisualLayer, layer));
    }

    private void UpdateAppearance(Entity<HelmetVisorComponent> ent)
    {
        if (TryComp<AppearanceComponent>(ent, out var appearance))
        {
            _appearance.SetData(ent, HelmetVisorVisuals.IsDown, !ent.Comp.IsActive, appearance);
        }
    }
}
