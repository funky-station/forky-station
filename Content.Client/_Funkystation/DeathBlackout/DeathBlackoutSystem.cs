using Content.Shared._Funkystation.CCVar;
using Content.Shared._Funkystation.DeathBlackout;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Client.Audio;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Timing;

namespace Content.Client._Funkystation.DeathBlackout;

public sealed partial class DeathBlackoutSystem : EntitySystem
{
    [Dependency] private IAudioManager _audio = null!;
    [Dependency] private IConfigurationManager _cfg = null!;
    [Dependency] private IGameTiming _timing = null!;
    [Dependency] private IPlayerManager _player = null!;
    [Dependency] private IUserInterfaceManager _ui = null!;
    [Dependency] private DamageableSystem _damage = null!;
    [Dependency] private MobThresholdSystem _thresholds = null!;

    [Dependency] private EntityQuery<DamageableComponent> _damageableQuery;
    [Dependency] private EntityQuery<DeathBlackoutComponent> _blackoutQuery;
    [Dependency] private EntityQuery<MobStateComponent> _mobStateQuery;
    [Dependency] private EntityQuery<MobThresholdsComponent> _thresholdsQuery;

    // how fast the crit fade catches up with the damage value
    private const float FadeCatchUp = 4f;

    private PanelContainer? _panel;
    private float _level;
    private bool _wasBlackout;
    private float _lastGain = -1f;
    private bool _muffled;

    public override void Shutdown()
    {
        base.Shutdown();

        RestoreAudio();

        _panel?.Orphan();
        _panel = null;
    }

    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);

        var blackout = IsBlackout();
        var target = blackout ? 1f : GetCritFadeTarget();

        if (blackout)
        {
            // hard cut in
            _level = 1f;
        }
        else if (_wasBlackout)
        {
            // hard cut out
            _level = target;
        }
        else
        {
            // ease the crit fade
            _level += (target - _level) * MathF.Min(1f, frameTime * FadeCatchUp);

            if (target <= 0f && _level < 0.001f)
                _level = 0f;
        }

        _wasBlackout = blackout;

        UpdatePanel();
        UpdateAudio();
    }

    // true while the player is inside the post death blackout window
    private bool IsBlackout()
    {
        if (!_cfg.GetCVar(DeathBlackoutCVars.Enabled))
            return false;

        if (_player.LocalEntity is not { } player)
            return false;

        return _blackoutQuery.TryComp(player, out var blackout) && _timing.CurTime < blackout.EndTime;
    }

    // 0 to 1 progress through hard crit towards death
    private float GetCritFadeTarget()
    {
        if (!_cfg.GetCVar(DeathBlackoutCVars.Enabled) || !_cfg.GetCVar(DeathBlackoutCVars.CritFade))
            return 0f;

        if (_player.LocalEntity is not { } player)
            return 0f;

        if (!_mobStateQuery.TryComp(player, out var mobState) ||
            !_thresholdsQuery.TryComp(player, out var thresholds) ||
            !_damageableQuery.HasComp(player))
            return 0f;

        // species without softcrit fade over their normal crit
        if (mobState.CurrentState is not (MobState.HardCritical or MobState.Critical))
            return 0f;

        if (!_thresholds.TryGetThresholdForState(player, mobState.CurrentState, out var start, thresholds) ||
            !_thresholds.TryGetDeadThreshold(player, out var dead, thresholds))
            return 0f;

        var range = (dead.Value - start.Value).Float();

        if (range <= 0f)
            return 0f;

        var progress = (_damage.GetTotalDamage(player) - start.Value).Float() / range; // how bout i do anyway
        return Math.Clamp(progress, 0f, 1f);
    }

    // black control sits in the popup root so it covers all the ui. this is a great idea and can have no negative repercussions :godo:
    private void UpdatePanel()
    {
        if (_level <= 0f)
        {
            if (_panel != null)
                _panel.Visible = false;

            return;
        }

        if (_panel == null)
        {
            _panel = new PanelContainer
            {
                Name = "DeathBlackout",
                MouseFilter = Control.MouseFilterMode.Ignore,
                PanelOverride = new StyleBoxFlat { BackgroundColor = Color.Black },
            };
            LayoutContainer.SetAnchorAndMarginPreset(_panel, LayoutContainer.LayoutPreset.Wide);
        }

        if (_panel.Parent == null)
            _ui.PopupRoot.AddChild(_panel);

        if (!_panel.Visible)
        {
            _panel.Visible = true;

            _panel.SetPositionInParent(_ui.PopupRoot.ChildCount - 1);
        }

        _panel.Modulate = Color.White.WithAlpha(_level);
    }

    // scales master gain down with the level
    private void UpdateAudio()
    {
        if (_level <= 0f)
        {
            RestoreAudio();
            return;
        }

        // squared
        var gain = _cfg.GetCVar(CVars.AudioMasterVolume) * (1f - _level) * (1f - _level);

        _muffled = true;

        if (MathF.Abs(gain - _lastGain) < 0.001f)
            return;

        _lastGain = gain;
        _audio.SetMasterGain(gain);
    }

    // puts master gain back to whatever the volume cvar says
    private void RestoreAudio()
    {
        if (!_muffled)
            return;

        _muffled = false;
        _lastGain = -1f;
        _audio.SetMasterGain(_cfg.GetCVar(CVars.AudioMasterVolume));
    }
}
