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
using Notes.Resources;

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

        public List<Models.Image> Images { get; } = new List<Models.Image>();

        #region Common
        /// <summary>
        /// Turns "The input string !- 1" into "The_input_string__1"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string CreateNoteName(string s)
        {
            StringBuilder name = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsLetterOrDigit(s[i])) name.Append(s[i]);
                else if (char.IsWhiteSpace(s[i])) name.Append('_');
            }
            return name.ToString();
        }
        #endregion

        #region Save
        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            string name = CreateNoteName(Name);
            string folderPath =
                DependencyService.Get<IPlatformSpecific>().GetAppFilesDirectory();
            Note note = new Note
            {
                Name = Name,
                Path = Path.Combine(folderPath, name + ".html"),
                DateTime = DateTime.UtcNow
            };
            var existingNote = App.Database.GetNotesAsync().Result
                .FirstOrDefault(i => i.Name == Name);
            if (existingNote != null)
            {
                string replace = await DisplayActionSheet(
                    Lang.OverwriteNotePrompt, Lang.No, Lang.Yes).ConfigureAwait(true);
                if (replace != Lang.Yes) return;
                if (File.Exists(existingNote.Path))
                    File.Delete(existingNote.Path);
                await App.Database.DeleteNoteAsync(existingNote).ConfigureAwait(true);
            }
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            File.WriteAllText
                (note.Path, (Navigation.NavigationStack[0] as MainPage).NoteContent);
            await App.Database.SaveNoteAsync(note).ConfigureAwait(true);
            DependencyService.Get<IPlatformSpecific>().SayShort(Lang.NoteSaved);
            (Navigation.NavigationStack[0] as MainPage).Note = note;
            (Navigation.NavigationStack[0] as MainPage).UnsavedData = false;
            (Navigation.NavigationStack[0] as MainPage).ToolbarItems[0].Text = Name;
            await Navigation.PopAsync().ConfigureAwait(false);
        }
        #endregion

        #region Export
        private async void HTMLButton_Clicked(object sender, EventArgs e)
        {
            string name = CreateNoteName(Name);
            string content = (Navigation.NavigationStack.ElementAt(0) as MainPage)
                .NoteContent.Replace("<br>", "");
            string html =
                $"<!DOCTYPE HTML><html>" +
                $"<head><meta charset=\"utf-8\"><title>{Name}</title>" +
                $"<style type=\"text/css\">* {{ font-family: sans-serif; }} " +
                $"img {{ width: 300px; }} " +
                $".imgdesc {{ color: #888; margin-top: 0; }}</style></head>" +
                $"<body>{content}</body></html>";
            string folderPath = Path.Combine(
                DependencyService.Get<IPlatformSpecific>().GetDocsDirectory(), Name);
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, $"{name}.html");
            if (File.Exists(filePath) && await DisplayActionSheet(
                Lang.OverwriteFilePrompt, Lang.No, Lang.Yes).ConfigureAwait(true) != Lang.Yes)
                return;
            if (Images != null)
            {
                string imgpath = Path.Combine(folderPath, "img");
                string imgname;
                if (!Directory.Exists(imgpath)) Directory.CreateDirectory(imgpath);
                for (int i = 0; i < Images.Count; i++)
                {
                    imgname = Path.Combine(imgpath, Images[i].Name);
                    if (File.Exists(imgname)) File.Delete(imgname);
                    File.Copy(Images[i].Path, imgname);
                }
            }
            File.WriteAllText(filePath, html);
            await DisplayAlert(Lang.SavedAt, folderPath, "OK").ConfigureAwait(false);
        }
        #endregion
    }
}