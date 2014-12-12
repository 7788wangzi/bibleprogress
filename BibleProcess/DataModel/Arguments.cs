using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleProcess
{
    public class Arguments
    {
        public string Value { get; set; }
    }

    public enum CustomFontSize
    {
        Normal =1,
        Big,
        Large
    }

    public enum BibleVersion
    {
        TCVs =1, //现代中文译本
        Cus, //和合本
        ESV, //英文标准版
        CLZZs, //吕振中译本
        CNVs, //中文新译本
        KJV, // King James Version
        NIV, // New International Version
        WEV // World English Version 
    }
}
