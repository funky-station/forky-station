// Persistence: Chat stacking from RMC14 - pull/7587
using Robust.Shared.GameStates;

namespace Content.Shared._RMC14.Chat;

[RegisterComponent, NetworkedComponent]
public sealed partial class ChatRepeatIgnoreSenderComponent : Component;
