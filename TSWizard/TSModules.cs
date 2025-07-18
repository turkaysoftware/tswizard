﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TSWizard{
    internal class TSModules{
        // LINK SYSTEM
        // ======================================================================================================
        public class TS_LinkSystem{
            public static string
            website_link        = "https://www.turkaysoftware.com",
            twitter_x_link      = "https://x.com/turkaysoftware",
            instagram_link      = "https://www.instagram.com/erayturkayy/",
            github_link         = "https://github.com/turkaysoftware",
            //
            github_link_lt      = "https://raw.githubusercontent.com/turkaysoftware/tswizard/main/TSWizard/SoftwareVersion.txt",
            github_link_lr      = "https://github.com/turkaysoftware/tswizard/releases/latest",
            //
            ts_bmac             = "https://buymeacoffee.com/turkaysoftware";
        }
        // VERSIONS
        // ======================================================================================================
        public class TS_VersionEngine{
            public static string TS_SofwareVersion(int v_type, int v_mode){
                string version_mode = "";
                string versionSubstring = v_mode == 0 ? Application.ProductVersion.Substring(0, 5) : Application.ProductVersion.Substring(0, 7);
                switch (v_type){
                    case 0:
                        version_mode = v_mode == 0 ? $"{Application.ProductName} - v{versionSubstring}" : $"{Application.ProductName} - v{Application.ProductVersion.Substring(0, 7)}";
                        break;
                    case 1:
                        version_mode = $"v{versionSubstring}";
                        break;
                    case 2:
                        version_mode = versionSubstring;
                        break;
                    default:
                        break;
                }
                return version_mode;
            }
        }
        // TS MESSAGEBOX ENGINE
        // ======================================================================================================
        public static class TS_MessageBoxEngine{
            private static readonly Dictionary<int, KeyValuePair<MessageBoxButtons, MessageBoxIcon>> TSMessageBoxConfig = new Dictionary<int, KeyValuePair<MessageBoxButtons, MessageBoxIcon>>(){
                { 1, new KeyValuePair<MessageBoxButtons, MessageBoxIcon>(MessageBoxButtons.OK, MessageBoxIcon.Information) },       // Ok ve Bilgi
                { 2, new KeyValuePair<MessageBoxButtons, MessageBoxIcon>(MessageBoxButtons.OK, MessageBoxIcon.Warning) },           // Ok ve Uyarı
                { 3, new KeyValuePair<MessageBoxButtons, MessageBoxIcon>(MessageBoxButtons.OK, MessageBoxIcon.Error) },             // Ok ve Hata
                { 4, new KeyValuePair<MessageBoxButtons, MessageBoxIcon>(MessageBoxButtons.YesNo, MessageBoxIcon.Question) },       // Yes/No ve Soru
                { 5, new KeyValuePair<MessageBoxButtons, MessageBoxIcon>(MessageBoxButtons.YesNo, MessageBoxIcon.Information) },    // Yes/No ve Bilgi
                { 6, new KeyValuePair<MessageBoxButtons, MessageBoxIcon>(MessageBoxButtons.YesNo, MessageBoxIcon.Warning) },        // Yes/No ve Uyarı
                { 7, new KeyValuePair<MessageBoxButtons, MessageBoxIcon>(MessageBoxButtons.YesNo, MessageBoxIcon.Error) },          // Yes/No ve Hata
                { 8, new KeyValuePair<MessageBoxButtons, MessageBoxIcon>(MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) },    // Retry/Cancel ve Hata
                { 9, new KeyValuePair<MessageBoxButtons, MessageBoxIcon>(MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) }  // Yes/No/Cancel ve Soru
            };
            public static DialogResult TS_MessageBox(Form m_form, int m_mode, string m_message, string m_title = ""){
                m_form.BringToFront();
                //
                string m_box_title = string.IsNullOrEmpty(m_title) ? Application.ProductName : m_title;
                //
                MessageBoxButtons m_button = MessageBoxButtons.OK;
                MessageBoxIcon m_icon = MessageBoxIcon.Information;
                //
                if (TSMessageBoxConfig.ContainsKey(m_mode)){
                    var m_serialize = TSMessageBoxConfig[m_mode];
                    m_button = m_serialize.Key;
                    m_icon = m_serialize.Value;
                }
                //
                return MessageBox.Show(m_form, m_message, m_box_title, m_button, m_icon);
            }
        }
        // TS SOFTWARE COPYRIGHT DATE
        // ======================================================================================================
        public class TS_SoftwareCopyrightDate{
            public static string ts_scd_preloader = string.Format("\u00a9 {0}, {1}.", DateTime.Now.Year, Application.CompanyName);
        }
        // SETTINGS SAVE PATHS
        // ======================================================================================================
        public static string ts_df = Application.StartupPath;
        public static string ts_sf = ts_df + @"\" + Application.ProductName.Replace(" ", string.Empty) + "Settings.ini";
        public static string ts_settings_container = Path.GetFileNameWithoutExtension(ts_sf);
        // SETTINGS SAVE CLASS
        // ======================================================================================================
        public class TSSettingsSave{
            [DllImport("kernel32.dll")]
            private static extern int WritePrivateProfileString(string section, string key, string val, string filePath);
            [DllImport("kernel32.dll")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
            private readonly string _settingFilePath;
            public TSSettingsSave(string filePath) { _settingFilePath = filePath; }
            public string TSReadSettings(string episode, string settingName){
                StringBuilder stringBuilder = new StringBuilder(4096);
                GetPrivateProfileString(episode, settingName, string.Empty, stringBuilder, 4096, _settingFilePath);
                return stringBuilder.ToString();
            }
            public int TSWriteSettings(string episode, string settingName, string value){
                return WritePrivateProfileString(episode, settingName, value, _settingFilePath);
            }
        }
        // READ LANG PATHS
        // ======================================================================================================
        public static string ts_lf = $"tswizard_langs";                     // Main Path
        public static string ts_lang_en = ts_lf + @"\English.ini";          // English      | en
        public static string ts_lang_tr = ts_lf + @"\Turkish.ini";          // Turkish      | tr
        // READ LANG CLASS
        // ======================================================================================================
        public class TSGetLangs{
            [DllImport("kernel32.dll")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
            private readonly string _readFilePath;
            public TSGetLangs(string filePath) { _readFilePath = filePath; }
            public string TSReadLangs(string episode, string settingName){
                StringBuilder stringBuilder = new StringBuilder(4096);
                GetPrivateProfileString(episode, settingName, string.Empty, stringBuilder, 4096, _readFilePath);
                return stringBuilder.ToString();
            }
        }
        // TS STRING ENCODER
        // ======================================================================================================
        public static string TS_String_Encoder(string get_text){
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(get_text)).Trim();
        }
        // TS THEME ENGINE
        // ======================================================================================================
        public class TS_ThemeEngine{
            // LIGHT THEME COLORS
            // ====================================
            public static readonly Dictionary<string, Color> LightTheme = new Dictionary<string, Color>{
                // TS PRELOADER
                { "TSBT_BGColor", Color.FromArgb(236, 242, 248) },
                { "TSBT_BGColor2", Color.White },
                { "TSBT_AccentColor", Color.FromArgb(194, 110, 10) },
                { "TSBT_LabelColor1", Color.FromArgb(51, 51, 51) },
                { "TSBT_LabelColor2", Color.FromArgb(100, 100, 100) },
                { "TSBT_CloseBG", Color.FromArgb(200, 255, 255, 255) },
                // HEADER MENU COLOR MODE
                { "HeaderBGColor", Color.FromArgb(236, 242, 248) },
                { "HeaderFEColor", Color.FromArgb(51, 51, 51) },
                { "HeaderBGColorMain", Color.White },
                { "HeaderFEColorMain", Color.FromArgb(51, 51, 51) },
                { "HeaderColorAccent", Color.FromArgb(194, 110, 10) },
                // UI COLOR
                { "UIBGColor1", Color.FromArgb(236, 242, 248) },
                { "UIBGColor2", Color.White },
                { "UIFEColor1", Color.FromArgb(100, 100, 100) },
                { "UIFEColor2", Color.FromArgb(51, 51, 51) },
                { "UIFEColor3", Color.FromArgb(194, 110, 10) },
                { "UIFEColor4", Color.FromArgb(210, 120, 13) },
                { "UIFEColor5", Color.White },
                //
                { "MainAccentColor", Color.FromArgb(194, 110, 10) },
                { "MainAccentColorHover", Color.FromArgb(210, 120, 13) },
                //
                { "DataGridBGColor", Color.White },
                { "DataGridFEColor", Color.FromArgb(51, 51, 51) },
                { "DataGridColor", Color.FromArgb(226, 226, 226) },
                { "DataGridAlternatingColor", Color.FromArgb(236, 242, 248) },
                { "DataGridSelectionColor", Color.White },
                { "DataGridSelectionBGColor", Color.White },
                { "DataGridSelectionFEColor", Color.White },
                // SOFTWARE COLORS
                { "AstelFE", Color.FromArgb(14, 76, 56) },
                { "AstelFEHover", Color.FromArgb(18, 94, 70) },
                { "GlowFE", Color.FromArgb(54, 95, 146) },
                { "GlowFEHover", Color.FromArgb(67, 116, 177) },
                { "VimeraFE", Color.FromArgb(105, 81, 147) },
                { "VimeraFEHover", Color.FromArgb(120, 93, 167) },
                { "YamiraFE", Color.FromArgb(114, 19, 42) },
                { "YamiraFEHover", Color.FromArgb(136, 24, 52) },
                // DYNAMIC BUTTON COLOR
                { "BtnLaunchBG", Color.FromArgb(47, 98, 131) },
                { "BtnLaunchBGHover", Color.FromArgb(53, 110, 147) },
                { "BtnUpdateBG", Color.FromArgb(120, 12, 222) },
                { "BtnUpdateBGHover", Color.FromArgb(132, 15, 241) },
                { "BtnDownloadBG", Color.FromArgb(25, 126, 45) },
                { "BtnDownloadBGHover", Color.FromArgb(30, 147, 53) },
                { "BtnDeleteBG", Color.FromArgb(153, 28, 58) },
                { "BtnDeleteBGHover", Color.FromArgb(176, 34, 68) },
                { "BtnFEColor1", Color.WhiteSmoke },
                { "BtnFEColor2", Color.FromArgb(25, 31, 42) }
            };
            // DARK THEME COLORS
            // ====================================
            public static readonly Dictionary<string, Color> DarkTheme = new Dictionary<string, Color>{
                // TS PRELOADER
                { "TSBT_BGColor", Color.FromArgb(21, 23, 32) },
                { "TSBT_BGColor2", Color.FromArgb(25, 31, 42) },
                { "TSBT_AccentColor", Color.FromArgb(245, 136, 6) },
                { "TSBT_LabelColor1", Color.WhiteSmoke },
                { "TSBT_LabelColor2", Color.FromArgb(176, 184, 196) },
                { "TSBT_CloseBG", Color.FromArgb(210, 25, 31, 42) },
                // HEADER MENU COLOR MODE
                { "HeaderBGColor", Color.FromArgb(21, 23, 32) },
                { "HeaderFEColor", Color.WhiteSmoke },
                { "HeaderBGColorMain", Color.FromArgb(25, 31, 42) },
                { "HeaderFEColorMain", Color.FromArgb(222, 222, 222) },
                { "HeaderColorAccent", Color.FromArgb(245, 136, 6) },
                // UI COLOR
                { "UIBGColor1", Color.FromArgb(21, 23, 32) },
                { "UIBGColor2", Color.FromArgb(25, 31, 42) },
                { "UIFEColor1", Color.FromArgb(176, 184, 196) },
                { "UIFEColor2", Color.WhiteSmoke },
                { "UIFEColor3", Color.FromArgb(245, 136, 6) },
                { "UIFEColor4", Color.FromArgb(255, 158, 41) },
                { "UIFEColor5", Color.FromArgb(21, 23, 32) },
                //
                { "MainAccentColor", Color.FromArgb(245, 136, 6) },
                { "MainAccentColorHover", Color.FromArgb(255, 158, 41) },
                //
                { "DataGridBGColor", Color.FromArgb(25, 31, 42) },
                { "DataGridFEColor", Color.WhiteSmoke },
                { "DataGridColor", Color.FromArgb(36, 45, 61) },
                { "DataGridAlternatingColor", Color.FromArgb(21, 23, 32) },
                { "DataGridSelectionColor", Color.WhiteSmoke },
                { "DataGridSelectionBGColor", Color.FromArgb(25, 31, 42) },
                { "DataGridSelectionFEColor", Color.WhiteSmoke },
                // SOFTWARE COLORS
                { "AstelFE", Color.FromArgb(35, 180, 64) },
                { "AstelFEHover", Color.FromArgb(38, 201, 71) },
                { "GlowFE", Color.FromArgb(84, 179, 241) },
                { "GlowFEHover", Color.FromArgb(109, 196, 253) },
                { "VimeraFE", Color.FromArgb(173, 134, 237) },
                { "VimeraFEHover", Color.FromArgb(194, 156, 255) },
                { "YamiraFE", Color.FromArgb(241, 43, 91) },
                { "YamiraFEHover", Color.FromArgb(252, 62, 108) },
                // DYNAMIC BUTTON COLOR
                { "BtnLaunchBG", Color.FromArgb(47, 98, 131) },
                { "BtnLaunchBGHover", Color.FromArgb(53, 110, 147) },
                { "BtnUpdateBG", Color.FromArgb(120, 12, 222) },
                { "BtnUpdateBGHover", Color.FromArgb(132, 15, 241) },
                { "BtnDownloadBG", Color.FromArgb(25, 126, 45) },
                { "BtnDownloadBGHover", Color.FromArgb(30, 147, 53) },
                { "BtnDeleteBG", Color.FromArgb(153, 28, 58) },
                { "BtnDeleteBGHover", Color.FromArgb(176, 34, 68) },
                { "BtnFEColor1", Color.WhiteSmoke },
                { "BtnFEColor2", Color.FromArgb(25, 31, 42) }
            };
            // THEME SWITCHER
            // ====================================
            public static Color ColorMode(int theme, string key){
                if (theme == 0){
                    return DarkTheme.ContainsKey(key) ? DarkTheme[key] : Color.Transparent;
                }else if (theme == 1){
                    return LightTheme.ContainsKey(key) ? LightTheme[key] : Color.Transparent;
                }
                return Color.Transparent;
            }
        }
        // DYNAMIC SIZE COUNT ALGORITHM
        // ======================================================================================================
        public static string TS_FormatSize(double bytes){
            string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int suffixIndex = 0;
            double doubleBytes = bytes;
            while (doubleBytes >= 1024 && suffixIndex < suffixes.Length - 1){
                doubleBytes /= 1024;
                suffixIndex++;
            }
            return $"{doubleBytes:0.##} {suffixes[suffixIndex]}";
        }
        public static double TS_FormatSizeNoType(double bytes){
            while (bytes >= 1024){
                bytes /= 1024;
            }
            return Math.Round(bytes, 2);
        }
        // INTERNET CONNECTION STATUS
        // ======================================================================================================
        public static bool IsNetworkCheck(){
            try{
                HttpWebRequest server_request = (HttpWebRequest)WebRequest.Create("http://clients3.google.com/generate_204");
                server_request.KeepAlive = false;
                server_request.Timeout = 2500;
                using (var server_response = (HttpWebResponse)server_request.GetResponse()){
                    return server_response.StatusCode == HttpStatusCode.NoContent;
                }
            }catch{
                return false;
            }
        }
        // TITLE BAR SETTINGS DWM API
        // ======================================================================================================
        [DllImport("DwmApi")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);
        // DPI AWARE
        // ======================================================================================================
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();
    }
}