using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomListViewCell : ContentView
    {
        public CustomListViewCell()
        {
            InitializeComponent();
            (Content as Grid).ColumnDefinitions[1].Width = DeviceDisplay.MainDisplayInfo.Width * 0.8 /
                                                           DeviceDisplay.MainDisplayInfo.Density;
        }

        public CustomListViewCell(string Value)
        {
            InitializeComponent();
            TXT.Text = Value;
            (Content as Grid).ColumnDefinitions[1].Width = DeviceDisplay.MainDisplayInfo.Width * 0.8 /
                                                           DeviceDisplay.MainDisplayInfo.Density;
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

        public string Text => TXT.Text;

        private void Item_Focused(object sender, FocusEventArgs e)
        {
            if (ParentList != null)
            {
                ParentList.CLV_Focused(sender, e);
            }
        }

        private void Item_Unfocused(object sender, FocusEventArgs e)
        {
            if (ParentList != null)
            {
                ParentList.CLV_Unfocused(sender, e);
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            ParentList.StackL.Children.Remove(this);
        }

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            ParentList.StackL.Children.Remove(this);
        }
    }
}