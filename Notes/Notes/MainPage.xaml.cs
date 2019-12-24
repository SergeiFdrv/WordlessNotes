using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Notes.Views;
using Notes.Models;

namespace Notes
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        { /* TODO: РЕАЛИЗОВАТЬ СОХРАНЕНИЕ ДАННЫХ ПРИ СВОРАЧИВАНИИ (в app.xaml.cs) */
            InitializeComponent();
            DocumentItemOptions = new List<string>()
            {
                "Header 1", "Header 2", "Header 3", "Text", "List", "Image"
            };
            picker.ItemsSource = DocumentItemOptions;
            picker.SelectedItem = picker.Items[0];
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Header1));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Header2));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Header3));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.List));
        }

        public List<string> DocumentItemOptions { get; }

        public bool Unsaved { get; set; } = true;

        private CustomView SelectedView;

        public CustomView Selected
        {
            get => SelectedView;
            set
            {
                SelectedView = value;
                Console.WriteLine((int)SelectedView.Type);
                picker.SelectedIndex = (int)SelectedView.Type;
            }
        }

        public string NoteContent { get; set; } // ?

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count));
        }

        public void SelectElement(object sender, EventArgs e)
        {
             if (Selected == null) (contentLayout.Children.Last() as CustomView).Focus();
        }

        public void DelEl(int index)
        {
            contentLayout.Children.Remove(contentLayout.Children.ElementAt(index));
            for (int i = index; i < contentLayout.Children.Count; i++)
            {
                (contentLayout.Children[i] as CustomView).Index--;
            }
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Selected != null) Selected.Type = (CustomViewTypes)((sender as Picker).SelectedIndex);
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            NoteSavePage page = new NoteSavePage();
            NavigationPage.SetHasBackButton(this, true);
            await Navigation.PushAsync(page);
        }

        void OnNewButtonClicked(object sender, EventArgs e)
        {
            return;
        }

        async void OnOpenButtonClicked(object sender, EventArgs e)
        {
            NotePickPage page = new NotePickPage();
            NavigationPage.SetHasBackButton(this, true);
            await Navigation.PushAsync(page);
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var p = await DisplayActionSheet("Delete note?", null, null, "Yes", "No");
            if (p == "Yes") await DisplayAlert(p, "Note deleted", "Got it");//OnDeleteButtonClicked(sender, e);
            else await DisplayAlert(p, "Deleting canceled", "OK");
        }
    }
}

// TODO:
// Добавление элемента перед текущим (свайп, кнопка и т.п.)
// Прокручивать ScrollView к новому элементу
// Картинки
// Списки
// База данных
