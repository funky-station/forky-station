// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Cloning
{
    [Serializable, NetSerializable]
    public enum AcceptCloningUiButton
    {
        Deny,
        Accept,
    }

    [Serializable, NetSerializable]
    public sealed class AcceptCloningChoiceMessage : EuiMessageBase
    {
        public readonly AcceptCloningUiButton Button;

        public AcceptCloningChoiceMessage(AcceptCloningUiButton button)
        {
            Button = button;
        }
    }
}
