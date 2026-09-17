using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    ///     Deadzone for drag-drop interactions.
    /// </summary>
    public static readonly CVarDef<float> DragDropDeadZone =
        CVarDef.Create("control.drag_dead_zone", 12f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    ///     Toggles whether the walking key is a toggle or a held key.
    /// </summary>
    public static readonly CVarDef<bool> ToggleWalk =
        CVarDef.Create("control.toggle_walk", false, CVar.CLIENTONLY | CVar.ARCHIVE);

    // The rationale behind the default limit is simply that I can easily get to 7 interactions per second by just
    // trying to spam toggle a light switch or lever (though the UseDelay component limits the actual effect of the
    // interaction).  I don't want to accidentally spam admins with alerts just because somebody is spamming a
    // key manually, nor do we want to alert them just because the player is having network issues and the server
    // receives multiple interactions at once. But we also want to try catch people with modified clients that spam
    // many interactions on the same tick. Hence, a very short period, with a relatively high count.

    /// <summary>
    ///     Maximum number of interactions that a player can perform within <see cref="InteractionRateLimitCount"/> seconds
    /// </summary>
    public static readonly CVarDef<int> InteractionRateLimitCount =
        CVarDef.Create("interaction.rate_limit_count", 5, CVar.SERVER | CVar.REPLICATED);

    /// <seealso cref="InteractionRateLimitCount"/>
    public static readonly CVarDef<float> InteractionRateLimitPeriod =
        CVarDef.Create("interaction.rate_limit_period", 0.5f, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    ///     Minimum delay (in seconds) between notifying admins about interaction rate limit violations. A negative
    ///     value disables admin announcements.
    /// </summary>
    public static readonly CVarDef<int> InteractionRateLimitAnnounceAdminsDelay =
        CVarDef.Create("interaction.rate_limit_announce_admins_delay", 120, CVar.SERVERONLY);

    /// <summary>
    ///     Whether or not the storage UI is static and bound to the hotbar, or unbound and allowed to be dragged anywhere.
    /// </summary>
    public static readonly CVarDef<bool> StaticStorageUI =
        CVarDef.Create("control.static_storage_ui", true, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    ///     Whether or not the storage window uses a transparent or opaque sprite.
    /// </summary>
    public static readonly CVarDef<bool> OpaqueStorageWindow =
        CVarDef.Create("control.opaque_storage_background", false, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Whether or not the storage window has a title of the entity name.
    /// </summary>
    public static readonly CVarDef<bool> StorageWindowTitle =
        CVarDef.Create("control.storage_window_title", false, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// How many storage windows are allowed to be open at once.
    /// Recommended that you utilise this in conjunction with <see cref="StaticStorageUI"/>
    /// </summary>
    public static readonly CVarDef<int> StorageLimit =
        CVarDef.Create("control.storage_limit", 1, CVar.REPLICATED | CVar.SERVER);

    /// <summary>
    /// Whether or not storage can be opened recursively.
    /// </summary>
    public static readonly CVarDef<bool> NestedStorage =
        CVarDef.Create("control.nested_storage", true, CVar.REPLICATED | CVar.SERVER);

    /// <summary>
    ///     If enabled, melee weapons that have click-to-attack patterns (unarmed, slow weapons) will continue attacking if the button is held.
    /// </summary>
    public static readonly CVarDef<bool> ControlHoldToAttackMelee =
        CVarDef.Create("control.hold_to_attack_melee", false, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    ///     If enabled, ranged weapons that have click-to-attack patterns (burst and semi-auto guns) will continue attacking if the button is held.
    /// </summary>
    public static readonly CVarDef<bool> ControlHoldToAttackRanged =
        CVarDef.Create("control.hold_to_attack_ranged", false, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Whether to use custom interaction outline colors, as defined by
    /// <see cref="CustomValidInteractionOutlineColor"/> and <see cref="CustomInvalidInteractionOutlineColor"/>.
    /// </summary>
    public static readonly CVarDef<bool> UseCustomInteractionOutlineColors = CVarDef.Create(
        "interaction.outline.use_custom_colors", false, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// The color of the outline for objects in-range of interaction.
    /// </summary>
    // default colors based on Resources/Prototypes/Shaders/outline.yml
    public static readonly CVarDef<string> CustomValidInteractionOutlineColor = CVarDef.Create(
        "interaction.outline.in_range_color", "#00FF0055", CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// The color of the outline for objects out-of-range of interaction.
    /// </summary>
    // default colors based on Resources/Prototypes/Shaders/outline.yml
    public static readonly CVarDef<string> CustomInvalidInteractionOutlineColor = CVarDef.Create(
        "interaction.outline.out_of_range_color", "#FF000055", CVar.CLIENTONLY | CVar.ARCHIVE);
}
