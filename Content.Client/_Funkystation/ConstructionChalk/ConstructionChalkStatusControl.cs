using Content.Client.Message;
using Content.Client.Stylesheets;
using Content.Shared._Funkystation.ConstructionChalk;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Shared.Timing;

namespace Content.Client._Funkystation.ConstructionChalk;

public sealed class ConstructionChalkStatusControl(Entity<ConstructionChalkComponent> ent) : Control
{
    private RichTextLabel? _label;

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        if (_label == null)
        {
            _label = new RichTextLabel { StyleClasses = { StyleClass.ItemStatus } };
            AddChild(_label);
        }
        else if (!ent.Comp.IsStatusControlUpdateRequired)
            return;

        UpdateLabel(_label);

        ent.Comp.IsStatusControlUpdateRequired = false;
    }

    private void UpdateLabel(RichTextLabel label)
    {
        var modeStringLocalized = Loc.GetString(ent.Comp.Mode switch
        {
            ChalkMode.Piping => "chalk-mode-piping",
            _ => "chalk-mode-construction",
        });

        label.SetMarkup(Loc.GetString("chalk-mode-status-label", ("modeString", modeStringLocalized)));
    }
}
