using Newtonsoft.Json;
using System.Collections.ObjectModel;

public static class Globals
{
    // list of all playlists
    public static ObservableCollection<PlayList> PlayLists { get; set; }
        = new ObservableCollection<PlayList>();

    // currently selected playlist
    public static PlayList SelectedPlayList { get; set; }

    // path to the json file
    private static string playListJsonFile = "";

    public static void LoadPlaylists()
    {
        string cacheDir = FileSystem.Current.CacheDirectory;
        playListJsonFile = $"{cacheDir}\\playlists.json";
        try
        {
            if (System.IO.File.Exists(playListJsonFile))
            {
                string json = System.IO.File.ReadAllText(playListJsonFile);
                var list = JsonConvert.DeserializeObject<List<PlayList>>(json);
                PlayLists = new ObservableCollection<PlayList>(list);
            }
        }
        catch (Exception ex)
        {

        }
    }

    public static void SavePlaylists()
    {
        if (playListJsonFile == "")
            LoadPlaylists();

        var list = PlayLists.ToList();
        var json = JsonConvert.SerializeObject(list);
        System.IO.File.WriteAllText(playListJsonFile, json);
    }
}