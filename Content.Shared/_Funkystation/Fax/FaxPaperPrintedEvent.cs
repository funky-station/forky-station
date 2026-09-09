namespace Content.Shared._Funkystation.Fax;

/// <summary>
/// Raised on a fax machine entity after a paper has been spawned from the print queue.
/// Set <see cref="Handled"/> to true to signal the paper has been redirected elsewhere.
/// </summary>
[ByRefEvent]
public struct FaxPaperPrintedEvent
{
    /// The newly spawned paper entity
    public EntityUid Paper;

    /// <summary>
    /// Set to true to signal the paper has been moved somewhere and should not remain at the fax machine's world coordinates.
    /// </summary>
    public bool Handled;

    public FaxPaperPrintedEvent(EntityUid paper)
    {
        Paper = paper;
        Handled = false;
    }
}

