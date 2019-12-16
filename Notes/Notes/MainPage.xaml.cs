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
        public List<string> TextOptions { get; }

        //public int SelectedIndex { get; set; }

        public CustomView Selected { get; set; }

        public MainPage()
        { /* TODO: РЕАЛИЗОВАТЬ СОХРАНЕНИЕ ДАННЫХ ПРИ СВОРАЧИВАНИИ (в app.xaml.cs) */
            InitializeComponent();
            TextOptions = new List<string>()
            {
                "Header 1", "Header 2", "Header 3", "Text"
            };
            picker.ItemsSource = TextOptions;
            picker.SelectedItem = "Text";
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Header1));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Header2));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Header3));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count));
            Selected = contentLayout.Children.Last() as CustomView;
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Image));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count));
            Selected = contentLayout.Children.Last() as CustomView;
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

        private void OpenMaster_Clicked(object sender, EventArgs e)
        {
            (Parent as MasterDetailPage).IsPresented = true;
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            //contentLayout.Children.OfType<CustomView>().Where((view) => view.Index == Selected.Index).FirstOrDefault().Type = (CustomViewTypes)(sender as Picker).SelectedIndex;
        }
    }
}
