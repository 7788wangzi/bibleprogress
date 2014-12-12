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
using Windows.UI.ViewManagement;
using System.Threading.Tasks;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace BibleProcess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        public MainPage()
        {
            this.InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            this.NavigationCacheMode = NavigationCacheMode.Required;
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // show bibles
            List<Arguments> _args = new List<Arguments>()
            {
                new Arguments()
                {
                    Value="和合本"
                },
                new Arguments()
                {
                    Value="现代中文译本"
                },
                new Arguments()
                {
                    Value="ESV"
                },
                new Arguments()
                {
                    Value="中文新译本"
                },
                new Arguments()
                {
                    Value="吕振中译本"
                },
                new Arguments()
                {
                    Value="NIV"
                },
                new Arguments()
                {
                    Value="WEV"
                },
                new Arguments()
                {
                    Value="KJV"
                }
            };

            gvBibleVersion.ItemsSource = _args;

            // remove toasts
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();

            // remove the toast from action center
            ToastNotificationHistory toastHistory = ToastNotificationManager.History;
            toastHistory.Remove("qbible");

            // remove all scheduled toast by Me from the history
            IList<ScheduledToastNotification> toasts = toastNotifier.GetScheduledToastNotifications().ToList();
            foreach (var item in toasts)
            {
                if (item.Tag == "qbible")
                    toastNotifier.RemoveFromSchedule(item);
            }

            // hide status bar
            //StatusBar statusbar = StatusBar.GetForCurrentView();
            //await statusbar.HideAsync();
        }

        public async Task<bool> LoadSettingData()
        {
            bool isComplete = false;
            

            // init all static variables
            int[] tasksIndex = new int[] { 1, 3 };

            int[] maliSettings = new int[] { 3, 0, 1, 0, 0 };

            // init the Setting.xml file in local folder
            data myData = new data();
            bool fileExist = await myData.IsFileExist();
            if (!fileExist)
            {
                bool fileSaved = await myData.InitSetting();
                while (!fileSaved)
                {
                    fileSaved = await myData.InitSetting();
                }
            }

            try
            {
                maliSettings = await myData.GetMaliSetting();
                tasksIndex = await myData.GetTasksIndex();
            }
            catch (Exception ex)
            {
                bool isFileCompleteness = await myData.CheckFileCompleteness();
                if (!isFileCompleteness)
                {
                    bool fileSaved = await myData.InitSetting();
                }
            }
            //finally
            //{
            //    //maliSettings = await myData.GetMaliSetting();
            //    //tasksIndex = await myData.GetTasksIndex();
            //}
            int chpsPerEach = maliSettings[0];
            int allPass = maliSettings[1];
            int fontSize = maliSettings[2];
            int toast = maliSettings[3];
            int language = maliSettings[4];
            if (toast == 0)
            {
                App.EnableToast = false;
            }
            else
                App.EnableToast = true;

            App.ChpsPerEach = chpsPerEach;
            App.ContentFontSize = (CustomFontSize)fontSize;
            App.AllPassBible = allPass;
            App.CurrentTasks = tasksIndex;
            App.SystemLanguage = language;

            myData = new data();
            // finished chapter index of last time
            string a = await myData.GetDisplayNameByIndex(tasksIndex[0] - 1);
            if (a == string.Empty)
            {
                if (App.SystemLanguage == 0)
                {
                    current.Text = "还未开始阅读!";
                }
                else if (App.SystemLanguage == 1)
                {
                    current.Text = "Not Started yet!";
                }
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
            isComplete = true;
            return isComplete;
        }




        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            bool isComplete =await LoadSettingData();
            if (isComplete)
            {
                // TODO: Prepare page for display here.

                // TODO: If your application contains multiple pages, ensure that you are
                // handling the hardware Back button by registering for the
                // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
                // If you are using the NavigationHelper provided by some templates,
                // this event is handled for you.
                // display language
                if (App.SystemLanguage == 0)
                {
                    //show chinese
                    this.title.Text = "读经进度";
                    //this.current.Text = "加载中...";
                    this.title1.Text = "今日读经";
                    //this.from.Text = "加载中...";
                    //this.to.Text = "加载中...";
                    this.Setting.Label = "设置";
                }
                else if (App.SystemLanguage == 1)
                {
                    // show english
                    this.title.Text = "Last Read";
                    //this.current.Text = "Loading...";
                    this.title1.Text = "For today";
                    //this.from.Text = "Loading...";
                    //this.to.Text = "Loading...";
                    this.Setting.Label = "Settings";
                }
            }
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }

        private void gvBibleVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridView gv = sender as GridView;
            if (gv != null && gv.SelectedValue != null)
            {
                string version = ((Arguments)gv.SelectedValue).Value.ToString();
                if (version == "和合本")
                {
                    App.CurrentBibleVersion = BibleVersion.Cus;
                }
                else if (version == "现代中文译本")
                {
                    App.CurrentBibleVersion = BibleVersion.TCVs;
                }
                else if (version == "ESV")
                {
                    App.CurrentBibleVersion = BibleVersion.ESV;
                }
                else if(version== "中文新译本")
                {
                    App.CurrentBibleVersion = BibleVersion.CNVs;
                }
                else if (version == "吕振中译本")
                {
                    App.CurrentBibleVersion = BibleVersion.CLZZs;
                }
                else if (version == "NIV")
                {
                    App.CurrentBibleVersion = BibleVersion.NIV;
                }
                else if (version == "WEV")
                {
                    App.CurrentBibleVersion = BibleVersion.WEV;
                }
                else if (version == "KJV")
                {
                    App.CurrentBibleVersion = BibleVersion.KJV;
                }
                this.Frame.Navigate(typeof(ContentDetails));
            }
        }
    }
}
