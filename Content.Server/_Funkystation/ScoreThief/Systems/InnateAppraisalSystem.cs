using Content.Server._Funkystation.ScoreThief.Components;
using Content.Shared.Examine;
using Content.Shared.Verbs;
using JetBrains.Annotations;
using Robust.Shared.Utility;
using Content.Server._Funkystation.Objectives.Systems;
using Content.Server.Cargo.Systems;
using Robust.Shared.Containers;

namespace Content.Server._Funkystation.ScoreThief.Systems;

[UsedImplicitly]
public sealed partial class InnateAppraisalSystem : EntitySystem
{

    [Dependency] private EntityQuery<InnateAppraisalComponent> _appraisalQuery = default!;
    [Dependency] private ExamineSystemShared _examineSystem = default!;
    [Dependency] private IEntityManager _entManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        // Transform component because everything you can examine has it. We can't make args ref without this.
        // Jank solution but it works sooooo whatever
        SubscribeLocalEvent<TransformComponent, GetVerbsEvent<ExamineVerb>>(OnExamineVerb);
    }

    // Parts of this method were taken from https://github.com/funky-station/forky-station/blob/272393a35fb0a88097f7041c49356fb595aac138/Content.Shared/Chemistry/EntitySystems/SharedSolutionContainerSystem.cs
    private void OnExamineVerb(Entity<TransformComponent> _, ref GetVerbsEvent<ExamineVerb> args)
    {
        if (!_appraisalQuery.TryComp(args.User, out var comp))
            return;
        var target = args.Target;
        var user = args.User;
        var verb = new ExamineVerb()
        {
            Act = () =>
            {
                var pricingSystem = _entManager.System<PricingSystem>();
                var scoreThiefConditionSystem = _entManager.System<ScoreThiefConditionSystem>();
                var price = 0;
                string? reason = null;
                if (comp.BlackMarket)
                {
                    price = scoreThiefConditionSystem.GetValue(target, out reason);
                }
                else
                {
                    price = (int)pricingSystem.GetPrice(target);
                }
                var markup = new FormattedMessage();
                markup.AddMarkupOrThrow(Loc.GetString("scorethief-inspect-one") + " [color=green]" + price + "[/color] " + Loc.GetString("scorethief-inspect-two"));
                if (reason != null)
                {
                    markup.AddMarkupOrThrow("\n\n[color=gold]" + reason + "[/color]");
                }
                _examineSystem.SendExamineTooltip(user, target, markup, false, false);
            },
            Text = Loc.GetString("scorethief-appraise"),
            Message = Loc.GetString("scorethief-appraise"),
            Category = VerbCategory.Examine,
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/Objects/Tools/appraisal-tool.rsi"), "icon"),
        };

        args.Verbs.Add(verb);
    }
}
