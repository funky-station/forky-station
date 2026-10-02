using Content.Shared._AS.EditableMetadata;

namespace Content.Server._AS.EditableMetadata;

/// <summary>
/// Applies <see cref="ASEditableMetadataComponent"/> overrides independently of any particular editor.
/// </summary>
public sealed partial class ASEditableMetadataSystem : EntitySystem
{
    [Dependency] private MetaDataSystem _metadata = null!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ASEditableMetadataComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<ASEditableMetadataComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnStartup(Entity<ASEditableMetadataComponent> ent, ref ComponentStartup args)
    {
        ent.Comp.MaxNameLength = Math.Clamp(ent.Comp.MaxNameLength,
            1,
            ASEditableMetadataComponent.AbsoluteMaxNameLength);
        ent.Comp.MaxDescriptionLength = Math.Clamp(ent.Comp.MaxDescriptionLength,
            1,
            ASEditableMetadataComponent.AbsoluteMaxDescriptionLength);

        var metadata = MetaData(ent);
        var prototype = Prototype(ent);
        var defaultName = prototype?.Name ?? metadata.EntityName;
        var defaultDescription = prototype?.Description ?? metadata.EntityDescription;
        if (ent.Comp.CustomName == defaultName)
            ent.Comp.CustomName = string.Empty;
        if (ent.Comp.Description == defaultDescription)
            ent.Comp.Description = string.Empty;

        Apply(ent, metadata);
        Dirty(ent);
    }

    private void OnShutdown(Entity<ASEditableMetadataComponent> ent, ref ComponentShutdown args)
    {
        if (TerminatingOrDeleted(ent.Owner))
            return;

        var metadata = MetaData(ent);
        var prototype = Prototype(ent);
        _metadata.SetEntityName(ent.Owner, prototype?.Name ?? metadata.EntityName, metadata);
        _metadata.SetEntityDescription(ent.Owner, prototype?.Description ?? metadata.EntityDescription, metadata);
    }

    /// <summary>
    /// Applies the current overrides and records them as synchronized.
    /// </summary>
    public void Apply(Entity<ASEditableMetadataComponent> ent, MetaDataComponent? metadata = null)
    {
        metadata ??= MetaData(ent);
        var prototype = Prototype(ent);
        var defaultName = prototype?.Name ?? metadata.EntityName;
        var defaultDescription = prototype?.Description ?? metadata.EntityDescription;
        _metadata.SetEntityName(ent.Owner,
            string.IsNullOrEmpty(ent.Comp.CustomName) ? defaultName : ent.Comp.CustomName,
            metadata);
        _metadata.SetEntityDescription(ent.Owner,
            string.IsNullOrEmpty(ent.Comp.Description) ? defaultDescription : ent.Comp.Description,
            metadata);
    }
}
