using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tools.Systems;
using Robust.Shared.Utility;
using Robust.Shared.Random;
using Robust.Shared.Audio.Systems;


namespace Content.Shared._Funkystation.SerialNumber
{
    // <summary>
    // This component exists to add a potentially, semi-random identification number or label to
    // an entity such as guns or devices that will appear in the examine window
    // </summary>
    public sealed class SerialNumberSystem : EntitySystem

    {
        [Dependency] private IRobustRandom _random = default!;
        [Dependency] private SharedToolSystem _tool = null!;
        [Dependency] private SharedPopupSystem _popup = null!;
        [Dependency] private SharedDoAfterSystem _doAfter = default!;
        [Dependency] private SharedAudioSystem _audio = default!;

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

        private void StartDoAfter(SerialNumberComponent comp, EntityUid user, TimeSpan delay)
        {
            var doAfterArgs = new DoAfterArgs(EntityManager,
                user,
                delay,
                new SerialNumberDoAfterEvent(),
                comp.Owner,
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

        private void OnRemoval(EntityUid uid, SerialNumberComponent comp, InteractUsingEvent args)
        {
            if (!comp.CanBeRemoved)
            {
                return;
            }
            if (comp.Removed)
            {
                return;
            }
            if (!_tool.HasQuality(args.Used, comp.ToolRequired))
            {
                return;
            }

            _audio.PlayPvs(comp.RemoveSound, uid);

            StartDoAfter(comp, args.User, comp.RemovalTime);
            args.Handled = true;


        }
        private void OnRemoveDoAfter(Entity<SerialNumberComponent> ent, ref SerialNumberDoAfterEvent args)
        {
            if (args.Cancelled || args.Handled)
                return;

            args.Handled = true;
            ent.Comp.Removed = true;
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
                var message = comp.Prefix + comp.Suffix;
                formattedmessage.AddText($"{message}\n");
                args.AddMessage(formattedmessage, -1);
                return;
            }
            var fullMessage = comp.Prefix + comp.SerialNumber + comp.Suffix;
            formattedmessage.AddText($"{fullMessage}\n");
            args.AddMessage(formattedmessage, -1);
        }
    }
}

