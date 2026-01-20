// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2025 DrSmugleaf
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2024 Magnus Larsen
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2023 Doru991
// SPDX-FileCopyrightText: 2023 LankLTE
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2019 metalgearsloth
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2021 FoLoKe
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2019 Acruid
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Remie Richards
// SPDX-FileCopyrightText: 2020 DmitriyRubetskoy
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 nuke
// SPDX-FileCopyrightText: 2020 ike709
// SPDX-FileCopyrightText: 2020 Hugal31
// SPDX-FileCopyrightText: 2020 AJCM-git
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 Memory
// SPDX-FileCopyrightText: 2020 FL-OZ
// SPDX-FileCopyrightText: 2020 Clyybber
// SPDX-FileCopyrightText: 2020 PrPleGoo
// SPDX-FileCopyrightText: 2019 moneyl
// SPDX-License-Identifier: MIT

using Content.Shared.Body.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Nutrition.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Nutrition.Components;

/// <summary>
/// This is used on an entity with a solution container to flag a specific solution as being able to have its
/// reagents consumed directly.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(IngestionSystem))]
public sealed partial class EdibleComponent : Component
{
    /// <summary>
    /// Name of the solution that stores the consumable reagents
    /// </summary>
    [DataField]
    public string Solution = "food";

    /// <summary>
    /// Should this entity be deleted when our solution is emptied?
    /// </summary>
    [DataField]
    public bool DestroyOnEmpty = true;

    /// <summary>
    /// Trash we spawn when eaten, will not spawn if the item isn't deleted when empty.
    /// </summary>
    [DataField]
    public List<EntProtoId> Trash = new();

    /// <summary>
    /// How much of our solution is eaten on a do-after completion. Set to null to eat the whole thing.
    /// </summary>
    [DataField]
    public FixedPoint2? TransferAmount = FixedPoint2.New(5);

    /// <summary>
    /// Acceptable utensils to use
    /// </summary>
    [DataField]
    public UtensilType Utensil = UtensilType.Fork; //There are more "solid" than "liquid" food

    /// <summary>
    /// Do we need a utensil to access this solution?
    /// </summary>
    [DataField]
    public bool UtensilRequired;

    /// <summary>
    ///     If this is set to true, food can only be eaten if you have a stomach with a
    ///     <see cref="StomachComponent.SpecialDigestible"/> that includes this entity in its whitelist,
    ///     rather than just being digestible by anything that can eat food.
    ///     Whitelist the food component to allow eating of normal food.
    /// </summary>
    [DataField]
    public bool RequiresSpecialDigestion;

    /// <summary>
    /// How long it takes to eat the food personally.
    /// </summary>
    [DataField]
    public TimeSpan Delay = TimeSpan.FromSeconds(1f);

    /// <summary>
    ///     This is how many seconds it takes to force-feed someone this food.
    ///     Should probably be smaller for small items like pills.
    /// </summary>
    [DataField]
    public TimeSpan ForceFeedDelay = TimeSpan.FromSeconds(3f);

    /// <summary>
    /// For mobs that are food, requires killing them before eating.
    /// </summary>
    [DataField]
    public bool RequireDead = true;

    /// <summary>
    /// An optional override for the sound made when consuming this item.
    /// Useful for if an edible type doesn't justify a new prototype, like with plushies.
    /// </summary>
    [DataField]
    public SoundSpecifier? UseSound;

    /// <summary>
    /// Verb, icon, and sound data for our edible.
    /// </summary>
    [DataField]
    public ProtoId<EdiblePrototype> Edible = IngestionSystem.Food;
}
