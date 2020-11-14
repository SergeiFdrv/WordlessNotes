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
using System.Globalization;

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
            ViewTypePicker.ItemsSource = KindsOfCustomViews.Select(i => i.Item1).ToList();
            ViewTypePicker.SelectedIndex = 0;
            AddElement();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            string imgsString = string.Empty;
            var imgs = App.Database.GetImagesAsync().Result;
            DisplayAlert("Images", $"{imgs.Count}", "OK");
            foreach (var img in imgs)
            {
                imgsString += img.Name + '\n';
            }
            DisplayAlert("Images", imgsString, "OK");
        }

        #region Override
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // CLEAR UNUSED IMAGES
            // Store image names from the page
            List<string> images = new List<string>();
            foreach (CustomView view in ContentLayout.Children)
            {
                if (view is ImageView) images.Add((view as ImageView).Image?.Name);
            }
            // Get image names from all notes
            var notes = App.Database.GetNotesAsync().Result; // Get all notes from the DB
            int i = 0, j = 0;
            for (; i < notes.Count; i++)
            {
                if (!File.Exists(notes[i].Path)) continue;
                List<string> lines = File.ReadAllText(notes[i].Path).Split(new [] { "<br>" }, StringSplitOptions.None)
                    .Where(elname => elname.StartsWith("<img", StringComparison.OrdinalIgnoreCase)).ToList();
                for (; j < lines.Count; j++)
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
        public List<(string, Type)> KindsOfCustomViews { get; } = new List<(string, Type)>
        {
            (Lang.Paragraph, typeof(ParagraphView)),
            ($"{Lang.Header} 1", typeof(HeaderView)),
            ($"{Lang.Header} 2", typeof(HeaderView)),
            ($"{Lang.Header} 3", typeof(HeaderView)),
            (Lang.List, typeof(ListItemView)),
            (Lang.Image, typeof(ImageView))
        };

        public bool UnsavedData
        {
            get => ToolbarItems[3].IsEnabled;
            set => ToolbarItems[3].IsEnabled = value;
        }

        private CustomView _SelView;

        public CustomView SelView
        {
            get => _SelView;
            set
            {
                if ((_SelView = value) == null) return;
                int zzz = 0;
                if (value is HeaderView)
                {
                    zzz = KindsOfCustomViews.FindIndex(i =>
                        i.Item1 == Lang.Header + ' ' + (value as HeaderView).Level);
                }
                else if (value is ListItemView)
                {
                    zzz = KindsOfCustomViews.FindIndex(i => i.Item1 == Lang.List);
                }
                else if (value is ImageView)
                {
                    zzz = KindsOfCustomViews.FindIndex(i => i.Item1 == Lang.Image);
                }
                else zzz = KindsOfCustomViews.FindIndex(i => i.Item2 == typeof(ParagraphView));
                ViewTypePicker.SelectedIndex = zzz;
                foreach (CustomView view in ContentLayout.Children) view.BackgroundColor = Color.Transparent;
                _SelView.BackgroundColor = Color.WhiteSmoke;
            }
        }

        public string NoteContent { get; set; }

        public Note Note { get; set; }
        #endregion

        #region AddingElement
        public void AddElement(int index = -1, string text = "")
        {
            if (index < 0) index = ContentLayout.Children.Count;
            IncrementIndicesFrom(index);
            ContentLayout.Children.Insert(index, new ParagraphView(index, text));
        }

        public void InsertElement(CustomView view, int index = -1)
        {
            if (view is null) return;
            if (index < 0) index = ContentLayout.Children.Count;
            IncrementIndicesFrom(index);
            view.Index = index;
            ContentLayout.Children.Insert(index, view);
        }

        private void IncrementIndicesFrom(int index)
        {
            for (int i = index; i < ContentLayout.Children.Count; i++)
                (ContentLayout.Children[i] as CustomView).Index++;
            UnsavedData = true;
        }

        private CustomView CreateView(Type type)
        {
            if (type == typeof(HeaderView))
            {
                int l = KindsOfCustomViews[ViewTypePicker.SelectedIndex].Item1.LastIndexOf(' ');
                return new HeaderView(
                    byte.Parse(KindsOfCustomViews[ViewTypePicker.SelectedIndex].Item1
                        .Substring(l + 1), CultureInfo.InvariantCulture));
            }
            if (type == typeof(ListItemView)) return new ListItemView();
            if (type == typeof(ImageView)) return new ImageView();
            return new ParagraphView();
        }
        #endregion
        #region DeletingElement
        public void DeleteElement(int index)
        {
            ContentLayout.Children.Remove(ContentLayout.Children.ElementAt(index));
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
            ContentLayout.Children.Clear();
            string[] lines = NoteContent.Split(new [] { "<br>" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("<h1>", StringComparison.OrdinalIgnoreCase))
                {
                    ContentLayout.Children.Add(
                        new HeaderView(1, lines[i].Substring(4, lines[i].Length - 9))
                        {
                            Index = ContentLayout.Children.Count
                        });
                }
                else if (lines[i].StartsWith("<h2>", StringComparison.OrdinalIgnoreCase))
                {
                    ContentLayout.Children.Add(
                        new HeaderView(2, lines[i].Substring(4, lines[i].Length - 9))
                        {
                            Index = ContentLayout.Children.Count
                        });
                }
                else if (lines[i].StartsWith("<h3>", StringComparison.OrdinalIgnoreCase))
                {
                    ContentLayout.Children.Add(
                        new HeaderView(3, lines[i].Substring(4, lines[i].Length - 9))
                        {
                            Index = ContentLayout.Children.Count
                        });
                }
                else if (lines[i].StartsWith("<p>", StringComparison.OrdinalIgnoreCase))
                {
                    ContentLayout.Children.Add(
                        new ParagraphView(
                            ContentLayout.Children.Count,
                            lines[i].Substring(3, lines[i].Length - 7)));
                }
                else if (lines[i].StartsWith("<ul>", StringComparison.OrdinalIgnoreCase))
                {
                    int j = i + 1;
                    for (; j < lines.Length && lines[j] != "</ul>"; j++)
                    {
                        ContentLayout.Children.Add(new ListItemView
                        {
                            Index = ContentLayout.Children.Count,
                            Text = lines[j].Substring(4, lines[j].Length - 9)
                        });
                    }
                    i = j;
                }
                else if (lines[i].StartsWith("<img", StringComparison.OrdinalIgnoreCase))
                {
                    ImageView ImageView = new ImageView
                    {
                        Index = ContentLayout.Children.Count,
                        Text = lines[i + 1].Substring(19, lines[i + 1].Length - 23),
                        Image = App.Database.GetImageByNameAsync(lines[i].Substring(14, lines[i].Length - 17)).Result
                    };
                    ImageView.ImageBox.Source = ImageSource.FromFile(ImageView.Image.Path);
                    ContentLayout.Children.Add(ImageView);
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
            for (int i = 0; i < ContentLayout.Children.Count; i++)
            {
                view = ContentLayout.Children[i] as CustomView;
                if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(view.Text)) name = view.Text;
                if (view is ListItemView)
                {
                    List<ListItemView> list = new List<ListItemView>
                    {
                        view as ListItemView
                    };
                    for (int j = i + 1; j < ContentLayout.Children.Count &&
                        ContentLayout.Children[j] is ListItemView; j++)
                    {
                        list.Add(ContentLayout.Children[j] as ListItemView);
                    }
                    content += ListItemView.HTMLUnorderedList(list, ref i);
                }
                else content += view.ToHTMLString();
            }
            return content;
        }
        #endregion

        #region ToolbarInteraction
        private void NoteNameTapped(object sender, EventArgs e)
        {
            (Application.Current as App).SetBarBackground();
        }

        void OnNewButtonClicked(object sender, EventArgs e)
        {
            // TODO: Suggest to save if there is unsaved data
            Note = null;
            ToolbarItems[0].Text = Lang.NewNote;
            ContentLayout.Children.Clear();
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
                foreach (CustomView view in ContentLayout.Children)
                {
                    if (!string.IsNullOrEmpty(view.Text)) { name = view.Text; break; }
                }
                OpenSavePage(name);
            }
        }

        async void OpenSavePage(string notename)
        {
            List<Models.Image> imgs = new List<Models.Image>();
            for (int i = 0; i < ContentLayout.Children.Count; i++)
            {
                if (ContentLayout.Children[i] is ImageView)
                {
                    imgs.Add((ContentLayout.Children[i] as ImageView).Image);
                }
            }
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
        static void DeleteImagesAndNote(Note note) => NoteLoadPage.DeleteImagesAndNote(note);

        async void OnReopenButtonClicked(object sender, EventArgs e)
        {
            if (await DisplayActionSheet(Lang.ReloadPrompt, Lang.No, Lang.Yes).ConfigureAwait(true) == Lang.Yes && Note != null)
            { ContentLayout.Children.Clear(); TryPopulate(Note); }
        }
        #endregion

        #region ElementInteraction
        private void AddElementClicked(object sender, EventArgs e)
        {
            Type type = KindsOfCustomViews[ViewTypePicker.SelectedIndex].Item2;
            CustomView view = CreateView(type);
            InsertElement(view, (SelView is null ? -1 : SelView.Index) + 1);
        }

        private void ContentLayoutTapped(object sender, EventArgs e)
        {
            foreach (CustomView view in ContentLayout.Children) view.BackgroundColor = Color.Transparent;
            SelView = null;
        }

        private void ContentLayout_ChildRemoved(object sender, ElementEventArgs e)
        {
            int index = (e.Element as CustomView).Index;
            for (int i = index; i < ContentLayout.Children.Count; i++)
            {
                (ContentLayout.Children[i] as CustomView).Index--;
            }
            if (index == 0 || ContentLayout.Children.Count == 0)
            {
                SelView = null;
            }
            else
            {
                SelView = ContentLayout.Children[index - 1] as CustomView;
            }
        }

        private void ContentLayout_SizeChanged(object sender, EventArgs e)
        {
            foreach (CustomView view in ContentLayout.Children)
            {
                view.Resize();
            }
        }
        #endregion
    }
}
