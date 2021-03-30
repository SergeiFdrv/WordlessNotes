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
            ViewTypePicker.ItemsSource = CustomViewTypes.Select(i => i.TypeName).ToList();
            ViewTypePicker.SelectedIndex = HeaderPicker.SelectedIndex = 0;
            InsertElement(new ParagraphView());
        }

        #region Override
#if DEBUG
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var notes = App.Database.GetNotesAsync().Result;
            foreach (var note in notes)
            {
                DisplayAlert(note.Name, note.Path, "OK");
            }
            StringBuilder imgsString = new StringBuilder();
            var imgs = App.Database.GetImagesAsync().Result;
            imgsString.AppendLine(imgs.Count.ToString());
            foreach (var img in imgs)
            {
                imgsString.AppendLine(img.Name);
                imgsString.AppendLine(img.Path);
                imgsString.AppendLine();
            }
            DisplayAlert("Images", imgsString.ToString(), "OK");
        }
#endif

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            /*// CLEAR UNUSED IMAGES
            int i = 0, j = 0;
            // Store image names from the page
            var pageImages = ContentLayout.Children.OfType<ImageView>()
                .Where(img => img.Image != null);
            var imageNames = new List<string>(pageImages.Count());
            for (; i < imageNames.Capacity; i++)
            {
                imageNames.Add(pageImages.ElementAt(i).Image.Name);
            }
            // Get image names from all notes
            var notes = App.Database.GetNotesAsync().Result; // Get all notes from the DB
            for (i = 0; i < notes.Count; i++)
            {
                if (!File.Exists(notes[i].Path)) continue;
                List<string> lines = File.ReadAllText(notes[i].Path)
                    .Split(new [] { "<br>" }, StringSplitOptions.None)
                    .Where(elname => elname.StartsWith(
                        "<img", StringComparison.OrdinalIgnoreCase)).ToList();
                for (; j < lines.Count; j++)
                {
                    imageNames.Add(lines[j].Substring(14, lines[j].Length - 17));
                }
            }
            // Get all images from the DB the names of which are not in the list
            var allDBImages = App.Database.GetImagesAsync().Result
                .SkipWhile(img => imageNames.Contains(img.Name)).ToArray();
            for (i = 0; i < allDBImages.Length; i++)
            {
                ref Models.Image image = ref allDBImages[i];
                if (File.Exists(image.Path)) File.Delete(image.Path);
                App.Database.DeleteImageAsync(image);
            }*/
        }
        #endregion

        #region Properties
        public List<(string TypeName, Type Type)> CustomViewTypes { get; }
            = new List<(string, Type)>
        {
            (Lang.Paragraph, typeof(ParagraphView)),
            (Lang.Header, typeof(HeaderView)),
            (Lang.List, typeof(ListItemView)),
            (Lang.Image, typeof(ImageView))
        };

        private CustomView ViewToAdd { get; set; }

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
                Type type = _SelectedView.GetType();
                ViewTypePicker.SelectedIndex
                    = CustomViewTypes.FindIndex(i => i.Type == type);
                foreach (CustomView view in ContentLayout.Children)
                {
                    view.BackgroundColor = Color.Transparent;
                }
                _SelectedView.BackgroundColor = Color.WhiteSmoke;
            }
        }

        public string NoteContent { get; set; }

        public Note Note { get; set; }
        #endregion

        #region AddingElement
        public void InsertElement(CustomView view, CustomView prevView = null)
        {
            if (view is null) return;
            if (prevView is null)
            {
                ContentLayout.Children.Add(view);
            }
            else
            {
                int index = ContentLayout.Children.IndexOf(prevView) + 1;
                ContentLayout.Children.Insert(index, view);
            }
        }

        private CustomView CreateView(Type type)
        {
            CustomView res;
            if (type == typeof(HeaderView))
            {
                res = new HeaderView(byte.Parse(
                    HeaderPicker.SelectedItem.ToString(), CultureInfo.InvariantCulture));
            }
            else res = type.GetConstructor(Array.Empty<Type>())
                .Invoke(Array.Empty<object>()) as CustomView;
            return res;
        }
        #endregion
        #region DeletingElement
        public void DeleteElement(Guid id)
        {
            ContentLayout.Children.Remove(
                ContentLayout.Children.FirstOrDefault(i => i.Id == id));
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
                if (note.Name.Length > 25)
                {
                    ToolbarItems[0].Text = note.Name.Substring(0, 25);
                    if (ToolbarItems[0].Text.Contains(' '))
                    {
                        ToolbarItems[0].Text = ToolbarItems[0].Text
                            .Substring(0, ToolbarItems[0].Text.LastIndexOf(' '));
                    }
                    ToolbarItems[0].Text += "...";
                }
                else ToolbarItems[0].Text = note.Name;
            }
            catch (FileNotFoundException)
            {
                await DisplayAlert(Lang.FileNotFound, null, "OK").ConfigureAwait(false);
            }
#if DEBUG
            catch (Exception x)
            {
                await DisplayAlert(x.Message, x.StackTrace, "OK").ConfigureAwait(false);
            }
#endif
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
                        new HeaderView(1, lines[i].Substring(4, lines[i].Length - 9)));
                }
                else if (lines[i].StartsWith("<h2>", StringComparison.OrdinalIgnoreCase))
                {
                    ContentLayout.Children.Add(
                        new HeaderView(2, lines[i].Substring(4, lines[i].Length - 9)));
                }
                else if (lines[i].StartsWith("<h3>", StringComparison.OrdinalIgnoreCase))
                {
                    ContentLayout.Children.Add(
                        new HeaderView(3, lines[i].Substring(4, lines[i].Length - 9)));
                }
                else if (lines[i].StartsWith("<p>", StringComparison.OrdinalIgnoreCase))
                {
                    ContentLayout.Children.Add(
                        new ParagraphView(lines[i].Substring(3, lines[i].Length - 7)));
                }
                else if (lines[i].StartsWith("<ul>", StringComparison.OrdinalIgnoreCase))
                {
                    int j = i + 1;
                    for (; j < lines.Length && lines[j] != "</ul>"; j++)
                    {
                        ref string line = ref lines[j];
                        ContentLayout.Children.Add(new ListItemView
                        {
                            Text = line.Substring(4, line.Length - 9)
                        });
                    }
                    i = j;
                }
                else if (lines[i].StartsWith("<img", StringComparison.OrdinalIgnoreCase))
                {
                    ImageView ImageView = new ImageView
                    {
                        Text = lines[i + 1].Substring(19, lines[i + 1].Length - 23),
                        Image = App.Database.GetImageByNameAsync(
                            lines[i].Substring(14, lines[i].Length - 17)).Result
                    };
                    ImageView.ImageBox.Source = ImageSource.FromFile(ImageView.Image.Path);
                    ContentLayout.Children.Add(ImageView);
                }
            }
        }
        #endregion
        #region Saving
        /// <summary>
        /// Parses the note content into a string
        /// and generates a note name based on the first line
        /// </summary>
        /// <param name="name">The generated note name</param>
        /// <returns>The note content as a string</returns>
        private string ContentParse(out string name)
        {
            string content = string.Empty;
            name = Note?.Name ?? string.Empty;
            CustomView view;
            bool isList = false;
            for (int i = 0; i < ContentLayout.Children.Count; i++)
            {
                view = ContentLayout.Children[i] as CustomView;
                if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(view.Text))
                    name = view.Text;
                if (view is ListItemView && !isList)
                {
                    content += "<ul><br>";
                    isList = true;
                }
                else if (isList && !(view is ListItemView))
                {
                    content += "</ul><br>";
                    isList = false;
                }
                content += view.ToHTMLString();
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
            InsertElement(new ParagraphView());
        }

        private async void OnOpenButtonClicked(object sender, EventArgs e)
        {
            NoteLoadPage page = new NoteLoadPage();
            NavigationPage.SetHasBackButton(this, true);
            await Navigation.PushAsync(page).ConfigureAwait(false);
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            NoteContent = ContentParse(out string name);
            if (string.IsNullOrEmpty(NoteContent) || string.IsNullOrEmpty(name))
            {
                UnsavedData = false;
            }
            else if (Note == null || App.Database.GetNoteAsync(Note.ID).Result == null)
                OpenSavePage(name);
            else if (await DisplayActionSheet(Lang.OverwriteNotePrompt, Lang.No, Lang.Yes)
                .ConfigureAwait(true) == Lang.Yes)
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
            if (string.IsNullOrEmpty(name))
                DependencyService.Get<IPlatformSpecific>().SayShort(Lang.CantDo);
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

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (Note == null)
            {
                DependencyService.Get<IPlatformSpecific>().SayShort(Lang.CantDo);
            }
            else if (await DisplayActionSheet(Lang.DeleteNotePrompt, Lang.No, Lang.Yes)
                .ConfigureAwait(true) == Lang.Yes)
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
            if (await DisplayActionSheet(Lang.ReloadPrompt, Lang.No, Lang.Yes)
                .ConfigureAwait(true) == Lang.Yes && Note != null)
            {
                ContentLayout.Children.Clear(); TryPopulate(Note);
            }
        }
        #endregion

        #region ElementInteraction
        private void ViewTypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Type type = CustomViewTypes[ViewTypePicker.SelectedIndex].Type;
            HeaderPicker.IsVisible = type == typeof(HeaderView);
            ViewToAdd = CreateView(type);
        }

        private void HeaderPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ViewToAdd is HeaderView headerView)
            {
                headerView.Level = byte.Parse(
                    HeaderPicker.SelectedItem.ToString(), CultureInfo.InvariantCulture);
            }
        }

        private void AddElementClicked(object sender, EventArgs e)
        {
            InsertElement(ViewToAdd, SelectedView);
            Type type = CustomViewTypes[ViewTypePicker.SelectedIndex].Type;
            CustomView newView = CreateView(type);
            if (newView is HeaderView hv)
            {
                hv.Level = (ViewToAdd as HeaderView).Level;
            }
            ViewToAdd = newView;
        }

        private void ContentLayoutTapped(object sender, EventArgs e)
        {
            foreach (CustomView view in ContentLayout.Children)
            {
                view.BackgroundColor = Color.Transparent;
            }
            SelectedView = null;
        }

        private void ContentLayout_ChildRemoved(object sender, ElementEventArgs e)
        {
            SelectedView = null;
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
