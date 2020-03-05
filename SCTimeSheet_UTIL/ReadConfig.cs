using System;
using System.Configuration;

namespace SCTimeSheet_UTIL
{
    public class ReadConfig
    {
        public static string GetValue(string strKey)
        {
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            string text;
            try
            {
                text = appSettingsReader.GetValue(strKey, typeof(string)).ToString();
                text = text.Trim();
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("Does not exist in the appSettings configuration") <= -1)
                {
                    throw new Exception("Error(s) at MUULibrary.Common.Utility.ReadConfigValue : " + ex.Message, ex);
                }
                text = null;
            }

            return text;
        }
    }
}
