using Microsoft.JSInterop;
using System.Collections.ObjectModel;
namespace MAUIDnr1.Pages;
public partial class Index : ComponentBase
{
    
    [Inject]
    protected NavigationManager _navigationManager { get; set; }

    [Inject]
    protected ApiService _apiService { get; set; }

    public ObservableCollection<Show> AllShows { get; set; } = new ObservableCollection<Show>();

    protected List<int> ShowNumbers { get; set; } = new List<int>();
    protected int RecordsToRead { get; set; } = 20;
    protected int LastShowNumber { get; set; }

    private string episodeFilter = "";
    public string EpisodeFilter
    {
        get
        {
            return episodeFilter;
        }
        set
        {
            episodeFilter = value;
            LastShowNumber = 0;
            try
            {
                var t = Task.Run(() => GetNextBatchOfShows());
                t.Wait();
                StateHasChanged();
            }
            catch { }
        }
    }

    public string PlayListButtonText
    {
        get
        {
            string value = "Playlists";
            if (Globals.SelectedPlayList != null)
            {
                value += $" ({Globals.SelectedPlayList.Name})";
            }
            return value;
        }
    }

    public void ResetFilter()
    {
        ShowNumbers.Clear();
        AllShows.Clear();
        EpisodeFilter = "";
    }

    public async Task GetNextBatchOfFilteredShows()
    {
        var nextBatch = await
            _apiService.GetFilteredShows(EpisodeFilter, AllShows.Count, RecordsToRead);
        if (nextBatch == null || nextBatch.Count == 0) return;
        foreach (var show in nextBatch)
        {
            AllShows.Add(show);
        }
    }

    public async Task GetNextBatchOfShows()
    {
        if (EpisodeFilter != "")
        {
            AllShows.Clear();
            await GetNextBatchOfFilteredShows();
            return;
        }

        if (ShowNumbers.Count == 0)
        {
            ShowNumbers = await _apiService.GetShowNumbers();
            if (ShowNumbers == null || ShowNumbers.Count == 0) return;
            LastShowNumber = ShowNumbers.First<int>() + 1;
        }

        var request = new GetByShowNumbersRequest()
        {
            ShowName = "dotnetrocks",
            Indexes = (from x in ShowNumbers where x < LastShowNumber && x >= (LastShowNumber - RecordsToRead) select x).ToList()
        };

        var nextBatch = await _apiService.GetByShowNumbers(request);
        if (nextBatch == null || nextBatch.Count == 0) return;

        foreach (var show in nextBatch)
        {
            AllShows.Add(show);
        }

        LastShowNumber = nextBatch.Last<Show>().ShowNumber;
    }

    public void NavigateToDetailPage(int ShowNumber)
    {
        var url = $"details/{ShowNumber}";
        _navigationManager.NavigateTo(url);
    }

    public void NavigateToPlayListPage()
    {
        _navigationManager.NavigateTo("playlists");
    }

    protected override async Task OnInitializedAsync()
    {
        Globals.LoadPlaylists();
        await GetNextBatchOfShows();
    }
}