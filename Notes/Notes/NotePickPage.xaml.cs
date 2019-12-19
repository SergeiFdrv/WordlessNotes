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

        public List<Models.Note> Items { get; set; }

        // ОТОБРАЗИТЬ СПИСОК ЗАМЕТОК
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Items = App.Database.GetNotesAsync().Result;
            if (Items.Count == 0) Content = new Label { Text = "Nothing found", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
            else MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;

            string responce = await DisplayActionSheet("Delete?", null, null, "Yes", "No");
            if (responce == "Yes")
            {
                Items.RemoveAt(e.ItemIndex);
                //await Navigation.PopAsync();
            }
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
