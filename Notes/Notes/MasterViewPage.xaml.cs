using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Notes.Data;
using Notes.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;

namespace Notes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterViewPage : ContentPage
    {

        public MasterViewPage()
        {
            InitializeComponent();
            Title = "Title";
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;

            if (e.Item.ToString() == "Open")
            {
                NotePickPage page = new NotePickPage();
                NavigationPage.SetHasBackButton((Application.Current.MainPage as MasterDetailPage).Detail, true);
                await (Application.Current.MainPage as MasterDetailPage).Detail.Navigation.PushAsync(page);
                (Application.Current.MainPage as MasterDetailPage).IsPresented = false;
            }
            else
            {
                await DisplayAlert("Item Tapped", $"The {e.Item} item was tapped.", "OK");
            }

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
