// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2020 Julian Giebel
// SPDX-FileCopyrightText: 2020 ancientpower
// SPDX-FileCopyrightText: 2020 py01
// SPDX-License-Identifier: MIT

using System.Reflection;
using Content.Shared.NodeContainer.NodeGroups;
using Robust.Shared.Reflection;

namespace Content.Server.NodeContainer.NodeGroups
{
    public interface INodeGroupFactory
    {
        /// <summary>
        ///     Performs reflection to associate <see cref="INodeGroup"/> implementations with the
        ///     string specified in their <see cref="NodeGroupAttribute"/>.
        /// </summary>
        void Initialize();

        /// <summary>
        ///     Returns a new <see cref="INodeGroup"/> instance.
        /// </summary>
        INodeGroup MakeNodeGroup(NodeGroupID id);
    }

    public sealed class NodeGroupFactory : INodeGroupFactory
    {
        [Dependency] private readonly IReflectionManager _reflectionManager = default!;
        [Dependency] private readonly IDynamicTypeFactory _typeFactory = default!;

        private readonly Dictionary<NodeGroupID, Type> _groupTypes = new();

        public void Initialize()
        {
            var nodeGroupTypes = _reflectionManager.GetAllChildren<INodeGroup>();
            foreach (var nodeGroupType in nodeGroupTypes)
            {
                var att = nodeGroupType.GetCustomAttribute<NodeGroupAttribute>();
                if (att != null)
                {
                    foreach (var groupID in att.NodeGroupIDs)
                    {
                        _groupTypes.Add(groupID, nodeGroupType);
                    }
                }
            }
        }

        public INodeGroup MakeNodeGroup(NodeGroupID id)
        {
            if (!_groupTypes.TryGetValue(id, out var type))
                throw new ArgumentException($"{id} did not have an associated {nameof(INodeGroup)} implementation.");

            var instance = _typeFactory.CreateInstance<INodeGroup>(type);
            instance.Create(id);
            return instance;
        }
    }
}
