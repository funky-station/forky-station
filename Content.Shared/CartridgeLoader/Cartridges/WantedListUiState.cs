// SPDX-FileCopyrightText: 2024 Эдуард <36124833+Ertanic@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.CriminalRecords;
using Content.Shared.CriminalRecords.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable, NetSerializable]
public sealed class WantedListUiState(List<WantedRecord> records) : BoundUserInterfaceState
{
    public List<WantedRecord> Records = records;
}
