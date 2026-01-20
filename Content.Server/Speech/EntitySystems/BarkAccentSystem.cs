// SPDX-FileCopyrightText: 2025 NoreUhh
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2023 Ed
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 metalgearsloth
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-License-Identifier: MIT

using Content.Shared.StatusEffectNew;
using Content.Server.Speech.Components;
using Content.Shared.Speech;
using Robust.Shared.Random;

namespace Content.Server.Speech.EntitySystems
{
    public sealed class BarkAccentSystem : EntitySystem
    {
        [Dependency] private readonly IRobustRandom _random = default!;

        private static readonly IReadOnlyList<string> Barks = new List<string>{
            " Woof!", " WOOF", " wof-wof"
        }.AsReadOnly();

        private static readonly IReadOnlyDictionary<string, string> SpecialWords = new Dictionary<string, string>()
        {
            { "ah", "arf" },
            { "Ah", "Arf" },
            { "oh", "oof" },
            { "Oh", "Oof" },
        };

        public override void Initialize()
        {
            SubscribeLocalEvent<BarkAccentComponent, AccentGetEvent>(OnAccent);
            SubscribeLocalEvent<BarkAccentComponent, StatusEffectRelayedEvent<AccentGetEvent>>(OnAccentRelayed);
        }

        public string Accentuate(string message)
        {
            foreach (var (word, repl) in SpecialWords)
            {
                message = message.Replace(word, repl);
            }

            return message.Replace("!", _random.Pick(Barks))
                .Replace("l", "r").Replace("L", "R");
        }

        private void OnAccent(Entity<BarkAccentComponent> entity, ref AccentGetEvent args)
        {
            args.Message = Accentuate(args.Message);
        }

        private void OnAccentRelayed(Entity<BarkAccentComponent> entity, ref StatusEffectRelayedEvent<AccentGetEvent> args)
        {
            args.Args.Message = Accentuate(args.Args.Message);
        }
    }
}
