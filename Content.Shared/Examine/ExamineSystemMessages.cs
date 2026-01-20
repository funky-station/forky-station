// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Jacob Tong
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Shared.Verbs;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Examine
{
    public static class ExamineSystemMessages
    {
        [Serializable, NetSerializable]
        public sealed class RequestExamineInfoMessage : EntityEventArgs
        {
            public readonly NetEntity NetEntity;

            public readonly int Id;

            public readonly bool GetVerbs;

            public RequestExamineInfoMessage(NetEntity netEntity, int id, bool getVerbs=false)
            {
                NetEntity = netEntity;
                Id = id;
                GetVerbs = getVerbs;
            }
        }

        [Serializable, NetSerializable]
        public sealed class ExamineInfoResponseMessage : EntityEventArgs
        {
            public readonly NetEntity EntityUid;
            public readonly int Id;
            public readonly FormattedMessage Message;

            public List<Verb>? Verbs;

            public readonly bool CenterAtCursor;
            public readonly bool OpenAtOldTooltip;

            public readonly bool KnowTarget;

            public ExamineInfoResponseMessage(NetEntity entityUid, int id, FormattedMessage message, List<Verb>? verbs=null,
                bool centerAtCursor=true, bool openAtOldTooltip=true, bool knowTarget = true)
            {
                EntityUid = entityUid;
                Id = id;
                Message = message;
                Verbs = verbs;
                CenterAtCursor = centerAtCursor;
                OpenAtOldTooltip = openAtOldTooltip;
                KnowTarget = knowTarget;
            }
        }
    }
}
