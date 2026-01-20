// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2025 Nikovnik
// SPDX-FileCopyrightText: 2025 Ignaz "Ian" Kraft
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2025 DrSmugleaf
// SPDX-FileCopyrightText: 2025 Orsoniks
// SPDX-FileCopyrightText: 2025 pathetic meowmeow
// SPDX-FileCopyrightText: 2025 Zachary Higgs
// SPDX-FileCopyrightText: 2024 Łukasz Lindert
// SPDX-FileCopyrightText: 2023 themias
// SPDX-FileCopyrightText: 2024 themias
// SPDX-FileCopyrightText: 2024 Cojoke
// SPDX-FileCopyrightText: 2024 beck-thompson
// SPDX-FileCopyrightText: 2024 SlamBamActionman
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 0x6273
// SPDX-FileCopyrightText: 2024 LordCarve
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Emisse
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Vordenburg
// SPDX-FileCopyrightText: 2023 Phill101
// SPDX-FileCopyrightText: 2023 LankLTE
// SPDX-FileCopyrightText: 2023 Whisper
// SPDX-FileCopyrightText: 2023 Ilushkins33
// SPDX-FileCopyrightText: 2023 faint
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2022 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Jezithyr
// SPDX-FileCopyrightText: 2023 Jezithyr
// SPDX-FileCopyrightText: 2022 Will Robson
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 EmoGarbage404
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Forensics;

namespace Content.Server.Body.Systems;

public sealed class BloodstreamSystem : SharedBloodstreamSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BloodstreamComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<BloodstreamComponent, GenerateDnaEvent>(OnDnaGenerated);
    }

    // not sure if we can move this to shared or not
    // it would certainly help if SolutionContainer was documented
    // but since we usually don't add the component dynamically to entities we can keep this unpredicted for now
    private void OnComponentInit(Entity<BloodstreamComponent> entity, ref ComponentInit args)
    {
        if (!SolutionContainer.EnsureSolution(entity.Owner,
                entity.Comp.BloodSolutionName,
                out var bloodSolution) ||
            !SolutionContainer.EnsureSolution(entity.Owner,
                entity.Comp.BloodTemporarySolutionName,
                out var tempSolution))
            return;

        bloodSolution.MaxVolume = entity.Comp.BloodReferenceSolution.Volume * entity.Comp.MaxVolumeModifier;
        tempSolution.MaxVolume = entity.Comp.BleedPuddleThreshold * 4; // give some leeway, for chemstream as well
        entity.Comp.BloodReferenceSolution.SetReagentData(GetEntityBloodData((entity, entity.Comp)));

        // Fill blood solution with BLOOD
        // The DNA string might not be initialized yet, but the reagent data gets updated in the GenerateDnaEvent subscription
        var solution = entity.Comp.BloodReferenceSolution.Clone();
        solution.ScaleTo(entity.Comp.BloodReferenceSolution.Volume - bloodSolution.Volume);
        bloodSolution.AddSolution(solution, PrototypeManager);
    }

    // forensics is not predicted yet
    private void OnDnaGenerated(Entity<BloodstreamComponent> entity, ref GenerateDnaEvent args)
    {
        if (SolutionContainer.ResolveSolution(entity.Owner, entity.Comp.BloodSolutionName, ref entity.Comp.BloodSolution, out var bloodSolution))
        {
            var data = NewEntityBloodData(entity);
            entity.Comp.BloodReferenceSolution.SetReagentData(data);

            foreach (var reagent in bloodSolution.Contents)
            {
                List<ReagentData> reagentData = reagent.Reagent.EnsureReagentData();
                reagentData.RemoveAll(x => x is DnaData);
                reagentData.AddRange(data);
            }
        }
        else
            Log.Error("Unable to set bloodstream DNA, solution entity could not be resolved");
    }
}
