using Content.Shared.DeviceLinking;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Impstation.PersonalEconomy.Components;

/// <summary>
/// This stores the destination account, charge & reason for a PoS system.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PosSystemComponent : Component
{
    [AutoNetworkedField]
    public AccountNumber OwnerAccount = 0;

    [AutoNetworkedField]
    public AccountNumber RecipientAccount = 0;

    [AutoNetworkedField]
    public int Amount = 0;

    [AutoNetworkedField]
    public string Reason = "";

    // merchant name, so it just doesnt show an account number
    [AutoNetworkedField]
    public string MerchantName = "";

    // device-link ports pulsed on a sale, so the POS can be wired to stuff (imagine a bomb lol)
    [DataField]
    public ProtoId<SourcePortPrototype> SuccessPort = "POSTransactionSucceeded";

    [DataField]
    public ProtoId<SourcePortPrototype> FailPort = "POSTransactionFailed";

    // played at the POS when a sale goes through
    [DataField]
    public SoundSpecifier? PurchaseSound = new SoundPathSpecifier("/Audio/Effects/kaching.ogg");

    // played at the POS when a sale is declined
    [DataField]
    public SoundSpecifier? DeclineSound = new SoundPathSpecifier("/Audio/Machines/buzz-sigh.ogg");
}
