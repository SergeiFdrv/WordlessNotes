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
    public partial class NotePickPage : ContentPage
    {
        public NotePickPage()
        {
            InitializeComponent();
        }

        public ObservableCollection<Models.Note> Items { get; set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Items = new ObservableCollection<Models.Note>(App.Database.GetNotesAsync().Result);
            if (Items.Count == 0) Content = new Label { Text = "Nothing found", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
            else MyListView.ItemsSource = Items;
        }

        public async void Delete_Clicked(object sender, EventArgs e)
        {
            if (MyListView.SelectedItem == null) return;
            string responce = await DisplayActionSheet("Delete?", null, null, "Yes", "No");
            if (responce == "Yes")
            {
                Models.Note note = MyListView.SelectedItem as Models.Note;
                await App.Database.DeleteNoteAsync(note);
                Items.Remove(note);
                if (System.IO.File.Exists(note.Path))
                    System.IO.File.Delete(note.Path);
                Content = new Label { Text = "Nothing found", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
            }
        }

        private async void Open_Clicked(object sender, EventArgs e)
        {
            if (MyListView.SelectedItem == null) return;
            Models.Note note = MyListView.SelectedItem as Models.Note;
            (Navigation.NavigationStack[0] as MainPage).TryPopulate(note);
            await Navigation.PopAsync();
        }
    }
}
