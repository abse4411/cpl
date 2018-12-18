using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace cpl
{
    static public class International
    {
        public static ResourceDictionary mResourceDictionaryLanguage = null;

        static public void SetCurrentLanguage(string lang = "")
        {
            string currentLang = "zh-CN";

            if (string.IsNullOrEmpty(lang))
            {
                try
                {
                    //mCurrentLang = Dispatcher.Thread.CurrentCulture.Name;//获取当前系统语言
                    currentLang = Thread.CurrentThread.CurrentCulture.Name;
                    if (string.IsNullOrEmpty(currentLang))
                    {
                        currentLang = "en-US";
                    }
                }
                catch (System.Exception ex)
                {
                    currentLang = "en-US";
                }
            }
            else
            {
                currentLang = lang;
            }

            string currlang = "Lang_";
            currlang += currentLang;
            string langType = ConfigurationManager.AppSettings.Get(currlang);
            if(langType==null)
            {
                MessageBox.Show("Configuration file Lost !!!", "Error");
                Application.Current.Shutdown();
                return;
            }

            if (!string.IsNullOrEmpty(langType))
            {
                if (App.Current.Resources.MergedDictionaries.Contains(mResourceDictionaryLanguage))
                {
                    App.Current.Resources.MergedDictionaries.Remove(mResourceDictionaryLanguage);
                }
            }

            try
            {
                mResourceDictionaryLanguage = new ResourceDictionary() { Source = new Uri(langType, UriKind.RelativeOrAbsolute) };
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                Application.Current.Shutdown();
                return;
            }

            App.Current.Resources.MergedDictionaries.Add(mResourceDictionaryLanguage);
        }

        static public string GetString(string key)
        {
            string value = key;

            try
            {
                value = App.Current.FindResource(key).ToString();
            }
            catch (Exception ex)
            {

            }

            return value;
        }
    }
}
