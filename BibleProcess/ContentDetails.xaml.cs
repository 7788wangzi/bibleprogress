using BibleProcess.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace BibleProcess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContentDetails : Page
    {

        private NavigationHelper navigationHelper;
        public int[] ContentScale { get; set; }
        public int currentIndex = 0;
        public ContentDetails()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            Loaded += ContentDetails_Loaded;
        }

        private void ContentDetails_Loaded(object sender, RoutedEventArgs e)
        {
            ContentScale = App.CurrentTasks;
            
            if(ContentScale[0]== ContentScale[1])
            {
                pre.IsEnabled = false;
                next.IsEnabled = false;
            }
            pre.IsEnabled = false;
            currentIndex = ContentScale[0];
            SetWebViewSource();
            SetPageTitle();
        }

        private async void SetWebViewSource()
        {
            data myData = new data();
            string html = string.Empty;
            if(App.ContentFontSize == CustomFontSize.Normal)
            {
                html = "<!DOCTYPE html><html><head><meta name='viewport' content='width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0'/><meta http-equiv='Content-Type' content='text/html; charset=utf-8'></head><body><div style='font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif'>";
            }
            else if(App.ContentFontSize == CustomFontSize.Big)
            {
                html = "<!DOCTYPE html><html><head><meta name='viewport' content='width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0'/><meta http-equiv='Content-Type' content='text/html; charset=utf-8'></head><body><div style='font-size:21px; font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif'>";
            }
            else
                html = "<!DOCTYPE html><html><head><meta name='viewport' content='width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0'/><meta http-equiv='Content-Type' content='text/html; charset=utf-8'></head><body><div style='font-size:30px; font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif'>";

            string _xmlChapter = string.Empty;

            // get correct bible version
            if(App.CurrentBibleVersion == BibleVersion.Cus)
            {
                _xmlChapter = string.Format(@"ms-appx:///DataModel/Cus/{0}.xml", currentIndex.ToString());
            }
            else if(App.CurrentBibleVersion == BibleVersion.TCVs)
            {
                _xmlChapter = string.Format(@"ms-appx:///DataModel/TCVs/{0}.xml", currentIndex.ToString());
            }
            else if(App.CurrentBibleVersion == BibleVersion.ESV)
            {
                _xmlChapter = string.Format(@"ms-appx:///DataModel/ESV/{0}.xml", currentIndex.ToString());
            }
            
            string content = await myData.GetChpsContent(_xmlChapter);
            html += content;
            html += "</div><body></html>";
            webViewer.NavigateToString(html);            
        }

        private async void SetPageTitle()
        {
            data myData = new data();
            string displayName = await myData.GetDisplayNameByIndex(currentIndex);
            
            chpTitle.Text = displayName;
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void pre_Click(object sender, RoutedEventArgs e)
        {
            if(currentIndex> ContentScale[0])
            {
                // hide complete button
                complete.Visibility = Visibility.Collapsed;
                currentIndex--;
                next.IsEnabled = true;
                SetWebViewSource();
                SetPageTitle();
                if (currentIndex == ContentScale[0])
                    pre.IsEnabled = false;
            }
            else
            {

            }
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            if(currentIndex< ContentScale[1])
            {
                currentIndex++;
                pre.IsEnabled = true;
                SetWebViewSource();
                SetPageTitle();
                if (currentIndex == ContentScale[1])
                {
                    next.IsEnabled = false;

                    // show the Reading Complete button
                    complete.Visibility = Visibility.Visible;
                }
            }
            else{
            }
            
        }

        private async void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            int value = App.CurrentTasks[1]; // or data.CurrentCHP
            data myData = new data();
            await myData.SetCurrentCHP(value);
            App.Current.Exit();
        }
    }
}
