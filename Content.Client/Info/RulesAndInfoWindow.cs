// SPDX-FileCopyrightText: 2025 Brandon Li
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Jezithyr
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Kevin Zheng
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2022 Visne
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 ike709
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2021 Swept
// SPDX-License-Identifier: MIT

using System.Numerics;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.ContentPack;

namespace Content.Client.Info
{
    public sealed class RulesAndInfoWindow : DefaultWindow
    {
        [Dependency] private readonly IResourceManager _resourceManager = default!;

        public RulesAndInfoWindow()
        {
            IoCManager.InjectDependencies(this);

            Title = Loc.GetString("ui-info-title");

            var rootContainer = new TabContainer();

            var rulesList = new RulesControl
            {
                Margin = new Thickness(10)
            };
            var tutorialList = new Info
            {
                Margin = new Thickness(10)
            };

            rootContainer.AddChild(rulesList);
            rootContainer.AddChild(tutorialList);

            TabContainer.SetTabTitle(rulesList, Loc.GetString("ui-info-tab-rules"));
            TabContainer.SetTabTitle(tutorialList, Loc.GetString("ui-info-tab-tutorial"));

            PopulateTutorial(tutorialList);

            ContentsContainer.AddChild(rootContainer);

            SetSize = new Vector2(650, 650);
        }

        private void PopulateTutorial(Info tutorialList)
        {
            AddSection(tutorialList, Loc.GetString("ui-info-header-intro"), "Intro.txt");
            var infoControlSection = new InfoControlsSection();
            tutorialList.InfoContainer.AddChild(infoControlSection);
            AddSection(tutorialList, Loc.GetString("ui-info-header-gameplay"), "Gameplay.txt", true);
            AddSection(tutorialList, Loc.GetString("ui-info-header-sandbox"), "Sandbox.txt", true);

            infoControlSection.ControlsButton.OnPressed += _ => UserInterfaceManager.GetUIController<OptionsUIController>().OpenWindow();
        }

        private static void AddSection(Info info, Control control)
        {
            info.InfoContainer.AddChild(control);
        }

        private void AddSection(Info info, string title, string path, bool markup = false)
        {
            AddSection(info, MakeSection(title, path, markup, _resourceManager));
        }

        private static Control MakeSection(string title, string path, bool markup, IResourceManager res)
        {
            return new InfoSection(title, res.ContentFileReadAllText($"/ServerInfo/{path}"), markup);
        }

    }
}
