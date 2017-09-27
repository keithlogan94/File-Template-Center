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
using Windows.Storage.Pickers;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace File_Template_Centre
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateTemplateSelector : Page
    {
        private FileTemplate template = null;

        public CreateTemplateSelector()
        {
            this.InitializeComponent();
        }

        private void FileDropBackground_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        }

        async private void FileDropBackground_Drop(object sender, DragEventArgs e)
        {
            //Collect data and open popup to get template settings
            var files = await e.DataView.GetStorageItemsAsync();
            if (files.Count > 0)
            {
                StorageFile file = files[0] as StorageFile;
                if (file == null) return;
                template = new FileTemplate(file);
                templateSettings.IsOpen = true;
            }
        }

        async private void selectFile_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.CommitButtonText = "Select File";
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.ViewMode = PickerViewMode.List;
            picker.FileTypeFilter.Add("*");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null) return;
            template = new FileTemplate(file);
            if (file == null)
            {
                //throw up message that user did not select file
                // or just cancel
            }
            else
            {
                //Collect data and set mainWindow Frame to template information collect page.
                templateSettings.IsOpen = true;
            }
        }

        private void templateSettings_Opened(object sender, object e)
        {
            selectFile.IsEnabled = false;
            //update template settings info
            localFileName.Text = template.getLocalFileName();
        }

        private void templateSettings_Closed(object sender, object e)
        {
            selectFile.IsEnabled = true;
        }

        private void cancelTemplate_Click(object sender, RoutedEventArgs e)
        {
            cancelTemplateFromSettings();
        }

        async private void cancelTemplateFromSettings()
        {
            //handle all work in thread to avoid slowing the user experience
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                //Handle canceling the template
                //...
                template = null;
                templateSettings.IsOpen = false;
            });
        }

        async private void saveTemplateFromSettings()
        {
            //handle all work in thread to avoid slowing the user experience
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                //Handle Saving the template
                //...
                template.saveTemplate();
                templateSettings.IsOpen = false;
            });
        }

        private void saveTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            saveTemplateFromSettings();
        }
    }
}
