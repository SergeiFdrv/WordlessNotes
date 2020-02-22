using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Notes.Views;
using Notes.Models;
using Notes.Interfaces;
using Notes.Resources;

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
        }

        #region Override
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // CLEAR UNUSED IMAGES
            // Store image names from the page
            List<string> images = new List<string>();
            int i, j;
            for (i = 0; i < contentLayout.Children.Count; i++)
            {
                CustomView view = contentLayout.Children[i] as CustomView;
                if (view.Image != null) images.Add(view.Image.Name);
            }
            // Get image names from all notes
            var notes = App.Database.GetNotesAsync().Result; // Get all notes from the DB
            for (i = 0; i < notes.Count; i++)
            {
                if (!File.Exists(notes[i].Path)) continue;
                List<string> lines = File.ReadAllText(notes[i].Path).Split(new [] { "<br>" }, StringSplitOptions.None)
                    .Where(elname => elname.StartsWith("<img", StringComparison.OrdinalIgnoreCase)).ToList();
                for (j = 0; j < lines.Count; j++)
                {
                    images.Add(lines[j].Substring(14, lines[j].Length - 17));
                }
            }
            // Get all images from the DB the names of which are not in the list
            var allImages = App.Database.GetImagesAsync().Result.SkipWhile(img => images.Contains(img.Name)).ToArray();
            for (i = 0; i < allImages.Length; i++)
            {
                if (File.Exists(allImages[i].Path)) File.Delete(allImages[i].Path);
                App.Database.DeleteImageAsync(allImages[i]);
            }
        }
        #endregion

        #region Properties
        public List<string> DocumentItemOptions { get; } = new List<string> {
            Lang.Paragraph, $"{Lang.Header} 1", $"{Lang.Header} 2", $"{Lang.Header} 3", Lang.List, Lang.Image };

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
                if ((_SelectedView = value) == null) return;
                ViewTypePicker.SelectedIndex = (int)_SelectedView.ViewType;
                foreach (CustomView view in contentLayout.Children) view.BackgroundColor = Color.White;
                _SelectedView.BackgroundColor = Color.WhiteSmoke;
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
                NoteContent = File.ReadAllText(note.Path);
                Populate();
                ToolbarItems[0].Text = note.Name;
            }
            catch (FileNotFoundException)
            {
                await DisplayAlert(Lang.FileNotFound, null, "OK").ConfigureAwait(false);
            }
        }

        private void Populate()
        {
            contentLayout.Children.Clear();
            string[] lines = NoteContent.Split(new [] { "<br>" }, StringSplitOptions.None);
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
            string content = string.Empty;
            name = Note?.Name ?? string.Empty;
            CustomView view;
            for (int i = 0; i < contentLayout.Children.Count; i++)
            {
                view = contentLayout.Children[i] as CustomView;
                if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(view.Text)) name = view.Text;
                if (view.ViewType == CustomViewType.List) content += HTMLUnorderedList(view, ref i);
                else content += view.ToHTMLString();
            }
            return content;
        }

        string HTMLUnorderedList(CustomView view, ref int i)
        {
            string content = $"<ul><br><li>{view.Text}</li><br>";
            int j = i + 1;
            for (; j < contentLayout.Children.Count &&
                (contentLayout.Children[j] as CustomView).ViewType == CustomViewType.List; j++)
            {
                content += $"<li>{(contentLayout.Children[j] as CustomView).Text}</li><br>";
            }
            content += "</ul><br>";
            i = j - 1;
            return content;
        }
        #endregion

        #region ToolbarInteraction
        private void NoteNameTapped(object sender, EventArgs e)
        {
            (App.Current.MainPage as NavigationPage).BarBackgroundColor =
                Color.FromRgb(App.Random.Next(192), App.Random.Next(192), App.Random.Next(192));
        }

        void OnNewButtonClicked(object sender, EventArgs e)
        {
            Note = null;
            ToolbarItems[0].Text = Lang.NewNote;
            contentLayout.Children.Clear();
            AddElement();
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
            else
            if (await DisplayActionSheet(Lang.OverwriteNotePrompt, Lang.No, Lang.Yes).ConfigureAwait(true) == Lang.Yes)
            {
                Note.DateTime = DateTime.UtcNow;
                await App.Database.SaveNoteAsync(Note).ConfigureAwait(true);
                File.WriteAllText(Note.Path, NoteContent);
                UnsavedData = false;
                DependencyService.Get<IPlatformSpecific>().SayShort(Lang.NoteSaved);
            }
        }

        private void OnSaveAsButtonClicked(object sender, EventArgs e)
        {
            NoteContent = ContentParse(out string name);
            if (string.IsNullOrEmpty(name)) DependencyService.Get<IPlatformSpecific>().SayShort(Lang.CantDo);
            else
            {
                foreach (CustomView view in contentLayout.Children)
                    if (!string.IsNullOrEmpty(view.Text)) { name = view.Text; break; }
                OpenSavePage(name);
            }
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
            if (Note == null) { DependencyService.Get<IPlatformSpecific>().SayShort(Lang.CantDo); return; }
            if (await DisplayActionSheet(Lang.DeleteNotePrompt, Lang.No, Lang.Yes).ConfigureAwait(true) == Lang.Yes)
            {
                DeleteImagesAndNote(Note);
                await App.Database.DeleteNoteAsync(Note).ConfigureAwait(true);
                DependencyService.Get<IPlatformSpecific>().SayShort(Lang.NoteDeleted);
                Note = null;
            }
            UnsavedData = true;
        }
        void DeleteImagesAndNote(Note note) => NoteLoadPage.DeleteImagesAndNote(note);

        async void OnReopenButtonClicked(object sender, EventArgs e)
        {
            if (await DisplayActionSheet(Lang.ReloadPrompt, Lang.No, Lang.Yes).ConfigureAwait(true) == Lang.Yes && Note != null)
            { contentLayout.Children.Clear(); TryPopulate(Note); }
        }
        #endregion

        #region ElementInteraction
        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedView != null) SelectedView.ViewType = (CustomViewType)((sender as Picker).SelectedIndex);
        }
        private void AddElementClicked(object sender, EventArgs e)
        {
            AddElement(contentLayout.Children.Count, (CustomViewType)ViewTypePicker.SelectedIndex);
            (contentLayout.Children.Last() as CustomView).Focus();
        }

        private void ContentLayoutTapped(object sender, EventArgs e)
        {
            foreach (CustomView view in contentLayout.Children) view.BackgroundColor = Color.White;
            SelectedView = null;
        }
        #endregion
    }
}
