using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Documents;

/// <summary>
/// Appearance data keys for the document printer
/// </summary>
[Serializable, NetSerializable]
public enum DocumentPrinterVisuals : byte
{
    VisualState,
    Unshaded,
    JamOverlay,
    JamOverlayUnshaded
}

/// <summary>
/// Values stored under DocumentPrinterVisuals.VisualState
/// </summary>
[Serializable, NetSerializable]
public enum DocumentPrinterVisualState : byte
{
    Normal,
    Printing,
    Jammed
}
