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
            this.navigationHelper = new NavigationHelper(this);
            cbChpsPerTime.SelectedIndex = App.ChpsPerEach - 1;
            cbFontSize.SelectedIndex = (int)App.ContentFontSize - 1;

            int eachPerTime = App.ChpsPerEach;
            int fontSize = (int)App.ContentFontSize;           
            allPass.Text = string.Format(@"已经读完{0}遍圣经。", App.AllPassBible.ToString());
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
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

            sbString.AppendLine(string.Format(@"每天读{0}章。", eachPerTime));
            sbString.AppendLine(string.Format(@"剩余章节大约{0}天读完。", costDays));

            msg.Text = sbString.ToString();
        }
        

        private async void Rate_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(string.Format(@"ms-windows-store:reviewapp?appid={0}", "ed52f492-c981-44d7-a4b7-54e7bbc202f0")));
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

                // update current variable
                App.ContentFontSize = (CustomFontSize)fontSizeValue;
                data myData = new data();
                myData.SetChpsPerEachAndFontSize(chpsPerTimeValue, fontSizeValue);

                ContentDialog cntDialog = new ContentDialog();
                cntDialog.Title = "提醒";
                cntDialog.Content = "应用设置已经更改。";
                await cntDialog.ShowAsync();
            }
            catch (Exception ex) { string value = ex.Message; }
        }

        private void cbChpsPerTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbChpsPerTime != null)
            {
                int selectedIndex = cbChpsPerTime.SelectedIndex;
                ShowMsg(selectedIndex + 1);
            }
        }
    }
}
