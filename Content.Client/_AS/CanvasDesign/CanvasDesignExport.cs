using System.IO;
using System.Text;
using System.Threading.Tasks;
using Robust.Client.UserInterface;

namespace Content.Client._AS.CanvasDesign;

/// <summary>
/// Writes canvas designs through the native file dialog using the format accepted by the canvas importer.
/// </summary>
public static class CanvasDesignExport
{
    public static async Task Save(IFileDialogManager fileDialogs, string exportedBy, string name, string description, int width, int height, uint background, uint[] pixels)
    {
        var text = new StringBuilder()
            .AppendLine($"exportedBy: \"{EscapeYaml(exportedBy)}\"")
            .AppendLine($"name: \"{EscapeYaml(name)}\"")
            .AppendLine($"description: \"{EscapeYaml(description)}\"")
            .AppendLine($"width: {width}")
            .AppendLine($"height: {height}")
            .AppendLine("pixels:");

        var byColor = new SortedDictionary<uint, List<int>>();
        for (var index = 0; index < pixels.Length; index++)
        {
            if (pixels[index] == background)
                continue;

            var color = pixels[index] & 0xFFFFFF;
            if (!byColor.TryGetValue(color, out var locations))
                byColor[color] = locations = [];
            locations.Add(index);
        }

        foreach (var (color, locations) in byColor)
        {
            text.AppendLine($"  \"{color:X6}\":");
            foreach (var index in locations)
            {
                text.AppendLine($"    - [{index % width}, {index / width}]");
            }
        }

        var file = await fileDialogs.SaveFile(new FileDialogFilters(new FileDialogFilters.Group("yml")));
        if (file == null)
            return;

        try
        {
            await using var writer = new StreamWriter(file.Value.fileStream, Encoding.UTF8);
            await writer.WriteAsync(text.ToString());
        }
        catch (Exception)
        {
            // This space is intentionally empty
        }
    }

    private static string EscapeYaml(string value)
    {
        return value.Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n");
    }
}
