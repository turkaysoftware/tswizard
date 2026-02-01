// ======================================================================================================
// TS Wizard - Türkay Software Solutions
// © Copyright 2025-2026, Eray Türkay.
// Project Type: Open Source
// License: MIT License
// Website: https://www.turkaysoftware.com/ts-wizard
// GitHub: https://github.com/turkaysoftware/tswizard
// ======================================================================================================

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
// TS MODULES
using static TSWizard.TSModules;

namespace TSWizard{
    public partial class TSWizardMain : Form{
        public TSWizardMain(){
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            //
            NotifyMode.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            NotifyMode.Text = Application.ProductName;
            NotifyMode.ContextMenuStrip = NotifyMenu;
            NotifyMode.DoubleClick += (s, e) => Context_show_software();
            // LANGUAGE SET MODES
            // ==================
            arabicToolStripMenuItem.Tag = "ar";
            chineseToolStripMenuItem.Tag = "zh";
            englishToolStripMenuItem.Tag = "en";
            dutchToolStripMenuItem.Tag = "nl";
            frenchToolStripMenuItem.Tag = "fr";
            germanToolStripMenuItem.Tag = "de";
            hindiToolStripMenuItem.Tag = "hi";
            italianToolStripMenuItem.Tag = "it";
            japaneseToolStripMenuItem.Tag = "ja";
            koreanToolStripMenuItem.Tag = "ko";
            polishToolStripMenuItem.Tag = "pl";
            portugueseToolStripMenuItem.Tag = "pt";
            russianToolStripMenuItem.Tag = "ru";
            spanishToolStripMenuItem.Tag = "es";
            turkishToolStripMenuItem.Tag = "tr";
            // LANGUAGE SET EVENTS
            // ==================
            arabicToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            chineseToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            englishToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            dutchToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            frenchToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            germanToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            hindiToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            italianToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            japaneseToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            koreanToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            polishToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            portugueseToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            russianToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            spanishToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            turkishToolStripMenuItem.Click += LanguageToolStripMenuItem_Click;
            //
            SystemEvents.UserPreferenceChanged += (s, e) => TSUseSystemTheme();
        }
        // GLOBAL VARIABLES
        // ======================================================================================================
        public static string lang, lang_path;
        public static int theme, ts_active_software;
        public static string[] ts_softwares_list = { "Astel", "Encryphix", "Glow", "VCardix", "Vimera", "Yamira", "Zafuse" };
        public static string __astel_version = null, __encryphix_version = null, __glow_version = null, __vcardix_version = null, __vimera_version = null, __yamira_version = null, __zafuse_version = null;
        // LOCAL VARIABLES | 0 = Astel / 1 = Encryphix / 2 = Glow / 3 = VCardix / 4 = Vimera / 5 = Yamira / 6 = Zafuse*
        // ======================================================================================================
        static readonly string ts_softwares_root_path = Path.Combine(StartupPath, "ts_softwares");
        static readonly string[] subfolders = { "ts_astel", "ts_encryphix", "ts_glow", "ts_vcardix", "ts_vimera", "ts_yamira", "ts_zafuse" };
        static readonly string[] ts_softwares_location = subfolders.Select(s => $"{ts_softwares_root_path}\\{s}").ToArray();
        static readonly string[] ts_softwares_location_exe_name = { "Astel_x64.exe", "Encryphix_x64.exe", "Glow_x64.exe", "VCardix_x64.exe", "Vimera_x64.exe", "Yamira_x64.exe", "Zafuse_x64.exe" };
        readonly string updater_exe_name = Path.Combine(StartupPath, "TSWizardUpdater_x64.exe");
        //
        readonly string[] ts_software_version_links ={
            "https://raw.githubusercontent.com/turkaysoftware/astel/main/Astel/SoftwareVersion.txt",            // Astel
            "https://raw.githubusercontent.com/turkaysoftware/encryphix/main/Encryphix/SoftwareVersion.txt",    // Encryphix
            "https://raw.githubusercontent.com/turkaysoftware/glow/main/Glow/SoftwareVersion.txt",              // Glow
            "https://raw.githubusercontent.com/turkaysoftware/vcardix/main/VCardix/SoftwareVersion.txt",        // VCardix
            "https://raw.githubusercontent.com/turkaysoftware/vimera/main/Vimera/SoftwareVersion.txt",          // Vimera
            "https://raw.githubusercontent.com/turkaysoftware/yamira/main/Yamira/SoftwareVersion.txt",          // Yamira
            "https://raw.githubusercontent.com/turkaysoftware/zafuse/main/Zafuse/SoftwareVersion.txt"           // Zafuse
        };
        //
        int __astel_i_status = 0, __encryphix_i_status = 0, __glow_i_status = 0, __vcardix_i_status = 0, __vimera_i_status = 0, __yamira_i_status = 0, __zafuse_i_status = 0, startup_status, behavior_mode_status, update_notifications_status, themeSystem;
        bool __astel_u_status = false, __encryphix_u_status = false, __glow_u_status = false, __vcardix_u_status = false, __vimera_u_status = false, __yamira_u_status = false, __zafuse_u_status = false, loop_status = true, exit_mode = false;
        readonly List<string> sUpdateNotList = new List<string>();
        private FormWindowState _lastState;
        readonly Architecture os_arch = RuntimeInformation.ProcessArchitecture;
        // ======================================================================================================
        // COLOR MODES / Index Mode | 0 = Dark - 1 = Light
        static readonly List<Color> header_colors = new List<Color>() { Color.Transparent, Color.Transparent, Color.Transparent };
        // HEADER SETTINGS
        // ======================================================================================================
        private class HeaderMenuColors : ToolStripProfessionalRenderer{
            public HeaderMenuColors() : base(new HeaderColors()){ }
            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e){ e.ArrowColor = header_colors[1]; base.OnRenderArrow(e); }
            protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e){
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                float dpiScale = g.DpiX / 96f;
                Rectangle rect = e.ImageRectangle;
                using (Pen anti_alias_pen = new Pen(header_colors[2], 2.2f * dpiScale)){
                    anti_alias_pen.StartCap = LineCap.Round;
                    anti_alias_pen.EndCap = LineCap.Round;
                    anti_alias_pen.LineJoin = LineJoin.Round;
                    PointF p1 = new PointF(rect.Left + rect.Width * 0.18f, rect.Top + rect.Height * 0.52f);
                    PointF p2 = new PointF(rect.Left + rect.Width * 0.38f, rect.Top + rect.Height * 0.72f);
                    PointF p3 = new PointF(rect.Left + rect.Width * 0.78f, rect.Top + rect.Height * 0.28f);
                    g.DrawLines(anti_alias_pen, new[] { p1, p2, p3 });
                }
            }
        }
        private class HeaderColors : ProfessionalColorTable{
            public override Color MenuItemSelected => header_colors[0];
            public override Color ToolStripDropDownBackground => header_colors[0];
            public override Color ImageMarginGradientBegin => header_colors[0];
            public override Color ImageMarginGradientEnd => header_colors[0];
            public override Color ImageMarginGradientMiddle => header_colors[0];
            public override Color MenuItemSelectedGradientBegin => header_colors[0];
            public override Color MenuItemSelectedGradientEnd => header_colors[0];
            public override Color MenuItemPressedGradientBegin => header_colors[0];
            public override Color MenuItemPressedGradientMiddle => header_colors[0];
            public override Color MenuItemPressedGradientEnd => header_colors[0];
            public override Color MenuItemBorder => header_colors[0];
            public override Color CheckBackground => header_colors[0];
            public override Color ButtonSelectedBorder => header_colors[0];
            public override Color CheckSelectedBackground => header_colors[0];
            public override Color CheckPressedBackground => header_colors[0];
            public override Color MenuBorder => header_colors[0];
            public override Color SeparatorLight => header_colors[1];
            public override Color SeparatorDark => header_colors[1];
        }
        // LOAD SOFTWARE SETTINGS
        // ======================================================================================================
        private void RunSoftwareEngine(){
            // THEME - LANG - VIEW MODE PRELOADER
            // ======================================================================================================
            TSSettingsSave software_read_settings = new TSSettingsSave(ts_sf);
            //
            int theme_mode = int.TryParse(software_read_settings.TSReadSettings(ts_settings_container, "ThemeStatus"), out int the_status) && (the_status == 0 || the_status == 1 || the_status == 2) ? the_status : 1;
            if (theme_mode == 2) { themeSystem = 2; Theme_engine(GetSystemTheme(2)); } else Theme_engine(theme_mode);
            darkThemeToolStripMenuItem.Checked = theme_mode == 0;
            lightThemeToolStripMenuItem.Checked = theme_mode == 1;
            systemThemeToolStripMenuItem.Checked = theme_mode == 2;
            //
            string lang_mode = software_read_settings.TSReadSettings(ts_settings_container, "LanguageStatus");
            var languageFiles = new Dictionary<string, (object langResource, ToolStripMenuItem menuItem, bool fileExists)>{
                { "ar", (ts_lang_ar, arabicToolStripMenuItem, File.Exists(ts_lang_ar)) },
                { "zh", (ts_lang_zh, chineseToolStripMenuItem, File.Exists(ts_lang_zh)) },
                { "en", (ts_lang_en, englishToolStripMenuItem, File.Exists(ts_lang_en)) },
                { "nl", (ts_lang_nl, dutchToolStripMenuItem, File.Exists(ts_lang_nl)) },
                { "fr", (ts_lang_fr, frenchToolStripMenuItem, File.Exists(ts_lang_fr)) },
                { "de", (ts_lang_de, germanToolStripMenuItem, File.Exists(ts_lang_de)) },
                { "hi", (ts_lang_hi, hindiToolStripMenuItem, File.Exists(ts_lang_hi)) },
                { "it", (ts_lang_it, italianToolStripMenuItem, File.Exists(ts_lang_it)) },
                { "ja", (ts_lang_ja, japaneseToolStripMenuItem, File.Exists(ts_lang_ja)) },
                { "ko", (ts_lang_ko, koreanToolStripMenuItem, File.Exists(ts_lang_ko)) },
                { "pl", (ts_lang_pl, polishToolStripMenuItem, File.Exists(ts_lang_pl)) },
                { "pt", (ts_lang_pt, portugueseToolStripMenuItem, File.Exists(ts_lang_pt)) },
                { "ru", (ts_lang_ru, russianToolStripMenuItem, File.Exists(ts_lang_ru)) },
                { "es", (ts_lang_es, spanishToolStripMenuItem, File.Exists(ts_lang_es)) },
                { "tr", (ts_lang_tr, turkishToolStripMenuItem, File.Exists(ts_lang_tr)) },
            };
            foreach (var langLoader in languageFiles) { langLoader.Value.menuItem.Enabled = langLoader.Value.fileExists; }
            var (langResource, selectedMenuItem, _) = languageFiles.ContainsKey(lang_mode) ? languageFiles[lang_mode] : languageFiles["en"];
            Lang_engine(Convert.ToString(langResource), lang_mode);
            selectedMenuItem.Checked = true;
            //
            string startup_mode = software_read_settings.TSReadSettings(ts_settings_container, "StartupStatus");
            startup_status = int.TryParse(startup_mode, out int str_status) && (str_status == 0 || str_status == 1) ? str_status : 0;
            WindowState = startup_status == 1 ? FormWindowState.Maximized : FormWindowState.Normal;
            windowedToolStripMenuItem.Checked = startup_status == 0;
            fullScreenToolStripMenuItem.Checked = startup_status == 1;
            //
            string window_behavior_mode = software_read_settings.TSReadSettings(ts_settings_container, "WindowBehavior");
            behavior_mode_status = int.TryParse(window_behavior_mode, out int wbm_status) && (wbm_status == 0 || wbm_status == 1) ? wbm_status : 0;
            iconStatusToolStripMenuItem.Checked = behavior_mode_status == 1;
            closeSoftwareToolStripMenuItem.Checked = behavior_mode_status == 0;
            //
            string update_notifications = software_read_settings.TSReadSettings(ts_settings_container, "UpdateNotifications");
            update_notifications_status = int.TryParse(update_notifications, out int un_status) && (un_status == 0 || un_status == 1) ? un_status : 0;
            notificationOnToolStripMenuItem.Checked = update_notifications_status == 1;
            notificationOffToolStripMenuItem.Checked = update_notifications_status == 0;
            //
            Set_random_header_image();
            //
            MPanelAstelImage.BackgroundImage = Properties.Resources.astel_banner;
            MPanelEncryphixImage.BackgroundImage = Properties.Resources.encryphix_banner;
            MPanelGlowImage.BackgroundImage = Properties.Resources.glow_banner;
            MPanelVCardixImage.BackgroundImage = Properties.Resources.vcardix_banner;
            MPanelVimeraImage.BackgroundImage = Properties.Resources.vimera_banner;
            MPanelYamiraImage.BackgroundImage = Properties.Resources.yamira_banner;
            MPanelZafuseImage.BackgroundImage = Properties.Resources.zafuse_banner;
            //
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            MPanelAstelWizardBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
            MPanelEncryphixWizardBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
            MPanelGlowWizardBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
            MPanelVCardixWizardBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
            MPanelVimeraWizardBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
            MPanelYamiraWizardBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
            MPanelZafuseWizardBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
            //
            MPanelAstelInText.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
            MPanelEncryphixInText.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
            MPanelGlowInText.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
            MPanelVCardixInText.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
            MPanelVimeraInText.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
            MPanelYamiraInText.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
            MPanelZafuseInText.Text = software_lang.TSReadLangs("TSWizardUI", "s_loader_text");
        }
        // SET RANDOM HEADER IMAGE
        // ======================================================================================================
        private void Set_random_header_image(){
            Image[] header_images = new Image[]{
                Properties.Resources.mb_1,
                Properties.Resources.mb_2,
                Properties.Resources.mb_3,
                Properties.Resources.mb_4,
                Properties.Resources.mb_5,
                Properties.Resources.mb_6,
                Properties.Resources.mb_7,
                Properties.Resources.mb_8,
            };
            if (Program.ts_pre_image_mode){
                HeaderBanner.BackgroundImage = header_images[0];
            }else{
                Random random_generate = new Random();
                HeaderBanner.BackgroundImage = header_images[random_generate.Next(header_images.Length)];
            }
        }
        // TOOLTIP SET CONFIG
        // ======================================================================================================
        private void MainToolTip_Draw(object sender, DrawToolTipEventArgs e) { e.DrawBackground(); e.DrawBorder(); e.DrawText(); }
        // DYNAMIC RESIZE UI
        // ======================================================================================================
        private void TSWizardMain_Resize(object sender, EventArgs e){
            if (_lastState == FormWindowState.Minimized && this.WindowState == FormWindowState.Normal){
                int scrollPos = InFLP.VerticalScroll.Value;
                InFLP.SuspendLayout();
                InFLP.PerformLayout();
                InFLP.ResumeLayout();
                InFLP.VerticalScroll.Value = Math.Min(scrollPos, InFLP.VerticalScroll.Maximum);
            }
            _lastState = this.WindowState;
        }
        // LOAD
        // ======================================================================================================
        private void TS_Wizard_Load(object sender, EventArgs e){
            Text = TS_VersionEngine.TS_SofwareVersion(0);
            HeaderMenu.Cursor = Cursors.Hand;
            NotifyMenu.Cursor = Cursors.Hand;
            // RUN ENGINE
            RunSoftwareEngine();
            // UPDATE CHECKER
            Task softwareUpdateCheck = Task.Run(() => Software_update_check(0));
            // AUTOMATIC UPDATE CONTROLLER
            Task ts_avaible_update = Task.Run(() => Auto_check_avaible_update());
            // START CHECK MODULES
            Task ts_wizard_start_module = Task.Run(() => {
                if (update_notifications_status == 1){
                    Ts_w_start_module(true);
                }else{
                    Ts_w_start_module(false);
                }
            });
        }
        // TS WIZARD START MODULE
        // ======================================================================================================
        private void Ts_w_start_module(bool __notifi_mode){
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            try{
                var wizardButtons = new[] { MPanelAstelWizardBtn, MPanelEncryphixWizardBtn, MPanelGlowWizardBtn, MPanelVCardixWizardBtn, MPanelVimeraWizardBtn, MPanelYamiraWizardBtn, MPanelZafuseWizardBtn };
                var removeButtons = new[] { MPanelAstelRemoveBtn, MPanelEncryphixRemoveBtn, MPanelGlowRemoveBtn, MPanelVCardixRemoveBtn, MPanelVimeraRemoveBtn, MPanelYamiraRemoveBtn, MPanelZafuseRemoveBtn };
                for (int i = 0; i < ts_softwares_location.Length; i++){
                    string dirPath = ts_softwares_location[i];
                    string exeName = File.Exists(Path.Combine(ts_softwares_location[i], ts_softwares_location_exe_name[i])) ? ts_softwares_location_exe_name[i] : (File.Exists(Path.Combine(ts_softwares_location[i], ts_softwares_location_exe_name[i].Replace("_x64", string.Empty))) ? ts_softwares_location_exe_name[i].Replace("_x64", string.Empty) : ts_softwares_location_exe_name[i]);
                    string exePath = Path.Combine(dirPath, exeName);
                    //
                    var wizardBtn = wizardButtons[i];
                    var removeBtn = removeButtons[i];
                    if (File.Exists(exePath)){
                        Software_update_checker(i, TS_VersionParser.ParseUINormalize(FileVersionInfo.GetVersionInfo(exePath).FileVersion), true, __notifi_mode);
                        removeBtn.Enabled = true;
                    }else{
                        Software_update_checker(i, "0.0.0.0", false, __notifi_mode);
                        wizardBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_download");
                        removeBtn.Enabled = false;
                    }
                }
                if (__notifi_mode){
                    Ts_notifi_send_module();
                }
            }catch (Exception){ }
            // Enable Software Details Page Buttons
            MPanelAstelLinkText.Enabled = true;
            MPanelEncryphixLinkText.Enabled = true;
            MPanelGlowLinkText.Enabled = true;
            MPanelVCardixLinkText.Enabled = true;
            MPanelVimeraLinkText.Enabled = true;
            MPanelYamiraLinkText.Enabled = true;
            MPanelZafuseLinkText.Enabled = true;
            checkForSoftwareUpdateToolStripMenuItem.Enabled = true;
        }
        // TIMED AUTOMATIC UPDATE CONTROLLER
        // ======================================================================================================
        private void Auto_check_avaible_update(){
            try{
                while (loop_status){
                    Thread.Sleep(1800000); // 30 min
                    if (WindowState == FormWindowState.Minimized){
                        Ts_w_start_module(true);
                    }else{
                        Ts_w_start_module(false);
                    }
                }
            }catch (Exception){ }
        }
        // SOFTWARE MODULE VERSION CHECKER
        // ======================================================================================================
        private void Software_update_checker(int __software_mode, string __software_version, bool __installed_status, bool __notify_mode){
            try{
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                //
                using (WebClient getLastVersion = new WebClient()){
                    string last_version_get = getLastVersion.DownloadString(ts_software_version_links[__software_mode]).Split('=')[1].Trim();
                    Version client_ver = Version.Parse(TS_VersionParser.ParseUINormalize(__software_version));
                    Version last_ver = Version.Parse(TS_VersionParser.ParseUINormalize(last_version_get));
                    bool isUpdateAvailable = client_ver < last_ver;
                    //
                    string[] softwareVersions = new string[7] { __astel_version, __encryphix_version, __glow_version, __vcardix_version, __vimera_version, __yamira_version, __zafuse_version };
                    softwareVersions[__software_mode] = last_version_get;
                    __astel_version = softwareVersions[0];
                    __encryphix_version = softwareVersions[1];
                    __glow_version = softwareVersions[2];
                    __vcardix_version = softwareVersions[3];
                    __vimera_version = softwareVersions[4];
                    __yamira_version = softwareVersions[5];
                    __zafuse_version = softwareVersions[6];
                    //
                    var wizardButtons = new Button[] { MPanelAstelWizardBtn, MPanelEncryphixWizardBtn, MPanelGlowWizardBtn, MPanelVCardixWizardBtn, MPanelVimeraWizardBtn, MPanelYamiraWizardBtn, MPanelZafuseWizardBtn };
                    var removeButtons = new Button[] { MPanelAstelRemoveBtn, MPanelEncryphixRemoveBtn, MPanelGlowRemoveBtn, MPanelVCardixRemoveBtn, MPanelVimeraRemoveBtn, MPanelYamiraRemoveBtn, MPanelZafuseRemoveBtn };
                    //
                    var soft_descriptions = new[]{
                        software_lang.TSReadLangs("TSWizardUI", "s_t_astel"),
                        software_lang.TSReadLangs("TSWizardUI", "s_t_encryphix"),
                        software_lang.TSReadLangs("TSWizardUI", "s_t_glow"),
                        software_lang.TSReadLangs("TSWizardUI", "s_t_vcardix"),
                        software_lang.TSReadLangs("TSWizardUI", "s_t_vimera"),
                        software_lang.TSReadLangs("TSWizardUI", "s_t_yamira"),
                        software_lang.TSReadLangs("TSWizardUI", "s_t_zafuse"),
                    };
                    // UI Elements
                    var infoLabels = new[] { MPanelAstelInText, MPanelEncryphixInText, MPanelGlowInText, MPanelVCardixInText, MPanelVimeraInText, MPanelYamiraInText, MPanelZafuseInText };
                    //
                    string statusText;
                    if (!__installed_status)
                        statusText = string.Format(software_lang.TSReadLangs("TSWizardUI", "s_u_not_downloaded"), last_version_get);
                    else if (isUpdateAvailable)
                        statusText = string.Format(software_lang.TSReadLangs("TSWizardUI", "s_u_available"), __software_version, "\n", last_version_get);
                    else
                        statusText = string.Format(software_lang.TSReadLangs("TSWizardUI", "s_u_no_available"), last_version_get, "\n", TS_FormatSize(new DirectoryInfo(ts_softwares_location[__software_mode]).GetFiles("*", SearchOption.AllDirectories).Sum(f => f.Length)));
                    //
                    if (__software_mode >= 0 && __software_mode < soft_descriptions.Length){
                        infoLabels[__software_mode].Text = $"{soft_descriptions[__software_mode]}\n{statusText}";
                    }
                    //
                    wizardButtons[__software_mode].Text = isUpdateAvailable ? software_lang.TSReadLangs("TSWizardUI", "s_update") : software_lang.TSReadLangs("TSWizardUI", "s_launch");
                    removeButtons[__software_mode].Enabled = true;
                    // -------------
                    int __colorStatus = 0;
                    if (isUpdateAvailable)
                        __colorStatus = __installed_status ? 2 : 1;
                        Dynamic_button_colors(__software_mode, __colorStatus);
                    if (__colorStatus != 0)
                        Dynamic_button_colors(__software_mode, __colorStatus);
                    //
                    if (__installed_status){
                        switch (__software_mode){
                            case 0: __astel_i_status = 1; Sc_v_mode(0, true); break;
                            case 1: __encryphix_i_status = 1; Sc_v_mode(1, true); break;
                            case 2: __glow_i_status = 1; Sc_v_mode(2, true); break;
                            case 3: __vcardix_i_status = 1; Sc_v_mode(3, true); break;
                            case 4: __vimera_i_status = 1; Sc_v_mode(4, true); break;
                            case 5: __yamira_i_status = 1; Sc_v_mode(5, true); break;
                            case 6: __zafuse_i_status = 1; Sc_v_mode(6, true); break;
                        }
                    }
                    //
                    bool updateStatus = (__installed_status && isUpdateAvailable);
                    switch (__software_mode){
                        case 0: __astel_u_status = updateStatus; break;
                        case 1: __encryphix_u_status = updateStatus; break;
                        case 2: __glow_u_status = updateStatus; break;
                        case 3: __vcardix_u_status = updateStatus; break;
                        case 4: __vimera_u_status = updateStatus; break;
                        case 5: __yamira_u_status = updateStatus; break;
                        case 6: __zafuse_u_status = updateStatus; break;
                    }
                    if (__notify_mode && updateStatus){
                        sUpdateNotList.Add(ts_softwares_list[__software_mode]);
                    }
                }
            }catch (Exception){ }
        }
        // NOTIFICATION SENDİNG MODULE
        // ======================================================================================================
        private void Ts_notifi_send_module(){
            try{
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                string messageWrapper;
                var contentTemplate = software_lang.TSReadLangs("TSWizardUI", "s_u_notifi_content");
                messageWrapper = sUpdateNotList.Count == 1 ? string.Format(contentTemplate, sUpdateNotList[0]) : string.Format(contentTemplate, string.Join(", ", sUpdateNotList.Take(sUpdateNotList.Count - 1)) + " " + string.Format(software_lang.TSReadLangs("TSWizardUI", "s_u_notifi_parser"), sUpdateNotList.Last()));
                sUpdateNotList.Clear();
                NotifyMode.ShowBalloonTip(5000, software_lang.TSReadLangs("TSWizardUI", "s_u_notifi_title"), messageWrapper, ToolTipIcon.Info);
            }catch (Exception){ }
        }
        // CREATE A SHORTCUT MODULE
        // ======================================================================================================
        private void Sc_v_mode(int __s_software, bool __s_mode){
            Control[] sc_btns = { MPanelAstelShortcutBtn, MPanelEncryphixShortcutBtn, MPanelGlowShortcutBtn, MPanelVCardixShortcutBtn, MPanelVimeraShortcutBtn, MPanelYamiraShortcutBtn, MPanelZafuseShortcutBtn };
            //
            if (__s_software >= 0 && __s_software < sc_btns.Length){
                sc_btns[__s_software].Enabled = __s_mode;
            }
        }
        private void Handle_shortcut_config(int softwareIndex){
            string get_s_name = ts_softwares_location_exe_name[softwareIndex].Trim();
            if (os_arch == Architecture.Arm64)
                get_s_name = get_s_name.Replace("x64", "arm64");
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            DialogResult sc_create_message = TS_MessageBoxEngine.TS_MessageBox(this, 5, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_sc_content"), ts_softwares_list[softwareIndex]));
            if (sc_create_message == DialogResult.Yes){
                Sc_create_script(Path.Combine(ts_softwares_location[softwareIndex], get_s_name), softwareIndex);
            }
        }
        private void MPanelAstelShortcutBtn_Click(object sender, EventArgs e){ Handle_shortcut_config(0); }
        private void MPanelEncryphixShortcutBtn_Click(object sender, EventArgs e) { Handle_shortcut_config(1); }
        private void MPanelGlowShortcutBtn_Click(object sender, EventArgs e){ Handle_shortcut_config(2); }
        private void MPanelVCardixShortcutBtn_Click(object sender, EventArgs e) { Handle_shortcut_config(3); }
        private void MPanelVimeraShortcutBtn_Click(object sender, EventArgs e){ Handle_shortcut_config(4); }
        private void MPanelYamiraShortcutBtn_Click(object sender, EventArgs e){ Handle_shortcut_config(5); }
        private void MPanelZafuseShortcutBtn_Click(object sender, EventArgs e){ Handle_shortcut_config(6); }
        private void Sc_create_script(string __target_exe_path, int __s_mode){
            int scrollPos = InFLP.VerticalScroll.Value;
            InFLP.SuspendLayout();
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            try{
                string desktop_location = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string shortcut_location = Path.Combine(desktop_location, ts_softwares_list[__s_mode] + ".lnk");
                string hover_message = Application.ProductName + " - " + ts_softwares_list[__s_mode];
                //
                Type shell_type = Type.GetTypeFromProgID("WScript.Shell");
                object shell_object = Activator.CreateInstance(shell_type);
                object shortcut_config = shell_type.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell_object, new object[] { shortcut_location });
                //
                Type shortcut_typer = shortcut_config.GetType();
                shortcut_typer.InvokeMember("TargetPath", BindingFlags.SetProperty, null, shortcut_config, new object[] { __target_exe_path });
                shortcut_typer.InvokeMember("WorkingDirectory", BindingFlags.SetProperty, null, shortcut_config, new object[] { Path.GetDirectoryName(__target_exe_path) });
                shortcut_typer.InvokeMember("WindowStyle", BindingFlags.SetProperty, null, shortcut_config, new object[] { 1 });
                shortcut_typer.InvokeMember("Description", BindingFlags.SetProperty, null, shortcut_config, new object[] { hover_message });
                shortcut_typer.InvokeMember("IconLocation", BindingFlags.SetProperty, null, shortcut_config, new object[] { __target_exe_path });
                shortcut_typer.InvokeMember("Save", BindingFlags.InvokeMethod, null, shortcut_config, null);
                //
                DialogResult sc_success_message = TS_MessageBoxEngine.TS_MessageBox(this, 5, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_sc_success"), ts_softwares_list[__s_mode], shortcut_location, "\n\n"));
                if (sc_success_message == DialogResult.Yes){
                    string sc_location = string.Format("/select, \"{0}\"", shortcut_location.Trim().Replace("/", @"\"));
                    ProcessStartInfo psi = new ProcessStartInfo("explorer.exe", sc_location);
                    Process.Start(psi);
                }
            }catch (Exception ex){
                TS_MessageBoxEngine.TS_MessageBox(this, 3, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_sc_error"), "\n\n", ex.Message));
            }finally{
                InFLP.ResumeLayout();
                InFLP.VerticalScroll.Value = Math.Min(scrollPos, InFLP.VerticalScroll.Maximum);
            }
        }
        // SOFTWARE DETAIL ROTATE BUTTONS
        // ======================================================================================================
        private void MPanelAstelLinkText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){ Launch_software_details_dynamic(0); }
        private void MPanelEncryphixLinkText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){ Launch_software_details_dynamic(1); }
        private void MPanelGlowLinkText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){ Launch_software_details_dynamic(2); }
        private void MPanelVCardixLinkText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){  Launch_software_details_dynamic(3); }
        private void MPanelVimeraLinkText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){ Launch_software_details_dynamic(4); }
        private void MPanelYamiraLinkText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){ Launch_software_details_dynamic(5); }
        private void MPanelZafuseLinkText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){ Launch_software_details_dynamic(6); }
        private void Launch_software_details_dynamic(int s_mode){
            int scrollPos = InFLP.VerticalScroll.Value;
            InFLP.SuspendLayout();
            ts_active_software = s_mode;
            try{
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                TS_SoftwareDetails tS_SoftwareDetails = new TS_SoftwareDetails();
                string tsw_about_name = "tswizard_software_details";
                tS_SoftwareDetails.Name = tsw_about_name;
                if (Application.OpenForms[tsw_about_name] == null){
                    tS_SoftwareDetails.ShowDialog();
                }else{
                    if (Application.OpenForms[tsw_about_name].WindowState == FormWindowState.Minimized){
                        Application.OpenForms[tsw_about_name].WindowState = FormWindowState.Normal;
                    }
                    Application.OpenForms[tsw_about_name].Activate();
                    TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(software_lang.TSReadLangs("HeaderHelp", "header_help_info_notification"), software_lang.TSReadLangs("HeaderMenu", "header_menu_about")));
                }
            }catch (Exception){ }
            finally{
                InFLP.ResumeLayout();
                InFLP.VerticalScroll.Value = Math.Min(scrollPos, InFLP.VerticalScroll.Maximum);
            }
        }
        // SOFTWARE OPERATION CONTROLLER MODULE
        // ======================================================================================================
        public static bool Software_operation_controller(string __target_software_path){
            var exeFiles = Directory.GetFiles(__target_software_path, "*.exe");
            var runned_process = Process.GetProcesses();
            foreach (var exe_path in exeFiles){
                string exe_name = Path.GetFileNameWithoutExtension(exe_path);
                if (runned_process.Any(p =>{
                    try{
                        return string.Equals(p.ProcessName, exe_name, StringComparison.OrdinalIgnoreCase);
                    }catch{
                        return false;
                    }
                })){
                    return true;
                }
            }
            return false;
        }
        // START OR DOWNLOAD AND UPDATE BUTTONS
        // ======================================================================================================
        private async void MPanelAstelWizardBtn_Click(object sender, EventArgs e){
            await HandleWizardButtonClickAsync(0, __astel_i_status, __astel_u_status, __astel_version);
        }
        private async void MPanelEncryphixWizardBtn_Click(object sender, EventArgs e){
            await HandleWizardButtonClickAsync(1, __encryphix_i_status, __encryphix_u_status, __encryphix_version);
        }
        private async void MPanelGlowWizardBtn_Click(object sender, EventArgs e){
            await HandleWizardButtonClickAsync(2, __glow_i_status, __glow_u_status, __glow_version);
        }
        private async void MPanelVCardixWizardBtn_Click(object sender, EventArgs e){
            await HandleWizardButtonClickAsync(3, __vcardix_i_status, __vcardix_u_status, __vcardix_version);
        }
        private async void MPanelVimeraWizardBtn_Click(object sender, EventArgs e){
            await HandleWizardButtonClickAsync(4, __vimera_i_status, __vimera_u_status, __vimera_version);
        }
        private async void MPanelYamiraWizardBtn_Click(object sender, EventArgs e){
            await HandleWizardButtonClickAsync(5, __yamira_i_status, __yamira_u_status, __yamira_version);
        }
        private async void MPanelZafuseWizardBtn_Click(object sender, EventArgs e){
            await HandleWizardButtonClickAsync(6, __zafuse_i_status, __zafuse_u_status, __zafuse_version);
        }
        private async Task HandleWizardButtonClickAsync(int softwareIndex, int installedStatus, bool updateStatus, string softwareVersion){
            var scrollPos = InFLP.VerticalScroll.Value;
            InFLP.SuspendLayout();
            try{
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                if (installedStatus == 1 && !updateStatus){
                    string app_path = $"{ts_softwares_location[softwareIndex]}\\{ts_softwares_location_exe_name[softwareIndex]}";
                    if (os_arch == Architecture.Arm64){
                        app_path = $"{Path.GetDirectoryName(app_path)}\\{Path.GetFileName(app_path).Replace("x64", "arm64")}";
                    }
                    //
                    if (!Software_operation_controller(ts_softwares_location[softwareIndex])){
                        Process.Start(new ProcessStartInfo { FileName = app_path, WorkingDirectory = Path.GetDirectoryName(app_path) });
                    }else{
                        TS_MessageBoxEngine.TS_MessageBox(this, 1, software_lang.TSReadLangs("TSWizardUI", "s_currently_operational"));
                    }
                    return;
                }
                //
                DialogResult userChoice;
                if (updateStatus){
                    if (!Software_operation_controller(ts_softwares_location[softwareIndex])){
                        userChoice = TS_MessageBoxEngine.TS_MessageBox(this, 5, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_update_select"), "\n\n", ts_softwares_list[softwareIndex]));
                    }else{
                        TS_MessageBoxEngine.TS_MessageBox(this, 2, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_delete_exe_active"), "\n", "\n\n", ts_softwares_list[softwareIndex]));
                        userChoice = DialogResult.No;
                    }
                }else{
                    userChoice = TS_MessageBoxEngine.TS_MessageBox(this, 5, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_download_select"), "\n\n", ts_softwares_list[softwareIndex]));
                }
                if (userChoice == DialogResult.Yes){
                    await TSWizardDownloaderModule(softwareIndex, softwareVersion, updateStatus);
                    //
                    switch (softwareIndex){
                        case 0: __astel_u_status = false; break;
                        case 1: __encryphix_u_status = false; break;
                        case 2: __glow_u_status = false; break;
                        case 3: __vcardix_u_status = false; break;
                        case 4: __vimera_u_status = false; break;
                        case 5: __yamira_u_status = false; break;
                        case 6: __zafuse_u_status = false; break;
                    }
                }
            }catch (Exception ex){
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                TS_MessageBoxEngine.TS_MessageBox(this, 3, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_error_dynamic"), "\n\n", ex.Message));
            }finally{
                InFLP.ResumeLayout();
                InFLP.VerticalScroll.Value = Math.Min(scrollPos, InFLP.VerticalScroll.Maximum);
            }
        }
        // ADVANCED DOWNLOAD AND EXTRACT MODULE
        // ======================================================================================================
        private async Task TSWizardDownloaderModule(int __software_mode, string __software_version, bool __software_update_mode){
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            string ___local_down_message = software_lang.TSReadLangs("TSWizardUI", "s_downloading");
            string ___local_down_message_2 = software_lang.TSReadLangs("TSWizardUI", "s_downloading_2");
            //
            string softwareName = ts_softwares_list[__software_mode].Trim();
            string softwareSlug = softwareName.ToLower();
            // Dynamic URL Module
            string dynamic_url = $"https://github.com/turkaysoftware/{softwareSlug}/releases/download/v{__software_version}/{softwareName}_v{__software_version}.zip";
            string targetFolder = ts_softwares_location[__software_mode];
            // Before download to settings save
            if (Directory.Exists(targetFolder)){
                foreach (var file in Directory.GetFiles(targetFolder)){
                    if (!file.EndsWith("Settings.ini", StringComparison.OrdinalIgnoreCase)){
                        File.Delete(file);
                    }
                }
                foreach (var dir in Directory.GetDirectories(targetFolder)){
                    Directory.Delete(dir, recursive: true);
                }
            }else{
                Directory.CreateDirectory(targetFolder);
            }
            //
            string zipFilePath = Path.Combine(targetFolder, $"{softwareName}_v{__software_version}.zip");
            // Disable the action button during the download process
            switch (__software_mode){
                case 0: MPanelAstelWizardBtn.Enabled = false; break;
                case 1: MPanelEncryphixWizardBtn.Enabled = false; break;
                case 2: MPanelGlowWizardBtn.Enabled = false; break;
                case 3: MPanelVCardixWizardBtn.Enabled = false; break;
                case 4: MPanelVimeraWizardBtn.Enabled = false; break;
                case 5: MPanelYamiraWizardBtn.Enabled = false; break;
                case 6: MPanelZafuseWizardBtn.Enabled = false; break;
            }
            // Downlod request process
            try{
                using (HttpClient get_data = new HttpClient())
                using (var in_response = await get_data.GetAsync(dynamic_url, HttpCompletionOption.ResponseHeadersRead)){
                    in_response.EnsureSuccessStatusCode();
                    //
                    var totalBytes = in_response.Content.Headers.ContentLength ?? -1L;
                    var canReportProgress = totalBytes != -1;
                    //
                    using (var contentStream = await in_response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true)){
                        var buffer = new byte[8192];
                        long totalRead = 0;
                        int read_active;
                        //
                        while ((read_active = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0){
                            await fileStream.WriteAsync(buffer, 0, read_active);
                            totalRead += read_active;
                            // Download Progress
                            if (canReportProgress){
                                int progress = (int)((totalRead * 100L) / totalBytes);
                                // switch (__software_mode){
                                //     case 0: MPanelAstelWizardBtn.Text = string.Format(___local_down_message, progress, TS_FormatSize(totalRead)); break;
                                //     case 1: MPanelEncryphixWizardBtn.Text = string.Format(___local_down_message, progress, TS_FormatSize(totalRead)); break;
                                //     case 2: MPanelGlowWizardBtn.Text = string.Format(___local_down_message, progress, TS_FormatSize(totalRead)); break;
                                //     case 3: MPanelVCardixWizardBtn.Text = string.Format(___local_down_message, progress, TS_FormatSize(totalRead)); break;
                                //     case 4: MPanelVimeraWizardBtn.Text = string.Format(___local_down_message, progress, TS_FormatSize(totalRead)); break;
                                //     case 5: MPanelYamiraWizardBtn.Text = string.Format(___local_down_message, progress, TS_FormatSize(totalRead)); break;
                                // }
                                switch (__software_mode){
                                    case 0: MPanelAstelWizardBtn.Text = string.Format(___local_down_message_2, progress); break;
                                    case 1: MPanelEncryphixWizardBtn.Text = string.Format(___local_down_message_2, progress); break;
                                    case 2: MPanelGlowWizardBtn.Text = string.Format(___local_down_message_2, progress); break;
                                    case 3: MPanelVCardixWizardBtn.Text = string.Format(___local_down_message_2, progress); break;
                                    case 4: MPanelVimeraWizardBtn.Text = string.Format(___local_down_message_2, progress); break;
                                    case 5: MPanelYamiraWizardBtn.Text = string.Format(___local_down_message_2, progress); break;
                                    case 6: MPanelZafuseWizardBtn.Text = string.Format(___local_down_message_2, progress); break;
                                }
                            }
                        }
                    }
                }
                //
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath)){
                    string rootFolderInZip = null;
                    foreach (var entry in archive.Entries){
                        if (!string.IsNullOrEmpty(entry.FullName)){
                            var parts = entry.FullName.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length > 0){
                                rootFolderInZip = parts[0];
                                break;
                            }
                        }
                    }
                    //
                    foreach (var entry in archive.Entries){
                        if (string.IsNullOrEmpty(entry.Name)) continue;
                        string relativePath = entry.FullName;
                        if (!string.IsNullOrEmpty(rootFolderInZip) && (relativePath.StartsWith(rootFolderInZip + "/") || relativePath.StartsWith(rootFolderInZip + "\\"))) relativePath = relativePath.Substring(rootFolderInZip.Length + 1);
                        if (Path.IsPathRooted(relativePath) || relativePath.IndexOf(':') >= 0) continue;
                        relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
                        string destinationPath = Path.GetFullPath(Path.Combine(targetFolder, relativePath));
                        string fullTargetPath = Path.GetFullPath(targetFolder + Path.DirectorySeparatorChar);
                        if (!destinationPath.StartsWith(fullTargetPath, StringComparison.OrdinalIgnoreCase)) continue;
                        try{
                            string destinationDir = Path.GetDirectoryName(destinationPath);
                            if (!Directory.Exists(destinationDir)) Directory.CreateDirectory(destinationDir);
                            entry.ExtractToFile(destinationPath, overwrite: true);
                        }catch { continue; }
                    }
                }
                File.Delete(zipFilePath);
                //
                var statuses = new Action[] { () => __astel_i_status = 1, () => __encryphix_i_status = 1, () => __glow_i_status = 1, () => __vcardix_i_status = 1, () => __vimera_i_status = 1, () => __yamira_i_status = 1, () => __zafuse_i_status = 1 };
                var wizardButtons = new Button[] { MPanelAstelWizardBtn, MPanelEncryphixWizardBtn, MPanelGlowWizardBtn, MPanelVCardixWizardBtn, MPanelVimeraWizardBtn, MPanelYamiraWizardBtn, MPanelZafuseWizardBtn };
                var removeButtons = new Button[] { MPanelAstelRemoveBtn, MPanelEncryphixRemoveBtn, MPanelGlowRemoveBtn, MPanelVCardixRemoveBtn, MPanelVimeraRemoveBtn, MPanelYamiraRemoveBtn, MPanelZafuseRemoveBtn };
                if (__software_mode >= 0 && __software_mode < statuses.Length){
                    statuses[__software_mode]();
                    wizardButtons[__software_mode].Text = software_lang.TSReadLangs("TSWizardUI", "s_launch");
                    wizardButtons[__software_mode].Enabled = true;
                    removeButtons[__software_mode].Enabled = true;
                    Dynamic_button_colors(__software_mode, 0);
                    Sc_v_mode(__software_mode, true);
                }
                // Label/Text UI Elements
                var soft_descriptions = new[]{
                   software_lang.TSReadLangs("TSWizardUI", "s_t_astel"),
                   software_lang.TSReadLangs("TSWizardUI", "s_t_encryphix"),
                   software_lang.TSReadLangs("TSWizardUI", "s_t_glow"),
                   software_lang.TSReadLangs("TSWizardUI", "s_t_vcardix"),
                   software_lang.TSReadLangs("TSWizardUI", "s_t_vimera"),
                   software_lang.TSReadLangs("TSWizardUI", "s_t_yamira"),
                   software_lang.TSReadLangs("TSWizardUI", "s_t_zafuse"),
                };
                var infoLabels = new[] { MPanelAstelInText, MPanelEncryphixInText, MPanelGlowInText, MPanelVCardixInText, MPanelVimeraInText, MPanelYamiraInText, MPanelZafuseInText };
                if (__software_mode >= 0 && __software_mode < soft_descriptions.Length){
                    infoLabels[__software_mode].Text = $"{soft_descriptions[__software_mode]}\n{string.Format(software_lang.TSReadLangs("TSWizardUI", "s_u_no_available"), __software_version, "\n", TS_FormatSize(new DirectoryInfo(ts_softwares_location[__software_mode]).GetFiles("*", SearchOption.AllDirectories).Sum(f => f.Length)))}";
                }
                //
                if (!__software_update_mode){
                    TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_download_success"), ts_softwares_list[__software_mode], "\n\n"));
                }else{
                    TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_update_success"), ts_softwares_list[__software_mode], "\n\n"));
                }
            }catch (Exception ex){
                if (!__software_update_mode){
                    TS_MessageBoxEngine.TS_MessageBox(this, 3, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_download_error"), "\n\n", "\n\n", ts_softwares_list[__software_mode], "\n\n", ex.Message));
                }else{
                    TS_MessageBoxEngine.TS_MessageBox(this, 3, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_update_error"), "\n\n", "\n\n", ts_softwares_list[__software_mode], "\n\n", ex.Message));
                }
            }
        }
        // REMOVE SOFTWARE MODULE
        // ======================================================================================================
        private void MPanelAstelRemoveBtn_Click(object sender, EventArgs e){
            try{
                Ts_module_deleter_check(0);
            }catch (Exception){ }
        }
        private void MPanelEncryphixRemoveBtn_Click(object sender, EventArgs e){
            try{
                Ts_module_deleter_check(1);
            }catch (Exception){ }
        }
        private void MPanelGlowRemoveBtn_Click(object sender, EventArgs e){
            try{
                Ts_module_deleter_check(2);
            }catch (Exception){ }
        }
        private void MPanelVCardixRemoveBtn_Click(object sender, EventArgs e){
            try{
                Ts_module_deleter_check(3);
            }catch (Exception){ }
        }
        private void MPanelVimeraRemoveBtn_Click(object sender, EventArgs e){
            try{
                Ts_module_deleter_check(4);
            }catch (Exception){ }
        }
        private void MPanelYamiraRemoveBtn_Click(object sender, EventArgs e){
            try{
                Ts_module_deleter_check(5);
            }catch (Exception){ }
        }
        private void MPanelZafuseRemoveBtn_Click(object sender, EventArgs e){
            try{
                Ts_module_deleter_check(6);
            }catch (Exception){ }
        }
        private void Ts_module_deleter_check(int __software_mode){
            int scrollPos = InFLP.VerticalScroll.Value;
            InFLP.SuspendLayout();
            try{
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                DialogResult lang_change_message = TS_MessageBoxEngine.TS_MessageBox(this, 6, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_delete_query"), "\n\n", ts_softwares_list[__software_mode]));
                if (lang_change_message == DialogResult.Yes){
                    Ts_module_deleter($"{ts_softwares_location[__software_mode]}", __software_mode);
                }
            }finally{
                InFLP.ResumeLayout();
                InFLP.VerticalScroll.Value = Math.Min(scrollPos, InFLP.VerticalScroll.Maximum);
            }
        }
        private void Ts_module_deleter(string __target_software_path, int __software_mode){
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            // No folder
            if (!Directory.Exists(__target_software_path)){
                return;
            }
            //
            if (Software_operation_controller(ts_softwares_location[__software_mode])){
                TS_MessageBoxEngine.TS_MessageBox(this, 2, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_delete_exe_active"), "\n", "\n\n", ts_softwares_list[__software_mode]));
            }else{
                try{
                    foreach (string file in Directory.GetFiles(__target_software_path))
                        File.Delete(file);
                    foreach (string dir in Directory.GetDirectories(__target_software_path))
                        Directory.Delete(dir, recursive: true);
                    //
                    Directory.Delete(__target_software_path, recursive: false);
                    //
                    var soft_status = new Action[] { () => __astel_i_status = 0, () => __encryphix_i_status = 0, () => __glow_i_status = 0, () => __vcardix_i_status = 0, () => __vimera_i_status = 0, () => __yamira_i_status = 0, () => __zafuse_i_status = 0 };
                    //
                    if (__software_mode >= 0 && __software_mode < soft_status.Length){
                        soft_status[__software_mode]();
                        Dynamic_button_colors(__software_mode, 1);
                        Sc_v_mode(__software_mode, false);
                        Task __run_reload_module = Task.Run(() => { Ts_w_start_module(false); });
                    }
                    TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_delete_success"), "\n\n", ts_softwares_list[__software_mode]));
                }catch (Exception ex){
                    TS_MessageBoxEngine.TS_MessageBox(this, 3, string.Format(software_lang.TSReadLangs("TSWizardUI", "s_delete_error"), "\n\n", "\n\n", ts_softwares_list[__software_mode], "\n\n", ex.Message));
                }
            }
        }
        // THEME MODE
        // ======================================================================================================
        private ToolStripMenuItem selected_theme = null;
        private void Select_theme_active(object target_theme){
            if (target_theme == null)
                return;
            ToolStripMenuItem clicked_theme = (ToolStripMenuItem)target_theme;
            if (selected_theme == clicked_theme)
                return;
            Select_theme_deactive();
            selected_theme = clicked_theme;
            selected_theme.Checked = true;
        }
        private void Select_theme_deactive(){
            foreach (ToolStripMenuItem theme in themeToolStripMenuItem.DropDownItems){
                theme.Checked = false;
            }
        }
        // THEME SWAP
        // ======================================================================================================
        private void SystemThemeToolStripMenuItem_Click(object sender, EventArgs e){
            themeSystem = 2; Theme_engine(GetSystemTheme(2)); SaveTheme(2); Select_theme_active(sender);
        }
        private void LightThemeToolStripMenuItem_Click(object sender, EventArgs e){
            themeSystem = 0; Theme_engine(1); SaveTheme(1); Select_theme_active(sender);
        }
        private void DarkThemeToolStripMenuItem_Click(object sender, EventArgs e){
            themeSystem = 0; Theme_engine(0); SaveTheme(0); Select_theme_active(sender);
        }
        private void TSUseSystemTheme(){ if (themeSystem == 2) Theme_engine(GetSystemTheme(2)); }
        private void SaveTheme(int ts){
            // SAVE CURRENT THEME
            try{
                TSSettingsSave software_setting_save = new TSSettingsSave(ts_sf);
                software_setting_save.TSWriteSettings(ts_settings_container, "ThemeStatus", Convert.ToString(ts));
            }catch (Exception){ }
        }
        // THEME ENGINE
        // ======================================================================================================
        private void Theme_engine(int ts){
            try{
                theme = ts;
                //
                TSThemeModeHelper.SetThemeMode(ts == 0);
                TSThemeModeHelper.InitializeThemeForForm(this);
                //
                if (theme == 1){
                    TSImageRenderer(settingsToolStripMenuItem, Properties.Resources.tm_settings_light, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(themeToolStripMenuItem, Properties.Resources.tm_theme_light, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(languageToolStripMenuItem, Properties.Resources.tm_language_light, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(startupToolStripMenuItem, Properties.Resources.tm_startup_light, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(windowBehaviorToolStripMenuItem, Properties.Resources.tm_behavior_light, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(updateNotificationsToolStripMenuItem, Properties.Resources.tm_notification_light, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(checkForUpdateToolStripMenuItem, Properties.Resources.tm_update_light, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(donateToolStripMenuItem, Properties.Resources.tm_donate_light, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(aboutToolStripMenuItem, Properties.Resources.tm_about_light, 0, ContentAlignment.MiddleRight);
                    //
                    TSImageRenderer(checkForSoftwareUpdateToolStripMenuItem, Properties.Resources.st_check_update_light, 0, ContentAlignment.MiddleRight);
                    //
                    TSImageRenderer(showAppToolStripMenuItem, Properties.Resources.st_show_light, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(softwareUpdateCheckToolStripMenuItem, Properties.Resources.st_check_update_light, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(exitToolStripMenuItem, Properties.Resources.st_stop_light, 0, ContentAlignment.MiddleRight);
                    //
                    TSImageRenderer(MPanelAstelShortcutBtn, Properties.Resources.ct_shortcut_light, 0, ContentAlignment.MiddleCenter);
                    TSImageRenderer(MPanelEncryphixShortcutBtn, Properties.Resources.ct_shortcut_light, 0, ContentAlignment.MiddleCenter);
                    TSImageRenderer(MPanelGlowShortcutBtn, Properties.Resources.ct_shortcut_light, 0, ContentAlignment.MiddleCenter);
                    TSImageRenderer(MPanelVCardixShortcutBtn, Properties.Resources.ct_shortcut_light, 0, ContentAlignment.MiddleCenter);
                    TSImageRenderer(MPanelVimeraShortcutBtn, Properties.Resources.ct_shortcut_light, 0, ContentAlignment.MiddleCenter);
                    TSImageRenderer(MPanelYamiraShortcutBtn, Properties.Resources.ct_shortcut_light, 0, ContentAlignment.MiddleCenter);
                    TSImageRenderer(MPanelZafuseShortcutBtn, Properties.Resources.ct_shortcut_light, 0, ContentAlignment.MiddleCenter);
                    //
                    TSImageRenderer(MPanelAstelRemoveBtn, Properties.Resources.ct_delete_light, 15, ContentAlignment.MiddleLeft);
                    TSImageRenderer(MPanelEncryphixRemoveBtn, Properties.Resources.ct_delete_light, 15, ContentAlignment.MiddleLeft);
                    TSImageRenderer(MPanelGlowRemoveBtn, Properties.Resources.ct_delete_light, 15, ContentAlignment.MiddleLeft);
                    TSImageRenderer(MPanelVCardixRemoveBtn, Properties.Resources.ct_delete_light, 15, ContentAlignment.MiddleLeft);
                    TSImageRenderer(MPanelVimeraRemoveBtn, Properties.Resources.ct_delete_light, 15, ContentAlignment.MiddleLeft);
                    TSImageRenderer(MPanelYamiraRemoveBtn, Properties.Resources.ct_delete_light, 15, ContentAlignment.MiddleLeft);
                    TSImageRenderer(MPanelZafuseRemoveBtn, Properties.Resources.ct_delete_light, 15, ContentAlignment.MiddleLeft);
                }else if (theme == 0){
                    TSImageRenderer(settingsToolStripMenuItem, Properties.Resources.tm_settings_dark, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(themeToolStripMenuItem, Properties.Resources.tm_theme_dark, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(languageToolStripMenuItem, Properties.Resources.tm_language_dark, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(startupToolStripMenuItem, Properties.Resources.tm_startup_dark, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(windowBehaviorToolStripMenuItem, Properties.Resources.tm_behavior_dark, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(updateNotificationsToolStripMenuItem, Properties.Resources.tm_notification_dark, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(checkForUpdateToolStripMenuItem, Properties.Resources.tm_update_dark, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(donateToolStripMenuItem, Properties.Resources.tm_donate_dark, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(aboutToolStripMenuItem, Properties.Resources.tm_about_dark, 0, ContentAlignment.MiddleRight);
                    //
                    TSImageRenderer(checkForSoftwareUpdateToolStripMenuItem, Properties.Resources.st_check_update_dark, 0, ContentAlignment.MiddleRight);
                    //
                    TSImageRenderer(showAppToolStripMenuItem, Properties.Resources.st_show_dark, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(softwareUpdateCheckToolStripMenuItem, Properties.Resources.st_check_update_dark, 0, ContentAlignment.MiddleRight);
                    TSImageRenderer(exitToolStripMenuItem, Properties.Resources.st_stop_dark, 0, ContentAlignment.MiddleRight);
                    //
                    TSImageRenderer(MPanelAstelShortcutBtn, Properties.Resources.ct_shortcut_dark, 0, ContentAlignment.MiddleCenter);
                    TSImageRenderer(MPanelEncryphixShortcutBtn, Properties.Resources.ct_shortcut_dark, 0, ContentAlignment.MiddleCenter);
                    TSImageRenderer(MPanelGlowShortcutBtn, Properties.Resources.ct_shortcut_dark, 0, ContentAlignment.MiddleCenter);
                    TSImageRenderer(MPanelVCardixShortcutBtn, Properties.Resources.ct_shortcut_dark, 0, ContentAlignment.MiddleCenter);
                    TSImageRenderer(MPanelVimeraShortcutBtn, Properties.Resources.ct_shortcut_dark, 0, ContentAlignment.MiddleCenter);
                    TSImageRenderer(MPanelYamiraShortcutBtn, Properties.Resources.ct_shortcut_dark, 0, ContentAlignment.MiddleCenter);
                    TSImageRenderer(MPanelZafuseShortcutBtn, Properties.Resources.ct_shortcut_dark, 0, ContentAlignment.MiddleCenter);
                    //
                    TSImageRenderer(MPanelAstelRemoveBtn, Properties.Resources.ct_delete_dark, 15, ContentAlignment.MiddleLeft);
                    TSImageRenderer(MPanelEncryphixRemoveBtn, Properties.Resources.ct_delete_dark, 15, ContentAlignment.MiddleLeft);
                    TSImageRenderer(MPanelGlowRemoveBtn, Properties.Resources.ct_delete_dark, 15, ContentAlignment.MiddleLeft);
                    TSImageRenderer(MPanelVCardixRemoveBtn, Properties.Resources.ct_delete_dark, 15, ContentAlignment.MiddleLeft);
                    TSImageRenderer(MPanelVimeraRemoveBtn, Properties.Resources.ct_delete_dark, 15, ContentAlignment.MiddleLeft);
                    TSImageRenderer(MPanelYamiraRemoveBtn, Properties.Resources.ct_delete_dark, 15, ContentAlignment.MiddleLeft);
                    TSImageRenderer(MPanelZafuseRemoveBtn, Properties.Resources.ct_delete_dark, 15, ContentAlignment.MiddleLeft);
                }
                // TOOLTIP
                MainToolTip.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                MainToolTip.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                // HEADER
                header_colors[0] = TS_ThemeEngine.ColorMode(theme, "HeaderBGColorMain");
                header_colors[1] = TS_ThemeEngine.ColorMode(theme, "HeaderFEColorMain");
                header_colors[2] = TS_ThemeEngine.ColorMode(theme, "HeaderColorAccent");
                HeaderMenu.Renderer = new HeaderMenuColors();
                NotifyMenu.Renderer = new HeaderMenuColors();
                // HEADER MENU
                // ===========================================
                var bg = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                var fg = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                HeaderMenu.ForeColor = fg;
                HeaderMenu.BackColor = bg;
                SetMenuStripColors(HeaderMenu, bg, fg);
                SetContextMenuColors(NotifyMenu, bg, fg);
                // UI
                // ===========================================
                BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                HeaderBanner.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                HeaderText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                // CONTENT
                // ===========================================
                InFLP.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelAstel.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelEncryphix.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelGlow.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelVCardix.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelVimera.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelYamira.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelZafuse.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                //
                MPanelAstelImage.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelEncryphixImage.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelGlowImage.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelVCardixImage.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelVimeraImage.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelYamiraImage.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelZafuseImage.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                //
                MPanelAstelHeadText.ForeColor = TS_ThemeEngine.ColorMode(theme, "AstelFE");
                MPanelEncryphixHeadText.ForeColor = TS_ThemeEngine.ColorMode(theme, "EncryphixFE");
                MPanelGlowHeadText.ForeColor = TS_ThemeEngine.ColorMode(theme, "GlowFE");
                MPanelVCardixHeadText.ForeColor = TS_ThemeEngine.ColorMode(theme, "VCardixFE");
                MPanelVimeraHeadText.ForeColor = TS_ThemeEngine.ColorMode(theme, "VimeraFE");
                MPanelYamiraHeadText.ForeColor = TS_ThemeEngine.ColorMode(theme, "YamiraFE");
                MPanelZafuseHeadText.ForeColor = TS_ThemeEngine.ColorMode(theme, "ZafuseFE");
                //
                MPanelAstelLinkText.LinkColor = TS_ThemeEngine.ColorMode(theme, "AstelFE");
                MPanelEncryphixLinkText.LinkColor = TS_ThemeEngine.ColorMode(theme, "EncryphixFE");
                MPanelGlowLinkText.LinkColor = TS_ThemeEngine.ColorMode(theme, "GlowFE");
                MPanelVCardixLinkText.LinkColor = TS_ThemeEngine.ColorMode(theme, "VCardixFE");
                MPanelVimeraLinkText.LinkColor = TS_ThemeEngine.ColorMode(theme, "VimeraFE");
                MPanelYamiraLinkText.LinkColor = TS_ThemeEngine.ColorMode(theme, "YamiraFE");
                MPanelZafuseLinkText.LinkColor = TS_ThemeEngine.ColorMode(theme, "ZafuseFE");
                //
                MPanelAstelInText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                MPanelEncryphixInText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                MPanelGlowInText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                MPanelVCardixInText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                MPanelVimeraInText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                MPanelYamiraInText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                MPanelZafuseInText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                //
                MPanelAstelLinkText.ActiveLinkColor = TS_ThemeEngine.ColorMode(theme, "AstelFEHover");
                MPanelEncryphixLinkText.ActiveLinkColor = TS_ThemeEngine.ColorMode(theme, "EncryphixFEHover");
                MPanelGlowLinkText.ActiveLinkColor = TS_ThemeEngine.ColorMode(theme, "GlowFEHover");
                MPanelVCardixLinkText.ActiveLinkColor = TS_ThemeEngine.ColorMode(theme, "VCardixFEHover");
                MPanelVimeraLinkText.ActiveLinkColor = TS_ThemeEngine.ColorMode(theme, "VimeraFEHover");
                MPanelYamiraLinkText.ActiveLinkColor = TS_ThemeEngine.ColorMode(theme, "YamiraFEHover");
                MPanelZafuseLinkText.ActiveLinkColor = TS_ThemeEngine.ColorMode(theme, "ZafuseFEHover");
                //
                MPanelAstelSCPanel.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelEncryphixSCPanel.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelGlowSCPanel.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelVCardixSCPanel.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelVimeraSCPanel.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelYamiraSCPanel.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelZafuseSCPanel.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                //
                MPanelAstelWizardBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelEncryphixWizardBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelGlowWizardBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelVCardixWizardBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelVimeraWizardBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelYamiraWizardBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelZafuseWizardBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                //
                Dynamic_button_colors(0, (__astel_i_status == 0) ? 1 : (__astel_u_status ? 2 : 0));
                Dynamic_button_colors(1, (__encryphix_i_status == 0) ? 1 : (__encryphix_u_status ? 2 : 0));
                Dynamic_button_colors(2, (__glow_i_status == 0) ? 1 : (__glow_u_status ? 2 : 0));
                Dynamic_button_colors(3, (__vcardix_i_status == 0) ? 1 : (__vcardix_u_status ? 2 : 0));
                Dynamic_button_colors(4, (__vimera_i_status == 0) ? 1 : (__vimera_u_status ? 2 : 0));
                Dynamic_button_colors(5, (__yamira_i_status == 0) ? 1 : (__yamira_u_status ? 2 : 0));
                Dynamic_button_colors(6, (__zafuse_i_status == 0) ? 1 : (__zafuse_u_status ? 2 : 0));
                // BUTTON REMOVE
                // ===========================================
                MPanelAstelRemoveBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelEncryphixRemoveBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelGlowRemoveBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelVCardixRemoveBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelVimeraRemoveBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelYamiraRemoveBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelZafuseRemoveBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                //
                MPanelAstelRemoveBtn.FlatAppearance.BorderColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelEncryphixRemoveBtn.FlatAppearance.BorderColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelGlowRemoveBtn.FlatAppearance.BorderColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelVCardixRemoveBtn.FlatAppearance.BorderColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelVimeraRemoveBtn.FlatAppearance.BorderColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelYamiraRemoveBtn.FlatAppearance.BorderColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelZafuseRemoveBtn.FlatAppearance.BorderColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                //
                MPanelAstelRemoveBtn.FlatAppearance.MouseOverBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelEncryphixRemoveBtn.FlatAppearance.MouseOverBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelGlowRemoveBtn.FlatAppearance.MouseOverBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelVCardixRemoveBtn.FlatAppearance.MouseOverBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelVimeraRemoveBtn.FlatAppearance.MouseOverBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelYamiraRemoveBtn.FlatAppearance.MouseOverBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelZafuseRemoveBtn.FlatAppearance.MouseOverBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                //
                MPanelAstelRemoveBtn.FlatAppearance.MouseDownBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelEncryphixRemoveBtn.FlatAppearance.MouseDownBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelGlowRemoveBtn.FlatAppearance.MouseDownBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelVCardixRemoveBtn.FlatAppearance.MouseDownBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelVimeraRemoveBtn.FlatAppearance.MouseDownBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelYamiraRemoveBtn.FlatAppearance.MouseDownBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelZafuseRemoveBtn.FlatAppearance.MouseDownBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                //
                MPanelAstelRemoveBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelEncryphixRemoveBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelGlowRemoveBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelVCardixRemoveBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelVimeraRemoveBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelYamiraRemoveBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelZafuseRemoveBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                // OTHER PAGE DYNAMIC UI
                Software_other_page_preloader();
            }catch (Exception){ }
        }
        private void SetMenuStripColors(MenuStrip menuStrip, Color bgColor, Color fgColor){
            if (menuStrip == null) return;
            foreach (ToolStripItem item in menuStrip.Items){
                if (item is ToolStripMenuItem menuItem){
                    SetMenuItemColors(menuItem, bgColor, fgColor);
                }
            }
        }
        private void SetMenuItemColors(ToolStripMenuItem menuItem, Color bgColor, Color fgColor){
            if (menuItem == null) return;
            menuItem.BackColor = bgColor;
            menuItem.ForeColor = fgColor;
            foreach (ToolStripItem item in menuItem.DropDownItems){
                if (item is ToolStripMenuItem subMenuItem){
                    SetMenuItemColors(subMenuItem, bgColor, fgColor);
                }
            }
        }
        private void SetContextMenuColors(ContextMenuStrip contextMenu, Color bgColor, Color fgColor){
            if (contextMenu == null) return;
            foreach (ToolStripItem item in contextMenu.Items){
                if (item is ToolStripMenuItem menuItem){
                    SetMenuItemColors(menuItem, bgColor, fgColor);
                }
            }
        }
        // DYNAMIC BUTTON COLORS
        // ======================================================================================================
        private void Dynamic_button_colors(int __button_mode, int __button_status){
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            Button selectedButton = null;
            string baseColorKey = null;
            string hoverColorKey = null;
            string baseText = null;
            Image statusImage = null;
            if (__button_mode == -1 || __button_status == -1){
                baseColorKey = "BtnLaunchBG";
                hoverColorKey = "BtnLaunchBGHover";
                statusImage = Convert.ToBoolean(theme) ? Properties.Resources.ct_download_light : Properties.Resources.ct_download_dark;
                baseText = software_lang.TSReadLangs("TSWizardUI", "s_download");
            }else{
                switch (__button_mode){
                    case 0:
                        selectedButton = MPanelAstelWizardBtn;
                        break;
                    case 1:
                        selectedButton = MPanelEncryphixWizardBtn;
                        break;
                    case 2:
                        selectedButton = MPanelGlowWizardBtn;
                        break;
                    case 3:
                        selectedButton = MPanelVCardixWizardBtn;
                        break;
                    case 4:
                        selectedButton = MPanelVimeraWizardBtn;
                        break;
                    case 5:
                        selectedButton = MPanelYamiraWizardBtn;
                        break;
                    case 6:
                        selectedButton = MPanelZafuseWizardBtn;
                        break;
                }
                switch (__button_status){
                    case 0:
                        baseColorKey = "BtnLaunchBG";
                        hoverColorKey = "BtnLaunchBGHover";
                        baseText = software_lang.TSReadLangs("TSWizardUI", "s_launch");
                        statusImage = Convert.ToBoolean(theme) ? Properties.Resources.ct_start_light : Properties.Resources.ct_start_dark;
                        break;
                    case 1:
                        baseColorKey = "BtnDownloadBG";
                        hoverColorKey = "BtnDownloadBGHover";
                        baseText = software_lang.TSReadLangs("TSWizardUI", "s_download");
                        statusImage = Convert.ToBoolean(theme) ? Properties.Resources.ct_download_light : Properties.Resources.ct_download_dark;
                        break;
                    case 2:
                        baseColorKey = "BtnUpdateBG";
                        hoverColorKey = "BtnUpdateBGHover";
                        baseText = software_lang.TSReadLangs("TSWizardUI", "s_update");
                        statusImage = Convert.ToBoolean(theme) ? Properties.Resources.ct_update_light : Properties.Resources.ct_update_dark;
                        break;
                }
            }
            var baseColor = TS_ThemeEngine.ColorMode(theme, baseColorKey);
            var hoverColor = TS_ThemeEngine.ColorMode(theme, hoverColorKey);
            if (__button_mode == -1){
                var buttons = new Button[] { MPanelAstelWizardBtn, MPanelEncryphixWizardBtn, MPanelGlowWizardBtn, MPanelVCardixWizardBtn, MPanelVimeraWizardBtn, MPanelYamiraWizardBtn, MPanelZafuseWizardBtn };
                foreach (var btn in buttons){
                    btn.BackColor = baseColor;
                    btn.FlatAppearance.BorderColor = baseColor;
                    btn.FlatAppearance.MouseOverBackColor = hoverColor;
                    btn.FlatAppearance.MouseDownBackColor = hoverColor;
                    btn.Image = statusImage;
                    TSImageRenderer(btn, statusImage, 15, ContentAlignment.MiddleLeft);
                    btn.Text = baseText + " ";
                }
            }else if (selectedButton != null){
                selectedButton.BackColor = baseColor;
                selectedButton.FlatAppearance.BorderColor = baseColor;
                selectedButton.FlatAppearance.MouseOverBackColor = hoverColor;
                selectedButton.FlatAppearance.MouseDownBackColor = hoverColor;
                TSImageRenderer(selectedButton, statusImage, 15, ContentAlignment.MiddleLeft);
                selectedButton.Text = baseText + " ";
            }
        }
        // MODULES PAGE DYNAMIC UI
        // ======================================================================================================
        private void Software_other_page_preloader(){
            // SOFTWARE ABOUT
            try{
                TS_SoftwareDetails software_details = new TS_SoftwareDetails();
                string software_details_name = "tswizard_software_details";
                software_details.Name = software_details_name;
                if (Application.OpenForms[software_details_name] != null){
                    software_details = (TS_SoftwareDetails)Application.OpenForms[software_details_name];
                    software_details.Preload_software_info();
                }
            }catch (Exception){ }
            try{
                TSWizardAbout software_about = new TSWizardAbout();
                string software_about_name = "tswizard_about";
                software_about.Name = software_about_name;
                if (Application.OpenForms[software_about_name] != null){
                    software_about = (TSWizardAbout)Application.OpenForms[software_about_name];
                    software_about.About_preloader();
                }
            }catch (Exception){ }
        }
        // LANG MODE
        // ======================================================================================================
        private void Select_lang_active(object target_lang){
            ToolStripMenuItem selected_lang = null;
            Select_lang_deactive();
            if (target_lang != null){
                if (selected_lang != (ToolStripMenuItem)target_lang){
                    selected_lang = (ToolStripMenuItem)target_lang;
                    selected_lang.Checked = true;
                }
            }
        }
        private void Select_lang_deactive(){
            foreach (ToolStripMenuItem disabled_lang in languageToolStripMenuItem.DropDownItems){
                disabled_lang.Checked = false;
            }
        }
        private void LanguageToolStripMenuItem_Click(object sender, EventArgs e){
            if (sender is ToolStripMenuItem menuItem && menuItem.Tag is string langCode){
                if (lang != langCode && AllLanguageFiles.ContainsKey(langCode)){
                    Lang_preload(AllLanguageFiles[langCode], langCode);
                    Select_lang_active(sender);
                    Task.Run(() => { Ts_w_start_module(false); });
                }
            }
        }
        private void Lang_preload(string lang_type, string lang_code){
            Lang_engine(lang_type, lang_code);
            try{
                TSSettingsSave software_setting_save = new TSSettingsSave(ts_sf);
                software_setting_save.TSWriteSettings(ts_settings_container, "LanguageStatus", lang_code);
            }catch (Exception){ }
            // LANG CHANGE NOTIFICATION
            //TSGetLangs software_lang = new TSGetLangs(lang_path);
            //DialogResult lang_change_message = TS_MessageBoxEngine.TS_MessageBox(this, 5, string.Format(software_lang.TSReadLangs("LangChange", "lang_change_notification"), "\n\n", "\n\n"));
            //if (lang_change_message == DialogResult.Yes) { Application.Restart(); }
        }
        private void Lang_engine(string lang_type, string lang_code){
            try{
                lang_path = lang_type;
                lang = lang_code;
                // GLOBAL ENGINE
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                // CONTEXT MENU
                showAppToolStripMenuItem.Text = software_lang.TSReadLangs("ContextMenu", "cm_show");
                softwareUpdateCheckToolStripMenuItem.Text = software_lang.TSReadLangs("ContextMenu", "cm_s_u_check");
                exitToolStripMenuItem.Text = software_lang.TSReadLangs("ContextMenu", "cm_exit");
                // SETTINGS
                settingsToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderMenu", "header_menu_settings");
                // THEMES
                themeToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderMenu", "header_menu_theme");
                lightThemeToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderThemes", "theme_light");
                darkThemeToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderThemes", "theme_dark");
                systemThemeToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderThemes", "theme_system");
                // LANGS
                languageToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderMenu", "header_menu_language");
                arabicToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_ar");
                chineseToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_zh");
                englishToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_en");
                dutchToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_nl");
                frenchToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_fr");
                germanToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_de");
                hindiToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_hi");
                italianToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_it");
                japaneseToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_ja");
                koreanToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_ko");
                polishToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_pl");
                portugueseToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_pt");
                russianToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_ru");
                spanishToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_es");
                turkishToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderLangs", "lang_tr");
                // STARTUP MODE
                startupToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderMenu", "header_menu_start");
                windowedToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderViewMode", "header_view_mode_windowed");
                fullScreenToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderViewMode", "header_view_mode_full_screen");
                // WINDOW BEHAVIOR
                windowBehaviorToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderMenu", "header_menu_window_behavior");
                iconStatusToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderWindowBehavior", "hwb_icon_mode");
                closeSoftwareToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderWindowBehavior", "hwb_exit");
                // UPDATE NOTIFICATIONS
                updateNotificationsToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderMenu", "header_menu_update_notifications");
                notificationOnToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderActiveMode", "header_a_on");
                notificationOffToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderActiveMode", "header_a_off");
                // UPDATE
                checkForUpdateToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderMenu", "header_menu_update");
                // CHECK FOR SOFTWARE UPDATE
                checkForSoftwareUpdateToolStripMenuItem.Text = software_lang.TSReadLangs("ContextMenu", "cm_s_u_check");
                // DONATE
                donateToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderMenu", "header_menu_donate");
                // ABOUT
                aboutToolStripMenuItem.Text = software_lang.TSReadLangs("HeaderMenu", "header_menu_about");
                //
                HeaderText.Text = string.Format(software_lang.TSReadLangs("TSWizardUI", "s_header_welcome"), Environment.UserName.Trim());
                //
                TS_SoftwareInfo ts_s_info = new TS_SoftwareInfo();
                for (int i = 0; i < ts_s_info.TSSoftwareList.Count; i++){
                    var s_name = ts_s_info.TSSoftwareList[i].Name;
                    switch (i){
                        case 0: MPanelAstelHeadText.Text = s_name; break;
                        case 1: MPanelEncryphixHeadText.Text = s_name; break;
                        case 2: MPanelGlowHeadText.Text = s_name; break;
                        case 3: MPanelVCardixHeadText.Text = s_name; break;
                        case 4: MPanelVimeraHeadText.Text = s_name; break;
                        case 5: MPanelYamiraHeadText.Text = s_name; break;
                        case 6: MPanelZafuseHeadText.Text = s_name; break;
                    }
                }
                //
                MPanelAstelLinkText.Text = software_lang.TSReadLangs("TSWizardUI", "s_d_text");
                MPanelEncryphixLinkText.Text = software_lang.TSReadLangs("TSWizardUI", "s_d_text");
                MPanelGlowLinkText.Text = software_lang.TSReadLangs("TSWizardUI", "s_d_text");
                MPanelVCardixLinkText.Text = software_lang.TSReadLangs("TSWizardUI", "s_d_text");
                MPanelVimeraLinkText.Text = software_lang.TSReadLangs("TSWizardUI", "s_d_text");
                MPanelYamiraLinkText.Text = software_lang.TSReadLangs("TSWizardUI", "s_d_text");
                MPanelZafuseLinkText.Text = software_lang.TSReadLangs("TSWizardUI", "s_d_text");
                //
                MPanelAstelRemoveBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_delete") + " ";
                MPanelEncryphixRemoveBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_delete") + " ";
                MPanelGlowRemoveBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_delete") + " ";
                MPanelVCardixRemoveBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_delete") + " ";
                MPanelVimeraRemoveBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_delete") + " ";
                MPanelYamiraRemoveBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_delete") + " ";
                MPanelZafuseRemoveBtn.Text = software_lang.TSReadLangs("TSWizardUI", "s_delete") + " ";
                //
                MainToolTip.SetToolTip(MPanelAstelShortcutBtn, software_lang.TSReadLangs("TSWizardUI", "s_sc_text"));
                MainToolTip.SetToolTip(MPanelEncryphixShortcutBtn, software_lang.TSReadLangs("TSWizardUI", "s_sc_text"));
                MainToolTip.SetToolTip(MPanelGlowShortcutBtn, software_lang.TSReadLangs("TSWizardUI", "s_sc_text"));
                MainToolTip.SetToolTip(MPanelVCardixShortcutBtn, software_lang.TSReadLangs("TSWizardUI", "s_sc_text"));
                MainToolTip.SetToolTip(MPanelVimeraShortcutBtn, software_lang.TSReadLangs("TSWizardUI", "s_sc_text"));
                MainToolTip.SetToolTip(MPanelYamiraShortcutBtn, software_lang.TSReadLangs("TSWizardUI", "s_sc_text"));
                MainToolTip.SetToolTip(MPanelZafuseShortcutBtn, software_lang.TSReadLangs("TSWizardUI", "s_sc_text"));
                //
                Dynamic_button_colors(0, (__astel_i_status == 0) ? 1 : (__astel_u_status ? 2 : 0));
                Dynamic_button_colors(1, (__encryphix_i_status == 0) ? 1 : (__encryphix_u_status ? 2 : 0));
                Dynamic_button_colors(2, (__glow_i_status == 0) ? 1 : (__glow_u_status ? 2 : 0));
                Dynamic_button_colors(3, (__vcardix_i_status == 0) ? 1 : (__vcardix_u_status ? 2 : 0));
                Dynamic_button_colors(4, (__vimera_i_status == 0) ? 1 : (__vimera_u_status ? 2 : 0));
                Dynamic_button_colors(5, (__yamira_i_status == 0) ? 1 : (__yamira_u_status ? 2 : 0));
                Dynamic_button_colors(6, (__zafuse_i_status == 0) ? 1 : (__zafuse_u_status ? 2 : 0));
                // OTHER PAGE DYNAMIC UI
                Software_other_page_preloader();
            }catch (Exception){ }
        }
        // STARTUP SETINGS
        // ======================================================================================================
        private void Select_startup_mode_active(object target_startup_mode){
            ToolStripMenuItem selected_startup_mode = null;
            Select_startup_mode_deactive();
            if (target_startup_mode != null){
                if (selected_startup_mode != (ToolStripMenuItem)target_startup_mode){
                    selected_startup_mode = (ToolStripMenuItem)target_startup_mode;
                    selected_startup_mode.Checked = true;
                }
            }
        }
        private void Select_startup_mode_deactive(){
            foreach (ToolStripMenuItem disabled_startup in startupToolStripMenuItem.DropDownItems){
                disabled_startup.Checked = false;
            }
        }
        private void WindowedToolStripMenuItem_Click(object sender, EventArgs e){
            if (startup_status != 0){ startup_status = 0; Startup_mode_settings("0"); Select_startup_mode_active(sender); }
        }
        private void FullScreenToolStripMenuItem_Click(object sender, EventArgs e){
            if (startup_status != 1){ startup_status = 1; Startup_mode_settings("1"); Select_startup_mode_active(sender); }
        }
        private void Startup_mode_settings(string get_startup_value){
            try{
                TSSettingsSave software_setting_save = new TSSettingsSave(ts_sf);
                software_setting_save.TSWriteSettings(ts_settings_container, "StartupStatus", get_startup_value);
            }catch (Exception){ }
        }
        // WINDOW BEHAVIOR MODE
        // ======================================================================================================
        private void Select_window_behavior_mode_active(object target_window_behavior_mode){
            ToolStripMenuItem selected_window_behavior_mode = null;
            Select_window_behavior_mode_deactive();
            if (target_window_behavior_mode != null){
                if (selected_window_behavior_mode != (ToolStripMenuItem)target_window_behavior_mode){
                    selected_window_behavior_mode = (ToolStripMenuItem)target_window_behavior_mode;
                    selected_window_behavior_mode.Checked = true;
                }
            }
        }
        private void Select_window_behavior_mode_deactive(){
            foreach (ToolStripMenuItem disabled_window_behavior_startup in windowBehaviorToolStripMenuItem.DropDownItems){
                disabled_window_behavior_startup.Checked = false;
            }
        }
        private void IconStatusToolStripMenuItem_Click(object sender, EventArgs e){
            if (behavior_mode_status != 1){ behavior_mode_status = 1; Window_behavior_mode_settings("1"); Select_window_behavior_mode_active(sender); }
        }
        private void CloseSoftwareToolStripMenuItem_Click(object sender, EventArgs e){
            if (behavior_mode_status != 0){ behavior_mode_status = 0; Window_behavior_mode_settings("0"); Select_window_behavior_mode_active(sender); }
        }
        private void Window_behavior_mode_settings(string get_window_behavior_value){
            try{
                TSSettingsSave software_setting_save = new TSSettingsSave(ts_sf);
                software_setting_save.TSWriteSettings(ts_settings_container, "WindowBehavior", get_window_behavior_value);
            }catch (Exception){ }
        }
        // UPDATE NOTIFICATIONS MODE
        // ======================================================================================================
        private void Select_update_notifications_mode_active(object target_update_notifications_mode){
            ToolStripMenuItem selected_update_notifications_mode = null;
            Select_update_notifications_mode_deactive();
            if (target_update_notifications_mode != null){
                if (selected_update_notifications_mode != (ToolStripMenuItem)target_update_notifications_mode){
                    selected_update_notifications_mode = (ToolStripMenuItem)target_update_notifications_mode;
                    selected_update_notifications_mode.Checked = true;
                }
            }
        }
        private void Select_update_notifications_mode_deactive(){
            foreach (ToolStripMenuItem disabled_update_notifications in updateNotificationsToolStripMenuItem.DropDownItems){
                disabled_update_notifications.Checked = false;
            }
        }
        private void NotificationOnToolStripMenuItem_Click(object sender, EventArgs e){
            if (update_notifications_status != 1){ update_notifications_status = 1; Update_notifications_mode_settings("1"); Select_update_notifications_mode_active(sender); }
        }
        private void NotificationOffToolStripMenuItem_Click(object sender, EventArgs e){
            if (update_notifications_status != 0){ update_notifications_status = 0; Update_notifications_mode_settings("0"); Select_update_notifications_mode_active(sender); }
        }
        private void Update_notifications_mode_settings(string get_update_notifications_value){
            try{
                TSSettingsSave software_setting_save = new TSSettingsSave(ts_sf);
                software_setting_save.TSWriteSettings(ts_settings_container, "UpdateNotifications", get_update_notifications_value);
            }catch (Exception){ }
        }
        // CHECK UPDATE
        // ======================================================================================================
        private void CheckForUpdateToolStripMenuItem_Click(object sender, EventArgs e){
            Task.Run(() => Software_update_check(1));
        }
        private void Software_update_check(int _check_update_ui){
            try{
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                SetUpdateMenuEnabled(false);
                if (!IsNetworkCheck()){
                    if (_check_update_ui == 1){
                        TS_MessageBoxEngine.TS_MessageBox(this, 2, string.Format(software_lang.TSReadLangs("SoftwareUpdate", "su_not_connection"), "\n\n"), string.Format(software_lang.TSReadLangs("SoftwareUpdate", "su_title"), Application.ProductName));
                    }
                    return;
                }
                using (WebClient getLastVersion = new WebClient()){
                    Version client_ver = Version.Parse(TS_VersionParser.ParseUINormalize(Application.ProductVersion));
                    Version last_ver = Version.Parse(TS_VersionParser.ParseUINormalize(getLastVersion.DownloadString(TS_LinkSystem.github_link_lv).Split('=')[1].Trim()));
                    string client_version_raw = client_ver.ToString();
                    string last_version_raw = last_ver.ToString();
                    bool isUpdateAvailable = client_ver < last_ver;
                    if (isUpdateAvailable){
                        if (TS_MessageBoxEngine.TS_MessageBox(this, 5, string.Format(software_lang.TSReadLangs("SoftwareUpdate", "su_available"), Application.ProductName, "\n\n", client_version_raw, "\n", last_version_raw, "\n\n", "\n\n"), string.Format(software_lang.TSReadLangs("SoftwareUpdate", "su_title"), Application.ProductName)) == DialogResult.Yes){
                            Process.Start(new ProcessStartInfo { FileName = updater_exe_name, WorkingDirectory = Path.GetDirectoryName(updater_exe_name) ?? Environment.CurrentDirectory });
                            Application.Exit();
                        }
                    }else if (_check_update_ui == 1){
                        string update_msg = client_ver == last_ver ? string.Format(software_lang.TSReadLangs("SoftwareUpdate", "su_not_available"), Application.ProductName, "\n", client_version_raw) : string.Format(software_lang.TSReadLangs("SoftwareUpdate", "su_newer"), "\n\n", $"v{client_version_raw}");
                        TS_MessageBoxEngine.TS_MessageBox(this, 1, update_msg, string.Format(software_lang.TSReadLangs("SoftwareUpdate", "su_title"), Application.ProductName));
                    }
                }
            }catch (Exception ex){
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                TS_MessageBoxEngine.TS_MessageBox(this, 3, string.Format(software_lang.TSReadLangs("SoftwareUpdate", "su_error"), "\n\n", ex.Message), string.Format(software_lang.TSReadLangs("SoftwareUpdate", "su_title"), Application.ProductName));
            }finally{
                SetUpdateMenuEnabled(true);
            }
        }
        private void SetUpdateMenuEnabled(bool enabled){
            if (InvokeRequired){
                BeginInvoke(new Action(() => checkForUpdateToolStripMenuItem.Enabled = enabled));
            }else{
                checkForUpdateToolStripMenuItem.Enabled = enabled;
            }
        }
        // CHECK FOR SOFTWARE UPDATE
        // ======================================================================================================
        private async void CheckForSoftwareUpdateToolStripMenuItem_Click(object sender, EventArgs e){
            checkForSoftwareUpdateToolStripMenuItem.Enabled = false;
            await Task.Run(() => { Ts_w_start_module(false); });
            checkForSoftwareUpdateToolStripMenuItem.Enabled = true;
            Dynamic_notification_ui();
        }
        private void Dynamic_notification_ui(){
            int[] install_status = new int[]{
                __astel_i_status,
                __encryphix_i_status,
                __glow_i_status,
                __vcardix_i_status,
                __vimera_i_status,
                __yamira_i_status,
                __zafuse_i_status,
            };
            bool[] update_status = new bool[]{
                __astel_u_status,
                __encryphix_u_status,
                __glow_u_status,
                __vcardix_u_status,
                __vimera_u_status,
                __yamira_u_status,
                __zafuse_u_status,
            };
            //
            if (!install_status.Any(i_status => i_status != 0)){
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                TS_MessageBoxEngine.TS_MessageBox(this, 1, software_lang.TSReadLangs("TSWizardUI", "s_u_no_installed_software"));
                return;
            }
            //
            List<string> update_list = new List<string>();
            for (int i = 0; i < update_status.Length; i++){
                if (install_status[i] != 0 && update_status[i]){
                    update_list.Add('\u25CF' + " " + ts_softwares_list[i]);
                }
            }
            //
            TSGetLangs softwareLang = new TSGetLangs(lang_path);
            if (update_list.Count > 0){
                TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(softwareLang.TSReadLangs("TSWizardUI", "s_u_available_updates"), "\n\n", string.Join(Environment.NewLine, update_list)));
            }else{
                TS_MessageBoxEngine.TS_MessageBox(this, 1, softwareLang.TSReadLangs("TSWizardUI", "s_u_no_available_updates"));
            }
            //
            update_list.Clear();
        }
        // DONATE LINK
        // ======================================================================================================
        private void DonateToolStripMenuItem_Click(object sender, EventArgs e){
            try{
                Process.Start(new ProcessStartInfo(TS_LinkSystem.ts_donate){ UseShellExecute = true });
            }catch (Exception){ }
        }
        // ABOUT
        // ======================================================================================================
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e){
            try{
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                TSWizardAbout tws_about = new TSWizardAbout();
                string tsw_about_name = "tswizard_about";
                tws_about.Name = tsw_about_name;
                if (Application.OpenForms[tsw_about_name] == null){
                    tws_about.Show();
                }else{
                    if (Application.OpenForms[tsw_about_name].WindowState == FormWindowState.Minimized){
                        Application.OpenForms[tsw_about_name].WindowState = FormWindowState.Normal;
                    }
                    Application.OpenForms[tsw_about_name].Activate();
                    TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(software_lang.TSReadLangs("HeaderHelp", "header_help_info_notification"), software_lang.TSReadLangs("HeaderMenu", "header_menu_about")));
                }
            }catch (Exception){ }
        }
        // CONTEXT MENU MODES
        // ======================================================================================================
        private void NotifyMode_BalloonTipClicked(object sender, EventArgs e){ Context_show_software(); }
        private void Context_show_software(){
            this.Show();
            if (WindowState == FormWindowState.Minimized){
                this.WindowState = FormWindowState.Normal;
            }
            this.BringToFront();
        }
        private void ShowAppToolStripMenuItem_Click(object sender, EventArgs e){
            Context_show_software();
        }
        private void SoftwareUpdateCheckToolStripMenuItem_Click(object sender, EventArgs e){
            Context_show_software();
            Task __run_reload_module = Task.Run(() => { Ts_w_start_module(false); });
            Dynamic_notification_ui();
        }
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e){
            Software_exit();
        }
        // SOFTWARE EXIT
        // ======================================================================================================
        private void Software_hide(FormClosingEventArgs e){ e.Cancel = true; this.Hide(); }
        private void Software_exit(){ exit_mode = true; loop_status = false; Application.Exit(); }
        private void Main_FormClosing(object sender, FormClosingEventArgs e){ if (behavior_mode_status == 1){ if (!exit_mode) { Software_hide(e); } }else{ Software_exit(); } }
    }
}