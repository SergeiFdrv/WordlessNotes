using System;
using System.Collections.Generic;
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

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            Note note = new Note
            {
                Name = NameEntry.Text,
                Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), NameEntry.Text + ".txt"),
                DateTime = DateTime.UtcNow
            };//(Note)BindingContext;
            await App.Database.SaveNoteAsync(note);
            await DisplayAlert("Saved", "Note saved", "OK");
            await Navigation.PopAsync();
        }
    }
}