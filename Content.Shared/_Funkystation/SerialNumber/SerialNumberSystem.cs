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
            SubscribeLocalEvent<SerialNumberComponent, InteractUsingEvent>(OnRemoval);
            SubscribeLocalEvent<SerialNumberComponent, SerialNumberDoAfterEvent>(OnRemoveDoAfter);
        }

        private void OnInit(EntityUid uid, SerialNumberComponent comp, MapInitEvent args)
        {
            var random = _random.Next(comp.StartNumber, comp.EndNumber);
            comp.SerialNumber = random.ToString();
            Dirty(uid, comp);
        }

        private void StartDoAfter(Entity<SerialNumberComponent> ent, EntityUid user, TimeSpan delay)
        {
            var doAfterArgs = new DoAfterArgs(EntityManager,
                user,
                delay,
                new SerialNumberDoAfterEvent(),
                ent.Owner,
                target: null,
                used: null)
            {
                BreakOnMove = true,
                NeedHand = true,
                BreakOnDamage = true,
                DistanceThreshold = 1.0f,
            };

            _doAfter.TryStartDoAfter(doAfterArgs);
        }

        private void OnRemoval(Entity<SerialNumberComponent> ent, EntityUid uid, SerialNumberComponent comp, InteractUsingEvent args)
        {
            if (!comp.CanBeRemoved)
            {
                return;
            }
            if (comp.Removed)
            {
                _popup.PopupEntity(Loc.GetString("serial-number-already-removed"), uid);
                return;
            }
            if (!_tool.HasQuality(args.Used, comp.ToolRequired))
            {
                return;
            }

            StartDoAfter(ent, args.User, ent.Comp.RemovalTime);
            args.Handled = true;

            comp.Removed = true;
            _popup.PopupEntity(Loc.GetString(comp.RemovedText), uid);
        }
        private void OnRemoveDoAfter(Entity<SerialNumberComponent> ent, ref SerialNumberDoAfterEvent args)
        {
            if (args.Cancelled || args.Handled)
                return;

            args.Handled = true;
        }

        private void OnExamined(EntityUid uid, SerialNumberComponent comp, ExaminedEvent args)
        {
            var formattedmessage = new FormattedMessage();

            if (comp.Removed)
            {
                formattedmessage.AddText($"{Loc.GetString(comp.RemovedText)}\n");
                args.AddMessage(formattedmessage, -1);
                return;
            }
            if (comp.NoNumber)
            {
                var message = comp.LeadUp + comp.Prefix;
                formattedmessage.AddText($"{message}\n");
                args.AddMessage(formattedmessage, -1);
                return;
            }
            var fullMessage = comp.LeadUp + comp.Prefix + comp.SerialNumber;
            formattedmessage.AddText($"{fullMessage}\n");
            args.AddMessage(formattedmessage, -1);
        }
    }
}

