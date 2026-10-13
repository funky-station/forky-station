using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tools.Systems;
using Robust.Shared.Utility;
using Robust.Shared.Random;


namespace Content.Shared._Funkystation.SerialNumber
{
    // <summary>
    //This component exists to add a semi-random identification number to an entity such as guns or devices
    // </summary>
    public sealed class SerialNumberSystem : EntitySystem

    {
        [Dependency] private IRobustRandom _random = default!;
        [Dependency] private SharedToolSystem _tool = null!;
        [Dependency] private SharedPopupSystem _popup = null!;
        [Dependency] private SharedDoAfterSystem _doAfter = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<SerialNumberComponent, ExaminedEvent>(OnExamined);
            SubscribeLocalEvent<SerialNumberComponent, MapInitEvent>(OnInit);
            SubscribeLocalEvent<SerialNumberComponent, InteractUsingEvent>(OnScratchOff);
            SubscribeLocalEvent<SerialNumberComponent, SerialNumberDoAfterEvent>(OnScratchOff);
        }

        private void OnInit(EntityUid uid, SerialNumberComponent comp, MapInitEvent args)
        {
            var random = _random.Next(comp.MinDigit, comp.MaxDigit);
            comp.SerialNumber = random.ToString();
            Dirty(uid, comp);
        }
        private void TryStartDoAfter(Entity<SerialNumberComponent> ent, EntityUid user, TimeSpan delay)
        {
            if (ent.Comp.SerialNumberDoAfter.HasValue)
                return;

            var doAfterEventArgs = new DoAfterArgs(EntityManager, user, delay, new SerialNumberDoAfterEvent(), ent.Owner, ent.Owner)
            {
                BreakOnDamage = true,
                NeedHand = true,
                BreakOnHandChange = true,
                MovementThreshold = 0.01f,
                DistanceThreshold = 1.0f,
            };

            if (_doAfterSystem.TryStartDoAfter(doAfterEventArgs, out var doAfterId))
                ent.Comp.SerialNumberDoAfter = doAfterId;
        }
        private void OnScratchOff(EntityUid uid, SerialNumberComponent comp, InteractUsingEvent args, ref SerialNumberDoAfterEvent)
        {
            if (comp.ScratchedOff)
            {
                _popup.PopupEntity(Loc.GetString("serial-number-already-scratched-off"), uid);
                return;
            }

            if (!_tool.HasQuality(args.Used, comp.ToolRequired))
            {
                return;
            }
            comp.ScratchedOff = true;
            _popup.PopupEntity(Loc.GetString("serial-number-removed"), uid);
        }

        private void OnExamined(EntityUid uid, SerialNumberComponent comp, ExaminedEvent args)
        {
            var formattedmessage = new FormattedMessage();

            if (comp.ScratchedOff)
            {
                formattedmessage.AddText($"{Loc.GetString("serial-number-is-scratched")}\n");
                args.AddMessage(formattedmessage, -1);
                return;
            }
            var fullMessage = comp.LeadUp + comp.Prefix + comp.SerialNumber;
            formattedmessage.AddText($"{fullMessage}\n");
            args.AddMessage(formattedmessage, -1);
        }
    }
}

