// SPDX-FileCopyrightText: 2024 Winkarst
// SPDX-FileCopyrightText: 2024 Julian Giebel
// SPDX-FileCopyrightText: 2023 faint
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Robust.Client.UserInterface.Controls;
using Robust.Shared.Utility;

namespace Content.Client.Message;

public static class RichTextLabelExt
{


     /// <summary>
     /// Sets the labels markup.
     /// </summary>
     /// <remarks>
     /// Invalid markup will cause exceptions to be thrown. Don't use this for user input!
     /// </remarks>
    public static RichTextLabel SetMarkup(this RichTextLabel label, string markup)
    {
        label.SetMessage(FormattedMessage.FromMarkupOrThrow(markup));
        return label;
    }

     /// <summary>
     /// Sets the labels markup.<br/>
     /// Uses <c>FormatedMessage.FromMarkupPermissive</c> which treats invalid markup as text.
     /// </summary>
    public static RichTextLabel SetMarkupPermissive(this RichTextLabel label, string markup)
    {
        label.SetMessage(FormattedMessage.FromMarkupPermissive(markup));
        return label;
    }
}
