using Content.Shared._Funkystation.Molotov.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.DoAfter;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Molotov;

public sealed partial class MolotovSystem : EntitySystem
{
    [Dependency] private SharedDoAfterSystem _doAfter = null!;
    [Dependency] private TagSystem _tag = null!;
    [Dependency] private SharedSolutionContainerSystem _solution = null!;
    [Dependency] private SharedStackSystem _stack = null!;
    [Dependency] private SharedHandsSystem _hands = null!;
    [Dependency] private INetManager _net = null!;

    private static readonly ProtoId<TagPrototype> ClothMade = "ClothMade";

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MolotovMaterialComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<MolotovMaterialComponent, MolotovCraftDoAfterEvent>(OnCraftDoAfter);
    }

    private void OnInteractUsing(Entity<MolotovMaterialComponent> ent, ref InteractUsingEvent args)
    {
        if (args.Handled || HasComp<MolotovComponent>(ent))
            return;

        if (!_tag.HasTag(args.Used, ClothMade))
            return;

        var doAfterArgs = new DoAfterArgs(EntityManager, args.User, ent.Comp.CraftDelay, new MolotovCraftDoAfterEvent(), ent, ent, args.Used)
        {
            BreakOnMove = true,
            NeedHand = true,
        };

        args.Handled = _doAfter.TryStartDoAfter(doAfterArgs);
    }

    private void OnCraftDoAfter(Entity<MolotovMaterialComponent> ent, ref MolotovCraftDoAfterEvent args)
    {
        if (args.Cancelled || args.Args.Used is not { } rag)
            return;

        if (_net.IsServer)
        {
            var coords = Transform(ent).Coordinates;
            var molotov = Spawn("MolotovCocktail", coords);

            // transfer solution from bottle to molotov if any exists
            if (_solution.TryGetSolution(ent.Owner, ent.Comp.Solution, out _, out var sourceSolution)
                && sourceSolution.Volume > 0
                && _solution.TryGetSolution(molotov, ent.Comp.Solution, out var target, out _))
            {
                _solution.TryAddSolution(target.Value, sourceSolution);
            }
            _hands.TryDrop(args.User, ent);
            QueueDel(ent);

            if (TryComp<StackComponent>(rag, out _))
            {
                _stack.TryUse(rag, 1);
            }
            else
            {
                _hands.TryDrop(args.User, rag);
                QueueDel(rag);
            }

            _hands.PickupOrDrop(args.User, molotov);
        }

        args.Handled = true;
    }
}
