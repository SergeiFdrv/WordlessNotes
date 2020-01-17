﻿using System;
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
            if (await DisplayActionSheet("Delete?", null, null, "Yes", "No") == "Yes")
            {
                Models.Note note = MyListView.SelectedItem as Models.Note;
                await App.Database.DeleteNoteAsync(note);
                Items.Remove(note);
                if (System.IO.File.Exists(note.Path))
                    System.IO.File.Delete(note.Path);
                if (Items.Count > 0) Content = MyListView;
                else Content = new Label { Text = "Nothing found", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
                ToolbarItems.Clear();
            }
        }

        private async void Open_Clicked(object sender, EventArgs e)
        {
            Models.Note note = MyListView.SelectedItem as Models.Note;
            (Navigation.NavigationStack[0] as MainPage).TryPopulate(note);
            await Navigation.PopAsync();
        }

        private void MyListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ToolbarItems.Add(new ToolbarItem { Text = "Delete" });
            ToolbarItems.Add(new ToolbarItem { Text = "Open" });
            ToolbarItems[0].Clicked += Delete_Clicked; ToolbarItems[1].Clicked += Open_Clicked;
        }
    }
}