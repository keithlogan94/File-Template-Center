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
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;
using System.Threading.Tasks;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace File_Template_Centre
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateTemplateSelector : Page
    {
        private FileTemplate template = null;
        private DispatcherTimer messageTimer = null;

        public CreateTemplateSelector()
        {
            this.InitializeComponent();
            messageTimer = new DispatcherTimer();
            messageTimer.Tick += closeMessageTimer_Tick;
            messageTimer.Interval = new TimeSpan(0, 0, 3);
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
                sendMessage("Template Canceled.");
            });
        }

        async private void saveTemplateFromSettings()
        {
            //handle all work in thread to avoid slowing the user experience
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                //Handle Saving the template
                //...
                template.setTemplateName(templateNameTextBox.Text);
                template.setDescription(descriptionTextBox.Text);
                template.setDateCreated("Sample Date Created");
                template.saveTemplate();
                templateSettings.IsOpen = false;
                templateSaveComplete(null, null);
            });
        }

        private void saveTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            saveTemplateFromSettings();
        }

        private void templateSaveComplete(object sender, RoutedEventArgs e)
        {
            // In a real app, these would be initialized with actual data
            string title = "File Template Center";
            string content = "Template Save Complete";
            string image = "ms-appdata:///Assets/LockScreenLogo.scale-200.png";
            string logo = "ms-appdata:///Assets/LockScreenLogo.scale-200.png";

            // Construct the visuals of the toast
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = title
                        },

                        new AdaptiveText()
                        {
                            Text = content
                        },

                        new AdaptiveImage()
                        {
                            Source = image
                        }
                    },

                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = logo,
                        HintCrop = ToastGenericAppLogoCrop.Circle
                    }
                }
            };

            // In a real app, these would be initialized with actual data
            int conversationId = 384928;

            // Construct the actions for the toast (inputs and buttons)
            ToastActionsCustom actions = new ToastActionsCustom()
            {
                Inputs =
                    {
                        new ToastTextBox("tbReply")
                        {
                            PlaceholderContent = "Type a response"
                        }
                    },
                Buttons =
                    {
                        new ToastButton("Reply", new QueryString()
                        {
                            { "action", "reply" },
                            { "conversationId", conversationId.ToString() }

                        }.ToString())
                        {
                            ActivationType = ToastActivationType.Background,
                            ImageUri = "Assets/Reply.png",

                            // Reference the text box's ID in order to
                            // place this button next to the text box
                            TextBoxId = "tbReply"
                        },

                        new ToastButton("Like", new QueryString()
                        {
                            { "action", "like" },
                            { "conversationId", conversationId.ToString() }

                        }.ToString())
                        {
                            ActivationType = ToastActivationType.Background
                        },

                        new ToastButton("View", new QueryString()
                        {
                            { "action", "viewImage" },
                            { "imageUrl", image }

                        }.ToString())
                    }
            };

            // Now we can construct the final toast content
            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                Actions = actions,

                // Arguments when the user taps body of toast
                Launch = new QueryString()
                {
                    { "action", "viewConversation" },
                    { "conversationId", conversationId.ToString() }

                }.ToString()
            };

            // And create the toast notification
            var toast = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toast);
            sendMessage("Template Saved.");
        }

        private void sendMessage(String message)
        {
            this.message.Text = message;
            this.messagePopup.IsOpen = true;
            messageTimer.Start();
        }

        private void cancelMessage_Click(object sender, RoutedEventArgs e)
        {
            this.messagePopup.IsOpen = false;
        }

        private void closeMessageTimer_Tick(object sender, object e)
        {
            this.messagePopup.IsOpen = false;
            messageTimer.Stop();
        }
    }
}
