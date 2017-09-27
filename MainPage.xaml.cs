using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace File_Template_Centre
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            mainWindow.Navigate(typeof(CreateTemplateSelector));
        }

        private void mainNavigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            ListBoxItem selectedItem = (ListBoxItem)lb.SelectedItem;
            switch (selectedItem.Name.ToString())
            {
                case "CreateTemplate":
                    mainWindow.Navigate(typeof(CreateTemplateSelector), mainWindow);
                    break;
                case "SearchTemplates":
                    mainWindow.Navigate(typeof(SearchTemplates));
                    break;
            }
        }

        private void changeToTemplateSettings()
        {
            mainWindow.Navigate(typeof(TemplateSettings));
        }
    }
}
