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
            ViewTypePicker.ItemsSource = DocumentItemOptions;
            ViewTypePicker.SelectedIndex = 0;
            AddElement();
            //contentLayout.Children.Add(new CustomView(contentLayout.Children.Count));
        }

        #region Properties
        public List<string> DocumentItemOptions { get; } =
            new List<string> { "Paragraph", "Header 1", "Header 2", "Header 3", "List", "Image" };

        public bool UnsavedData
        {
            get => ToolbarItems[3].IsEnabled;
            set => ToolbarItems[3].IsEnabled = value;
        }

        private CustomView _SelectedView;

        public CustomView SelectedView
        {
            get => _SelectedView;
            set
            {
                if (value == null) return;
                _SelectedView = value;
                ViewTypePicker.SelectedIndex = (int)_SelectedView.ViewType;
            }
        }

        public string NoteContent { get; set; }

        public Note Note { get; set; }
        #endregion

        #region AddingElement

        public void AddElement(int index = -1, CustomViewType type = CustomViewType.Paragraph, string text = "")
        {
            if (index < 0) index = contentLayout.Children.Count;
            IncrementIndicesFrom(index);
            contentLayout.Children.Insert(index, new CustomView(index, type, text));
            if (SelectedView == null) (contentLayout.Children[index] as CustomView).Focus();
        }

        private void IncrementIndicesFrom(int index)
        {
            for (int i = index; i < contentLayout.Children.Count; i++) (contentLayout.Children[i] as CustomView).Index++;
            UnsavedData = true;
        }

        public void SelectAddedElement(object sender, EventArgs e) // TODO: Выявить назначение или удалить
        {
             if (SelectedView == null) (contentLayout.Children.Last() as CustomView).Focus();
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
                if (note == null) return;
                NoteContent = System.IO.File.ReadAllText(note.Path);
                Populate();
                ToolbarItems[0].Text = note.Name;
            }
            catch (System.IO.FileNotFoundException)
            {
                await DisplayAlert(Properties.Resources.FileNotFound, null, "OK").ConfigureAwait(false);
            }
        }

        private void Populate()
        {
            contentLayout.Children.Clear();
            string[] lines = NoteContent.Split(new string[] { "<br>" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("<h1>", StringComparison.OrdinalIgnoreCase))
                {
                    contentLayout.Children.Add(
                        new CustomView(contentLayout.Children.Count, CustomViewType.Header1)
                        {
                            Text = lines[i].Substring(4, lines[i].Length - 9)
                        });
                }
                else if (lines[i].StartsWith("<h2>", StringComparison.OrdinalIgnoreCase))
                {
                    contentLayout.Children.Add(
                        new CustomView(contentLayout.Children.Count, CustomViewType.Header2)
                        {
                            Text = lines[i].Substring(4, lines[i].Length - 9)
                        });
                }
                else if (lines[i].StartsWith("<h3>", StringComparison.OrdinalIgnoreCase))
                {
                    contentLayout.Children.Add(
                        new CustomView(contentLayout.Children.Count, CustomViewType.Header3)
                        {
                            Text = lines[i].Substring(4, lines[i].Length - 9)
                        });
                }
                else if (lines[i].StartsWith("<p>", StringComparison.OrdinalIgnoreCase))
                {
                    contentLayout.Children.Add(
                        new CustomView(contentLayout.Children.Count, CustomViewType.Paragraph)
                        {
                            Text = lines[i].Substring(3, lines[i].Length - 7)
                        });
                }
                else if (lines[i].StartsWith("<ul>", StringComparison.OrdinalIgnoreCase))
                {
                    int j = i + 1;
                    for (; j < lines.Length && lines[j] != "</ul>"; j++)
                    {
                        contentLayout.Children.Add(new CustomView(contentLayout.Children.Count, CustomViewType.List)
                        {
                            Text = lines[j].Substring(4, lines[j].Length - 9)
                        });
                    }
                    i = j;
                }
                else if (lines[i].StartsWith("<img", StringComparison.OrdinalIgnoreCase))
                {
                    CustomView customView = new CustomView(contentLayout.Children.Count, CustomViewType.Image)
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
            if (Note != null) name = Note.Name;
            CustomView view;
            for (int i = 0; i < contentLayout.Children.Count; i++)
            {
                view = contentLayout.Children[i] as CustomView;
                if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(view.Text))
                {
                    name = view.Text;
                }
                if (view.ViewType == CustomViewType.Header1)
                {
                    content += $"<h1>{view.Text}</h1><br>";
                }
                else if (view.ViewType == CustomViewType.Header2)
                {
                    content += $"<h2>{view.Text}</h2><br>";
                }
                else if (view.ViewType == CustomViewType.Header3)
                {
                    content += $"<h3>{view.Text}</h3><br>";
                }
                else if (view.ViewType == CustomViewType.Image && (contentLayout.Children[i] as CustomView).Image != null)
                {
                    content += $"<img src=\"img/{(contentLayout.Children[i] as CustomView).Image.Name}\"/><br><p class=\"imgdesc\">{view.Text}</p><br>";
                }
                else if (view.ViewType == CustomViewType.List)
                {
                    content += $"<ul><br><li>{view.Text}</li><br>";
                    int j = i + 1;
                    for (; j < contentLayout.Children.Count &&
                        (contentLayout.Children[j] as CustomView).ViewType == CustomViewType.List; j++)
                    {
                        content += $"<li>{(contentLayout.Children[j] as CustomView).Text}</li><br>";
                    }
                    content += "</ul><br>";
                    i = j - 1;
                }
                else if (view.ViewType == CustomViewType.Paragraph) content += $"<p>{view.Text}</p><br>";
            }
            return content;
        }
        #endregion

        #region ToolbarInteraction
        private void NoteNameTapped(object sender, EventArgs e)
        {
            (App.Current.MainPage as NavigationPage).BarBackgroundColor = Color.FromRgb(App.Random.Next(192), App.Random.Next(192), App.Random.Next(192));
        }

        void OnNewButtonClicked(object sender, EventArgs e)
        {
            Note = null;
            ToolbarItems[0].Text = Properties.Resources.NewNote;
            contentLayout.Children.Clear();
        }

        async void OnOpenButtonClicked(object sender, EventArgs e)
        {
            NoteLoadPage page = new NoteLoadPage();
            NavigationPage.SetHasBackButton(this, true);
            await Navigation.PushAsync(page).ConfigureAwait(false);
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            NoteContent = ContentParse(out string name);
            if (string.IsNullOrEmpty(NoteContent) || string.IsNullOrEmpty(name))
            {
                UnsavedData = false; return;
            }
            if (Note == null || App.Database.GetNoteAsync(Note.ID).Result == null) OpenSavePage(name);
            else if (App.Database.GetNoteAsync(Note.ID).Result != null && await DisplayActionSheet(
                "Do you want to overwrite the existing note?", "No", "Yes").ConfigureAwait(true) == "Yes")
            {
                Note.DateTime = DateTime.UtcNow;
                await App.Database.SaveNoteAsync(Note).ConfigureAwait(true);
                System.IO.File.WriteAllText(Note.Path, NoteContent);
                UnsavedData = false;
                DependencyService.Get<IPlatformSpecific>().SayShort(Properties.Resources.NoteSaved);
            }
        }

        private void OnSaveAsButtonClicked(object sender, EventArgs e)
        {
            NoteContent = ContentParse(out string name);
            if (string.IsNullOrEmpty(name))
                DependencyService.Get<IPlatformSpecific>().SayShort(Properties.Resources.CantDo);
            else OpenSavePage(name);
        }

        async void OpenSavePage(string notename)
        {
            List<Models.Image> imgs = new List<Models.Image>();
            for (int i = 0; i < contentLayout.Children.Count; i++)
                if ((contentLayout.Children[i] as CustomView).ViewType == CustomViewType.Image)
                    imgs.Add((contentLayout.Children[i] as CustomView).Image);
            NoteSavePage page = new NoteSavePage(notename, imgs);
            NavigationPage.SetHasBackButton(this, true);
            await Navigation.PushAsync(page).ConfigureAwait(true);
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (Note == null) { DependencyService.Get<IPlatformSpecific>().SayShort(Properties.Resources.CantDo); return; }
            if (await DisplayActionSheet("Delete note?", "No", "Yes").ConfigureAwait(true) == "Yes")
            {
                DeleteImagesAndNote(Note);
                await App.Database.DeleteNoteAsync(Note).ConfigureAwait(true);
                DependencyService.Get<IPlatformSpecific>().SayShort(Properties.Resources.NoteDeleted);
                Note = null;
            }
            UnsavedData = true;
        }

        void DeleteImagesAndNote(Note note) => NoteLoadPage.DeleteImagesAndNote(note);
        #endregion

        #region AddElementInteraction
        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedView != null) SelectedView.ViewType = (CustomViewType)((sender as Picker).SelectedIndex);
        }
        private void AddElementClicked(object sender, EventArgs e)
        {
            AddElement(contentLayout.Children.Count, (CustomViewType)ViewTypePicker.SelectedIndex);
            (contentLayout.Children.Last() as CustomView).Focus();
        }
        #endregion
    }
}

