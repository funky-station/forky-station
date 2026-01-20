// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-License-Identifier: MIT


namespace Content.Server.Voting
{
    public delegate void VoteFinishedEventHandler(IVoteHandle sender, VoteFinishedEventArgs args);
    public delegate void VoteCancelledEventHandler(IVoteHandle sender);
}
