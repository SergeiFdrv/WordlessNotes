using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Models;

namespace Notes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoteSavePage : ContentPage
    {
        public NoteSavePage()
        {
            InitializeComponent();
        }

        public string Name
        {
            get => NameEntry.Text;
            set => NameEntry.Text = value;
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            Note note = new Note
            {
                Name = Name,
                Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Name + ".html"),
                DateTime = DateTime.UtcNow
            };
            File.WriteAllText(note.Path, (Navigation.NavigationStack.ElementAt(0) as MainPage).NoteContent);
            await App.Database.SaveNoteAsync(note);
            await DisplayAlert("Saved", "Note saved", "OK");
            (Navigation.NavigationStack.ElementAt(0) as MainPage).UnsavedData = false;
            (Navigation.NavigationStack.ElementAt(0) as MainPage).ToolbarItems[0].Text = Name;
            await Navigation.PopAsync();
        }
    }
}