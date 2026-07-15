using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Content.Shared.BloodCult;
using Content.Shared.BloodCult.Components;
using Content.Shared.BloodCult.Prototypes;
using Content.Client.BloodCult.UI;

namespace Content.Client.BloodCult;

public sealed partial class SpellsBoundUserInterface : BoundUserInterface
{
	[Dependency] private IClyde _displayManager = default!;
	[Dependency] private IInputManager _inputManager = default!;
	[Dependency] private IEntitySystemManager _entitySystemManager = default!;

	private SpellRadialMenu? _spellRitualMenu;

	public SpellsBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
	{
	}

	protected override void Open()
	{
		base.Open();

		_spellRitualMenu = this.CreateWindow<SpellRadialMenu>();
		_spellRitualMenu.InitializeDependencies(_entitySystemManager.DependencyCollection);
		_spellRitualMenu.SetEntity(Owner);
		_spellRitualMenu.SendSpellsMessageAction += SendSpellsMessage;

		var vpSize = _displayManager.ScreenSize;
		_spellRitualMenu.OpenCenteredAt(_inputManager.MouseScreenPosition.Position / vpSize);
	}

	private void SendSpellsMessage(ProtoId<CultAbilityPrototype> protoId)
	{
		SendMessage(new SpellsMessage(protoId));
	}
}
