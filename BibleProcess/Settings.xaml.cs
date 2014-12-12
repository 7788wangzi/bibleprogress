using BibleProcess.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace BibleProcess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        private NavigationHelper navigationHelper;
        public Settings()
        {
            this.InitializeComponent();
            Loaded += Settings_Loaded;
            this.navigationHelper = new NavigationHelper(this);
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            cbChpsPerTime.SelectedIndex = App.ChpsPerEach - 1;
            cbFontSize.SelectedIndex = (int)App.ContentFontSize - 1;
            cbLanguage.SelectedIndex = App.SystemLanguage;
            if (App.EnableToast)
                tsToast.IsOn = true;
            else
                tsToast.IsOn = false;

            int eachPerTime = App.ChpsPerEach;
            int fontSize = (int)App.ContentFontSize;
            if (App.SystemLanguage == 0)
            {
                allPass.Text = string.Format(@"已经读完{0}遍圣经。", App.AllPassBible.ToString());
            }
            else if (App.SystemLanguage == 1)
            {
                allPass.Text = string.Format(@"Already Completed Reading for {0} times.", App.AllPassBible.ToString());
            }
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            if(App.SystemLanguage==0)
            {
                this.title.Text = "读经进度";
                this.pageTitle.Text = "设置";
                this.cbChpsPerTime.Header = "每天读几章";
                this.cbChpsPerTime.PlaceholderText = "这里选择要更改的值";
                this.cbFontSize.Header = "经文字体大小";
                this.cbFontSize.PlaceholderText = "这里选择要更改的值";
                this.tsToast.Header = "提醒功能";
                this.set.Label = "应用更改";
                this.Rate.Label = "评论";
                this.tsToast.OnContent = "开";
                this.tsToast.OffContent = "关";
                this.cbLanguage.Header = "系统语言";
                this.cbLanguage.PlaceholderText = "这里选择要更改的值";

                this.cbLanguage.ItemsSource = new List<string>
                {
                    "中文",
                    "英文"
                };
                cbFontSize.ItemsSource = new List<string> {
                    "正常",
                    "大",
                    "超大"
                };
                cbChpsPerTime.ItemsSource = new List<string> {
                                        "1 章",
                    "2 章",
                    "3 章",
                    "4 章",
                    "5 章",
                    "6 章",
                    "7 章",
                    "8 章",
                    "9 章",
                    "10 章",
                    "11 章",
                    "12 章",
                    "13 章",
                    "14 章",
                    "15 章",
                };   

            }else if(App.SystemLanguage==1)
            {
                this.title.Text = "Simple Reading";
                this.pageTitle.Text = "Settings";
                this.cbChpsPerTime.Header = "Chapters Per Day";
                this.cbChpsPerTime.PlaceholderText = "Select a Value";
                this.cbFontSize.Header = "Font Size for Content";
                this.cbFontSize.PlaceholderText = "Select a Value";
                this.tsToast.Header = "Toast";
                this.set.Label = "Apply";
                this.Rate.Label = "Rate Me";
                this.tsToast.OnContent = "On";
                this.tsToast.OffContent = "Off";
                this.cbLanguage.Header = "System Language";
                this.cbLanguage.PlaceholderText = "Select a Value";

                this.cbLanguage.ItemsSource = new List<string>
                {
                    "Chinese",
                    "English"
                };
                cbFontSize.ItemsSource = new List<string> {
                    "Normal",
                    "Big",
                    "Large"
                };
                cbChpsPerTime.ItemsSource = new List<string> {
                    "1 Chapter",
                    "2 Chapters",
                    "3 Chapters",
                    "4 Chapters",
                    "5 Chapters",
                    "6 Chapters",
                    "7 Chapters",
                    "8 Chapters",
                    "9 Chapters",
                    "10 Chapters",
                    "11 Chapters",
                    "12 Chapters",
                    "13 Chapters",
                    "14 Chapters",
                    "15 Chapters",
                };
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void ShowMsg(int eachPerTime)
        {
            int currentChpsIndex = 0;
            data myData = new data();
            currentChpsIndex = App.CurrentTasks[0] - 1;
            int costDays = (1189 - currentChpsIndex) / eachPerTime;
            if (costDays == 0)
                costDays = 1;

            System.Text.StringBuilder sbString = new System.Text.StringBuilder();

            if (App.SystemLanguage == 0)
            {
                sbString.AppendLine(string.Format(@"每天读{0}章。", eachPerTime));
                sbString.AppendLine(string.Format(@"剩余章节大约{0}天读完。", costDays));
            }
            else if (App.SystemLanguage == 1)
            {
                sbString.AppendLine(string.Format(@"{0} chapters each day.", eachPerTime));
                sbString.AppendLine(string.Format(@"cost {0} days to complete the remaining chapters.", costDays));
            }

            msg.Text = sbString.ToString();
        }
        

        private async void Rate_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(string.Format(@"ms-windows-store:reviewapp?appid={0}", "2a9c2a83-c59c-4949-bbf6-e51abd0736b8")));
        }

        private async void set_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // for amount of chapters per time
                int chpsPerTimeIndex = cbChpsPerTime.SelectedIndex;
                int chpsPerTimeValue = chpsPerTimeIndex + 1;                
                App.ChpsPerEach = chpsPerTimeValue;

                // for font size
                int selectedIndex = cbFontSize.SelectedIndex;
                int fontSizeValue = selectedIndex + 1;

                // for language
                int selectedLanIndex = cbLanguage.SelectedIndex;
                int languageValue = selectedLanIndex;
                App.SystemLanguage = languageValue;

                // set toast notification
                int toastEnableValue = 0;
                if(tsToast.IsOn)
                {
                    App.EnableToast = true;
                    toastEnableValue = 1;
                }
                else
                {
                    App.EnableToast = false;
                    toastEnableValue = 0;
                }

                // update current variable
                App.ContentFontSize = (CustomFontSize)fontSizeValue;
                data myData = new data();
                bool isSaved = await myData.SetChpsPerEachAndFontSize(chpsPerTimeValue, fontSizeValue, toastEnableValue, languageValue);

                if (isSaved)
                {
                    ContentDialog cntDialog = new ContentDialog();
                    if (App.SystemLanguage == 0)
                    {
                        cntDialog.Title = "提醒";
                        cntDialog.Content = "应用设置已经更改。";
                    }
                    else if(App.SystemLanguage==1)
                    {
                        cntDialog.Title = "Info";
                        cntDialog.Content = "Settings be Changed!";
                    }
                    
                    await cntDialog.ShowAsync();
                }
            }
            catch (Exception ex) { string value = ex.Message;
                ContentDialog cntDialog = new ContentDialog();
                cntDialog.Title = "Err";
                cntDialog.Content = value;
                await cntDialog.ShowAsync();
            }
        }

        private void cbChpsPerTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbChpsPerTime != null&&cbChpsPerTime.SelectedValue!=null)
            {
                int selectedIndex = cbChpsPerTime.SelectedIndex;
                ShowMsg(selectedIndex + 1);
            }
        }
    }
}
