using Content.Server.Station.Systems;
using Content.Shared._Funkystation.HandheldNewsReader;
using Content.Shared.MassMedia.Components;
using Content.Shared.MassMedia.Systems;

namespace Content.Server._Funkystation.HandheldNewsReader;

/// <summary>
/// newsreader, independent of the pda cartridge loader because it was driving me fucking insane
/// </summary>
public sealed partial class HandheldNewsReaderSystem : EntitySystem
{
    [Dependency] private SharedUserInterfaceSystem _ui = null!;
    [Dependency] private StationSystem _station = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HandheldNewsReaderComponent, BoundUIOpenedEvent>(OnUiOpened);
        SubscribeLocalEvent<HandheldNewsReaderComponent, HandheldNewsReaderUiMessage>(OnUiMessage);
    }

    private void OnUiOpened(Entity<HandheldNewsReaderComponent> ent, ref BoundUIOpenedEvent args)
    {
        UpdateUi(ent);
    }

    private void OnUiMessage(Entity<HandheldNewsReaderComponent> ent, ref HandheldNewsReaderUiMessage args)
    {
        if (!TryGetArticles(ent, out var articles) || articles.Count == 0)
            return;

        ent.Comp.ArticleNumber = args.Action switch
        {
            HandheldNewsReaderUiAction.Next => Math.Min(ent.Comp.ArticleNumber + 1, articles.Count - 1),
            HandheldNewsReaderUiAction.Prev => Math.Max(ent.Comp.ArticleNumber - 1, 0),
            _ => ent.Comp.ArticleNumber,
        };

        UpdateUi(ent);
    }

    private void UpdateUi(Entity<HandheldNewsReaderComponent> ent)
    {
        if (!TryGetArticles(ent, out var articles) || articles.Count == 0)
        {
            _ui.SetUiState(ent.Owner, HandheldNewsReaderUiKey.Key, new HandheldNewsReaderBoundUserInterfaceState(null, 0, 0));
            return;
        }

        ent.Comp.ArticleNumber = Math.Clamp(ent.Comp.ArticleNumber, 0, articles.Count - 1);

        var state = new HandheldNewsReaderBoundUserInterfaceState(articles[ent.Comp.ArticleNumber], ent.Comp.ArticleNumber + 1, articles.Count);
        _ui.SetUiState(ent.Owner, HandheldNewsReaderUiKey.Key, state);
    }

    private bool TryGetArticles(EntityUid uid, out List<NewsArticle> articles)
    {
        if (_station.GetOwningStation(uid) is not { } station || !TryComp<StationNewsComponent>(station, out var stationNews))
        {
            articles = new List<NewsArticle>();
            return false;
        }

        articles = stationNews.Articles;
        return true;
    }
}
