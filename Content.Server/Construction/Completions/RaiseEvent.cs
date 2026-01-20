// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Construction;
using JetBrains.Annotations;

namespace Content.Server.Construction.Completions
{
    [UsedImplicitly]
    public sealed partial class RaiseEvent : IGraphAction
    {
        [DataField("event", required:true)]
        public EntityEventArgs? Event { get; private set; }

        [DataField("directed")]
        public bool Directed { get; private set; } = true;

        [DataField("broadcast")]
        public bool Broadcast { get; private set; } = true;

        public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
        {
            if (Event == null || !Directed && !Broadcast)
                return;

            if(Directed)
                entityManager.EventBus.RaiseLocalEvent(uid, (object)Event);

            if(Broadcast)
                entityManager.EventBus.RaiseEvent(EventSource.Local, (object)Event);
        }
    }
}
