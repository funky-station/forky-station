using Content.Shared.Examine;
using Content.Shared.Random.Helpers;
using Robust.Shared.Utility;


namespace Content.Shared._Funkystation.SerialNumber
{
    // <summary>
    //This component exists to add a semi-random identification number to an entity such as guns or devices
    // </summary>
    public sealed class SerialNumberSystem : EntitySystem
    {
        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<SerialNumberComponent, ExaminedEvent>(OnExamined);
        }

        private void RandomNumber(SerialNumberComponent comp)
        {
            var serialNumber = comp.SerialNumber;
            serialNumber = SerialNumber.Next(comp.MinDigits, comp.MaxDigits);
        }

        private void OnExamined(EntityUid uid, SerialNumberComponent comp, ExaminedEvent args)
        {
            var formattedmessage = new FormattedMessage();
            var LeadUp = comp.LeadUp;
            var Prefix = comp.Prefix;
            var serialNumber = comp.serialNumber;
            var FullMessage = LeadUp + Prefix + serialNumber;

            if (string.IsNullOrWhiteSpace(FullMessage))
            {
                formattedmessage.AddText($"{Loc.GetString("serial-number-is-blank")}\n");
                return;
            }

            formattedmessage.AddText($"{FullMessage}\n");
            args.AddMessage(formattedmessage, -1);
        }
    }
}
