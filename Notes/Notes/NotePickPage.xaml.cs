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
        public ObservableCollection<string> Items { get; set; }

        public NotePickPage()
        {
            InitializeComponent();
            
            /*Items = new ObservableCollection<string>
            {
                "Item 1",
                "Item 2",
                "Item 3",
                "Item 4",
                "Item 5"
            };*/

            //MyListView.ItemsSource = Items;
        }

        // ОТОБРАЗИТЬ СПИСОК ЗАМЕТОК
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            MyListView.ItemsSource = await App.Database.GetNotesAsync();
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
