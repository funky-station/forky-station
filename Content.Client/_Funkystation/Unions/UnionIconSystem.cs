using Content.Shared._Funkystation.Traits.Unions;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client._Funkystation.Unions;

public sealed partial class UnionIconSystem : EntitySystem
{
    private readonly ProtoId<FactionIconPrototype> _unionLeadershipFaction = new("UnionLeadershipFaction");
    private readonly ProtoId<FactionIconPrototype> _unionStewardFaction = new("UnionStewardFaction");
    
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<UnionLeaderComponent, GetStatusIconsEvent>(OnGetLeaderIcon);
        SubscribeLocalEvent<UnionStewardComponent, GetStatusIconsEvent>(OnGetStewardIcon);
    }

    private void OnGetLeaderIcon(Entity<UnionLeaderComponent> ent, ref GetStatusIconsEvent args)
    {
        if (ProtoMan.TryIndex(_unionLeadershipFaction, out var icon))
            args.StatusIcons.Add(icon);
    }

    private void OnGetStewardIcon(Entity<UnionStewardComponent> ent, ref GetStatusIconsEvent args)
    {
        if (ProtoMan.TryIndex(_unionStewardFaction, out var icon))
            args.StatusIcons.Add(icon);
    }
}
