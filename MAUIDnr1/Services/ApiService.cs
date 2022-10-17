using System.Text;
using Newtonsoft.Json;

namespace MAUIDnr1.Services;

public class ApiService
{
    private HttpClient httpClient; //
    private string ShowName = "dotnetrocks";
    string baseUrl = "https://pwopclientapi.azurewebsites.net/shows/";

    public ApiService()
    {
        httpClient = new HttpClient() { BaseAddress = new Uri(baseUrl) };
    }

    /// <summary>
    /// returns True if we are connected to the Internet
    /// </summary>
    /// <returns></returns>
    public bool IsOnline()
    {
        return (Connectivity.Current.NetworkAccess == NetworkAccess.Internet);
    }

    /// <summary>
    /// Download all show metadata minus the details
    /// </summary>
    /// <returns></returns>
    public async Task<List<Show>> GetAllShows()
    {
        var fileName = $"{FileSystem.Current.CacheDirectory}\\allShows.json";
        try
        {
            if (IsOnline())
            {
                // We have an Internet connection. Download as usual

                string Url = ShowName;
                var result = await httpClient.GetAsync(Url);
                result.EnsureSuccessStatusCode();
                var response = await result.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<Show>>(response);

                // write the list to allShows.json in the cache folder
                System.IO.File.WriteAllText(fileName, response);

                return list;
            }
            else if (System.IO.File.Exists(fileName))
            {
                // We are offline, and allShows.json has been written to.
                // Return the items in the saved list
                var json = System.IO.File.ReadAllText(fileName);
                var list = JsonConvert.DeserializeObject<List<Show>>(json);
                return list;
            }
            else
            {
                // we are offline and allShows.json does not exist.
                return new List<Show>();
            }
        }
        catch (Exception ex)
        {
            return new List<Show>();
        }
    }

    /// <summary>
    /// Returns a list of shows in a range where the ShowTitle contains the specified text.
    /// </summary>
    /// <param name="ShowTitle"></param>
    /// <param name="StartIndex"></param>
    /// <param name="Count"></param>
    /// <returns></returns>
    public async Task<List<Show>> GetFilteredShows(string ShowTitle,
                                                   int StartIndex, int Count)
    {
        try
        {
            if (IsOnline())
            {
                // We have an Internet connection. Download as usual
                string Url =
                    $"{ShowName}/{ShowTitle}/{StartIndex}/{Count}/getfilteredshows";
                var result = await httpClient.GetAsync(Url);
                result.EnsureSuccessStatusCode();
                var response = await result.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Show>>(response);
                // Save each show to it's own Json file
                foreach (var item in items)
                {
                    var fileName = $"{FileSystem.Current.CacheDirectory}\\show-{item.ShowNumber}.json";
                    var showJson = JsonConvert.SerializeObject(item);
                    System.IO.File.WriteAllText(fileName, showJson);
                }
                return items;
            }
            else
            {
                // We are offline. Check for the existence of each show's json file
                var list = new List<Show>();
                int lastIndex = StartIndex + Count;
                for (int index = StartIndex; index < lastIndex; index++)
                {
                    var fileName =
                        $"{FileSystem.Current.CacheDirectory}\\show-{index}.json";
                    if (System.IO.File.Exists(fileName))
                    {
                        // The file exists. Deserialize it to a Show
                        var showJson = System.IO.File.ReadAllText(fileName);
                        var item = JsonConvert.DeserializeObject<Show>(showJson);
                        // Only return it if the ShowTitle matches
                        if (item.ShowTitle.ToLower().Contains(ShowTitle.ToLower()))
                            list.Add(item);
                    }
                }
                return list;
            }
        }
        catch (Exception ex)
        {
            return new List<Show>();
        }
    }

    public async Task<int> GetCount()
    {
        var fileName = $"{FileSystem.Current.CacheDirectory}\\showsCount.json";
        try
        {
            if (IsOnline())
            {
                // We have an Internet connection. Download as usual
                string Url = $"{ShowName}/getcount";
                var result = await httpClient.GetAsync(Url);
                result.EnsureSuccessStatusCode();
                var response = await result.Content.ReadAsStringAsync();
                // Save the count to showsCount.json
                System.IO.File.WriteAllText(fileName, response);
                return Convert.ToInt32(response);
            }
            else if (System.IO.File.Exists(fileName))
            {
                // Read and return the value from our offline file
                var countString = System.IO.File.ReadAllText(fileName);
                return Convert.ToInt32(countString);
            }
            else
                return 0;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    /// <summary>
    /// Loads a given show with the guests, links, and mp3 file
    /// </summary>
    /// <param name="ShowNumber"></param>
    /// <returns></returns>
    public async Task<Show> GetShowWithDetails(int ShowNumber)
    {
        var fileName = $"{FileSystem.Current.CacheDirectory}\\show-{ShowNumber}.json";
        try
        {
            if (IsOnline())
            {
                // We have an Internet connection. Download as usual
                string Url = $"{ShowName}/{ShowNumber}/getwithdetails";
                var result = await httpClient.GetAsync(Url);
                result.EnsureSuccessStatusCode();
                var response = await result.Content.ReadAsStringAsync();
                var show = JsonConvert.DeserializeObject<Show>(response);

                // save the json file offline
                System.IO.File.WriteAllText(fileName, response);

                return show;
            }
            else if (System.IO.File.Exists(fileName))
            {
                // We are offline and the json file exists. Load it and return
                var json = System.IO.File.ReadAllText(fileName);
                var show = JsonConvert.DeserializeObject<Show>(json);
                // return null if there are no details
                if (show.ShowDetails == null)
                    return null;
                else
                    return show;
            }
            else
            {
                return new Show();
            }
        }
        catch (Exception ex)
        {
            return new Show();
        }
    }

    /// <summary>
    /// Returns the entire list of show numbers
    /// </summary>
    /// <returns></returns>
    public async Task<List<int>> GetShowNumbers()
    {
        var fileName = $"{FileSystem.Current.CacheDirectory}\\showNumbers.json";

        try
        {
            if (IsOnline())
            {
                // We have an Internet connection. Download as usual
                string Url = $"{ShowName}/getshownumbers";
                var result = await httpClient.GetAsync(Url);
                result.EnsureSuccessStatusCode();
                var response = await result.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<int>>(response);

                // Save the json file offline
                System.IO.File.WriteAllText(fileName, response);

                return list;
            }
            else if (System.IO.File.Exists(fileName))
            {
                // We are offline and the json file exists. Load it and return the list
                var json = System.IO.File.ReadAllText(fileName);
                var list = JsonConvert.DeserializeObject<List<int>>(json);
                return list;
            }
            else
            {
                return new List<int>();
            }
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            return new List<int>();
        }
    }

    /// <summary>
    /// Returns a batch of shows given a list of shownumbers (and the show name)
    /// </summary>
    /// <param name="Request"></param>
    /// <returns></returns>
    public async Task<List<Show>> GetByShowNumbers(GetByShowNumbersRequest Request)
    {
        try
        {
            if (IsOnline())
            {
                // We have an Internet connection. Download as usual
                var http = new HttpClient();
                var url = baseUrl;
                string json = JsonConvert.SerializeObject(Request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await http.PostAsync(url, content);
                result.EnsureSuccessStatusCode();
                string responseBody = await result.Content.ReadAsStringAsync();
                var shows = JsonConvert.DeserializeObject<List<Show>>(responseBody);

                // Save each show to its own json file
                foreach (var show in shows)
                {
                    var fileName = $"{FileSystem.Current.CacheDirectory}\\show-{show.ShowNumber}.json";
                    var showJson = JsonConvert.SerializeObject(show);
                    System.IO.File.WriteAllText(fileName, showJson);
                }
                return shows;
            }
            else
            {
                // We are offline. Look for each file offline
                var list = new List<Show>();
                foreach (var index in Request.Indexes)
                {
                    var fileName =
                        $"{FileSystem.Current.CacheDirectory}\\show-{index}.json";
                    if (System.IO.File.Exists(fileName))
                    {
                        // The json file is there. Load it and include it in our list
                        var showJson = System.IO.File.ReadAllText(fileName);
                        var item = JsonConvert.DeserializeObject<Show>(showJson);
                        list.Add(item);
                    }
                }
                return list;
            }
        }
        catch (Exception ex)
        {
            return new List<Show>();
        }
    }
}