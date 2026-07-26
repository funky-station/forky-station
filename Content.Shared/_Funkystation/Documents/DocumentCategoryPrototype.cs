using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._Funkystation.Documents;

/// <summary>
/// Defines a category that document printer UIs group documents under
/// </summary>
[Prototype]
public sealed partial class DocumentCategoryPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = null!;

    /// <summary>
    /// .ftl for the display name shown on the tab
    /// </summary>
    [DataField("name", required: true)]
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Icon shown alongside the tab name in the printer UI
    /// </summary>
    [DataField("icon")]
    public SpriteSpecifier Icon { get; private set; } = SpriteSpecifier.Invalid;

    /// <summary>
    /// Controls tab ordering, ascending
    /// </summary>
    [DataField("priority")]
    public int Priority { get; private set; }

    /// <summary>
    /// Tabs with this will have a scrambled, glitchy name
    /// </summary>
    [DataField("scrambleName")]
    public bool ScrambleName;
}
