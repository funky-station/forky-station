using Content.Shared.MassMedia.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.HandheldNewsReader;

[Serializable, NetSerializable]
public sealed class HandheldNewsReaderBoundUserInterfaceState(NewsArticle? article, int targetNum, int totalNum)
    : BoundUserInterfaceState
{
    public NewsArticle? Article = article;
    public int TargetNum = targetNum;
    public int TotalNum = totalNum;
}
