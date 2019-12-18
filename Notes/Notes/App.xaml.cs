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

            MainPage = new MasterDetailPage
            {
                Master = new MasterViewPage(),
                Detail = new NavigationPage(new MainPage())
            };
        }

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
        }

        protected override void OnSleep()
        { // TODO: РЕАЛИЗОВАТЬ СОХРАНЕНИЕ ДАННЫХ ПРИ СВОРАЧИВАНИИ
        }

        protected override void OnResume()
        {
        }
    }
}
