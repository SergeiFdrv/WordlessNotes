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
            TextOptions = new List<string>()
            {
                "Header 1", "Header 2", "Header 3", "Text", "List", "Image"
            };
            picker.ItemsSource = TextOptions;
            picker.SelectedItem = picker.Items[0];
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Header1));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Header2));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Header3));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count));
        }

        public List<string> TextOptions { get; }

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
        
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;
            note.DateTime = DateTime.UtcNow;
            await App.Database.SaveNoteAsync(note);
            await Navigation.PopAsync();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;
            await App.Database.DeleteNoteAsync(note);
            await Navigation.PopAsync();
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Selected != null) Selected.Type = (CustomViewTypes)((sender as Picker).SelectedIndex);
        }

        public async void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            Console.WriteLine("--- ELEMENT TOUCHED ---");
            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");
        }
    }
}

// TODO:
// Добавление элемента перед текущим (свайп, кнопка и т.п.)
// Прокручивать ScrollView кновому элементу
// Картинки
// Списки
// База данных
