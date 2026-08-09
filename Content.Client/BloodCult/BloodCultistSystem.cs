using Content.Shared.BloodCult;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client.BloodCult;

public sealed partial class BloodCultistSystem : SharedBloodCultistSystem
{

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BloodCultistComponent, GetStatusIconsEvent>(GetBloodCultistIcon);
    }

	private void GetBloodCultistIcon(Entity<BloodCultistComponent> ent, ref GetStatusIconsEvent args)
    {
        if (ProtoMan.TryIndex(ent.Comp.StatusIcon, out var iconPrototype))
            args.StatusIcons.Add(iconPrototype);
    }
}
