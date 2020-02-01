using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Models;
using Notes.Interfaces;

namespace Notes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoteSavePage : ContentPage
    {
        public NoteSavePage()
        {
            InitializeComponent();
        }

        public NoteSavePage(string name, List<Models.Image> images)
        {
            InitializeComponent();
            Name = name;
            Images.AddRange(images);
        }

        public string Name
        {
            get => NameEntry.Text;
            set => NameEntry.Text = value;
        }

        #region Save
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            string name = string.Empty;
            for (int j = 0; j < Name.Length; j++)
            {
                if (char.IsLetterOrDigit(Name[j])) name += Name[j];
                else if (char.IsWhiteSpace(Name[j])) name += '_';
            }
            Note note = new Note
            {
                Name = Name,
                Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), name + ".html"),
                DateTime = DateTime.UtcNow
            };
            var notes = App.Database.GetNotesAsync().Result;
            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].Name == Name)
                {
                    string replace = await DisplayActionSheet(
                        "Do you want to overwrite the existing note?", null, null, "Yes", "No").ConfigureAwait(true);
                    if (replace != "Yes") return;
                    if (File.Exists(notes[i].Path))
                        File.Delete(notes[i].Path);
                    await App.Database.DeleteNoteAsync(notes[i]).ConfigureAwait(true);
                    break;
                }
            }
            File.WriteAllText(note.Path, (Navigation.NavigationStack[0] as MainPage).NoteContent);
            await App.Database.SaveNoteAsync(note).ConfigureAwait(true);
            DependencyService.Get<IPlatformSpecific>().SayShort(Properties.Resources.NoteSaved);
            (Navigation.NavigationStack[0] as MainPage).Note = note;
            (Navigation.NavigationStack[0] as MainPage).UnsavedData = false;
            (Navigation.NavigationStack[0] as MainPage).ToolbarItems[0].Text = Name;
            await Navigation.PopAsync().ConfigureAwait(false);
        }
        #endregion

        #region Export
        public List<Models.Image> Images { get; } = new List<Models.Image>();

        private async void HTMLButton_Clicked(object sender, EventArgs e)
        {
            string name = string.Empty; int i = 0;
            for (; i < Name.Length; i++)
            {
                if (char.IsLetterOrDigit(Name[i])) name += Name[i];
                else if (char.IsWhiteSpace(Name[i])) name += '_';
            }
            string content = (Navigation.NavigationStack.ElementAt(0) as MainPage).NoteContent.Replace("<br>", "");
            string html =
                $"<!DOCTYPE HTML><html><head><meta charset=\"utf-8\"><title>{Name}</title>" +
                $"<style type=\"text/css\">* {{ font-family: sans-serif; }} img {{ width: 300px; }} " +
                $".imgdesc {{ color: #888; margin-top: 0; }}</style></head>" +
                $"<body>{content}</body></html>";
            string path = Path.Combine(DependencyService.Get<IPlatformSpecific>().GetDocsDirectory(), name + ".html");
            if (File.Exists(path) && await DisplayActionSheet(
                "Do you want to overwrite the existing file?", "No", "Yes").ConfigureAwait(true) != "Yes")
                return;
            if (Images != null)
            {
                string imgpath = Path.Combine(DependencyService.Get<IPlatformSpecific>().GetDocsDirectory(), "img"), imgname;
                if (!Directory.Exists(imgpath)) Directory.CreateDirectory(imgpath);
                for (i = 0; i < Images.Count; i++)
                {
                    imgname = Path.Combine(imgpath, Images[i].Name);
                    if (File.Exists(imgname)) File.Delete(imgname);
                    File.Copy(Images[i].Path, imgname);
                }
            }
            File.WriteAllText(path, html);
            await DisplayAlert("Saved at:", path, "OK").ConfigureAwait(false);
        }
        #endregion
    }
}