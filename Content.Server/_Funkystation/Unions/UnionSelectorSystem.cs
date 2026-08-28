using System.Linq;
using Content.Server.GameTicking;
using Content.Server.Station.Events;
using Content.Server.Storage.EntitySystems;
using Content.Server.Traits;
using Content.Shared._Funkystation.Traits.Unions;
using Content.Shared.GameTicking;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._Funkystation.Unions;

public sealed partial class UnionSelectorSystem : EntitySystem
{
    [Dependency] private ILogManager _logManager = default!;
    [Dependency] private StorageSystem _storageSystem = default!;
    [Dependency] private InventorySystem _inventorySystem = default!;
    [Dependency] private SharedHandsSystem _handsSystem = default!;
    [Dependency] private IPrototypeManager _prototypeManager = default!;
    [Dependency] private IRobustRandom _random = default!;

    private ISawmill _sawmill = default!;

    private static readonly ProtoId<UnionRoleItemSetPrototype> UnionLeaderItemSetProto = "unionItemSet";

    public override void Initialize()
    {
        base.Initialize();

        // gotta run after so we know for sure we got everything we need
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(PlayerSpawnedEvent, after: [typeof(TraitSystem)]);
        SubscribeLocalEvent<StationPostInitEvent>(StationCreatedEvent);
        SubscribeLocalEvent<GameRunLevelChangedEvent>(OnRunLevelChanged);

        _sawmill = _logManager.GetSawmill("unions");
    }

    private void StationCreatedEvent(ref StationPostInitEvent ev)
    {
        EnsureComp<StationUnionsComponent>(ev.Station.Owner);
    }

    private void OnRunLevelChanged(GameRunLevelChangedEvent ev)
    {
        // when game starts, run merges
        // i got this from the TipSystem, is this the best way or..
        if (ev.New != GameRunLevel.InRound || ev.Old == GameRunLevel.InRound)
            return;

        ApplyMerges();
        AssignLeaders();
    }

    private void PlayerSpawnedEvent(PlayerSpawnCompleteEvent ev)
    {
        if (TryComp<UnionLeaderComponent>(ev.Mob, out _))
        {
            AssignToUnion(ev, true);
            return;
        }

        if (TryComp<UnionMemberComponent>(ev.Mob, out _))
        {
            AssignToUnion(ev, false);
            // TODO: setup other stuff surrounding membership
        }
    }

    private void AssignToUnion(PlayerSpawnCompleteEvent ev, bool eligibleForUnionLeader)
    {
        var departmentId = GetDepartmentFromJob(ev.JobId!);
        if (departmentId == null || !TryGetGroupingForDepartment(departmentId, out var grouping))
        {
            // dept has no grouping proto, so just remove it (command for example)
            RemComp<UnionLeaderComponent>(ev.Mob);
            RemComp<UnionMemberComponent>(ev.Mob);
            return;
        }

        if (!TryGetStationUnionsComponent(out var unionsComp))
            return;

        var info = new UnionMemberInfo
        {
            EligibleForLeader = eligibleForUnionLeader,
            GroupingId = grouping.ID,
        };

        var existing = unionsComp.Unions.FirstOrDefault(u => u.Departments.Contains(departmentId));
        if (existing != null)
        {
            existing.Members[ev.Mob] = info;
            EnsureCorrectMemberComponent(ev.Mob, existing);
            return;
        }

        var newUnion = new StationUnion
        {
            GroupingIds = [grouping.ID],
            Departments = [grouping.Department],
            Members = new() {{ ev.Mob, info }},
            Leader = EntityUid.Invalid,
            OnStrike = false,
        };
        newUnion.Name = GenerateUnionName(newUnion.Departments);
        unionsComp.Unions.Add(newUnion);
        EnsureCorrectMemberComponent(ev.Mob, newUnion);
    }

    // make sure the player has the correct components when they join a union
    // only the union head gets UnionLeaderComponent
    private void EnsureCorrectMemberComponent(EntityUid member, StationUnion union)
    {
        // maybe just let them keep both? idk
        if (union.Leader == member)
        {
            EnsureComp<UnionLeaderComponent>(member);
            RemComp<UnionMemberComponent>(member);
        }
        else
        {
            RemComp<UnionLeaderComponent>(member);
            EnsureComp<UnionMemberComponent>(member);
        }
    }

    // only roundstart assignment
    private void AssignLeaders()
    {
        if (!TryGetStationUnionsComponent(out var unionsComp))
            return;

        var leaderless = new List<StationUnion>();

        foreach (var union in unionsComp.Unions)
        {
            if (union.Leader != EntityUid.Invalid)
                continue;

            var eligible = new List<EntityUid>();
            foreach (var (memberUid, info) in union.Members)
            {
                if (info.EligibleForLeader)
                    eligible.Add(memberUid);
            }

            if (eligible.Count > 0)
            {
                var chosen = _random.Pick(eligible);
                union.Leader = chosen;
                GiveUnionLeaderItems(chosen, union.Members[chosen].GroupingId);
            }
            else
            {
                leaderless.Add(union);
            }
        }

        if (leaderless.Count > 0)
            AssignFallbackLeaders(leaderless);

        foreach (var union in unionsComp.Unions)
        {
            foreach (var memberUid in union.Members.Keys)
            {
                EnsureCorrectMemberComponent(memberUid, union);
            }
        }
    }
    
    // take a scenario like, ok, theres a union thats both sci and engi, but no one became the leader. right?
    // this essentially picks one over the other, to be the head of the new combined union, or force someone LOL
    private void AssignFallbackLeaders(List<StationUnion> leaderless)
    {
        foreach (var union in leaderless)
        {
            if (union.Members.Count == 0)
                continue;

            var chosen = _random.Pick(union.Members.Keys.ToList());
            union.Leader = chosen;
            GiveUnionLeaderItems(chosen, union.Members[chosen].GroupingId);
        }
    }

    private void ApplyMerges()
    {
        if (!TryGetStationUnionsComponent(out var unionsComp))
            return;

        bool merged;
        do
        {
            merged = false;
            foreach (var union in unionsComp.Unions.ToList())
            {
                if (!unionsComp.Unions.Contains(union))
                    continue;

                if (!IsUnionUnderstaffed(union))
                    continue;

                if (!CanUnionMerge(union))
                    continue;

                var target = FindMergeTarget(unionsComp, union);
                if (target == null)
                    continue;

                MergeUnions(unionsComp, source: union, target: target);
                merged = true;
            }
        } while (merged);
    }

    private bool IsUnionUnderstaffed(StationUnion union)
    {
        var threshold = 0;
        foreach (var gid in union.GroupingIds)
        {
            if (_prototypeManager.TryIndex<UnionGroupingPrototype>(gid, out var g) && g.MinMembers > threshold)
                threshold = g.MinMembers;
        }
        return union.Members.Count < threshold;
    }

    private bool CanUnionMerge(StationUnion union)
    {
        foreach (var gid in union.GroupingIds)
        {
            if (_prototypeManager.TryIndex<UnionGroupingPrototype>(gid, out var g) && !g.CanMerge)
                return false;
        }
        return true;
    }

    private StationUnion? FindMergeTarget(StationUnionsComponent unionsComp, StationUnion source)
    {
        foreach (var gid in source.GroupingIds)
        {
            if (!_prototypeManager.TryIndex<UnionGroupingPrototype>(gid, out var g))
                continue;

            // you DO NOT want this as a LINQ exp
            foreach (var partnerId in g.MergesWith)
            {
                if (source.GroupingIds.Contains(partnerId))
                    continue;

                var candidate = unionsComp.Unions
                    .FirstOrDefault(u => u != source && u.GroupingIds.Contains(partnerId));

                if (candidate != null && CanUnionMerge(candidate))
                    return candidate;
            }
        }
        return null;
    }

    private void MergeUnions(StationUnionsComponent unionsComp, StationUnion source, StationUnion target)
    {
        // bigger union leader wins
        var winner = source.Members.Count > target.Members.Count ? source : target;
        // check if leader is still around lol
        var mergedLeader = winner.Leader != EntityUid.Invalid ? winner.Leader : target.Leader;

        foreach (var (member, info) in source.Members)
        {
            target.Members[member] = info;
        }

        foreach (var dept in source.Departments.Where(dept => !target.Departments.Contains(dept)))
        {
            target.Departments.Add(dept);
        }

        foreach (var gid in source.GroupingIds.Where(gid => !target.GroupingIds.Contains(gid)))
        {
            target.GroupingIds.Add(gid);
        }

        target.Leader = mergedLeader;
        target.Name = GenerateUnionName(target.Departments);
        unionsComp.Unions.Remove(source);
    }

    private string GenerateUnionName(List<string> departments)
    {
        if (departments.Count == 1 && string.Equals(departments[0], "Security", StringComparison.OrdinalIgnoreCase))
            return $"SECU-{_random.Next(0, 100):D2}";

        var initials = string.Concat(departments
            .Where(d => !string.IsNullOrWhiteSpace(d))
            .Select(GetDepartmentInitial));

        return $"NTWU-{initials} {_random.Next(0, 100):D2}";
    }

    private char GetDepartmentInitial(string departmentId)
    {
        foreach (var g in _prototypeManager.EnumeratePrototypes<UnionGroupingPrototype>())
        {
            if (g.Department == departmentId && g.DisplayInitial != null)
                return char.ToUpper(g.DisplayInitial.Value);
        }
        return char.ToUpper(departmentId.Trim()[0]);
    }

    private string? GetDepartmentFromJob(string jobId)
    {
        foreach (var department in _prototypeManager.EnumeratePrototypes<DepartmentPrototype>())
        {
            if (department.Roles.Any(role => role == jobId))
                return department.ID;
        }
        return null;
    }

    private bool TryGetGroupingForDepartment(string departmentId, out UnionGroupingPrototype grouping)
    {
        foreach (var g in _prototypeManager.EnumeratePrototypes<UnionGroupingPrototype>())
        {
            if (g.Department == departmentId)
            {
                grouping = g;
                return true;
            }
        }
        grouping = default!;
        return false;
    }

    // should only ever be a single one
    private bool TryGetStationUnionsComponent(out StationUnionsComponent comp)
    {
        comp = default!;
        var found = EntityQuery<StationUnionsComponent>().ToArray();
        switch (found.Length)
        {
            case 0:
                _sawmill.Error("No StationUnionsComponent found.");
                return false;
            case > 1:
                _sawmill.Warning($"Found multiple station unions: {found.Length}. Using first");
                break;
        }

        comp = found[0];
        return true;
    }

    public bool TryGetUnionForGrouping(string groupingId, out StationUnion? union)
    {
        union = null;
        if (string.IsNullOrEmpty(groupingId) || !TryGetStationUnionsComponent(out var unionsComp))
            return false;

        union = unionsComp.Unions.FirstOrDefault(u => u.GroupingIds.Contains(groupingId));
        return union != null;
    }

    public bool TrySetUnionLeader(StationUnion union, EntityUid newLeader)
    {
        if (!union.Members.ContainsKey(newLeader))
            return false;

        var previousLeader = union.Leader;
        union.Leader = newLeader;

        RefreshLeaderStatus(newLeader);
        if (previousLeader != EntityUid.Invalid && previousLeader != newLeader)
            RefreshLeaderStatus(previousLeader);

        return true;
    }

    private void RefreshLeaderStatus(EntityUid member)
    {
        if (!TryGetStationUnionsComponent(out var unionsComp))
            return;

        if (unionsComp.Unions.Any(u => u.Leader == member))
        {
            EnsureComp<UnionLeaderComponent>(member);
            RemComp<UnionMemberComponent>(member);
        }
        else
        {
            RemComp<UnionLeaderComponent>(member);
            EnsureComp<UnionMemberComponent>(member);
        }
    }

    private void GiveUnionLeaderItems(EntityUid playerMobUid, string groupingId)
    {
        UnionRoleItemSetPrototype? itemSet = null;

        // if theres a unique item set, use that instead of default
        if (_prototypeManager.TryIndex<UnionGroupingPrototype>(groupingId, out var grouping)
            && grouping.LeaderItemSet is { } perGroupingProto)
        {
            _prototypeManager.TryIndex(perGroupingProto, out itemSet);
        }

        // some how default isnt there??
        if (itemSet == null && !_prototypeManager.TryIndex(UnionLeaderItemSetProto, out itemSet))
        {
            _sawmill.Error($"Item set '{groupingId}' not found, and no default ({UnionLeaderItemSetProto.ToString()}) defined!");
            return;
        }

        foreach (var itemSetId in itemSet.Items)
        {
            AddItemsToStorage(playerMobUid, itemSetId, groupingId);
        }
    }

    private void AddItemsToStorage(EntityUid playerMobUid, EntProtoId item, string groupingId)
    {
        var spawned = Spawn(item, Transform(playerMobUid).Coordinates);

        if (TryComp<MegaphoneComponent>(spawned, out var megaphone))
            megaphone.GroupingId = groupingId;

        // try to stash in the player's backpack; falls back to dropping at feet
        // if anyone knows a better way lmk..
        if (_inventorySystem.TryGetSlotEntity(playerMobUid, "back", out var backpack)
            && _storageSystem.Insert(backpack.Value, spawned, out _))
        {
            return;
        }

        _handsSystem.TryPickupAnyHand(playerMobUid, spawned);
    }
}
