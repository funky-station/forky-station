using System.Linq;
using Content.Client.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Piping.Trinary.Components;
using Content.Shared.Localizations;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Shared.Toolshed.Commands.Values;

namespace Content.Client.Atmos.UI;

/// <summary>
/// Initializes a <see cref="GasFilterWindow"/> and updates it from the entity's <see cref="GasFilterComponent"/>.
/// </summary>
[UsedImplicitly]
public sealed partial class GasFilterBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [Dependency] private AtmosphereSystem _atmosphere = default!;

    [ViewVariables]
    private GasFilterWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<GasFilterWindow>();
        _window.PopulateGasList(_atmosphere.Gases);

        _window.ToggleStatusButtonPressed += OnToggleStatusButtonPressed;
        _window.FilterTransferRateChanged += OnFilterTransferRatePressed;
        _window.SelectGasPressed += OnSelectGasPressed;

        Update();
    }

    public override void Update()
    {
        base.Update();

        if (_window == null || !EntMan.TryGetComponent(Owner, out GasFilterComponent? filter))
            return;

        _window.Title = EntMan.GetComponent<MetaDataComponent>(Owner).EntityName;
        _window.SetFilterStatus(filter.Enabled);
        _window.SetTransferRate(filter.TransferRate);
    }

    private void OnToggleStatusButtonPressed(bool status)
    {
        SendPredictedMessage(new GasFilterToggleStatusMessage(status));
    }

    private void OnFilterTransferRatePressed(string value)
    {
        var rate = UserInputParser.TryFloat(value, out var parsed) ? parsed : 0f;

        SendPredictedMessage(new GasFilterChangeRateMessage(rate));
    }

    private void OnSelectGasPressed()
    {
        if (_window is null)
            return;
        // Funky - Start
        if (_window.SelectedGases is null)
        {
            SendPredictedMessage(new GasFilterSelectGasesMessage(new List<Gas>()));
        }
        else
        {
            var gases = _window.SelectedGases.Where(gasId =>
            {
                return Enum.TryParse<Gas>(gasId, out var gas);
            }).Select(Enum.Parse<Gas>).ToList();

            SendPredictedMessage(new GasFilterSelectGasesMessage(gases));
        }
        // Funky - End
    }
}
