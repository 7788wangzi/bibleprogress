using BibleProcess.Common;
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
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace BibleProcess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            data myData = new data();

            // init all static variables
            int[] tasksIndex = await myData.GetTasksIndex();
            int chpsPerEach = await myData.GetChpsPerTime();
            int fontSize = await myData.GetFontSize();
            int allPass = await myData.GetallPass();

            App.ChpsPerEach = chpsPerEach;
            App.ContentFontSize = (CustomFontSize)fontSize;
            App.AllPassBible = allPass;
            App.CurrentTasks = tasksIndex;

            // finished chapter index of last time
            string a = await myData.GetDisplayNameByIndex(tasksIndex[0]-1);
            if (a == string.Empty)
            {
                current.Text = "还未开始阅读!";
            }
            else if (a == "Err")
            {
                current.Text = "Oops, Error Occur!";
            }
            else
                current.Text = a;

            // for this time, the start chapter and stop chapter
            from.Text = await myData.GetDisplayNameByIndex(tasksIndex[0]);
            to.Text = await myData.GetDisplayNameByIndex(tasksIndex[1]);

            /*
            // show a toast after 24 hours later
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var toastText = toastXml.GetElementsByTagName("text");
            (toastText[0] as XmlElement).InnerText = "该读圣经了...";
            var customAlarmScheduledToast = new ScheduledToastNotification(toastXml, DateTime.Now.AddHours(24));
            toastNotifier.AddToSchedule(customAlarmScheduledToast);
            */
        }




        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }


        private void viewContent_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            if(clickedButton.Name == "cus_ViewContent")
            {
                App.CurrentBibleVersion = BibleVersion.Cus;
            }
            else if(clickedButton.Name == "TCVs_ViewContent")
            {
                App.CurrentBibleVersion = BibleVersion.TCVs;
            }
            else if(clickedButton.Name == "ESV_ViewContent")
            {
                App.CurrentBibleVersion = BibleVersion.ESV;
            }
            this.Frame.Navigate(typeof(ContentDetails));
        }
    }
}
