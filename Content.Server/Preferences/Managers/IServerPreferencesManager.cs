// SPDX-FileCopyrightText: 2025 YotaXP
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2019 DamianX
// SPDX-FileCopyrightText: 2020 DamianX
// SPDX-License-Identifier: MIT

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Preferences;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Preferences.Managers
{
    public interface IServerPreferencesManager
    {
        void Init();

        Task LoadData(ICommonSession session, CancellationToken cancel);
        void FinishLoad(ICommonSession session);
        void OnClientDisconnected(ICommonSession session);

        bool TryGetCachedPreferences(NetUserId userId, [NotNullWhen(true)] out PlayerPreferences? playerPreferences);
        PlayerPreferences GetPreferences(NetUserId userId);
        PlayerPreferences? GetPreferencesOrNull(NetUserId? userId);
        IEnumerable<KeyValuePair<NetUserId, ICharacterProfile>> GetSelectedProfilesForPlayers(List<NetUserId> userIds);
        bool HavePreferencesLoaded(ICommonSession session);

        Task SetProfile(NetUserId userId, int slot, ICharacterProfile profile);
        Task SetConstructionFavorites(NetUserId userId, List<ProtoId<ConstructionPrototype>> favorites);
    }
}
