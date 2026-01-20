// SPDX-FileCopyrightText: 2025 YotaXP
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 DamianX
// SPDX-FileCopyrightText: 2020 DamianX
// SPDX-License-Identifier: MIT

using Content.Shared.Construction.Prototypes;
using Content.Shared.Preferences;
using Robust.Shared.Prototypes;

namespace Content.Client.Lobby
{
    public interface IClientPreferencesManager
    {
        event Action OnServerDataLoaded;

        bool ServerDataLoaded => Settings != null;

        GameSettings? Settings { get; }
        PlayerPreferences? Preferences { get; }
        void Initialize();
        void SelectCharacter(ICharacterProfile profile);
        void SelectCharacter(int slot);
        void UpdateCharacter(ICharacterProfile profile, int slot);
        void CreateCharacter(ICharacterProfile profile);
        void DeleteCharacter(ICharacterProfile profile);
        void DeleteCharacter(int slot);
        void UpdateConstructionFavorites(List<ProtoId<ConstructionPrototype>> favorites);
    }
}
