using System;
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

        public ObservableCollection<string> Items { get; set; }

        // ОТОБРАЗИТЬ СПИСОК ЗАМЕТОК
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (App.Database.GetNotesAsync().Result.Count == 0) Content = new Label { Text = "Nothing found", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
            else MyListView.ItemsSource = await App.Database.GetNotesAsync();
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;

            //await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
