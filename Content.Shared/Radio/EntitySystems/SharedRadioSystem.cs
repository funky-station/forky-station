using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared.Radio.EntitySystems;

/// <summary>
/// This system handles intrinsic radios and the general process of converting radio messages into chat messages.
/// </summary>
public abstract partial class SharedRadioSystem : EntitySystem
{
    /// <summary>
    /// Send radio message to all active radio listeners.
    /// </summary>
    [PublicAPI]
    public void SendRadioMessage(EntityUid messageSource,
        string message,
        ProtoId<RadioChannelPrototype> channel,
        EntityUid radioSource,
        bool escapeMarkup = true,
        string? overrideName = null // funky - add override option
        )
    {
        SendRadioMessage(messageSource, message, ProtoMan.Index(channel), radioSource, escapeMarkup: escapeMarkup, overrideName: overrideName); // funky - override name option
    }

    /// <summary>
    /// Sends a radio message to all active radio listeners.
    /// </summary>
    /// <param name="messageSource">Entity that spoke the message.</param>
    /// <param name="message">Message to send over the radio.</param>
    /// <param name="channel">Radio channel to send the message on.</param>
    /// <param name="radioSource">Entity transmitting the message.</param>
    /// <param name="escapeMarkup">Whether markup in the message should be escaped.</param>
    /// <param name="overrideName">Funky - Name to use instead of the entity's name</param>
    [PublicAPI]
    public virtual void SendRadioMessage(EntityUid messageSource,
        string message,
        RadioChannelPrototype channel,
        EntityUid radioSource,
        bool escapeMarkup = true,
        string? overrideName = null // funky - add override option
        )
    {

    }
}
