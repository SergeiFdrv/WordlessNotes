using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Resources;

namespace Notes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoteLoadPage : ContentPage
    {
        public NoteLoadPage()
        {
            InitializeComponent();
            SearchEntry.FontSize = NotFoundLabel.FontSize = App.FontSize;
        }

        public ObservableCollection<Models.Note> Items { get; private set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Items = new ObservableCollection<Models.Note>(App.Database.GetNotesAsync().Result);
            RefreshNoteList();
        }

        #region Deleting
        async void Delete_Clicked(object sender, EventArgs e)
        {
            if (await DisplayActionSheet(Lang.DeleteNotePrompt, Lang.No, Lang.Yes).ConfigureAwait(true) == Lang.Yes)
            {
                Models.Note note = MyListView.SelectedItem as Models.Note;
                DeleteImagesAndNote(note);
                await App.Database.DeleteNoteAsync(note).ConfigureAwait(true);
                Items.Remove(note);
                RefreshNoteList();
                ToolbarItems[0].Clicked -= Delete_Clicked; ToolbarItems[1].Clicked -= Open_Clicked;
            }
        }

        void RefreshNoteList()
        {
            if (MyListView.IsVisible = !(NotFoundLabel.IsVisible = Items.Count == 0)) MyListView.ItemsSource = Items;
        }

        public static void DeleteImagesAndNote(Models.Note note)
        {
            if (note != null && File.Exists(note.Path))
            {
                string[] lines = File.ReadAllText(note.Path).Split(new string[] { "<br>" }, StringSplitOptions.None);
                string imgpath;
                for (int i = 0; i < lines.Length; i++)
                    if (lines[i].StartsWith("<img", StringComparison.OrdinalIgnoreCase))
                    {
                        imgpath = Path.Combine(note.Path, lines[i].Substring(19, lines[i + 1].Length - 23));
                        if (File.Exists(imgpath))
                            File.Delete(imgpath);
                    }
                File.Delete(note.Path);
            }
        }
        #endregion

        private async void Open_Clicked(object sender, EventArgs e)
        {
            Models.Note note = MyListView.SelectedItem as Models.Note;
            (Navigation.NavigationStack[0] as MainPage).Note = note;
            (Navigation.NavigationStack[0] as MainPage).TryPopulate(note);
            await Navigation.PopAsync().ConfigureAwait(false);
        }

        private void MyListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ToolbarItems[0].Text = Lang.Delete; ToolbarItems[1].Text = Lang.Open;
            ToolbarItems[0].Clicked += Delete_Clicked; ToolbarItems[1].Clicked += Open_Clicked;
        }

        private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var I = SearchEntry.Text.Length == 0 ? Items : Items.Where(note => note.Name.Contains(SearchEntry.Text));
            MyListView.ItemsSource = I;
            NotFoundLabel.IsVisible = !(MyListView.IsVisible = I.Any());
        }
    }
}
