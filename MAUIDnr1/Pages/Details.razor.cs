namespace MAUIDnr1.Pages
{
    public partial class Details : ComponentBase
    {
        [Inject]
        protected ApiService _apiService { get; set; }

        [Inject]
        protected NavigationManager _navigationManager { get; set; }

        [Inject]
        private IAudioManager _audioManager { get; set; }

        [Parameter]
        public string ShowNumber { get; set; }

        protected Show ThisShow { get; set; }

        protected IAudioPlayer? player = null;       // media player
        protected FileStream? stream = null;         // stream used for playing
        protected MediaState mediaState;
        protected string url = "";
        protected string AudioMessage = "";         // Downloading, Playing, Paused or error
        protected double Percentage = 0;            // percentage of audio played used to set progress bar value
        protected string ProgressPercent = "";      // formatted percentage string (not shown in this demo)
        protected string PlayPosition = "";         // calculated from current position
        protected string ControlsOpacity = ".5";    // .5 for 'disabled' 
        protected string PlayOpacity = "1";         // 1 for 'enabled'
        protected string playButtonClass = "imageButton";   // image reacts when pressed
        protected string pauseButtonClass = "";     // see SetState to see these classes in action
        protected string stopButtonClass = "";
        protected string rewindButtonClass = "";
        protected string forwardButtonClass = "";
        private System.Timers.Timer? timer = null;   // Used to report current position

        /// <summary>
        /// Change UI depending on the state
        /// </summary>
        /// <param name="state"></param>
        private void SetState(MediaState state)
        {
            mediaState = state;
            if (state == MediaState.Playing)
            {
                ControlsOpacity = "1";
                PlayOpacity = ".5";
                AudioMessage = "Playing";
                playButtonClass = "";
                pauseButtonClass = "imageButton";
                stopButtonClass = "imageButton";
                rewindButtonClass = "imageButton";
                forwardButtonClass = "imageButton";
            }
            else if (state == MediaState.Paused || state == MediaState.Stopped)
            {
                ControlsOpacity = ".5";
                PlayOpacity = "1";
                playButtonClass = "imageButton";
                pauseButtonClass = "";
                stopButtonClass = "";
                rewindButtonClass = "";
                forwardButtonClass = "";
                if (state == MediaState.Stopped)
                {
                    Percentage = 0;
                    AudioMessage = "";
                    PlayPosition = "";
                }
                else
                {
                    AudioMessage = "Paused";
                }
            }
        }

        public async Task NavigateHome()
        {
            await Cleanup();
            _navigationManager.NavigateTo("/");
        }

        /// <summary>
        /// Called from the Play button
        /// </summary>
        /// <returns></returns>
        protected async Task PlayAudio()
        {
            // ignore if we're already playing
            if (mediaState == MediaState.Playing) return;

            // are we paused?
            if (mediaState == MediaState.Paused && player != null)
            {
                // yes. Continue playing
                player.Play();
                SetState(MediaState.Playing);
                return;
            }

            // exit if there is no url specified
            if (string.IsNullOrEmpty(url))
            {
                AudioMessage = "Please enter a URL to an MP3 file";
                return;
            }

            // here we go!
            try
            {
                // This is where we are storing local audio files
                string cacheDir = FileSystem.Current.CacheDirectory;

                // get the fully qualified path to the local file
                var fileName = url.Substring(8).Replace("/", "-");
                var localFile = $"{cacheDir}\\{fileName}";

                // download if need be
                if (!System.IO.File.Exists(localFile))
                {
                    // let the user know we're trying to download
                    AudioMessage = "Downloading...";
                    await InvokeAsync(StateHasChanged);

                    // this code downloads the file from the URL
                    using (var client = new HttpClient())
                    {
                        var uri = new Uri(url);
                        var response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            var fileInfo = new FileInfo(localFile);
                            using (var fileStream = fileInfo.OpenWrite())
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                        }
                    }
                }

                // File exists now. Read it
                stream = System.IO.File.OpenRead(localFile);

                // create the audio player
                player = _audioManager.CreatePlayer(stream);

                // handle the PlaybackEnded event
                player.PlaybackEnded += Player_PlaybackEnded;

                // create a timer to report progress
                timer = new System.Timers.Timer(50);

                // handle the Elapsed event
                timer.Elapsed += async (state, args) =>
                {
                    // calculate the percentage complete
                    Percentage = (player.CurrentPosition * 100) / player.Duration;

                    // Not used, but if you want to show the percent completed...
                    ProgressPercent = Percentage.ToString("N2") + "%";

                    // calculate the PlayPosition string to report "current time / total time"
                    var tsCurrent = TimeSpan.FromSeconds(player.CurrentPosition);
                    var tsTotal = TimeSpan.FromSeconds(player.Duration);
                    var durationString = $"{tsTotal.Minutes.ToString("D2")}:{tsTotal.Seconds.ToString("D2")}";
                    var currentString = $"{tsCurrent.Minutes.ToString("D2")}:{tsCurrent.Seconds.ToString("D2")}";
                    PlayPosition = $"{currentString} / {durationString}";

                    // update the UI
                    await InvokeAsync(StateHasChanged);
                };

                // start the timer
                timer.Start();
                // start playing
                player.Play();
                // configure the UI for playing
                SetState(MediaState.Playing);
                // update the UI
                await InvokeAsync(StateHasChanged);

            }
            catch (Exception e)
            {
                AudioMessage = "An error occurred. Please try again later.";
            }
        }

        /// <summary>
        /// Skip forward 10 seconds
        /// </summary>
        protected void Forward()
        {
            if (mediaState == MediaState.Playing)
            {
                var pos = player.CurrentPosition + 10;
                if (pos < player.Duration)
                    player.Seek(pos);
                else
                    StopAudio();
            }
        }

        /// <summary>
        /// Stop
        /// </summary>
        protected void StopAudio()
        {
            if (mediaState == MediaState.Playing)
            {
                player.Stop();
                SetState(MediaState.Stopped);
            }
        }

        /// <summary>
        /// Pause
        /// </summary>
        protected void PauseAudio()
        {
            if (mediaState == MediaState.Playing)
            {
                player.Pause();
                SetState(MediaState.Paused);
            }
        }

        /// <summary>
        /// Rewind 10 seconds (or to the beginning)
        /// </summary>
        protected void Rewind()
        {
            if (mediaState == MediaState.Playing)
            {
                var pos = player.CurrentPosition - 10;
                if (pos < 0)
                    pos = 0;
                player.Seek(pos);
            }
        }

        private async Task Cleanup()
        {
            // Update the UI
            SetState(MediaState.Stopped);
            // dispose the stream
            stream?.Dispose();
            // stop and dispose the timer
            timer?.Stop();
            timer?.Dispose();
            // unhook this event 
            if (player != null)
            {
                player.PlaybackEnded -= Player_PlaybackEnded;
                // dispose the player
                player.Dispose();
            }
            // update the ui
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Tear down everything when playback ends
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void Player_PlaybackEnded(object sender, EventArgs e)
        {
            await Cleanup();
        }

        protected override async Task OnInitializedAsync()
        {
            if (ShowNumber != null)
            {
                try
                {
                    int showNumber = Convert.ToInt32(ShowNumber);
                    ThisShow = await _apiService.GetShowWithDetails(showNumber);
                    if (ThisShow != null)
                        url = ThisShow.ShowDetails.File.Url;
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}