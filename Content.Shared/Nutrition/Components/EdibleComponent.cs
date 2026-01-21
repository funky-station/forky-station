// SPDX-FileCopyrightText: 2019, 2021-2022 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2019, 2021-2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2019-2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2019 moneyl <8206401+Moneyl@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020-2023, 2025 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 DmitriyRubetskoy <75271456+DmitriyRubetskoy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 nuke <47336974+nuke-makes-games@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 ike709 <ike709@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Hugal31 <hugo.laloge@gmail.com>
// SPDX-FileCopyrightText: 2020 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 chairbender <kwhipke1@gmail.com>
// SPDX-FileCopyrightText: 2020 Memory <58238103+FL-OZ@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 FL-OZ <58238103+FL-OZ@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Clyybber <darkmine956@gmail.com>
// SPDX-FileCopyrightText: 2020 PrPleGoo <PrPleGoo@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021-2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021-2022 Vera Aguilera Puerto <zddm@outlook.es>
// SPDX-FileCopyrightText: 2021-2022 mirrorcult <notzombiedude@gmail.com>
// SPDX-FileCopyrightText: 2021 FoLoKe <36813380+FoLoKe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Ygg01 <y.laughing.man.y@gmail.com>
// SPDX-FileCopyrightText: 2021 20kdc <asdd2808@gmail.com>
// SPDX-FileCopyrightText: 2021 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Galactic Chimp <63882831+GalacticChimp@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2021 Remie Richards <remierichards@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023-2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Doru991 <75124791+Doru991@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 LankLTE <135308300+LankLTE@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Magnus Larsen <i.am.larsenml@gmail.com>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
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
