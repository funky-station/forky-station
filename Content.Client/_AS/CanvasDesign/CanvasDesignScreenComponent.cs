using Robust.Client.Graphics;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client._AS.CanvasDesign;

/// <summary>
/// Client texture and pixel buffer owned by a canvasdesign entity.
/// </summary>
[RegisterComponent]
public sealed partial class CanvasDesignScreenComponent : Component
{
    public OwnedTexture? Texture; // The texture used to render the canvas design. This is created and managed by the CanvasDesignSystem.
    public Rgba32[] Buffer = []; // Pixel buffer for the texture, stored as Rgba32 values.
    public int TextureWidth;
    public int TextureHeight;
}
