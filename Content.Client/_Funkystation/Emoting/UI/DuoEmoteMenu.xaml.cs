using System.Linq;
using Content.Client.UserInterface.Controls;
using Content.Shared._Funkystation.Emoting.Components;
using Content.Shared._Funkystation.Emoting.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Client._Funkystation.Emoting.UI;

public sealed class DuoEmoteMenu
{
    public event Action<ProtoId<DuoEmotePrototype>>? OnEmoteSelected;

    private readonly SimpleRadialMenu _menu;

    public DuoEmoteMenu(EntityUid player, Entity<DuoEmoteComponent> target)
    {
        _menu = new SimpleRadialMenu();
        _menu.Track(target.Owner);

        var options = new List<RadialMenuOptionBase>();
        var prototypeManager = IoCManager.Resolve<IPrototypeManager>();
        var entityManager = IoCManager.Resolve<IEntityManager>();

        var hasTail = entityManager.HasComponent<TailDuoEmoteComponent>(player);

        foreach (var proto in prototypeManager.EnumeratePrototypes<DuoEmotePrototype>()
                     .Where(p => !p.RequireTail || hasTail)
                     .OrderBy(p => p.ID))
        {
            var capturedId = new ProtoId<DuoEmotePrototype>(proto.ID);
            options.Add(new RadialMenuActionOption<ProtoId<DuoEmotePrototype>>(
                s => OnEmoteSelected?.Invoke(s),
                capturedId
            )
            {
                ToolTip = Loc.GetString(proto.Name),
                IconSpecifier = RadialMenuIconSpecifier.With(proto.Icon),
            });
        }

        _menu.SetButtons(options);
    }

    public void Close()
    {
        _menu.Close();
    }

    public void OpenOverMouseScreenPosition()
    {
        _menu.OpenOverMouseScreenPosition();
    }
}
