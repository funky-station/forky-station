// SPDX-FileCopyrightText: 2026 Gansu <peat.allan13@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.Cargo.Serialization;


[TypeSerializer]
public sealed class CargoBountyItemEntryTypeSerializer : ITypeReader<ICargoBountyEntry, MappingDataNode>
{
    private Type? GetType(MappingDataNode node)
    {
        if (node.Has("whitelist"))
        {
            return typeof(CargoBountyItemEntry);
        }

        if (node.Has("reagent"))
        {
            return typeof(CargoBountyReagentEntry);
        }

        if (node.Has("gas"))
        {
            return typeof(CargoBountyGasEntry);
        }

        return null;
    }
    public ICargoBountyEntry Read(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<ICargoBountyEntry>? instanceProvider = null)
    {
        var type = GetType(node) ??
                   throw new ArgumentException(
                       "Tried to convert invalid YAML node mapping to ConstructionGraphStep!");
        return (ICargoBountyEntry)serializationManager.Read(type, node, hookCtx, context)!;
    }
    public ValidationNode Validate(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        ISerializationContext? context = null)
    {
        var type = GetType(node);
        if (type == null)
            return new ErrorNode(node, "No construction graph step type found.");
        return serializationManager.ValidateNode(type, node, context);
    }
}
