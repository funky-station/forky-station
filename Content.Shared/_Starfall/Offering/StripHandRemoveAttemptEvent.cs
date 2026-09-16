namespace Content.Shared._Starfall.Offering;

/// <summary>
/// Raised before placing an active-hand item into a hand through the stripping menu.
/// </summary>
[ByRefEvent]
public sealed class StripHandInsertAttemptEvent(EntityUid user, EntityUid recipient, EntityUid item, string handName) : EntityEventArgs
{
    public readonly EntityUid User = user;
    public readonly EntityUid Recipient = recipient;
    public readonly EntityUid Item = item;
    public readonly string HandName = handName;
}

/// <summary>
/// Raised before removing an item from a hand through the stripping menu.
/// </summary>
[ByRefEvent]
public sealed class StripHandRemoveAttemptEvent(EntityUid user, EntityUid holder, EntityUid item, string handName) : EntityEventArgs
{
    public readonly EntityUid User = user;
    public readonly EntityUid Holder = holder;
    public readonly EntityUid Item = item;
    public readonly string HandName = handName;
    public bool Handled;
}
