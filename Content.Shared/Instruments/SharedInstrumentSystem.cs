// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2023 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 EmoGarbage404
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-License-Identifier: MIT

namespace Content.Shared.Instruments;

public abstract class SharedInstrumentSystem : EntitySystem
{
    public abstract bool ResolveInstrument(EntityUid uid, ref SharedInstrumentComponent? component);

    public virtual void SetupRenderer(EntityUid uid, bool fromStateChange, SharedInstrumentComponent? instrument = null)
    {
    }

    public virtual void EndRenderer(EntityUid uid, bool fromStateChange, SharedInstrumentComponent? instrument = null)
    {
    }

    public void SetInstrumentProgram(EntityUid uid, SharedInstrumentComponent component, byte program, byte bank)
    {
        component.InstrumentBank = bank;
        component.InstrumentProgram = program;
        Dirty(uid, component);
    }
}
