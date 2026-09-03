using System.Linq;
using Content.Shared._Funkystation.Pager.Components;
using Content.Shared.Examine;
using Content.Shared.PDA;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Utility;

namespace Content.Shared._Funkystation.Pager;

public abstract partial class SharedPagerSystem : EntitySystem
{
    [Dependency] protected SharedPopupSystem Popup = null!;
    [Dependency] private SharedRingerSystem _ringer = null!;

    public const int MinNumber = 1000;
    public const int MaxNumber = 9999;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PagerComponent, GetVerbsEvent<AlternativeVerb>>(OnGetAltVerbs);
        SubscribeLocalEvent<PagerComponent, GetVerbsEvent<Verb>>(OnGetVerbs);
        SubscribeLocalEvent<PagerComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(Entity<PagerComponent> ent, ref ExaminedEvent args)
    {
        using (args.PushGroup(nameof(PagerComponent)))
        {
            args.PushMarkup(Loc.GetString("pager-examine-number", ("number", ent.Comp.Number)));
        }
    }

    private void OnGetVerbs(Entity<PagerComponent> ent, ref GetVerbsEvent<Verb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        var pager = ent;
        var user = args.User;

        args.Verbs.Add(new Verb
        {
            Text = Loc.GetString("pager-verb-configure-ringer"),
            Icon = new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/settings.svg.192dpi.png")),
            Act = () => _ringer.TryToggleRingerUi(pager, user),
        });
    }

    private void OnGetAltVerbs(Entity<PagerComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        var pager = ent;
        var user = args.User;
        args.Verbs.Add(new AlternativeVerb
        {
            Text = Loc.GetString("pager-verb-cycle-mode", ("mode", Loc.GetString(ModeLocKey(pager.Comp.Mode)))),
            Icon = new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/refresh.svg.192dpi.png")),
            Act = () => CycleMode(pager, user),
        });
    }

    private void CycleMode(Entity<PagerComponent> ent, EntityUid user)
    {
        ent.Comp.Mode = ent.Comp.Mode switch
        {
            PagerMode.Beep => PagerMode.Buzz,
            PagerMode.Buzz => PagerMode.Mute,
            _ => PagerMode.Beep,
        };

        Dirty(ent, ent.Comp);

        Popup.PopupEntity(Loc.GetString("pager-mode-set", ("mode", Loc.GetString(ModeLocKey(ent.Comp.Mode)))), ent, user);
    }

    private static string ModeLocKey(PagerMode mode)
    {
        return mode switch
        {
            PagerMode.Beep => "pager-mode-beep",
            PagerMode.Buzz => "pager-mode-buzz",
            _ => "pager-mode-mute",
        };
    }

    public static bool IsValidNumber(int number)
    {
        return number is >= MinNumber and <= MaxNumber;
    }

    public static bool IsValidCode(string? code)
    {
        if (string.IsNullOrEmpty(code))
            return true;

        if (code.Length > 8)
            return false;

        return code.All(char.IsLetterOrDigit);
    }

    protected int GetNumber(Entity<PagerComponent> ent)
    {
        return ent.Comp.Number;
    }

    protected void SetNumber(Entity<PagerComponent> ent, int number)
    {
        ent.Comp.Number = number;
        Dirty(ent, ent.Comp);
    }

    protected PagerLogEntry? GetCurrentPage(Entity<PagerComponent> ent)
    {
        return ent.Comp.CurrentPage;
    }

    protected PagerMode GetMode(Entity<PagerComponent> ent)
    {
        return ent.Comp.Mode;
    }

    protected SoundSpecifier GetBeepSound(Entity<PagerComponent> ent)
    {
        return ent.Comp.BeepSound;
    }

    protected SoundSpecifier GetBuzzSound(Entity<PagerComponent> ent)
    {
        return ent.Comp.BuzzSound;
    }

    protected bool TryConsumeCooldown(Entity<PagerComponent> ent, TimeSpan now)
    {
        if (now < ent.Comp.LastSent + ent.Comp.SendCooldown)
            return false;

        ent.Comp.LastSent = now;
        Dirty(ent, ent.Comp);
        return true;
    }

    protected void SetCurrentPage(Entity<PagerComponent> ent, int senderNumber, string? code, TimeSpan receivedAt)
    {
        ent.Comp.CurrentPage = new PagerLogEntry
        {
            SenderNumber = senderNumber,
            Code = code,
            ReceivedAt = receivedAt,
        };

        Dirty(ent, ent.Comp);
    }
}
