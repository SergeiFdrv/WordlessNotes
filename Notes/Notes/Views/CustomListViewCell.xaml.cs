using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomListViewCell : ContentView
    {
        public CustomListViewCell()
        {
            InitializeComponent();
        }

        public CustomListView ParentList
        {
            get
            {
                var parent = Parent;
                while (parent != null)
                {
                    if (parent is CustomListView)
                    {
                        return parent as CustomListView;
                    }
                    parent = parent.Parent;
                }
                return null;
            }
        }

        private void Item_Focused(object sender, FocusEventArgs e)
        {
            Console.WriteLine("--- LIST CELL FOCUSED ---");
            if (ParentList != null)
            {
                ParentList.ParentView.ParentPage.Selected = ParentList.ParentView;
                if (!ParentList.AddButton.IsVisible) ParentList.AddButton.IsVisible = true;
            }
            //if (ParentList.)
        }
    }
}