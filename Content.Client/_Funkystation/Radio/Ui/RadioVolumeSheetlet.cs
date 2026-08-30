using System.Numerics;
using Content.Client.Resources;
using Content.Client.Stylesheets;
using Content.Client.Stylesheets.Stylesheets;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using static Content.Client.Stylesheets.StylesheetHelpers;

namespace Content.Client._Funkystation.Radio.Ui;

[CommonSheetlet]
public sealed class RadioVolumeSheetlet : Sheetlet<NanotrasenStylesheet>
{
    public override StyleRule[] GetRules(NanotrasenStylesheet sheet, object config)
    {
        var sliderKnob = new StyleBoxTexture
        {
            Texture = ResCache.GetTexture("/Textures/_Funkystation/Interface/Radio/slider-knob.png"),
        };
        sliderKnob.SetPatchMargin(StyleBox.Margin.All, 8);
        sliderKnob.TextureScale = 2*Vector2.One;

        var sliderBackground = new StyleBoxTexture
        {
            Texture = ResCache.GetTexture("/Textures/_Funkystation/Interface/Radio/slider-back.png"),
        };
        sliderBackground.SetPatchMargin(StyleBox.Margin.All, 7);
        sliderBackground.TextureScale = 2*Vector2.One;

        var sliderForeground = new StyleBoxEmpty();
        var sliderFill = new StyleBoxEmpty();

        return
        [
            E<Slider>().Identifier("VolumeSlider")
                .Prop(Slider.StylePropertyGrabber, sliderKnob)
                .Prop(Slider.StylePropertyBackground, sliderBackground)
                .Prop(Slider.StylePropertyForeground, sliderForeground)
                .Prop(Slider.StylePropertyFill, sliderFill),
        ];
    }
}
