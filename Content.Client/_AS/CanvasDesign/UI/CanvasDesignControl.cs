using System.Numerics;
using Content.Shared._AS.CanvasDesign;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Input;

namespace Content.Client._AS.CanvasDesign.UI;

public sealed class CanvasDesignControl : Control
{
    private const int Scale = 16;
    private bool _drawing;
    private Vector2i _shapeStart;
    private uint[]? _shapeBase;
    private int _width = 1;
    private int _height = 1;
    private uint _background = 0xFFFFFFFFu;
    private uint[] _pixels = [0xFFFFFFFFu];
    public Color SelectedColor = Color.White;
    public CanvasDesignTool Tool = CanvasDesignTool.Pencil;
    public event Action? Changed;
    public event Action<Color>? Picked;

    public CanvasDesignControl()
    {
        SetSize = new Vector2(Scale, Scale);
        MouseFilter = MouseFilterMode.Stop;
    }

    /// <summary>
    /// Configures the canvas size and background color. Resets the pixel buffer to the background color.
    /// </summary>
    public void Configure(int width, int height, uint background)
    {
        if (!CanvasDesignComponent.DimensionsWithinLimit(width, height))
            return;

        _width = width;
        _height = height;
        _background = background;
        if (_pixels.Length != width * height)
        {
            _pixels = new uint[width * height];
            Array.Fill(_pixels, _background);
        }
        SetSize = new Vector2(width * Scale, height * Scale);
        InvalidateMeasure();
    }

    public uint[] GetPixels() => (uint[]) _pixels.Clone();
    public void Clear()
    {
        Array.Fill(_pixels, _background);
        Changed?.Invoke();
        InvalidateMeasure();
    }

    public void SetPixels(uint[] pixels)
    {
        if (pixels.Length == _width * _height)
            _pixels = (uint[]) pixels.Clone();
        InvalidateMeasure();
    }

    protected override void Draw(DrawingHandleScreen handle)
    {
        for (var y = 0; y < _height; y++)
        for (var x = 0; x < _width; x++)
        {
            var value = _pixels[y * _width + x];
            var color = new Color((byte) (value >> 16), (byte) (value >> 8), (byte) value,
                (byte) (value >> 24));
            var box = UIBox2.FromDimensions(new Vector2(x, y) * Scale, new Vector2(Scale - 1));
            handle.DrawRect(box, color);
        }
    }

    /// <summary>
    /// Handles mouse input for drawing on the canvas.
    /// </summary>
    protected override void KeyBindDown(GUIBoundKeyEventArgs args)
    {
        base.KeyBindDown(args);
        if (args.Function != EngineKeyFunctions.UIClick)
            return;
        _drawing = true;
        if (IsShapeTool())
        {
            _shapeStart = PixelAt(args.RelativePosition);
            _shapeBase = (uint[]) _pixels.Clone();
        }
        UseAt(args.RelativePosition);
        args.Handle();
    }

    protected override void KeyBindUp(GUIBoundKeyEventArgs args)
    {
        base.KeyBindUp(args);
        if (args.Function == EngineKeyFunctions.UIClick)
        {
            _drawing = false;
            if (_shapeBase != null)
            {
                _shapeBase = null;
                Changed?.Invoke();
            }
        }
    }

    protected override void MouseMove(GUIMouseMoveEventArgs args)
    {
        base.MouseMove(args);
        if (_drawing && (Tool is CanvasDesignTool.Pencil or CanvasDesignTool.Eraser || IsShapeTool()))
            UseAt(args.RelativePosition);
    }

    /// <summary>
    /// Applies the current tool at the given position on the canvas.
    /// </summary>
    private void UseAt(Vector2 position)
    {
        var point = PixelAt(position);
        var x = point.X;
        var y = point.Y;
        if (x < 0 || y < 0 || x >= _width || y >= _height)
            return;
        var index = y * _width + x;
        if (Tool == CanvasDesignTool.Picker)
        {
            var value = _pixels[index];
            Picked?.Invoke(new Color((byte) (value >> 16), (byte) (value >> 8), (byte) value));
            return;
        }

        var color = Tool == CanvasDesignTool.Eraser ? _background : PackedSelectedColor();
        if (IsShapeTool())
        {
            if (_shapeBase == null)
                return;
            Array.Copy(_shapeBase, _pixels, _pixels.Length);
            DrawShape(_shapeStart, point, color);
            InvalidateMeasure();
            return;
        }

        if (Tool == CanvasDesignTool.Fill)
            FloodFill(x, y, color);
        else
            _pixels[index] = color;

        Changed?.Invoke();
        InvalidateMeasure();
    }

    private uint PackedSelectedColor() => 0xFF000000u | ((uint) SelectedColor.RByte << 16) |
                                          ((uint) SelectedColor.GByte << 8) | SelectedColor.BByte;

    private Vector2i PixelAt(Vector2 position) => new(
        Math.Clamp((int) (position.X / Scale), 0, _width - 1),
        Math.Clamp((int) (position.Y / Scale), 0, _height - 1));

    private bool IsShapeTool() => Tool is CanvasDesignTool.Line or CanvasDesignTool.Square or CanvasDesignTool.Circle;

    private void DrawShape(Vector2i start, Vector2i end, uint color)
    {
        switch (Tool)
        {
            case CanvasDesignTool.Line:
                DrawLine(start.X, start.Y, end.X, end.Y, color);
                break;
            case CanvasDesignTool.Square:
                DrawRectangle(start, end, color);
                break;
            case CanvasDesignTool.Circle:
                DrawEllipse(start, end, color);
                break;
        }
    }

    private void DrawLine(int x0, int y0, int x1, int y1, uint color)
    {
        var dx = Math.Abs(x1 - x0);
        var sx = x0 < x1 ? 1 : -1;
        var dy = -Math.Abs(y1 - y0);
        var sy = y0 < y1 ? 1 : -1;
        var error = dx + dy;
        while (true)
        {
            SetPixel(x0, y0, color);
            if (x0 == x1 && y0 == y1)
                break;
            var twiceError = error * 2;
            if (twiceError >= dy)
            {
                error += dy;
                x0 += sx;
            }
            if (twiceError <= dx)
            {
                error += dx;
                y0 += sy;
            }
        }
    }

    private void DrawRectangle(Vector2i start, Vector2i end, uint color)
    {
        var left = Math.Min(start.X, end.X);
        var right = Math.Max(start.X, end.X);
        var top = Math.Min(start.Y, end.Y);
        var bottom = Math.Max(start.Y, end.Y);
        for (var x = left; x <= right; x++)
        {
            SetPixel(x, top, color);
            SetPixel(x, bottom, color);
        }
        for (var y = top; y <= bottom; y++)
        {
            SetPixel(left, y, color);
            SetPixel(right, y, color);
        }
    }

    private void DrawEllipse(Vector2i start, Vector2i end, uint color)
    {
        var centerX = (start.X + end.X) / 2f;
        var centerY = (start.Y + end.Y) / 2f;
        var radiusX = Math.Max(Math.Abs(end.X - start.X) / 2f, 0.5f);
        var radiusY = Math.Max(Math.Abs(end.Y - start.Y) / 2f, 0.5f);
        var steps = Math.Max(12, (int) Math.Ceiling(Math.PI * (radiusX + radiusY) * 2));
        Vector2i? previous = null;
        for (var i = 0; i <= steps; i++)
        {
            var angle = Math.Tau * i / steps;
            var point = new Vector2i(
                (int) Math.Round(centerX + radiusX * Math.Cos(angle)),
                (int) Math.Round(centerY + radiusY * Math.Sin(angle)));
            if (previous != null)
                DrawLine(previous.Value.X, previous.Value.Y, point.X, point.Y, color);
            previous = point;
        }
    }

    private void SetPixel(int x, int y, uint color)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
            _pixels[y * _width + x] = color;
    }

    private void FloodFill(int startX, int startY, uint replacement)
    {
        var target = _pixels[startY * _width + startX];
        if (target == replacement)
            return;

        var pending = new Queue<(int X, int Y)>();
        pending.Enqueue((startX, startY));
        while (pending.TryDequeue(out var point))
        {
            if (point.X < 0 || point.Y < 0 ||
                point.X >= _width || point.Y >= _height)
                continue;

            var index = point.Y * _width + point.X;
            if (_pixels[index] != target)
                continue;

            _pixels[index] = replacement;
            pending.Enqueue((point.X - 1, point.Y));
            pending.Enqueue((point.X + 1, point.Y));
            pending.Enqueue((point.X, point.Y - 1));
            pending.Enqueue((point.X, point.Y + 1));
        }
    }
}

public enum CanvasDesignTool : byte
{
    Pencil,
    Eraser,
    Picker,
    Fill,
    Line,
    Square,
    Circle
}
