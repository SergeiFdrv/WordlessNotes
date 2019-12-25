using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomListView : ContentView
    {
        public CustomListView()
        {
            InitializeComponent();
        }

        public new CustomView ParentView
        {
            get
            {
                var parent = Parent;
                while (parent != null)
                {
                    if (parent is CustomView)
                    {
                        return parent as CustomView;
                    }
                    parent = parent.Parent;
                }
                return null;
            }
        }

        public StackLayout StackL => SL;

        public Button AddButton => AddToListButton;

        private void Item_Focused(object sender, FocusEventArgs e)
        {
            Console.WriteLine("--- LIST FOCUSED ---");
            if (ParentView != null) ParentView.ParentPage.Selected = ParentView;
            if (!AddToListButton.IsVisible) AddToListButton.IsVisible = true;
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            SL.Children.Add(new CustomListViewCell());
        }

        public void PopulateList(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                SL.Children.Add(new CustomListViewCell(lines[i]));
            }
        }
    }
}