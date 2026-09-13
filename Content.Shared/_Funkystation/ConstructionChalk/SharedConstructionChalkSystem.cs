using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.ConstructionChalk;

public abstract partial class SharedConstructionChalkSystem : EntitySystem
{
    [Dependency] private INetManager _net = null!;
    [Dependency] private SharedPopupSystem _popup = null!;
    [Dependency] private IPrototypeManager _proto = null!;
    [Dependency] private SharedAudioSystem _audio = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ConstructionChalkMarkComponent, GetVerbsEvent<AlternativeVerb>>(OnGetMarkVerbs);
        SubscribeLocalEvent<ConstructionChalkComponent, GetVerbsEvent<AlternativeVerb>>(OnGetChalkVerbs);
        SubscribeLocalEvent<ConstructionChalkMarkComponent, ExaminedEvent>(OnExamine);
    }

    private void OnExamine(Entity<ConstructionChalkMarkComponent> ent, ref ExaminedEvent args)
    {
        if (!_proto.TryIndex(ent.Comp.ConstructionPrototype, out var recipe))
            return;

        if (!_proto.TryIndex(recipe.Graph, out var graph))
            return;

        var startNode = graph.Nodes[recipe.StartNode];
        var targetNode = graph.Nodes[recipe.TargetNode];
        var path = graph.Path(startNode.Name, targetNode.Name);

        if (path == null || path.Length == 0)
            return;

        var edge = startNode.GetEdge(path[0].Name);
        if (edge == null || edge.Steps.Count == 0)
            return;

        edge.Steps[0].DoExamine(args);
    }

    private void OnGetMarkVerbs(Entity<ConstructionChalkMarkComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        var mark = ent;

        args.Verbs.Add(new AlternativeVerb
        {
            Text = Loc.GetString("chalk-mark-erase-verb"),
            Act = () => EraseMark(mark),
        });
    }

    private void EraseMark(Entity<ConstructionChalkMarkComponent> mark)
    {
        if (_net.IsClient)
            return;

        if (Deleted(mark.Owner))
            return;

        _audio.PlayPvs(mark.Comp.EraseSound, Transform(mark.Owner).Coordinates);
        QueueDel(mark.Owner);
    }

    private void OnGetChalkVerbs(Entity<ConstructionChalkComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess || !args.Using.HasValue || args.Using.Value != ent.Owner)
            return;

        var chalk = ent;
        var user = args.User;

        args.Verbs.Add(new AlternativeVerb
        {
            Text = Loc.GetString("chalk-verb-switch-mode"),
            Act = () => ToggleMode(chalk, user),
            Priority = 99,
        });
    }

    private void ToggleMode(Entity<ConstructionChalkComponent> chalk, EntityUid user)
    {
        if (_net.IsClient)
            return;

        chalk.Comp.Mode = chalk.Comp.Mode switch
        {
            ChalkMode.Construction => ChalkMode.Piping,
            _ => ChalkMode.Construction,
        };

        Dirty(chalk);

        var modeName = Loc.GetString(chalk.Comp.Mode switch
        {
            ChalkMode.Piping => "chalk-mode-piping",
            _ => "chalk-mode-construction",
        });

        _popup.PopupEntity(Loc.GetString("chalk-mode-switched", ("mode", modeName)), user, user);
    }
}
