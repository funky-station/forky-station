// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Jessica M <jessica@jessicamaybe.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Actions;

namespace Content.Shared.Store.Events;

/// <summary>
/// Opens a store specified by <see cref="StoreComponent"/>
/// Used for entities with a store built into themselves like Revenant or PAI
/// </summary>
public sealed partial class IntrinsicStoreActionEvent : InstantActionEvent
{
}
