namespace Content.Server._Funkystation.SignalKey;

[RegisterComponent]
public sealed partial class SignalKeyComponent: Component
{
    /// <summary>
    /// The type of lock that the key activates.
    /// This needs to be the same value as the KeyType on the lock's SignalLock component.
    /// </summary>
    [DataField]
    public string? KeyType;
}
