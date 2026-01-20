// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-License-Identifier: MIT

namespace Content.Server.Disposal.Unit
{
    public record DoInsertDisposalUnitEvent(EntityUid? User, EntityUid ToInsert, EntityUid Unit);
}
