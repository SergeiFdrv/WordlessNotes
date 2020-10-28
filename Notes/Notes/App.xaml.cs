using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Data;

namespace Notes
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        public static bool LoadTestPage { get; }

        public static double FontSize => Device.GetNamedSize(NamedSize.Medium, typeof(Editor));

        public static Random Random { get; } = new Random();

        private static NoteDatabase DB { get; set; }

        public static NoteDatabase Database
        {
            get
            {
                if (DB == null)
                {
                    DB = new NoteDatabase(Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Notes.db3"));
                }
                return DB;
            }
        }

        public void SetBarBackground()
        {
            int r, g, b;
            do
            {
                r = Random.Next(256); g = Random.Next(256); b = Random.Next(256);
            } while ((r + g + b) / 3 > 127);
            Resources["Color0"] = Color.FromRgb(r, g, b);
        }

        protected override void OnStart()
        {
            SetBarBackground();
            try
            {
                if (LoadTestPage)
                {
                    MainPage = new Page1();
                }
                else
                {
                    MainPage = new NavigationPage(new MainPage());
                }
            }
            catch (Exception e)
            {
                ContentPage mainPage = new ContentPage
                {
                    Content = new Label
                    {
                        Text = e.Message + "\n\n" + e.Source + "\n\n" + e.StackTrace + "\n\n" + e.Data
                    }
                };
                MainPage = new NavigationPage(mainPage);
            }
        }

        protected override void OnSleep()
        { // TODO: ПРОВЕРИТЬ СОХРАНЕНИЕ ДАННЫХ ПРИ СВОРАЧИВАНИИ
        }

        protected override void OnResume()
        {
        }
    }
}
