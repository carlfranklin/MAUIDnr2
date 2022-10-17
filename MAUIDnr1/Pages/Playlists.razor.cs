using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MAUIDnr1.Pages
{
    public partial class Playlists : ComponentBase
    {
        [Inject]
        protected NavigationManager _navigationManager { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        // Are we adding, editing, or neither?
        protected PlaylistEditAction PlaylistEditAction { get; set; }

        // Used to disable the command buttons if we're adding or editing
        protected bool CommandButtonsDisabled => (PlaylistEditAction != PlaylistEditAction.None);

        // This is the PlayList object we use to add or edit
        protected PlayList PlayListToAddOrEdit;

        /// <summary>
        /// Go back
        /// </summary>
        protected void NavigateHome()
        {
            _navigationManager.NavigateTo("/");
        }

        /// <summary>
        /// Set the selected playlist when selected from the <select> element
        /// </summary>
        /// <param name="args"></param>
        protected void PlayListSelected(ChangeEventArgs args)
        {
            Globals.SelectedPlayList = (from x in Globals.PlayLists where x.Id.ToString() == args.Value.ToString() select x).FirstOrDefault();
        }

        /// <summary>
        /// Because PlayListSelected won't fire when there is only one item in the list
        /// </summary>
        protected void PlayListsClicked()
        {
            if (Globals.PlayLists.Count == 1)
            {
                Globals.SelectedPlayList = Globals.PlayLists.First();
            }
        }

        /// <summary>
        /// Add a PlayList
        /// </summary>
        protected async Task AddButtonClicked()
        {
            // Create a new PlayList
            PlayListToAddOrEdit = new PlayList();
            PlayListToAddOrEdit.Id = PlayList.CreateGuid(); // don't forget this!
            PlayListToAddOrEdit.DateCreated = DateTime.Now;
            PlaylistEditAction = PlaylistEditAction.Adding;
            await JSRuntime.InvokeVoidAsync("SetFocus", "InputName");
        }

        /// <summary>
        /// Edit the SelectedPlayList
        /// </summary>
        protected async Task EditButtonClicked()
        {
            // Clone it, so we don't clobber it accidentally.
            PlayListToAddOrEdit = (PlayList)Globals.SelectedPlayList.Clone();
            PlaylistEditAction = PlaylistEditAction.Editing;
            await JSRuntime.InvokeVoidAsync("SetFocus", "InputName");
        }

        /// <summary>
        /// Easy Peasy
        /// </summary>
        protected void DeleteButtonClicked()
        {
            Globals.PlayLists.Remove(Globals.SelectedPlayList);
            Globals.SavePlaylists();
            Globals.SelectedPlayList = null;
            PlaylistEditAction = PlaylistEditAction.None;
        }

        /// <summary>
        /// Commit the Add or Edit action
        /// </summary>
        protected void SubmitPlayListClicked()
        {
            if (PlaylistEditAction == PlaylistEditAction.Adding)
            {
                // Simply add the new PlayList.
                Globals.PlayLists.Add(PlayListToAddOrEdit);
                // Select it
                int index = Globals.PlayLists.IndexOf(PlayListToAddOrEdit);
                Globals.SelectedPlayList = Globals.PlayLists[index];
            }
            else if (PlaylistEditAction == PlaylistEditAction.Editing)
            {
                // Get the index of the selected play list
                int index = Globals.PlayLists.IndexOf(Globals.SelectedPlayList);
                // Replace it in the list
                Globals.PlayLists[index] = PlayListToAddOrEdit;
                // Get the new object reference
                Globals.SelectedPlayList = Globals.PlayLists[index];
            }
            // Save the data!
            Globals.SavePlaylists();
            PlaylistEditAction = PlaylistEditAction.None;
        }

        /// <summary>
        /// Easy Peasy
        /// </summary>
        protected void CancelButtonPressed()
        {
            PlayListToAddOrEdit = null;
            PlaylistEditAction = PlaylistEditAction.None;
        }
    }
}
