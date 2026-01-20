// SPDX-FileCopyrightText: 2024 Julian Giebel
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-License-Identifier: MIT

using Content.Shared.Anomaly;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client.Anomaly.Ui;

[UsedImplicitly]
public sealed class AnomalyGeneratorBoundUserInterface : BoundUserInterface
{
    private AnomalyGeneratorWindow? _window;

    public AnomalyGeneratorBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<AnomalyGeneratorWindow>();
        _window.SetEntity(Owner);

        _window.OnGenerateButtonPressed += () =>
        {
            SendMessage(new AnomalyGeneratorGenerateButtonPressedEvent());
        };
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not AnomalyGeneratorUserInterfaceState msg)
            return;
        _window?.UpdateState(msg);
    }
}

