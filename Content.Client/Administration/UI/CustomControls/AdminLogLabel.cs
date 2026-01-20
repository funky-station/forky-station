// SPDX-FileCopyrightText: 2025 B_Kirill
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Southbridge
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-License-Identifier: MIT

using Content.Shared.Administration.Logs;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI.CustomControls;

public sealed class AdminLogLabel : RichTextLabel
{
    public AdminLogLabel(ref SharedAdminLog log, HSeparator separator)
    {
        Log = log;
        Separator = separator;

        SetMessage($"{log.Date:HH:mm:ss}: {log.Message}");
        OnVisibilityChanged += VisibilityChanged;
    }

    public new SharedAdminLog Log { get; }

    public HSeparator Separator { get; }

    private void VisibilityChanged(Control control)
    {
        Separator.Visible = Visible;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        OnVisibilityChanged -= VisibilityChanged;
    }
}
