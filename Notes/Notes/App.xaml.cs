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

        protected override void OnStart()
        {
            try
            {
                if (LoadTestPage)
                {
                    MainPage = new Page1();
                }
                else
                {
                    MainPage = new NavigationPage(new MainPage())
                    {
                        BarBackgroundColor = Color.FromRgb(Random.Next(192), Random.Next(192), Random.Next(192)),
                    };
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
                MainPage = new NavigationPage(mainPage)
                {
                    BarBackgroundColor = Color.FromRgb(Random.Next(192), Random.Next(192), Random.Next(192)),
                };
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
