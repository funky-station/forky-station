using Content.Shared.Interaction.Events;
using Content.Shared.Mind;
using Content.Shared.Ninja.Components;
using Content.Shared.Ninja.Systems;
using Content.Server.Popups;
using Content.Server.Roles;
using Content.Shared.Roles.Components;
using Content.Server.Objectives;
using Content.Server.Station.Systems;

namespace Content.Server.Ninja.Systems;

public sealed partial class NinjaPagerSystem : SharedNinjaPagerSystem
{
    private const float MinimumStationDistance = 70f;

    [Dependency] private PopupSystem _popup = default!;
    [Dependency] private SharedMindSystem _mind = default!;
    [Dependency] private RoleSystem _role = default!;
    [Dependency] private ObjectivesSystem _objectives = default!;
    [Dependency] private SpaceNinjaSystem _ninja = default!;
    [Dependency] private StationSystem _station = default!;
    [Dependency] private SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NinjaPagerComponent, UseInHandEvent>(OnUseInHand);
    }

    /// <summary>
    /// Performs checks, extracts ninja and handles greentext.
    /// </summary>
    private void OnUseInHand(EntityUid ent, NinjaPagerComponent comp, ref UseInHandEvent args)
    {
        var user = args.User;

        if (!_mind.TryGetMind(user, out var mindId, out var mind))
            return;

        if (!_role.MindHasRole<NinjaRoleComponent>(mindId)) //check that its a ninja
        {
            _popup.PopupEntity(Loc.GetString("ninja-pager-not-ninja"), user, user);
            args.Handled = true;
            return;
        }

        foreach (var objective in mind.Objectives) //check that all objectives are completed
        {
            if (!_objectives.IsCompleted(objective, (mindId, mind)) && MetaData(objective).EntityPrototype?.ID != "NinjaSurviveObjective")
            {
                _popup.PopupEntity(Loc.GetString("ninja-pager-incomplete-objectives"), user, user);
                args.Handled = true;
                return;
            }
        }

        if (_station.GetDistanceFromStation(user) < MinimumStationDistance)
        {
            _popup.PopupEntity(Loc.GetString("ninja-pager-too-close"), user, user);
            args.Handled = true;
            return;
        }

        var coordinates = Transform(user).Coordinates;
        _popup.PopupCoordinates(Loc.GetString("ninja-pager-success"), coordinates, user);
        if (TryComp<SpaceNinjaComponent>(user, out var ninja))
            _ninja.PagerUsed((user, ninja));
        QueueDel(user);
        args.Handled = true;
    }
}
