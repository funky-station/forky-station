// SPDX-FileCopyrightText: 2020, 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Client;

namespace Content.Client
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ContentStart.Start(args);
        }
    }
}
