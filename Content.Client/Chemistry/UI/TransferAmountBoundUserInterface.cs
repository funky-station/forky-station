// SPDX-FileCopyrightText: 2026 slarticodefast
// SPDX-FileCopyrightText: 2024 Brandon Hu
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-License-Identifier: MIT

using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client.Chemistry.UI;

[UsedImplicitly]
public sealed class TransferAmountBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private TransferAmountWindow? _window;

    protected override void Open()
    {
        base.Open();
        _window = this.CreateWindow<TransferAmountWindow>();

        if (EntMan.TryGetComponent<SolutionTransferComponent>(Owner, out var comp))
            _window.SetBounds(comp.MinimumTransferAmount.Int(), comp.MaximumTransferAmount.Int());

        _window.ApplyButton.OnPressed += _ =>
        {
            if (int.TryParse(_window.AmountLineEdit.Text, out var i))
            {
                SendPredictedMessage(new TransferAmountSetValueMessage(FixedPoint2.New(i)));
                _window.Close();
            }
        };
    }
}
