using Content.Shared.Examine;
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

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<SerialNumberComponent, ExaminedEvent>(OnExamined);
            SubscribeLocalEvent<SerialNumberComponent, MapInitEvent>(OnInit);
        }

        private void OnInit(EntityUid uid, SerialNumberComponent comp, MapInitEvent args)
        {
            var random = _random.Next(comp.MinDigit, comp.MaxDigit);
            comp.SerialNumber = random.ToString();
            Dirty(uid, comp);
        }

        private void OnExamined(EntityUid uid, SerialNumberComponent comp, ExaminedEvent args)
        {
            var formattedmessage = new FormattedMessage();
            var fullMessage = comp.LeadUp + comp.Prefix + comp.SerialNumber;
            formattedmessage.AddText($"{fullMessage}\n");
            args.AddMessage(formattedmessage, -1);
        }
    }
}
