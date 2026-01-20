// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2023 Timothy Teakettle
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.Construction.Steps
{
    [TypeSerializer]
    public sealed class ConstructionGraphStepTypeSerializer : ITypeReader<ConstructionGraphStep, MappingDataNode>
    {
        private Type? GetType(MappingDataNode node)
        {
            if (node.Has("material"))
            {
                return typeof(MaterialConstructionGraphStep);
            }

            if (node.Has("tool"))
            {
                return typeof(ToolConstructionGraphStep);
            }

            if (node.Has("component"))
            {
                return typeof(ComponentConstructionGraphStep);
            }

            if (node.Has("tag"))
            {
                return typeof(TagConstructionGraphStep);
            }

            if (node.Has("allTags") || node.Has("anyTags"))
            {
                return typeof(MultipleTagsConstructionGraphStep);
            }

            if (node.Has("minTemperature") || node.Has("maxTemperature"))
            {
                return typeof(TemperatureConstructionGraphStep);
            }

            if (node.Has("assemblyId") || node.Has("guideString"))
            {
                return typeof(PartAssemblyConstructionGraphStep);
            }

            return null;
        }

        public ConstructionGraphStep Read(ISerializationManager serializationManager,
            MappingDataNode node,
            IDependencyCollection dependencies,
            SerializationHookContext hookCtx,
            ISerializationContext? context = null,
            ISerializationManager.InstantiationDelegate<ConstructionGraphStep>? instanceProvider = null)
        {
            var type = GetType(node) ??
                       throw new ArgumentException(
                           "Tried to convert invalid YAML node mapping to ConstructionGraphStep!");

            return (ConstructionGraphStep)serializationManager.Read(type, node, hookCtx, context)!;
        }

        public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node,
            IDependencyCollection dependencies,
            ISerializationContext? context = null)
        {
            var type = GetType(node);

            if (type == null)
                return new ErrorNode(node, "No construction graph step type found.");

            return serializationManager.ValidateNode(type, node, context);
        }
    }
}
