using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoteLoadPage : ContentPage
    {
        public NoteLoadPage()
        {
            InitializeComponent();
        }

        public ObservableCollection<Models.Note> Items { get; private set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Items = new ObservableCollection<Models.Note>(App.Database.GetNotesAsync().Result);
            if (Items.Count == 0) Content = new Label
            {
                Text = Properties.Resources.NothingFound,
                VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center
            };
            else MyListView.ItemsSource = Items;
        }

        async void Delete_Clicked(object sender, EventArgs e)
        {
            if (await DisplayActionSheet("Delete?", null, null, "Yes", "No").ConfigureAwait(false) == "Yes")
            {
                Models.Note note = MyListView.SelectedItem as Models.Note;
                await App.Database.DeleteNoteAsync(note).ConfigureAwait(false);
                Items.Remove(note);
                if (System.IO.File.Exists(note.Path))
                    System.IO.File.Delete(note.Path);
                if (Items.Count > 0) Content = MyListView;
                else Content = new Label
                {
                    Text = Properties.Resources.NothingFound,
                    VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center
                };
                ToolbarItems.Clear();
            }
        }

        private async void Open_Clicked(object sender, EventArgs e)
        {
            Models.Note note = MyListView.SelectedItem as Models.Note;
            (Navigation.NavigationStack[0] as MainPage).Note = note;
            (Navigation.NavigationStack[0] as MainPage).TryPopulate(note);
            await Navigation.PopAsync().ConfigureAwait(false);
        }

        private void MyListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ToolbarItems.Add(new ToolbarItem { Text = Properties.Resources.Delete });
            ToolbarItems.Add(new ToolbarItem { Text = Properties.Resources.Open });
            ToolbarItems[0].Clicked += Delete_Clicked; ToolbarItems[1].Clicked += Open_Clicked;
        }
    }
}
