using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Content.Shared.Eui;

namespace Content.Shared._AS.CanvasDesign;

/// <summary>
/// Stores an editable pixel canvas and configures its editor and sprite rendering.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(raiseAfterAutoHandleState: true)]
public sealed partial class CanvasDesignComponent : Component
{
    public const int MaxWidth = 64;
    public const int MaxHeight = 64;
    public const int MaxPixelCount = MaxWidth * MaxHeight;
    public const int MaxTextureSize = 128;

    /// <summary>
    /// Editable canvas width in pixels. Limited to <see cref="MaxWidth"/>
    /// </summary>
    [DataField, AutoNetworkedField]
    public int Width = 16;

    /// <summary>
    /// Editable canvas height in pixels. Limited to <see cref="MaxHeight"/>
    /// </summary>
    [DataField, AutoNetworkedField]
    public int Height = 16;

    /// <summary>
    /// Horizontal position of the canvas's top-left corner within the generated texture.
    /// The default centers a 16×16 canvas on a 32×32 sprite
    /// </summary>
    [DataField, AutoNetworkedField]
    public int OffsetX = 8;

    /// <summary>
    /// Vertical position of the canvas's top-left corner within the generated texture.
    /// The default centers a 16×16 canvas on a 32×32 sprite
    /// </summary>
    [DataField, AutoNetworkedField]
    public int OffsetY = 8;

    /// <summary>
    /// Color assigned to a new canvas. This is the only color allowed transparency, colors applied with drawing tools are opaque.
    /// Defaults to white
    /// </summary>
    [DataField, AutoNetworkedField]
    public Color BackgroundColor = Color.White;

    /// <summary>
    /// Initial color selected by the editor's drawing tools.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Color DefaultDrawingColor = Color.Black;

    /// <summary>
    /// Title displayed by the canvas editor window
    /// </summary>
    [DataField]
    public string EditorTitle = "Canvas Editor";

    /// <summary>
    /// Shader prototype applied to the canvas sprite. The shader must support a <c>_MainTex</c> texture property
    /// </summary>
    [DataField, AutoNetworkedField]
    public string? Shader;

    /// <summary>
    /// The canvas's pixel data. The array length must equal <c>Width * Height</c>
    /// </summary>
    [DataField, AutoNetworkedField]
    public uint[] Pixels = Array.Empty<uint>();

    /// <summary>
    /// Number of editable pixels implied by the configured dimensions.
    /// </summary>
    public int PixelCount => Width * Height;

    /// <summary>
    /// Cooldown in seconds between save attempts
    /// </summary>
    [DataField]
    public TimeSpan SaveCooldown;

    /// <summary>
    /// Adds a generic interaction verb for opening the editor
    /// </summary>
    [DataField]
    public bool AddEditorVerb = true;

    /// <summary>
    /// Requires a present and open wires panel before the editor can open or save
    /// </summary>
    [DataField]
    public bool RequireOpenPanel;

    /// <summary>
    /// Shows the canvas layer only while the entity reports receiving power
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool RequirePowerToDisplay;

    /// <summary>
    /// Packed representation of <see cref="BackgroundColor"/>
    /// </summary>
    public uint PackedBackground => PackColor(BackgroundColor);

    public uint PackedDefaultDrawingColor => PackColor(DefaultDrawingColor) | 0xFF000000;

    /// <summary>
    /// Checks that the given dimensions are within the component's configured limits.
    /// </summary>
    public static bool DimensionsWithinLimit(int width, int height)
    {
        return width is > 0 and <= MaxWidth &&
               height is > 0 and <= MaxHeight;
    }

    /// <summary>
    /// Checks that the given texture dimensions and offsets are within the component's configured limits.
    /// </summary>
    public bool RenderBoundsAreValid(int textureWidth, int textureHeight)
    {
        return textureWidth is > 0 and <= MaxTextureSize &&
               textureHeight is > 0 and <= MaxTextureSize &&
               OffsetX >= 0 && OffsetY >= 0 &&
               (long) OffsetX + Width <= textureWidth &&
               (long) OffsetY + Height <= textureHeight;
    }

    /// <summary>
    /// Encodes a color as <c>0xAARRGGBB</c>.
    /// </summary>
    public static uint PackColor(Color color) =>
        ((uint) color.AByte << 24) | ((uint) color.RByte << 16) | ((uint) color.GByte << 8) | color.BByte;
}

/// <summary>
/// Allows a canvas to be permanently locked against further editing.
/// Other systems may lock it through <c>CanvasDesignSystem.Lock</c>.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class CanvasDesignLockComponent : Component
{
    /// <summary>Whether the canvas can no longer be edited.</summary>
    [DataField, AutoNetworkedField]
    public bool Locked;
}

[Serializable, NetSerializable]
public enum CanvasDesignUiKey : byte
{
    Key
}

/// <summary>
/// Raised when a user attempts to open the canvas editor. The event can be cancelled to prevent the editor from opening.
/// </summary>
[ByRefEvent]
public record struct CanvasDesignEditAttemptEvent(EntityUid User, bool Cancelled = false);

[Serializable, NetSerializable]
public readonly record struct CanvasPixelChange(ushort Index, uint Color);

/// <summary>
/// Message sent from the client to the server when a user attempts to save their changes. The server will validate the changes and update the authoritative state if valid.
/// </summary>
[Serializable, NetSerializable]
public sealed class CanvasDesignSaveMessage(CanvasPixelChange[] changes, string name, string description)
    : BoundUserInterfaceMessage
{
    public CanvasPixelChange[] Changes { get; } = changes;
    public string Name { get; } = name;
    public string Description { get; } = description;
}

/// <summary>
/// Immutable state sent from the server to the client when the canvas editor is opened. Contains the current canvas data and configuration.
/// </summary>
[Serializable, NetSerializable]
public sealed class CanvasDesignUiState(
    int width,
    int height,
    uint background,
    uint defaultDrawingColor,
    bool metadataEnabled,
    int maxNameLength,
    int maxDescriptionLength,
    string editorTitle,
    uint[] pixels,
    string name,
    string description,
    string defaultName,
    string defaultDescription) : BoundUserInterfaceState
{
    public int Width { get; } = width;
    public int Height { get; } = height;
    public uint Background { get; } = background;
    public uint DefaultDrawingColor { get; } = defaultDrawingColor;
    public bool MetadataEnabled { get; } = metadataEnabled;
    public int MaxNameLength { get; } = maxNameLength;
    public int MaxDescriptionLength { get; } = maxDescriptionLength;
    public string EditorTitle { get; } = editorTitle;
    public uint[] Pixels { get; } = pixels;
    public string Name { get; } = name;
    public string Description { get; } = description;
    public string DefaultName { get; } = defaultName;
    public string DefaultDescription { get; } = defaultDescription;
}

/// <summary>
/// Immutable state sent from the server to the client when a canvas preview is requested. Contains the current canvas data and configuration.
/// </summary>
[Serializable, NetSerializable]
public sealed class CanvasDesignPreviewData(
    int previewId,
    int width,
    int height,
    uint background,
    uint[] pixels,
    string name,
    string description,
    string savedBy,
    long savedAt,
    int serverOffsetMinutes)
{
    public int PreviewId { get; } = previewId;
    public int Width { get; } = width;
    public int Height { get; } = height;
    public uint Background { get; } = background;
    public uint[] Pixels { get; } = pixels;
    public string Name { get; } = name;
    public string Description { get; } = description;
    public string SavedBy { get; } = savedBy;
    public long SavedAt { get; } = savedAt;
    public int ServerOffsetMinutes { get; } = serverOffsetMinutes;
}

/// <summary>
/// Supplies the retained revision list and initially selected revision to the history viewer.
/// </summary>
[Serializable, NetSerializable]
public sealed class CanvasDesignHistoryEuiState(
    CanvasDesignHistoryEntry[] entries,
    CanvasDesignPreviewData? selected,
    bool showTargets) : EuiStateBase
{
    public CanvasDesignHistoryEntry[] Entries { get; } = entries;
    public CanvasDesignPreviewData? Selected { get; } = selected;
    public bool ShowTargets { get; } = showTargets;
}

[Serializable, NetSerializable]
public sealed record CanvasDesignHistoryEntry(int PreviewId, string SavedBy, int EntityId, string EntityName);

[Serializable, NetSerializable]
public sealed class CanvasDesignHistorySelectMessage(int previewId) : EuiMessageBase
{
    public int PreviewId { get; } = previewId;
}

[Serializable, NetSerializable]
public sealed class CanvasDesignHistoryPreviewMessage(CanvasDesignPreviewData preview) : EuiMessageBase
{
    public CanvasDesignPreviewData Preview { get; } = preview;
}
