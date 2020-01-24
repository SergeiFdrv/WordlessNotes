using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Notes.Views;
using Notes.Models;
using Notes.Interfaces;

namespace Notes
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            picker.ItemsSource = DocumentItemOptions;
            picker.SelectedItem = picker.Items[0];
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Header1));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Header2));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.Header3));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count));
            contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.List));
        }

        #region Properties
        public List<string> DocumentItemOptions =>
            new List<string> { "Header 1", "Header 2", "Header 3", "Paragraph", "List", "Image" };

        public bool UnsavedData
        {
            get => savebtn.IsEnabled;
            set => savebtn.IsEnabled = value;
        }

        private CustomView SelectedView;

        public CustomView Selected
        {
            get => SelectedView;
            set
            {
                SelectedView = value;
                picker.SelectedIndex = (int)SelectedView.Type;
            }
        }

        public string NoteContent { get; set; }

        public Note Note { get; set; }
        #endregion

        #region AddingElement
        private void AddParagraphClicked(object sender, EventArgs e)
        {
            AddElement(contentLayout.Children.Count, CustomViewTypes.Paragraph);
        }

        private async void AddElementClicked(object sender, EventArgs e)
        {
            string res = await DisplayActionSheet(null, "Cancel", null, DocumentItemOptions.ToArray());
            if (!string.IsNullOrEmpty(res))
                AddElement(contentLayout.Children.Count, (CustomViewTypes)DocumentItemOptions.FindIndex(_ => _ == res));
        }

        public void AddElement(int index, CustomViewTypes type)
        {
            contentLayout.Children.Insert(index, new CustomView(index, type));
            IncrementIndicesFrom(index + 1);
            //(contentLayout.Children[index] as CustomView).Focus();
            UnsavedData = true;
        }

        public void AddElement(int index, CustomView view)
        {
            contentLayout.Children.Insert(index, view);
            IncrementIndicesFrom(index + 1);
            //(contentLayout.Children[index] as CustomView).Focus();
            UnsavedData = true;
        }

        private void IncrementIndicesFrom(int index)
        {
            for (int i = index; i < contentLayout.Children.Count; i++) (contentLayout.Children[i] as CustomView).Index++;
        }

        public void SelectAddedElement(object sender, EventArgs e)
        {
             if (Selected == null) (contentLayout.Children.Last() as CustomView).Focus();
        }
        #endregion
        #region DeletingElement
        public void DeleteElement(int index)
        {
            contentLayout.Children.Remove(contentLayout.Children.ElementAt(index));
            for (int i = index; i < contentLayout.Children.Count; i++)
            {
                (contentLayout.Children[i] as CustomView).Index--;
            }
            UnsavedData = true;
        }
        #endregion

        #region Loading
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
                    contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewTypes.List));
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
                    CustomView customView = new CustomView(contentLayout.Children.Count, CustomViewTypes.Image)
                    {
                        Text = lines[i + 1].Substring(19, lines[i + 1].Length - 23),
                        Image = App.Database.GetImageByNameAsync(lines[i].Substring(14, lines[i].Length - 17)).Result
                    };
                    customView.ImageBox.Source = ImageSource.FromFile(customView.Image.Path);
                    contentLayout.Children.Add(customView);
                }
            }
        }
        #endregion
        #region Saving
        private string ContentParse(out string name)
        {
            string content = name = string.Empty;
            if (Note != null) name = Note.Name; // TODO: при замене Export на Save As... удалить эту строку?
            for (int i = 0; i < contentLayout.Children.Count; i++)
            {
                CustomView view = contentLayout.Children[i] as CustomView;
                if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(view.Text))
                {
                    name = view.Text;
                }
                if (view.Type == CustomViewTypes.Header1)
                {
                    content += $"<h1>{view.Text}</h1><br>";
                }
                else if (view.Type == CustomViewTypes.Header2)
                {
                    content += $"<h2>{view.Text}</h2><br>";
                }
                else if (view.Type == CustomViewTypes.Header3)
                {
                    content += $"<h3>{view.Text}</h3><br>";
                }
                else if (view.Type == CustomViewTypes.Image)
                {
                    content += $"<img src=\"img/{(contentLayout.Children[i] as CustomView).Image.Name}\"/><br><p class=\"imgdesc\">{view.Text}</p><br>";
                }
                else if (view.Type == CustomViewTypes.List)
                {
                    content += "<ul><br>";
                    for (int j = 0; j < view.ListV.StackL.Children.Count; j++)
                    {
                        content += $"<li>{(view.ListV.StackL.Children[j] as CustomListViewCell).Text}</li><br>";
                    }
                    content += "</ul><br>";
                }
                else content += $"<p>{view.Text}</p><br>";
            }
            return content;
        }
        #endregion

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            (App.Current.MainPage as NavigationPage).BarBackgroundColor = Color.FromRgb(App.Random.Next(192), App.Random.Next(192), App.Random.Next(192));
        }

        #region HeaderMenuInteraction
        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Selected != null) Selected.Type = (CustomViewTypes)((sender as Picker).SelectedIndex);
        }

        void OnNewButtonClicked(object sender, EventArgs e)
        {
            Note = null;
            ToolbarItems[0].Text = "New note";
            contentLayout.Children.Clear();
        }

        async void OnOpenButtonClicked(object sender, EventArgs e)
        {
            NoteLoadPage page = new NoteLoadPage();
            NavigationPage.SetHasBackButton(this, true);
            await Navigation.PushAsync(page);
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            NoteContent = ContentParse(out string name);
            if (string.IsNullOrEmpty(NoteContent) || string.IsNullOrEmpty(name))
            {
                UnsavedData = false; return;
            }
            if (Note == null)
            {
                NoteSavePage page = new NoteSavePage { Name = name };
                NavigationPage.SetHasBackButton(this, true);
                await Navigation.PushAsync(page);
            }
            else if (App.Database.GetNoteAsync(Note.ID).Result != null &&
                await DisplayActionSheet("Do you want to overwrite the existing note?", null, null, "Yes", "No") == "Yes")
            {
                await App.Database.DeleteNoteAsync(Note);
                Note.DateTime = DateTime.UtcNow;
                await App.Database.SaveNoteAsync(Note);
                System.IO.File.WriteAllText(Note.Path, NoteContent);
                UnsavedData = false;
            }
        }

        private async void OnExportButtonClicked(object sender, EventArgs e)
        {
            NoteContent = ContentParse(out string name);
            if (string.IsNullOrEmpty(name)) return;//name = "Empty note";
            List<Models.Image> imgs = new List<Models.Image>();
            for (int i = 0; i < contentLayout.Children.Count; i++)
                if ((contentLayout.Children[i] as CustomView).Type == CustomViewTypes.Image)
                    imgs.Add((contentLayout.Children[i] as CustomView).Image);
            NoteSavePage page = new NoteSavePage
            {
                Name = name, Imgs = imgs
            };
            NavigationPage.SetHasBackButton(this, true);
            await Navigation.PushAsync(page);
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (Note == null) { DependencyService.Get<IPlatformSpecific>().SayShort("Can't do that"); return; }
            if (await DisplayActionSheet("Delete note?", null, null, "Yes", "No") == "Yes")
            {
                await App.Database.DeleteNoteAsync(Note);
                if (System.IO.File.Exists(Note.Path))
                    System.IO.File.Delete(Note.Path);
                DependencyService.Get<IPlatformSpecific>().SayShort("Note deleted");
                Note = null;
            }
            UnsavedData = true;
        }
        #endregion
    }
}

// TODO:
// Добавление элемента перед текущим (свайп, кнопка и т.п.)
// Прокручивать ScrollView к новому элементу
