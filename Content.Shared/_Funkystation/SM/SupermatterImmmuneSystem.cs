using Content.Shared._Funkystation.SM.Components;
using Content.Shared.Armor;

namespace Content.Shared._Funkystation.SM;
/// <summary>
/// Handles armor examine for supermatter immunity.
/// </summary>
public partial class SupermatterImmmuneSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SupermatterImmuneComponent, ArmorExamineEvent>(OnArmorExamine);
    }

    private void OnArmorExamine(Entity<SupermatterImmuneComponent> ent, ref ArmorExamineEvent args)
    {
        args.Msg.PushNewline();
        args.Msg.AddMarkupOrThrow(Loc.GetString("supermatter-immune-examine"));
    }
}
