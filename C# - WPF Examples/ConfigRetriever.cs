using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Linq;
using SpectroVision_Setup_Wizard.Properties;

namespace SpectroVision_Setup_Wizard.OtherClasses
{
    public sealed class ConfigRetriever
    {
        //Singleton
        private static readonly Lazy<ConfigRetriever> Lazy = new Lazy<ConfigRetriever>(() => new ConfigRetriever());

        public static ConfigRetriever Instance => Lazy.Value;

        private XElement _configFile;

        public string LastError = "";

        public bool SettingsLoaded { get; set; } = false;

        public bool LoadSettings()
        {
            SettingsList.Clear();

            // Loads in the config in XML format
            try
            {
                _configFile = XElement.Load(Settings.Default.ChromaConfigPath);

                if (_configFile == null)
                {
                    SettingsLoaded = false;
                    return SettingsLoaded;
                }
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();
                Hs.Instance._log.LogException("Error Loading Chroma Config", ex);
                SettingsLoaded = false;
                return SettingsLoaded;
            }

            // Parses out the XML doc into settings
            try
            {
                foreach (var setting in _configFile.Descendants("setting"))
                {
                    if (setting.Attribute("name") == null) continue;

                    var name = setting.Attribute("name")?.Value;
                    var val = setting.Value;

                    SettingsList.Add(new ChromaSetting() { SettingName = name, SettingValue = val });
                }

                SettingsLoaded = true;
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();
                Hs.Instance._log.LogException("Error Parsing Chroma Config", ex);
                return false;
            }

            return true;
        }

        public bool SaveSettings()
        {
            if (!SettingsLoaded)
            {
                Hs.Instance._log.LogError("ConfigRetriever", "Chroma settings are not loaded");
                return false;
            }

            //Saves settings to motor
            if (!Hs.Instance.Transport.IsNullOrDisconnected)
                Hs.Instance.Transport.SendSettings();
            else
                LastError = "Motor not connected: Can't set config";

            try
            {
                _configFile.Save(Settings.Default.ChromaConfigPath);
                return true;
            }
            catch (Exception ex)
            {
                LastError = $"Could not save config. Error: {ex.ToString()}";
                Hs.Instance._log.LogException("Error Saving Chroma Config", ex);
                return false;
            }
        }

        public string GetSetting(string settingName)
        {
            if (!SettingsLoaded)
            {
                Hs.Instance._log.LogError("ConfigRetriever", "Chroma settings are not loaded");
                return "";
            }

            try
            {
                if (_configFile == null)
                    LoadSettings();
                return _configFile?.Descendants("setting").FirstOrDefault(el =>
                        el.Attribute("name")?.Value == settingName)?.Value;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                Hs.Instance._log.LogException("Error Reading Chroma Config", ex);
                return null;
            }
        }

        public bool SetSetting(string settingName, string value)
        {
            if (!SettingsLoaded)
            {
                Hs.Instance._log.LogError("ConfigRetriever", "Chroma settings are not loaded");
                return false;
            }

            try
            {
                _configFile.Descendants("setting").Single(el =>
                    el.Attribute("name")?.Value == settingName).Element("value")
                    ?.SetValue(value);
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    Add(settingName, value);
                }
                catch (Exception ex2)
                {
                    LastError = ex2.Message;
                    Hs.Instance._log.LogException("Error Writing to Chroma Config", ex);
                    return false;
                }

                LastError = ex.Message;
                Hs.Instance._log.LogException("Error Writing to Chroma Config", ex);
                return false;
            }
        }

        private void Add(string settingName, string value)
        {
            try
            {
                var element = new XElement(
                    "setting",
                    new XAttribute("name", settingName),
                    new XAttribute("serializeAs", "String"),
                    new XElement("value",
                        value));
                _configFile.Element("userSettings")?.Descendants().First().Add(element);
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                Hs.Instance._log.LogException("Error Adding to Chroma Config", ex);
            }
        }

        public List<ChromaSetting> SettingsList { get; private set; } = new List<ChromaSetting>();

        public class ChromaSetting
        {
            public string SettingName { get; set; }
            public string SettingValue { get; set; }
        }
    }
}