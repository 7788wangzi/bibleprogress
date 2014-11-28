using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;


namespace BibleProcess
{
    public class data
    {
        string dataFile = @"ms-appx:///DataModel/coreData.xml";
        string settingFile = @"ms-appx:///DataModel/Setting.xml";

        public static int CurrentCHP { get; set; }

        public async Task<int[]> GetTasksIndex()
        {
            int[] result = new int[2];

            Uri fileUrl = new Uri(settingFile);
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(fileUrl);
            string sStream = await FileIO.ReadTextAsync(sFile);

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(sStream);

            XmlNodeList settings = xDoc.GetElementsByTagName("Settings");
            if (settings.Count >= 1)
            {
                int chpsPerTime = Int32.Parse(settings[0].SelectSingleNode("setEach").InnerText);
                int currIndex = Int32.Parse(settings[0].SelectSingleNode("currentIndex").InnerText);
                int total = Int32.Parse(settings[0].SelectSingleNode("total").InnerText);
                if (total > 1189)
                    total = 1189;
                int fromIndex = 0;
                int toIndex = 0;
                if (currIndex == total)
                {
                    fromIndex = 1;
                    toIndex = fromIndex + chpsPerTime - 1;
                    currIndex = toIndex;
                }
                else
                {
                    fromIndex = currIndex + 1;
                    toIndex = fromIndex + chpsPerTime - 1;
                    if (toIndex <= total)
                    {
                        currIndex = toIndex;
                    }
                    else
                    {
                        toIndex = total;
                        currIndex = toIndex;
                    }
                }

                result[0] = fromIndex;
                result[1] = toIndex;
                CurrentCHP = toIndex;
            }
            return result;
        }

        // no longer used, used GetTasksIndex and GetDisplayNameByIndex
        private async Task<string[]> GetTasksResult()
        {
            int[] _tasksIndex = await GetTasksIndex();
            string from = await GetDisplayNameByIndex(_tasksIndex[0]);
            string to = await GetDisplayNameByIndex(_tasksIndex[1]);

            return new string[] { from, to };
        }

        public async Task<string> GetDisplayNameByIndex(int index)
        {
            string result = string.Empty;
            Uri fileUrl = new Uri(dataFile);
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(fileUrl);
            string sStream = await FileIO.ReadTextAsync(sFile);

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(sStream);
            if(index<1)
            {
                return string.Empty;
            }

            XmlNodeList contents = xDoc.GetElementsByTagName("Contents");
            if (contents.Count >= 1)
            {
                var _node = contents[0].SelectSingleNode(string.Format(@"//book[@index='{0}']", index));
                result = _node.SelectSingleNode("displayName").InnerText;
            }

            return result;
        }

        public async Task<int> GetChpsPerTime()
        {
            Uri fileUrl = new Uri(settingFile);
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(fileUrl);
            string sStream = await FileIO.ReadTextAsync(sFile);

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(sStream);

            XmlNodeList settings = xDoc.GetElementsByTagName("Settings");
            int result = 0;
            if (settings.Count >= 1)
            {
                int chpsPerTime = Int32.Parse(settings[0].SelectSingleNode("setEach").InnerText);
                result = chpsPerTime;
            }
            return result;
        }

        public async Task<int> GetallPass()
        {
            Uri fileUrl = new Uri(settingFile);
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(fileUrl);
            string sStream = await FileIO.ReadTextAsync(sFile);

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(sStream);
            XmlNodeList settings = xDoc.GetElementsByTagName("Settings");
            int result = 0;
            if (settings.Count >= 1)
            {
                int chpsPerTime = Int32.Parse(settings[0].SelectSingleNode("allPass").InnerText);
                result = chpsPerTime;
            }

            return result;
        }

        // no longer used, used GetTasksIndex, then Index[0]-1 is the current index
        private async Task<int> GetCurrentChpIndex()
        {
            Uri fileUrl = new Uri(settingFile);
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(fileUrl);
            string sStream = await FileIO.ReadTextAsync(sFile);

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(sStream);
            XmlNodeList settings = xDoc.GetElementsByTagName("Settings");
            int result = 0;
            if (settings.Count >= 1)
            {
                int currentIndex = Int32.Parse(settings[0].SelectSingleNode("currentIndex").InnerText);
                result = currentIndex;
            }

            return result;
        }

        // no longer used, replaced by using GetCurrentChpIndex and GetDisplayNameByIndex
        private async Task<string> GetCurrentChp()
        {
            Uri fileUrl = new Uri(settingFile);
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(fileUrl);
            string sStream = await FileIO.ReadTextAsync(sFile);

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(sStream);
            XmlNodeList settings = xDoc.GetElementsByTagName("Settings");
            int result = 0;
            if (settings.Count >= 1)
            {
                int currentIndex = Int32.Parse(settings[0].SelectSingleNode("currentIndex").InnerText);
                result = currentIndex;
            }

            if (result == 0)
                return string.Empty;
            else
            {
                string currChpDisplay = await GetDisplayNameByIndex(result);
                return currChpDisplay;
            }
        }

        public async Task<int> GetFontSize()
        {
            Uri fileUrl = new Uri(settingFile);
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(fileUrl);
            string sStream = await FileIO.ReadTextAsync(sFile);

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(sStream);

            XmlNodeList settings = xDoc.GetElementsByTagName("Settings");
            int value = 0;
            if (settings.Count >= 1)
            {
                value = int.Parse(settings[0].SelectSingleNode("fontSize").InnerText.ToString());
            }

            return value;
        }
        /// <summary>
        /// get chapter content
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetChpsContent(string xmlFile)
        {            
            Uri fileUrl = new Uri(xmlFile);
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(fileUrl);
            string sStream = await FileIO.ReadTextAsync(sFile);
            string content = Converter.DoConversion(sStream);
            return content;
        }
        
        public async void SetChpsPerEachAndFontSize(int chpsPerEachValue, int fontSizeValue)
        {
            Uri fileUrl = new Uri(settingFile);
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(fileUrl);
            string sStream = await FileIO.ReadTextAsync(sFile);

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(sStream);

            XmlNodeList settings = xDoc.GetElementsByTagName("Settings");
            string result = string.Empty;
            if (settings.Count >= 1)
            {
                settings[0].SelectSingleNode("setEach").InnerText = chpsPerEachValue.ToString();
                settings[0].SelectSingleNode("fontSize").InnerText = fontSizeValue.ToString();
                await xDoc.SaveToFileAsync(sFile);
            }
        }

        public async Task<bool> SetCurrentCHP(int value)
        {
            bool complete = false;
            Uri fileUrl = new Uri(settingFile);
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(fileUrl);
            string sStream = await FileIO.ReadTextAsync(sFile);

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(sStream);

            XmlNodeList settings = xDoc.GetElementsByTagName("Settings");
            string result = string.Empty;
            if (settings.Count >= 1)
            {
                if (value == 1189)
                {
                    int _allPass = Int32.Parse(settings[0].SelectSingleNode("allPass").InnerText);
                    _allPass++;
                    settings[0].SelectSingleNode("allPass").InnerText = _allPass.ToString();
                }
                settings[0].SelectSingleNode("currentIndex").InnerText = value.ToString();
                await xDoc.SaveToFileAsync(sFile);
                complete = true;
            }
            return complete;
        }

        public async void InitSetting()
        {
            Uri fileUrl = new Uri(settingFile);
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(fileUrl);
            string sStream = await FileIO.ReadTextAsync(sFile);

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(sStream);

            XmlNodeList settings = xDoc.GetElementsByTagName("Settings");
            string result = string.Empty;
            if (settings.Count >= 1)
            {
                settings[0].SelectSingleNode("setEach").InnerText = "1";
                settings[0].SelectSingleNode("currentIndex").InnerText = "0";
                settings[0].SelectSingleNode("total").InnerText = "1189";
                settings[0].SelectSingleNode("allPass").InnerText = "0";
                settings[0].SelectSingleNode("fontSize").InnerText = "1";
                await xDoc.SaveToFileAsync(sFile);
            }
        }
        
    }
}
