using Content.Shared._Funkystation.Traits.Unions;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;

namespace Content.Client._Funkystation.Unions;

public sealed partial class UnionIconSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<UnionLeaderComponent, GetStatusIconsEvent>(OnGetLeaderIcon);
        SubscribeLocalEvent<UnionStewardComponent, GetStatusIconsEvent>(OnGetStewardIcon);
    }

    private void OnGetLeaderIcon(Entity<UnionLeaderComponent> ent, ref GetStatusIconsEvent args)
    {
        if (ProtoMan.TryIndex<FactionIconPrototype>("UnionLeadershipFaction", out var icon))
            args.StatusIcons.Add(icon);
    }

    private void OnGetStewardIcon(Entity<UnionStewardComponent> ent, ref GetStatusIconsEvent args)
    {
        if (ProtoMan.TryIndex<FactionIconPrototype>("UnionStewardFaction", out var icon))
            args.StatusIcons.Add(icon);
    }
}
