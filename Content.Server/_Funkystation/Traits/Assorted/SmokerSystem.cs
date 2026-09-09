using Content.Shared.Chemistry.Components;
using Content.Server.Chat.Systems;
using Content.Shared.Bed.Sleep;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Speech;
using Content.Shared.Speech.Muting;
using Content.Shared._Funkystation.Traits.Assorted;
using Content.Shared.Body.Components;
using Content.Server.Chat.Managers;
using Robust.Shared.Containers;

namespace Content.Server._Funkystation.Traits.Assorted;

public sealed partial class SmokerSystem : EntitySystem
{
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private ChatSystem _chatSystem = default!;
    [Dependency] private MobStateSystem _mobStateSystem = default!;
    [Dependency] private IChatManager _chat = default!;
    private const float UpdateInterval = 2f;
    private float _updateTimer;
    public override void Initialize()
    {
        SubscribeLocalEvent<SmokerComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, SmokerComponent smoker, ComponentStartup args)
    {
        // Initialise default values
        smoker.TimeSinceSmoking = 0f;
        smoker.WithdrawalStage = 0;
        smoker.CurrentNicotineLevel = 0f;
        smoker.NextWithdrawalTime = smoker.WithdrawalInterval;
        EnsureComp<ContainerManagerComponent>(uid);

        // Identify the mob's solutions component
        if (TryComp<ContainerManagerComponent>(uid, out var containerManager))
        {
            foreach (var containers in containerManager!.Containers)
            {
                if (containers.Key != "solutions")
                    continue;

                // Check solutions until bloodstream is found
                foreach (var solutionComponent in containers.Value.ContainedEntities)
                {
                    TryComp<SolutionComponent>(solutionComponent, out var solution);
                    if (solution!.Id == "bloodstream")
                    {
                        smoker.Bloodstream = solutionComponent;
                    }
                }
            }

        }

    }
    /// <summary>
    /// Updates the SmokerSystem.
    /// </summary>
    /// <param name="frameTime">Time in seconds covered by the current game tick.</param>
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        _updateTimer += frameTime;
        while (_updateTimer >= UpdateInterval)
        {
            _updateTimer -= UpdateInterval;

            // Get every entity with a SmokerComponent
            var smokerQuery = EntityQueryEnumerator<SmokerComponent>();
            while (smokerQuery.MoveNext(out var uid, out var smoker))
            {
                // Don't update if mob is incapacitated or sleeping
                if (_mobStateSystem.IsIncapacitated(uid) || TryComp<SleepingComponent>(uid, out _))
                    continue;
                smoker.TimeSinceSmoking += UpdateInterval;

                // Check if the mob is smoking
                if (CheckNicotineLevel(uid, smoker.Bloodstream, smoker))
                    continue;

                // Update withdrawal stage
                SetWithdrawalStage(uid, smoker);
            }
        }
    }
    /// <summary>
    /// Checks the current nicotine levels inside solution@chemicals. If the levels are rising (user is smoking)
    /// resets the WithdrawalStage and TimeSinceSmoking and return True.
    /// </summary>
    /// <param name="uid">User's UId.</param>
    /// <param name="solutionUid">Uid of the Bloodstream solution.</param>
    /// <param name="smoker">User's instanced SmokerComponent.</param>
    /// <returns>True if user is smoking, false otherwise.</returns>
    private bool CheckNicotineLevel(EntityUid uid, EntityUid solutionUid, SmokerComponent smoker)
    {
        // Get the current nicotine level from the smoker component
        var currentNicotine = smoker.CurrentNicotineLevel;

        // Check if the mob has a solutions component
        if (!TryComp<SolutionComponent>(solutionUid, out var solution))
            return false;

        // Iterate through all solutions in the mob
        foreach (var name in solution.Solution.Contents)
        {
            // Only check for Nicotine
            if (name.Reagent.Prototype == "Nicotine")
            {
                // If Nicotine levels decreased -> user is not smoking
                if (name.Quantity <= currentNicotine)
                {
                    smoker.CurrentNicotineLevel = name.Quantity;
                    return false;
                }

                // User is smoking, reset WithDrawalStage and TimeSinceSmoking
                smoker.CurrentNicotineLevel = name.Quantity;
                smoker.TimeSinceSmoking = 0;
                smoker.NextWithdrawalTime = smoker.WithdrawalInterval;
                smoker.WithdrawalStage = 0;

                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Check's if the user TimeSinceSmoking is above the threshold and if applicable updates the WithdrawalStage and
    /// gives an updated time.
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="smoker"></param>
    private void SetWithdrawalStage(EntityUid uid, SmokerComponent smoker)
    {
        // No dividing by zero.
        if (smoker.WithdrawalStage < 0)
            smoker.WithdrawalStage = 0;
        // If it's not the time, continue.
        if (!(smoker.TimeSinceSmoking >= smoker.NextWithdrawalTime))
            return;
        smoker.WithdrawalStage++;
        // Ensures that ignoring the need to smoke will get harder longer they go.
        smoker.NextWithdrawalTime += smoker.WithdrawalInterval / (1 + Math.Clamp(smoker.WithdrawalStage, 0, 7));
        switch (smoker.WithdrawalStage)
        {
            case 0:
                _popup.PopupEntity("All's fine in the world!", uid, uid);
                break;
            case 1:
                _popup.PopupEntity(Loc.GetString("trait-smoker-stage1"), uid, uid);
                break;
            case 2:
                _popup.PopupEntity(Loc.GetString("trait-smoker-stage2"), uid, uid, PopupType.Medium);
                break;
            // Stage 3: Mob sighs
            case 3:
                _popup.PopupEntity(Loc.GetString("trait-smoker-stage3"), uid, uid, PopupType.MediumCaution);
                if (TryComp<SpeechComponent>(uid, out _) && !TryComp<MutedStatusEffectComponent>(uid, out _))
                    _chatSystem.TryEmoteWithChat(uid, "Sigh");
                break;
            case 4:
                _popup.PopupEntity(Loc.GetString("trait-smoker-stage4"), uid, uid, PopupType.MediumCaution);
                break;
            case 5:
                _popup.PopupEntity(Loc.GetString("trait-smoker-stage5"), uid, uid, PopupType.MediumCaution);
                break;
            // Stage 6+: Mob screams every 3 stages
            default:
                _popup.PopupEntity(Loc.GetString("trait-smoker-stage6"), uid, uid, PopupType.MediumCaution);
                if (TryComp<SpeechComponent>(uid, out _) && !TryComp<MutedStatusEffectComponent>(uid, out _) && smoker.WithdrawalStage % 3 == 0)
                    _chatSystem.TryEmoteWithChat(uid, "Scream");
                break;
        }
    }
}
