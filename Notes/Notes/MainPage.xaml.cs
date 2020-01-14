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

        public bool UnsavedData
        {
            get => savebtn.IsEnabled;
            set
            {
                savebtn.IsEnabled = value;
            }
        }

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

        public string NoteContent { get; set; }

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
            NoteContent = string.Empty;
            for (int i = 0; i < contentLayout.Children.Count; i++)
            {
                CustomView view = contentLayout.Children[i] as CustomView;
                if (view.Type == CustomViewTypes.Header1)
                {
                    NoteContent += $"<h1>{view.Text}</h1><br>";
                }
                else if (view.Type == CustomViewTypes.Header2)
                {
                    NoteContent += $"<h2>{view.Text}</h2><br>";
                }
                else if (view.Type == CustomViewTypes.Header3)
                {
                    NoteContent += $"<h3>{view.Text}</h3><br>";
                }
                else if (view.Type == CustomViewTypes.Image)
                {
                    NoteContent += $"<img src=\"{(contentLayout.Children[i] as CustomView).ImgID}\"/><br><p class=\"imgdesc\">{view.Text}</p><br>";
                }
                else
                {
                    NoteContent += $"<p>{view.Text}</p><br>";
                    if (view.Type == CustomViewTypes.List)
                    {
                        NoteContent += "<ul><br>";
                        for (int j = 0; j < view.ListV.StackL.Children.Count; j++)
                        {
                            NoteContent += $"<li>{(view.ListV.StackL.Children[j] as CustomListViewCell).Text}</li><br>";
                        }
                        NoteContent += "</ul><br>";
                    }
                }
            }
            if (NoteContent == "")
            {
                UnsavedData = false;
                return;
            }
            NoteSavePage page = new NoteSavePage
            {
                Name = ToolbarItems[0].Text
            };
            NavigationPage.SetHasBackButton(this, true);
            await Navigation.PushAsync(page);
        }

        void OnNewButtonClicked(object sender, EventArgs e)
        {
            ToolbarItems[0].Text = "New note";
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

        public async void TryPopulate(Note note)
        {
            try
            {
                NoteContent = System.IO.File.ReadAllText(note.Path);
                Populate();
                ToolbarItems[0].Text = note.Name;
            }
            catch (System.IO.FileNotFoundException)
            {
                await DisplayAlert("Error", "File not found", "OK");
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", e.ToString(), "OK");
            }
        }

        private void Populate()
        {
            contentLayout.Children.Clear();
            string[] lines = NoteContent.Split(new string[] { "<br>" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("<h1>"))
                {
                    contentLayout.Children.Add(
                        new CustomView(contentLayout.Children.Count, CustomViewTypes.Header1)
                        {
                            Text = lines[i].Substring(4, lines[i].Length - 9)
                        });
                }
                else if (lines[i].StartsWith("<h2>"))
                {
                    contentLayout.Children.Add(
                        new CustomView(contentLayout.Children.Count, CustomViewTypes.Header2)
                        {
                            Text = lines[i].Substring(4, lines[i].Length - 9)
                        });
                }
                else if (lines[i].StartsWith("<h3>"))
                {
                    contentLayout.Children.Add(
                        new CustomView(contentLayout.Children.Count, CustomViewTypes.Header3)
                        {
                            Text = lines[i].Substring(4, lines[i].Length - 9)
                        });
                }
                else if (lines[i].StartsWith("<p>"))
                {
                    contentLayout.Children.Add(
                        new CustomView(contentLayout.Children.Count, CustomViewTypes.Paragraph)
                        {
                            Text = lines[i].Substring(3, lines[i].Length - 7)
                        });
                }
                else if (lines[i].StartsWith("<ul>"))
                {
                    (contentLayout.Children.Last() as CustomView).Type = CustomViewTypes.List;
                    List<string> listlines = new List<string>();
                    int j = i + 1;
                    for (; j < lines.Length && lines[j] != "</ul>"; j++)
                    {
                        listlines.Add(lines[j].Substring(4, lines[j].Length - 9));
                    }
                    (contentLayout.Children.Last() as CustomView).ListV.PopulateList(listlines);
                    i = j;
                }
                else if (lines[i].StartsWith("<img"))
                {
                    contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Image)
                    {
                        Text = lines[i + 1].Substring(19, lines[i + 1].Length - 23)
                    });
                    Models.Image image = App.Database.GetImageAsync(int.Parse(lines[i].Substring(10, lines[i].Length - 13))).Result;
                    ImageSource ImgSource = new ImageSourceConverter().ConvertFromInvariantString(image.Path) as ImageSource;
                    (contentLayout.Children.Last() as CustomView).ImageBox.Source = ImgSource;
                }
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            string rename = await DisplayPromptAsync("Rename", null);
            if (!string.IsNullOrEmpty(rename)) (sender as MenuItem).Text = rename;
        }
    }
}

// TODO:
// Добавление элемента перед текущим (свайп, кнопка и т.п.)
// Прокручивать ScrollView к новому элементу
// Картинки
// Списки
// База данных
