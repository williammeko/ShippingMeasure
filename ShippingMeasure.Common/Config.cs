using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Common
{
    public class Config
    {
        public static string Language
        {
            get
            {
                string language = ConfigurationManager.AppSettings["Language"];
                if (!String.IsNullOrEmpty(language))
                {
                    return language.Trim();
                }
                return String.Empty;
            }

            set
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["Language"].Value = value;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.AppSettings["Language"] = value;
            }
        }

        public static bool AutoCalculate
        {
            get
            {
                string autoCalculate = ConfigurationManager.AppSettings["AutoCalculate"];
                if (!String.IsNullOrEmpty(autoCalculate))
                {
                    autoCalculate = autoCalculate.Trim().ToUpper();
                    if (autoCalculate.Equals("TRUE") || autoCalculate.Equals("1"))
                    {
                        return true;
                    }
                }
                return false;
            }

            set
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["AutoCalculate"].Value = value.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.AppSettings["AutoCalculate"] = value.ToString();
            }
        }

        public static string ConsentInfo
        {
            get
            {
                string consentInfo = ConfigurationManager.AppSettings["ConsentInfo"];
                if (!String.IsNullOrEmpty(consentInfo))
                {
                    try
                    {
                        return CommonHelper.DecryptStringFromBytesWithAes(Convert.FromBase64String(consentInfo));
                    }
                    catch(Exception ex)
                    {
                        LogHelper.Write(ex);
                        return String.Empty;
                    }
                }
                return String.Empty;
            }

            set
            {
                string consentInfo = Convert.ToBase64String(CommonHelper.EncryptStringToBytesWithAes(value));
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["ConsentInfo"].Value = consentInfo;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.AppSettings["ConsentInfo"] = consentInfo;
            }
        }

        public static string DataConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["DataConnectionString"];
            }
        }
    }
}
