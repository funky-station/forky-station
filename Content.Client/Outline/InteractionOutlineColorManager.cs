using Content.Shared._Funkystation.CCVar;
using Content.Shared.CCVar;
using JetBrains.Annotations;
using Robust.Shared.Configuration;

namespace Content.Client.Outline;

public sealed partial class InteractionOutlineColorManager : IPostInjectInit
{
    [Dependency] private IConfigurationManager _cfg = null!;
    [Dependency] private ILogManager _logManager = null!;

    private ISawmill _sawmill = null!;

    private static readonly CVarDef<string> ValidInteractionCvar = InteractionOutlineCVars.ValidInteractionOutlineColor;
    private static readonly CVarDef<string> InvalidInteractionCvar = InteractionOutlineCVars.InvalidInteractionOutlineColor;
    private static readonly CVarDef<bool> UseCustomOutlineCvar = InteractionOutlineCVars.UseCustomInteractionOutlineColors;

    private Color _validInteractionOutlineColor;
    private Color _invalidInteractionOutlineColor;

    private bool _useCustomColors;

    /// <summary>
    /// Gets the client's custom interaction outline colors set via <see cref="InteractionOutlineCVars.ValidInteractionOutlineColor"/>
    /// and <see cref="InteractionOutlineCVars.InvalidInteractionOutlineColor"/> if the client has enabled them via <see cref="InteractionOutlineCVars.UseCustomInteractionOutlineColors"/>,
    /// otherwise gets the default colors.
    /// </summary>
    /// <param name="inRange">Whether the thing we're getting the outline for is in-range or out-of-range.</param>
    [PublicAPI]
    public Color GetOutlineColor(bool inRange)
    {
        return inRange ? _validInteractionOutlineColor : _invalidInteractionOutlineColor;
    }

    private Color GetColorFromCvar(string hexString, CVarDef<string> cvar)
    {
        if (!Color.TryFromHex(hexString, out var newColor))
        {
            _sawmill.Warning($"{cvar.Name} is set to an invalid color (\"{hexString}\", expected a valid hex color). Attempting to reset CVar to default.");
            if (!Color.TryFromHex(cvar.DefaultValue, out newColor))
                _sawmill.Error($"{cvar.Name} has an invalid default value (\"{cvar.DefaultValue}\", expected a valid hex color).");
            else
                _cfg.SetCVar(cvar, cvar.DefaultValue);
        }
        return newColor;
    }

    private Color GetDefaultColorFromCvar(CVarDef<string> cvar)
    {
        if (!Color.TryFromHex(cvar.DefaultValue, out var newColor))
            _sawmill.Error($"{cvar.Name} has an invalid default value (\"{cvar.DefaultValue}\", expected a valid hex color).");
        return newColor;
    }

    private void SetDefaults(bool useCustom)
    {
        _useCustomColors = useCustom;
        if (useCustom)
        {
            _validInteractionOutlineColor = GetColorFromCvar(_cfg.GetCVar(ValidInteractionCvar), ValidInteractionCvar);
            _invalidInteractionOutlineColor = GetColorFromCvar(_cfg.GetCVar(InvalidInteractionCvar), InvalidInteractionCvar);
        }
        else
        {
            _validInteractionOutlineColor = GetDefaultColorFromCvar(ValidInteractionCvar);
            _invalidInteractionOutlineColor = GetDefaultColorFromCvar(InvalidInteractionCvar);
        }
    }

    void IPostInjectInit.PostInject()
    {
        _sawmill = _logManager.GetSawmill("interaction.outline_color");

        _cfg.OnValueChanged(
            ValidInteractionCvar,
            val =>
            {
                if (_useCustomColors)
                    _validInteractionOutlineColor = GetColorFromCvar(val, ValidInteractionCvar);
            });
        _cfg.OnValueChanged(
            InvalidInteractionCvar,
            val =>
            {
                if (_useCustomColors)
                    _invalidInteractionOutlineColor = GetColorFromCvar(val, InvalidInteractionCvar);
            });

        _cfg.OnValueChanged(UseCustomOutlineCvar, SetDefaults, true);
    }
}
