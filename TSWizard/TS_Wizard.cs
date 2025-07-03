// ======================================================================================================
// TS Wizard - Türkay Software Solutions
// © Copyright 2025, Eray Türkay.
// Project Type: Open Source
// License: MIT License
// Website: https://www.turkaysoftware.com/ts-wizard
// GitHub: https://github.com/turkaysoftware/tswizard
// ======================================================================================================

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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
// TS MODULES
using static TSWizard.TSModules;

namespace TSWizard{
    public partial class TS_Wizard : Form{
        public TS_Wizard(){
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            typeof(TableLayoutPanel).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, InTLP, new object[] { true });
            //
            NotifyMode.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            NotifyMode.Text = Application.ProductName;
            NotifyMode.ContextMenuStrip = NotifyMenu;
            NotifyMode.DoubleClick += (s, e) => context_show_software();
        }
        // GLOBAL VARIABLES
        // ======================================================================================================
        public static string lang, lang_path, software_launcher_mode;
        public static int theme, ts_active_software;
        public static string[] ts_softwares_list = { "Astel", "Glow", "Vimera", "Yamira" };
        public static string __astel_version = null, __glow_version = null, __vimera_version = null, __yamira_version = null;
        // LOCAL VARIABLES | 0 = Astel / 1 = Glow / 2 = Vimera / 3 = Yamira
        // ======================================================================================================
        static string ts_softwares_root_path = "ts_softwares";
        static string[] subfolders = { "ts_astel", "ts_glow", "ts_vimera", "ts_yamira" };
        static string[] ts_softwares_location = subfolders.Select(s => $"{ts_softwares_root_path}\\{s}").ToArray();
        static string[] ts_softwares_location_exe_name = { "Astel_x64.exe", "Glow_x64.exe", "Vimera_x64.exe", "Yamira_x64.exe" };
        string updater_exe_name = "TSWizardUpdater_x64.exe";
        //
        string[] ts_software_version_links ={
            "https://raw.githubusercontent.com/turkaysoftware/astel/main/Astel/SoftwareVersion.txt",      // Astel
            "https://raw.githubusercontent.com/turkaysoftware/glow/main/Glow/SoftwareVersion.txt",        // Glow
            "https://raw.githubusercontent.com/turkaysoftware/vimera/main/Vimera/SoftwareVersion.txt",    // Vimera
            "https://raw.githubusercontent.com/turkaysoftware/yamira/main/Yamira/SoftwareVersion.txt"     // Yamira
        };
        //
        int __astel_i_status = 0, __glow_i_status = 0, __vimera_i_status = 0, __yamira_i_status = 0, initial_status, behavior_mode_status, update_notifications_status, architecture_mode_status;
        bool __astel_u_status = false, __glow_u_status = false, __vimera_u_status = false, __yamira_u_status = false, dynamic_net_status, loop_status = true, exit_mode = false;
        // ======================================================================================================
        // COLOR MODES / Index Mode | 0 = Dark - 1 = Light
        static List<Color> header_colors = new List<Color>() { Color.Transparent, Color.Transparent, Color.Transparent };
        // HEADER SETTINGS
        // ======================================================================================================
        private class HeaderMenuColors : ToolStripProfessionalRenderer{
            public HeaderMenuColors() : base(new HeaderColors()){ }
            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e){ e.ArrowColor = header_colors[1]; base.OnRenderArrow(e); }
            protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e){
                // e.Graphics.FillRectangle(new SolidBrush(header_colors[0]), e.ImageRectangle); // BG Color Check
                using (Pen anti_alias_pen = new Pen(header_colors[2], 3)){
                    Point p1 = new Point(e.ImageRectangle.Left + 3, e.ImageRectangle.Top + e.ImageRectangle.Height / 2);
                    Point p2 = new Point(e.ImageRectangle.Left + 7, e.ImageRectangle.Bottom - 4);
                    Point p3 = new Point(e.ImageRectangle.Right - 2, e.ImageRectangle.Top + 3);
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawLines(anti_alias_pen, new Point[] { p1, p2, p3 });
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
            int theme_mode = int.TryParse(TS_String_Encoder(software_read_settings.TSReadSettings(ts_settings_container, "ThemeStatus")), out int the_status) ? the_status : 1;
            theme_engine(theme_mode);
            darkThemeToolStripMenuItem.Checked = theme_mode == 0;
            lightThemeToolStripMenuItem.Checked = theme_mode == 1;
            string lang_mode = TS_String_Encoder(software_read_settings.TSReadSettings(ts_settings_container, "LanguageStatus"));
            var languageFiles = new Dictionary<string, (object langResource, ToolStripMenuItem menuItem, bool fileExists)>{
                { "en", (ts_lang_en, englishToolStripMenuItem, File.Exists(ts_lang_en)) },
                { "tr", (ts_lang_tr, turkishToolStripMenuItem, File.Exists(ts_lang_tr)) },
            };
            foreach (var langLoader in languageFiles) { langLoader.Value.menuItem.Enabled = langLoader.Value.fileExists; }
            var (langResource, selectedMenuItem, _) = languageFiles.ContainsKey(lang_mode) ? languageFiles[lang_mode] : languageFiles["en"];
            lang_engine(Convert.ToString(langResource), lang_mode);
            selectedMenuItem.Checked = true;
            //
            string initial_mode = TS_String_Encoder(software_read_settings.TSReadSettings(ts_settings_container, "InitialStatus"));
            initial_status = int.TryParse(initial_mode, out int ini_status) && (ini_status == 0 || ini_status == 1) ? ini_status : 0;
            WindowState = initial_status == 1 ? FormWindowState.Maximized : FormWindowState.Normal;
            windowedToolStripMenuItem.Checked = initial_status == 0;
            fullScreenToolStripMenuItem.Checked = initial_status == 1;
            //
            string window_behavior_mode = TS_String_Encoder(software_read_settings.TSReadSettings(ts_settings_container, "WindowBehavior"));
            behavior_mode_status = int.TryParse(window_behavior_mode, out int wbm_status) && (wbm_status == 0 || wbm_status == 1) ? wbm_status : 0;
            iconStatusToolStripMenuItem.Checked = behavior_mode_status == 1;
            closeSoftwareToolStripMenuItem.Checked = behavior_mode_status == 0;
            //
            string update_notifications = TS_String_Encoder(software_read_settings.TSReadSettings(ts_settings_container, "UpdateNotifications"));
            update_notifications_status = int.TryParse(update_notifications, out int un_status) && (un_status == 0 || un_status == 1) ? un_status : 0;
            notificationOnToolStripMenuItem.Checked = update_notifications_status == 1;
            notificationOffToolStripMenuItem.Checked = update_notifications_status == 0;
            //
            string architecture_mode = TS_String_Encoder(software_read_settings.TSReadSettings(ts_settings_container, "ArchitectureMode"));
            architecture_mode_status = int.TryParse(architecture_mode, out int am_status) && (am_status >= 0 && am_status <= 2) ? am_status : 0;
            x64BitToolStripMenuItem.Checked = architecture_mode_status == 0;
            aRM64ToolStripMenuItem.Checked = architecture_mode_status == 1;
            manualArchitectureToolStripMenuItem.Checked = architecture_mode_status == 2;
            //
            set_random_header_image();
            //
            MPanelAstelImage.BackgroundImage = Properties.Resources.astel_banner;
            MPanelGlowImage.BackgroundImage = Properties.Resources.glow_banner;
            MPanelVimeraImage.BackgroundImage = Properties.Resources.vimera_banner;
            MPanelYamiraImage.BackgroundImage = Properties.Resources.yamira_banner;
            //
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            MPanelAstelWizardBtn.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_loader_text"));
            MPanelGlowWizardBtn.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_loader_text"));
            MPanelVimeraWizardBtn.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_loader_text"));
            MPanelYamiraWizardBtn.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_loader_text"));
            //
            MPanelAstelInText.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_loader_text"));
            MPanelGlowInText.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_loader_text"));
            MPanelVimeraInText.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_loader_text"));
            MPanelYamiraInText.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_loader_text"));
            // UI REPOSITIONING AND DYNAMICIZING
            InTLP.ColumnCount = 4;
            InTLP.RowCount = 1;
            //
            InTLP.ColumnStyles.Clear();
            InTLP.RowStyles.Clear();
            //
            for (int i = 0; i < InTLP.ColumnCount; i++){
                InTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / InTLP.ColumnCount));
            }
            for (int i = 0; i < InTLP.RowCount; i++){
                InTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / InTLP.RowCount));
            }
        }
        // SET RANDOM HEADER IMAGE
        // ======================================================================================================
        private void set_random_header_image(){
            Image[] header_images = new Image[]{
                Properties.Resources.header_banner_1,
                Properties.Resources.header_banner_2,
                Properties.Resources.header_banner_3,
                Properties.Resources.header_banner_4,
                Properties.Resources.header_banner_5
            };
            Random random_generate = new Random();
            HeaderBanner.BackgroundImage = header_images[random_generate.Next(header_images.Length)];
        }
        // TOOLTIP SET CONFIG
        // ======================================================================================================
        private void MainToolTip_Draw(object sender, DrawToolTipEventArgs e) { e.DrawBackground(); e.DrawBorder(); e.DrawText(); }
        // LOAD
        // ======================================================================================================
        private void TS_Wizard_Load(object sender, EventArgs e){
            Text = TS_VersionEngine.TS_SofwareVersion(0, Program.ts_version_mode);
            HeaderMenu.Cursor = Cursors.Hand;
            NotifyMenu.Cursor = Cursors.Hand;
            // RUN ENGINE
            RunSoftwareEngine();
            // UPDATE CHECKER
            // Task softwareUpdateCheck = Task.Run(() => software_update_check(0));
            // SERVER CONNECTION DELAY
            Task ts_website_control = Task.Run(() => { ts_website_uptime_module(); });
            // AUTOMATIC UPDATE CONTROLLER
            Task ts_avaible_update = Task.Run(() => { auto_check_avaible_update(); });
            // START CHECK MODULES
            Task ts_wizard_start_module = Task.Run(() => {
                if (update_notifications_status == 1){
                    ts_w_start_module(true);
                }else{
                    ts_w_start_module(false);
                }
            });
        }
        // WEBSITE DELAY MODULE
        // ======================================================================================================
        private void ts_website_uptime_module(){
            while (loop_status){
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                try{
                    bool net_status = IsNetworkCheck();
                    Image successImage = (theme == 1) ? Properties.Resources.m_network_success_light : Properties.Resources.m_network_success_dark;
                    Image failImage = (theme == 1) ? Properties.Resources.m_network_failed_light : Properties.Resources.m_network_failed_dark;
                    MLNetwork.Image = net_status ? successImage : failImage;
                    dynamic_net_status = net_status;
                    BottomText.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", net_status ? "s_active_internet" : "s_deactive_internet"));
                }catch (Exception){ }
                Thread.Sleep(45000);
            }
        }
        // TS WIZARD START MODULE
        // ======================================================================================================
        private void ts_w_start_module(bool __notifi_mode){
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            try{
                if (!Directory.Exists(ts_softwares_root_path))
                    Directory.CreateDirectory(ts_softwares_root_path);
                //
                var wizardButtons = new[]{ MPanelAstelWizardBtn, MPanelGlowWizardBtn, MPanelVimeraWizardBtn, MPanelYamiraWizardBtn };
                var removeButtons = new[]{ MPanelAstelRemoveBtn, MPanelGlowRemoveBtn, MPanelVimeraRemoveBtn, MPanelYamiraRemoveBtn };
                for (int i = 0; i < ts_softwares_location.Length; i++){
                    string dirPath = ts_softwares_location[i];
                    string exeName = ts_softwares_location_exe_name[i];
                    string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dirPath, exeName);
                    //
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);
                    //
                    var wizardBtn = wizardButtons[i];
                    var removeBtn = removeButtons[i];
                    if (File.Exists(exePath)){
                        var downloaded_software_version = string.Empty;
                        if (Program.ts_version_mode == 0){
                            downloaded_software_version = FileVersionInfo.GetVersionInfo(exePath).FileVersion.Substring(0, 5);
                        }else{
                            downloaded_software_version = FileVersionInfo.GetVersionInfo(exePath).FileVersion.Substring(0, 7);
                        }
                        software_update_checker(i, downloaded_software_version, true, __notifi_mode);
                        removeBtn.Enabled = true;
                    }else{
                        software_update_checker(i, "0", false, __notifi_mode);
                        wizardBtn.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_download"));
                        removeBtn.Enabled = false;
                    }
                }
            }catch (Exception){ }
            // Enable Software Details Page Buttons
            MPanelAstelLinkText.Enabled = true;
            MPanelGlowLinkText.Enabled = true;
            MPanelVimeraLinkText.Enabled = true;
            MPanelYamiraLinkText.Enabled = true;
        }
        // TIMED AUTOMATIC UPDATE CONTROLLER
        // ======================================================================================================
        private void auto_check_avaible_update(){
            try{
                while (loop_status){
                    Thread.Sleep(1800000); // 30 min
                    if (WindowState == FormWindowState.Minimized){
                        ts_w_start_module(true);
                    }else{
                        ts_w_start_module(false);
                    }
                }
            }catch (Exception){ }
        }
        // SOFTWARE MODULE VERSION CHECKER
        // ======================================================================================================
        private void software_update_checker(int __software_mode, string __software_version, bool __installed_status, bool __notify_mode){
            try{
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                //
                using (WebClient webClient = new WebClient()){
                    string client_version = __software_version;
                    int client_num_version = Convert.ToInt32(client_version.Replace(".", string.Empty));
                    //
                    string[] version_content = webClient.DownloadString(ts_software_version_links[__software_mode]).Split('=');
                    string last_version = version_content[1].Trim();
                    int last_num_version = Convert.ToInt32(last_version.Replace(".", string.Empty));
                    //
                    string[] softwareVersions = new string[4] { __astel_version, __glow_version, __vimera_version, __yamira_version };
                    softwareVersions[__software_mode] = last_version;
                    __astel_version = softwareVersions[0];
                    __glow_version = softwareVersions[1];
                    __vimera_version = softwareVersions[2];
                    __yamira_version = softwareVersions[3];
                    //
                    var wizardButtons = new Button[] { MPanelAstelWizardBtn, MPanelGlowWizardBtn, MPanelVimeraWizardBtn, MPanelYamiraWizardBtn };
                    var removeButtons = new Button[] { MPanelAstelRemoveBtn, MPanelGlowRemoveBtn, MPanelVimeraRemoveBtn, MPanelYamiraRemoveBtn };
                    bool isUpdateAvailable = client_num_version < last_num_version;
                    //
                    var soft_descriptions = new[]{
                        TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_t_astel")),
                        TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_t_glow")),
                        TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_t_vimera")),
                        TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_t_yamira"))
                    };
                    // UI Elements
                    var infoLabels = new[]{ MPanelAstelInText, MPanelGlowInText, MPanelVimeraInText, MPanelYamiraInText };
                    //
                    string statusText;
                    if (!__installed_status)
                        statusText = string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_u_not_downloaded")), last_version);
                    else if (isUpdateAvailable)
                        statusText = string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_u_available")), __software_version, "\n", last_version);
                    else
                        statusText = string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_u_no_available")), last_version, "\n", TS_FormatSize(new DirectoryInfo(ts_softwares_location[__software_mode]).GetFiles("*", SearchOption.AllDirectories).Sum(f => f.Length)));
                    //
                    if (__software_mode >= 0 && __software_mode < soft_descriptions.Length){
                        infoLabels[__software_mode].Text = $"{soft_descriptions[__software_mode]}\n{statusText}";
                    }
                    //
                    wizardButtons[__software_mode].Text = isUpdateAvailable ? TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_update")) : TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_launch"));
                    removeButtons[__software_mode].Enabled = true;
                    // -------------
                    int __colorStatus = 0;
                    if (isUpdateAvailable)
                        __colorStatus = __installed_status ? 2 : 1;
                        dynamic_button_colors(__software_mode, __colorStatus);
                    if (__colorStatus != 0)
                        dynamic_button_colors(__software_mode, __colorStatus);
                    //
                    if (__installed_status){
                        switch (__software_mode){
                            case 0: __astel_i_status = 1; sc_v_mode(0, true); break;
                            case 1: __glow_i_status = 1; sc_v_mode(1, true); break;
                            case 2: __vimera_i_status = 1; sc_v_mode(2, true); break;
                            case 3: __yamira_i_status = 1; sc_v_mode(3, true); break;
                        }
                    }
                    //
                    bool updateStatus = (__installed_status && isUpdateAvailable);
                    switch (__software_mode){
                        case 0: __astel_u_status = updateStatus; if (__notify_mode && __astel_u_status) ts_notifi_send_module(__software_mode, last_version); break;
                        case 1: __glow_u_status = updateStatus; if (__notify_mode && __glow_u_status) ts_notifi_send_module(__software_mode, last_version); break;
                        case 2: __vimera_u_status = updateStatus; if (__notify_mode && __vimera_u_status) ts_notifi_send_module(__software_mode, last_version); break;
                        case 3: __yamira_u_status = updateStatus; if (__notify_mode && __yamira_u_status) ts_notifi_send_module(__software_mode, last_version); break;
                    }
                }
            }catch (Exception){ }
        }
        // NOTIFICATION SENDİNG MODULE
        // ======================================================================================================
        private void ts_notifi_send_module(int __software_mode, string __software_version){
            try{
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                NotifyMode.ShowBalloonTip(4500, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_u_notifi_title")), ts_softwares_list[__software_mode]), string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_u_notifi_content")), ts_softwares_list[__software_mode], __software_version), ToolTipIcon.Info);
            }catch (Exception){ }
        }
        // CREATE A SHORTCUT MODULE
        // ======================================================================================================
        private void sc_v_mode(int __s_software, bool __s_mode){
            Control[] sc_btns = { MPanelAstelShortcutBtn, MPanelGlowShortcutBtn, MPanelVimeraShortcutBtn, MPanelYamiraShortcutBtn };
            //
            if (__s_software >= 0 && __s_software < sc_btns.Length){
                sc_btns[__s_software].Enabled = __s_mode;
            }
        }
        private void handle_shortcut_config(int softwareIndex){
            string get_s_name = ts_softwares_location_exe_name[softwareIndex].Trim();
            if (architecture_mode_status == 1){
                get_s_name = get_s_name.Replace("x64", "arm64");
            }
            string path = Path.Combine(Application.StartupPath, ts_softwares_location[softwareIndex], get_s_name);
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            DialogResult sc_create_message = TS_MessageBoxEngine.TS_MessageBox(this, 5, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_sc_content")), ts_softwares_list[softwareIndex]));
            if (sc_create_message == DialogResult.Yes){
                sc_create_script(path, softwareIndex);
            }
        }
        private void MPanelAstelShortcutBtn_Click(object sender, EventArgs e){ handle_shortcut_config(0); }
        private void MPanelGlowShortcutBtn_Click(object sender, EventArgs e){ handle_shortcut_config(1); }
        private void MPanelVimeraShortcutBtn_Click(object sender, EventArgs e){ handle_shortcut_config(2); }
        private void MPanelYamiraShortcutBtn_Click(object sender, EventArgs e){ handle_shortcut_config(3); }
        private void sc_create_script(string __target_exe_path, int __s_mode){
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            try{
                string desktop_location = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string shortcut_location = Path.Combine(desktop_location, ts_softwares_list[__s_mode] + ".lnk");
                string hover_message = Application.CompanyName + " - " + Application.ProductName;
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
                DialogResult sc_success_message = TS_MessageBoxEngine.TS_MessageBox(this, 5, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_sc_success")), ts_softwares_list[__s_mode], shortcut_location, "\n\n"));
                if (sc_success_message == DialogResult.Yes){
                    string sc_location = string.Format("/select, \"{0}\"", shortcut_location.Trim().Replace("/", @"\"));
                    ProcessStartInfo psi = new ProcessStartInfo("explorer.exe", sc_location);
                    Process.Start(psi);
                }
            }catch (Exception ex){
                DialogResult sc_success_message = TS_MessageBoxEngine.TS_MessageBox(this, 3, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_sc_success")), "\n\n", ex.Message));
            }
        }
        // SOFTWARE DETAIL ROTATE BUTTONS
        // ======================================================================================================
        private void MPanelAstelLinkText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){ launch_software_details_dynamic(0); }
        private void MPanelGlowLinkText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){ launch_software_details_dynamic(1); }
        private void MPanelVimeraLinkText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){ launch_software_details_dynamic(2); }
        private void MPanelYamiraLinkText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){ launch_software_details_dynamic(3); }
        private void launch_software_details_dynamic(int s_mode){
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
                    TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(TS_String_Encoder(software_lang.TSReadLangs("HeaderHelp", "header_help_info_notification")), TS_String_Encoder(software_lang.TSReadLangs("HeaderMenu", "header_menu_about"))));
                }
            }catch (Exception){ }
        }
        // SOFTWARE OPERATION CONTROLLER MODULE
        // ======================================================================================================
        public static bool software_operation_controller(string __target_software_path){
            var exeFiles = Directory.GetFiles(__target_software_path, "*.exe");
            var runned_process = Process.GetProcesses();
            foreach (var exe_path in exeFiles){
                string exe_name = Path.GetFileNameWithoutExtension(exe_path);
                if (runned_process.Any(p =>{
                    try{
                        return string.Equals(p.ProcessName, exe_name, StringComparison.OrdinalIgnoreCase);
                    }catch{
                        return false; // erişim hatalarında false döndür
                    }
                })){
                    return true; // çalışan exe bulundu
                }
            }
            return false; // hiçbiri çalışmıyorsa
        }
        // START OR DOWNLOAD AND UPDATE BUTTONS
        // ======================================================================================================
        private async void MPanelAstelWizardBtn_Click(object sender, EventArgs e){
            await HandleWizardButtonClickAsync(0, __astel_i_status, __astel_u_status, __astel_version);
        }
        private async void MPanelGlowWizardBtn_Click(object sender, EventArgs e){
            await HandleWizardButtonClickAsync(1, __glow_i_status, __glow_u_status, __glow_version);
        }
        private async void MPanelVimeraWizardBtn_Click(object sender, EventArgs e){
            await HandleWizardButtonClickAsync(2, __vimera_i_status, __vimera_u_status, __vimera_version);
        }
        private async void MPanelYamiraWizardBtn_Click(object sender, EventArgs e){
            await HandleWizardButtonClickAsync(3, __yamira_i_status, __yamira_u_status, __yamira_version);
        }
        private async Task HandleWizardButtonClickAsync(int softwareIndex, int installedStatus, bool updateStatus, string softwareVersion){
            try{
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                if (installedStatus == 1 && !updateStatus){
                    string app_path = $"{Application.StartupPath}\\{ts_softwares_location[softwareIndex]}\\{ts_softwares_location_exe_name[softwareIndex]}";
                    software_launcher_mode = app_path;
                    if (architecture_mode_status == 2){
                        TS_AppLauncher ts_appLauncher = new TS_AppLauncher();
                        ts_appLauncher.ShowDialog();
                    }else{
                        if (architecture_mode_status == 1){
                            app_path = $"{Path.GetDirectoryName(app_path)}\\{Path.GetFileName(app_path).Replace("x64", "arm64")}";
                        }
                        //
                        if (!software_operation_controller(ts_softwares_location[softwareIndex])){
                            Process.Start(new ProcessStartInfo { FileName = app_path, WorkingDirectory = Path.GetDirectoryName(app_path) });
                        }else{
                            TS_MessageBoxEngine.TS_MessageBox(this, 1, TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_currently_operational")));
                        }
                    }
                    return;
                }
                //
                DialogResult userChoice;
                if (updateStatus){
                    if (!software_operation_controller(ts_softwares_location[softwareIndex])){
                        userChoice = TS_MessageBoxEngine.TS_MessageBox(this, 5, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_update_select")), "\n\n", ts_softwares_list[softwareIndex]));
                    }else{
                        TS_MessageBoxEngine.TS_MessageBox(this, 2, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_delete_exe_active")), "\n", "\n\n", ts_softwares_list[softwareIndex]));
                        userChoice = DialogResult.No;
                    }
                }else{
                    userChoice = TS_MessageBoxEngine.TS_MessageBox(this, 5, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_download_select")), "\n\n", ts_softwares_list[softwareIndex]));
                }
                if (userChoice == DialogResult.Yes){
                    await ts_downloader_engine_async(softwareIndex, softwareVersion, updateStatus);
                    //
                    switch (softwareIndex){
                        case 0: __astel_u_status = false; break;
                        case 1: __glow_u_status = false; break;
                        case 2: __vimera_u_status = false; break;
                        case 3: __yamira_u_status = false; break;
                    }
                }
            }catch (Exception ex){
                Debug.WriteLine($"[WizardButtonError - Index {softwareIndex}] {ex.Message}");
            }
        }
        // ADVANCED DOWNLOAD AND EXTRACT MODULE
        // ======================================================================================================
        private async Task ts_downloader_engine_async(int __software_mode, string __software_version, bool __software_update_mode){
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            string ___local_down_message = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_downloading"));
            //
            string softwareName = ts_softwares_list[__software_mode].Trim();
            string softwareSlug = softwareName.ToLower();
            // Dynamic URL Module
            string dynamic_url = $"https://github.com/turkaysoftware/{softwareSlug}/releases/download/v{__software_version}/{softwareName}_v{__software_version}.zip";
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string targetFolder = Path.Combine(baseDir, ts_softwares_location[__software_mode]);
            // Download or Update before delete folder
            if (Directory.Exists(targetFolder)){
                Directory.Delete(targetFolder, recursive: true);
            }
            Directory.CreateDirectory(targetFolder);
            //
            string zipFilePath = Path.Combine(targetFolder, $"{softwareName}_v{__software_version}.zip");
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
                                switch (__software_mode){
                                    case 0: MPanelAstelWizardBtn.Text = string.Format(___local_down_message, progress, TS_FormatSize(totalRead)); break;
                                    case 1: MPanelGlowWizardBtn.Text = string.Format(___local_down_message, progress, TS_FormatSize(totalRead)); break;
                                    case 2: MPanelVimeraWizardBtn.Text = string.Format(___local_down_message, progress, TS_FormatSize(totalRead)); break;
                                    case 3: MPanelYamiraWizardBtn.Text = string.Format(___local_down_message, progress, TS_FormatSize(totalRead)); break;
                                }
                            }
                        }
                    }
                }
                // Console.WriteLine("Download completed.");
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
                        if (string.IsNullOrEmpty(entry.Name)){
                            continue;
                        }
                        //
                        string relativePath = entry.FullName;
                        if (!string.IsNullOrEmpty(rootFolderInZip) && (relativePath.StartsWith(rootFolderInZip + "/") || relativePath.StartsWith(rootFolderInZip + "\\"))){
                            relativePath = relativePath.Substring(rootFolderInZip.Length + 1);
                        }
                        //
                        string destinationPath = Path.Combine(targetFolder, relativePath);
                        string destinationDir = Path.GetDirectoryName(destinationPath);
                        //
                        if (!Directory.Exists(destinationDir)){
                            Directory.CreateDirectory(destinationDir);
                        }
                        entry.ExtractToFile(destinationPath, overwrite: true);
                    }
                }
                File.Delete(zipFilePath);
                // Console.WriteLine("Extraction completed, zip file deleted.");
                //
                var statuses = new Action[]{ () => __astel_i_status = 1, () => __glow_i_status = 1, () => __vimera_i_status = 1, () => __yamira_i_status = 1 };
                var wizardButtons = new Button[]{ MPanelAstelWizardBtn, MPanelGlowWizardBtn, MPanelVimeraWizardBtn, MPanelYamiraWizardBtn };
                var removeButtons = new Button[]{ MPanelAstelRemoveBtn, MPanelGlowRemoveBtn, MPanelVimeraRemoveBtn, MPanelYamiraRemoveBtn };
                if (__software_mode >= 0 && __software_mode < statuses.Length){
                    statuses[__software_mode]();
                    wizardButtons[__software_mode].Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_launch"));
                    removeButtons[__software_mode].Enabled = true;
                    dynamic_button_colors(__software_mode, 0);
                    sc_v_mode(__software_mode, true);
                }
                // Label/Text UI Elements
                var soft_descriptions = new[]{
                   TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_t_astel")),
                   TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_t_glow")),
                   TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_t_vimera")),
                   TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_t_yamira"))
                };
                var infoLabels = new[]{ MPanelAstelInText, MPanelGlowInText, MPanelVimeraInText, MPanelYamiraInText };
                if (__software_mode >= 0 && __software_mode < soft_descriptions.Length){
                    infoLabels[__software_mode].Text = $"{soft_descriptions[__software_mode]}\n{string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_u_no_available")), __software_version, "\n", TS_FormatSize(new DirectoryInfo(ts_softwares_location[__software_mode]).GetFiles("*", SearchOption.AllDirectories).Sum(f => f.Length)))}";
                }
                //
                if (!__software_update_mode){
                    TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_download_success")), ts_softwares_list[__software_mode], "\n\n"));
                }else{
                    TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_update_success")), ts_softwares_list[__software_mode], "\n\n"));
                }
            }catch (Exception ex){
                if (!__software_update_mode){
                    TS_MessageBoxEngine.TS_MessageBox(this, 3, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_download_error")), "\n\n", "\n\n", ts_softwares_list[__software_mode], "\n\n", ex.Message));
                }else{
                    TS_MessageBoxEngine.TS_MessageBox(this, 3, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_update_error")), "\n\n", "\n\n", ts_softwares_list[__software_mode], "\n\n", ex.Message));
                }
            }
        }
        // REMOVE SOFTWARE MODULE
        // ======================================================================================================
        private void MPanelAstelRemoveBtn_Click(object sender, EventArgs e){
            try{
                ts_module_deleter_check(0);
            }catch (Exception){ }
        }
        private void MPanelGlowRemoveBtn_Click(object sender, EventArgs e){
            try{
                ts_module_deleter_check(1);
            }catch (Exception){ }
        }
        private void MPanelVimeraRemoveBtn_Click(object sender, EventArgs e){
            try{
                ts_module_deleter_check(2);
            }catch (Exception){ }
        }
        private void MPanelYamiraRemoveBtn_Click(object sender, EventArgs e){
            try{
                ts_module_deleter_check(3);
            }catch (Exception){ }
        }
        private void ts_module_deleter_check(int __software_mode){
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            DialogResult lang_change_message = TS_MessageBoxEngine.TS_MessageBox(this, 6, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_delete_query")), "\n\n", ts_softwares_list[__software_mode]));
            if (lang_change_message == DialogResult.Yes){
                ts_module_deleter($"{Application.StartupPath}\\{ts_softwares_location[__software_mode]}", __software_mode);
            }
        }
        private void ts_module_deleter(string __target_software_path, int __software_mode){
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            // No folder
            if (!Directory.Exists(__target_software_path)){
                return;
            }
            //
            if (software_operation_controller(ts_softwares_location[__software_mode])){
                TS_MessageBoxEngine.TS_MessageBox(this, 2, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_delete_exe_active")), "\n", "\n\n", ts_softwares_list[__software_mode]));
            }else{
                try{
                    foreach (string file in Directory.GetFiles(__target_software_path))
                        File.Delete(file);
                    foreach (string dir in Directory.GetDirectories(__target_software_path))
                        Directory.Delete(dir, recursive: true);
                    //
                    var soft_status = new Action[]{ () => __astel_i_status = 0, () => __glow_i_status = 0, () => __vimera_i_status = 0, () => __yamira_i_status = 0 };
                    //
                    if (__software_mode >= 0 && __software_mode < soft_status.Length){
                        soft_status[__software_mode]();
                        dynamic_button_colors(__software_mode, 1);
                        sc_v_mode(__software_mode, false);
                        Task __run_reload_module = Task.Run(() => { ts_w_start_module(false); });
                    }
                    TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_delete_success")), "\n\n", ts_softwares_list[__software_mode]));
                }catch (Exception ex){
                    TS_MessageBoxEngine.TS_MessageBox(this, 3, string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_delete_error")), "\n\n", "\n\n", ts_softwares_list[__software_mode], "\n\n", ex.Message));
                }
            }
        }
        // THEME MODE
        // ======================================================================================================
        private void select_theme_active(object target_theme){
            ToolStripMenuItem selected_theme = null;
            select_theme_deactive();
            if (target_theme != null){
                if (selected_theme != (ToolStripMenuItem)target_theme){
                    selected_theme = (ToolStripMenuItem)target_theme;
                    selected_theme.Checked = true;
                }
            }
        }
        private void select_theme_deactive(){
            foreach (ToolStripMenuItem disabled_theme in themeToolStripMenuItem.DropDownItems){
                disabled_theme.Checked = false;
            }
        }
        private void lightThemeToolStripMenuItem_Click(object sender, EventArgs e){
            if (theme != 1){ theme_engine(1); select_theme_active(sender); }
        }
        private void darkThemeToolStripMenuItem_Click(object sender, EventArgs e){
            if (theme != 0){ theme_engine(0); select_theme_active(sender); }
        }
        // THEME ENGINE
        // ======================================================================================================
        private void theme_engine(int ts){
            try{
                theme = ts;
                int set_attribute = theme == 1 ? 20 : 19;
                if (DwmSetWindowAttribute(Handle, set_attribute, new[] { 1 }, 4) != theme){
                    DwmSetWindowAttribute(Handle, 20, new[] { theme == 1 ? 0 : 1 }, 4);
                }
                //
                if (theme == 1){
                    settingsToolStripMenuItem.Image = Properties.Resources.m_settings_light;
                    themeToolStripMenuItem.Image = Properties.Resources.m_themes_light;
                    languageToolStripMenuItem.Image = Properties.Resources.m_language_light;
                    initialViewToolStripMenuItem.Image = Properties.Resources.m_start_view_light;
                    windowBehaviorToolStripMenuItem.Image = Properties.Resources.m_tab_mode_light;
                    updateNotificationsToolStripMenuItem.Image = Properties.Resources.m_notifications_light;
                    architectureModeToolStripMenuItem.Image = Properties.Resources.m_architecture_light;
                    checkForUpdateToolStripMenuItem.Image = Properties.Resources.m_update_light;
                    aboutToolStripMenuItem.Image = Properties.Resources.m_about_light;
                    //
                    checkForSoftwareUpdateToolStripMenuItem.Image = Properties.Resources.m_s_check_update_light;
                    //
                    showAppToolStripMenuItem.Image = Properties.Resources.m_show_light;
                    softwareUpdateCheckToolStripMenuItem.Image = Properties.Resources.m_s_check_update_light;
                    exitToolStripMenuItem.Image = Properties.Resources.m_exit_light;
                    //
                    MPanelAstelShortcutBtn.Image = Properties.Resources.m_shortcut_light;
                    MPanelGlowShortcutBtn.Image = Properties.Resources.m_shortcut_light;
                    MPanelVimeraShortcutBtn.Image = Properties.Resources.m_shortcut_light;
                    MPanelYamiraShortcutBtn.Image = Properties.Resources.m_shortcut_light;
                    //
                    MLWeb.Image = Properties.Resources.m_web_light;
                    MLX.Image = Properties.Resources.m_x_light;
                    MLInstagram.Image = Properties.Resources.m_instagram_light;
                    MLGitHub.Image = Properties.Resources.m_github_light;
                    //
                    if (dynamic_net_status){
                        MLNetwork.Image = Properties.Resources.m_network_success_light;
                    }else if (!dynamic_net_status){
                        MLNetwork.Image = Properties.Resources.m_network_failed_light;
                    }
                }else if (theme == 0){
                    settingsToolStripMenuItem.Image = Properties.Resources.m_settings_dark;
                    themeToolStripMenuItem.Image = Properties.Resources.m_themes_dark;
                    languageToolStripMenuItem.Image = Properties.Resources.m_language_dark;
                    initialViewToolStripMenuItem.Image = Properties.Resources.m_start_view_dark;
                    windowBehaviorToolStripMenuItem.Image = Properties.Resources.m_tab_mode_dark;
                    updateNotificationsToolStripMenuItem.Image = Properties.Resources.m_notifications_dark;
                    architectureModeToolStripMenuItem.Image = Properties.Resources.m_architecture_dark;
                    checkForUpdateToolStripMenuItem.Image = Properties.Resources.m_update_dark;
                    aboutToolStripMenuItem.Image = Properties.Resources.m_about_dark;
                    //
                    checkForSoftwareUpdateToolStripMenuItem.Image = Properties.Resources.m_s_check_update_dark;
                    //
                    showAppToolStripMenuItem.Image = Properties.Resources.m_show_dark;
                    softwareUpdateCheckToolStripMenuItem.Image = Properties.Resources.m_s_check_update_dark;
                    exitToolStripMenuItem.Image = Properties.Resources.m_exit_dark;
                    //
                    MPanelAstelShortcutBtn.Image = Properties.Resources.m_shortcut_dark;
                    MPanelGlowShortcutBtn.Image = Properties.Resources.m_shortcut_dark;
                    MPanelVimeraShortcutBtn.Image = Properties.Resources.m_shortcut_dark;
                    MPanelYamiraShortcutBtn.Image = Properties.Resources.m_shortcut_dark;
                    //
                    MLWeb.Image = Properties.Resources.m_web_dark;
                    MLX.Image = Properties.Resources.m_x_dark;
                    MLInstagram.Image = Properties.Resources.m_instagram_dark;
                    MLGitHub.Image = Properties.Resources.m_github_dark;
                    //
                    if (dynamic_net_status){
                        MLNetwork.Image = Properties.Resources.m_network_success_dark;
                    }else if (!dynamic_net_status){
                        MLNetwork.Image = Properties.Resources.m_network_failed_dark;
                    }
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
                // CONTEXT MENU
                showAppToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                showAppToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                softwareUpdateCheckToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                softwareUpdateCheckToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                exitToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                exitToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                // HEADER MENU
                HeaderMenu.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                HeaderMenu.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                // HEADER MENU CONTENT
                // SETTINGS
                settingsToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                settingsToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                // THEMES
                themeToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                themeToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                lightThemeToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                lightThemeToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                darkThemeToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                darkThemeToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                // LANGS
                languageToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                languageToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                englishToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                englishToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                turkishToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                turkishToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                // INITIAL VIEW
                initialViewToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                initialViewToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                windowedToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                windowedToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                fullScreenToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                fullScreenToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                // WINDOW BEHAVIOR
                windowBehaviorToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                windowBehaviorToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                iconStatusToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                iconStatusToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                closeSoftwareToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                closeSoftwareToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                // NOTIFICATIONS
                updateNotificationsToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                updateNotificationsToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                notificationOnToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                notificationOnToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                notificationOffToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                notificationOffToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                // ARCHITECTURE
                architectureModeToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                architectureModeToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                x64BitToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                x64BitToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                aRM64ToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                aRM64ToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                manualArchitectureToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                manualArchitectureToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                // UPDATE
                checkForUpdateToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                checkForUpdateToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                // CHECK FOR SOFTWARE UPDATE
                checkForSoftwareUpdateToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                checkForSoftwareUpdateToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                // ABOUT
                aboutToolStripMenuItem.BackColor = TS_ThemeEngine.ColorMode(theme, "HeaderBGColor");
                aboutToolStripMenuItem.ForeColor = TS_ThemeEngine.ColorMode(theme, "HeaderFEColor");
                // UI
                // ===========================================
                BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                HeaderBanner.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                HeaderText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                // CONTENT
                // ===========================================
                InTLP.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelAstel.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelGlow.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelVimera.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MPanelYamira.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                //
                MPanelAstelImage.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelGlowImage.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelVimeraImage.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelYamiraImage.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                //
                MPanelAstelHeadText.ForeColor = TS_ThemeEngine.ColorMode(theme, "AstelFE");
                MPanelGlowHeadText.ForeColor = TS_ThemeEngine.ColorMode(theme, "GlowFE");
                MPanelVimeraHeadText.ForeColor = TS_ThemeEngine.ColorMode(theme, "VimeraFE");
                MPanelYamiraHeadText.ForeColor = TS_ThemeEngine.ColorMode(theme, "YamiraFE");
                //
                MPanelAstelLinkText.LinkColor = TS_ThemeEngine.ColorMode(theme, "AstelFE");
                MPanelGlowLinkText.LinkColor = TS_ThemeEngine.ColorMode(theme, "GlowFE");
                MPanelVimeraLinkText.LinkColor = TS_ThemeEngine.ColorMode(theme, "VimeraFE");
                MPanelYamiraLinkText.LinkColor = TS_ThemeEngine.ColorMode(theme, "YamiraFE");
                //
                MPanelAstelInText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                MPanelGlowInText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                MPanelVimeraInText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                MPanelYamiraInText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                //
                MPanelAstelLinkText.ActiveLinkColor = TS_ThemeEngine.ColorMode(theme, "AstelFEHover");
                MPanelGlowLinkText.ActiveLinkColor = TS_ThemeEngine.ColorMode(theme, "GlowFEHover");
                MPanelVimeraLinkText.ActiveLinkColor = TS_ThemeEngine.ColorMode(theme, "VimeraFEHover");
                MPanelYamiraLinkText.ActiveLinkColor = TS_ThemeEngine.ColorMode(theme, "YamiraFEHover");
                //
                MPanelAstelShortcutBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelGlowShortcutBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelVimeraShortcutBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                MPanelYamiraShortcutBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                //
                MPanelAstelWizardBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelGlowWizardBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelVimeraWizardBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelYamiraWizardBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                // BUTTON REMOVE
                // ===========================================
                MPanelAstelRemoveBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelGlowRemoveBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelVimeraRemoveBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelYamiraRemoveBtn.BackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                //
                MPanelAstelRemoveBtn.FlatAppearance.BorderColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelGlowRemoveBtn.FlatAppearance.BorderColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelVimeraRemoveBtn.FlatAppearance.BorderColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                MPanelYamiraRemoveBtn.FlatAppearance.BorderColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBG");
                //
                MPanelAstelRemoveBtn.FlatAppearance.MouseOverBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelGlowRemoveBtn.FlatAppearance.MouseOverBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelVimeraRemoveBtn.FlatAppearance.MouseOverBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelYamiraRemoveBtn.FlatAppearance.MouseOverBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                //
                MPanelAstelRemoveBtn.FlatAppearance.MouseDownBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelGlowRemoveBtn.FlatAppearance.MouseDownBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelVimeraRemoveBtn.FlatAppearance.MouseDownBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                MPanelYamiraRemoveBtn.FlatAppearance.MouseDownBackColor = TS_ThemeEngine.ColorMode(theme, "BtnDeleteBGHover");
                //
                MPanelAstelRemoveBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelGlowRemoveBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelVimeraRemoveBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                MPanelYamiraRemoveBtn.ForeColor = TS_ThemeEngine.ColorMode(theme, "BtnFEColor1");
                // BOTTOM
                // ===========================================
                BottomPanel.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor1");
                BottomText.ForeColor = TS_ThemeEngine.ColorMode(theme, "UIFEColor2");
                //
                MLNetwork.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MLGitHub.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MLWeb.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MLX.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                MLInstagram.BackColor = TS_ThemeEngine.ColorMode(theme, "UIBGColor2");
                // OTHER PAGE DYNAMIC UI
                software_other_page_preloader();
                // SAVE CURRENT THEME
                // ===========================================
                try{
                    TSSettingsSave software_setting_save = new TSSettingsSave(ts_sf);
                    software_setting_save.TSWriteSettings(ts_settings_container, "ThemeStatus", Convert.ToString(ts));
                }catch (Exception){ }
            }catch (Exception){ }
        }
        // DYNAMIC BUTTON COLORS
        // ======================================================================================================
        private void dynamic_button_colors(int __button_mode, int __button_status){
            Button selectedButton = null;
            string baseColorKey = "BtnLaunchBG";
            string hoverColorKey = "BtnLaunchBGHover";
            // -1  = Default | Launch
            if (__button_mode == -1 || __button_status == -1){
                baseColorKey = "BtnLaunchBG";
                hoverColorKey = "BtnLaunchBGHover";
            }else{
                switch (__button_mode){
                    case 0:
                        selectedButton = MPanelAstelWizardBtn;
                        break;
                    case 1:
                        selectedButton = MPanelGlowWizardBtn;
                        break;
                    case 2:
                        selectedButton = MPanelVimeraWizardBtn;
                        break;
                    case 3:
                        selectedButton = MPanelYamiraWizardBtn;
                        break;
                }
                switch (__button_status){
                    case 0:
                        baseColorKey = "BtnLaunchBG";
                        hoverColorKey = "BtnLaunchBGHover";
                        break;
                    case 1:
                        baseColorKey = "BtnDownloadBG";
                        hoverColorKey = "BtnDownloadBGHover";
                        break;
                    case 2:
                        baseColorKey = "BtnUpdateBG";
                        hoverColorKey = "BtnUpdateBGHover";
                        break;
                }
            }
            // If -1 = All Buttons
            if (__button_mode == -1){
                // Tüm butonları listele
                var buttons = new Button[] { MPanelAstelWizardBtn, MPanelGlowWizardBtn, MPanelVimeraWizardBtn, MPanelYamiraWizardBtn };
                var baseColor = TS_ThemeEngine.ColorMode(theme, baseColorKey);
                var hoverColor = TS_ThemeEngine.ColorMode(theme, hoverColorKey);
                foreach (var btn in buttons){
                    btn.BackColor = baseColor;
                    btn.FlatAppearance.BorderColor = baseColor;
                    btn.FlatAppearance.MouseOverBackColor = hoverColor;
                    btn.FlatAppearance.MouseDownBackColor = hoverColor;
                }
            }else if (selectedButton != null){
                var baseColor = TS_ThemeEngine.ColorMode(theme, baseColorKey);
                var hoverColor = TS_ThemeEngine.ColorMode(theme, hoverColorKey);
                //
                selectedButton.BackColor = baseColor;
                selectedButton.FlatAppearance.BorderColor = baseColor;
                selectedButton.FlatAppearance.MouseOverBackColor = hoverColor;
                selectedButton.FlatAppearance.MouseDownBackColor = hoverColor;
            }
        }
        // MODULES PAGE DYNAMIC UI
        // ======================================================================================================
        private void software_other_page_preloader(){
            // SOFTWARE ABOUT
            try{
                TS_SoftwareDetails software_details = new TS_SoftwareDetails();
                string software_details_name = "tswizard_software_details";
                software_details.Name = software_details_name;
                if (Application.OpenForms[software_details_name] != null){
                    software_details = (TS_SoftwareDetails)Application.OpenForms[software_details_name];
                    software_details.preload_software_info();
                }
            }catch (Exception){ }
            try{
                TSWizardAbout software_about = new TSWizardAbout();
                string software_about_name = "tswizard_about";
                software_about.Name = software_about_name;
                if (Application.OpenForms[software_about_name] != null){
                    software_about = (TSWizardAbout)Application.OpenForms[software_about_name];
                    software_about.about_preloader();
                }
            }catch (Exception){ }
        }
        // LANG MODE
        // ======================================================================================================
        private void select_lang_active(object target_lang){
            ToolStripMenuItem selected_lang = null;
            select_lang_deactive();
            if (target_lang != null){
                if (selected_lang != (ToolStripMenuItem)target_lang){
                    selected_lang = (ToolStripMenuItem)target_lang;
                    selected_lang.Checked = true;
                }
            }
        }
        private void select_lang_deactive(){
            foreach (ToolStripMenuItem disabled_lang in languageToolStripMenuItem.DropDownItems){
                disabled_lang.Checked = false;
            }
        }
        private void englishToolStripMenuItem_Click(object sender, EventArgs e){
            if (lang != "en"){ lang_preload(ts_lang_en, "en"); select_lang_active(sender); }
        }
        private void turkishToolStripMenuItem_Click(object sender, EventArgs e){
            if (lang != "tr"){ lang_preload(ts_lang_tr, "tr"); select_lang_active(sender); }
        }
        private void lang_preload(string lang_type, string lang_code){
            lang_engine(lang_type, lang_code);
            try{
                TSSettingsSave software_setting_save = new TSSettingsSave(ts_sf);
                software_setting_save.TSWriteSettings(ts_settings_container, "LanguageStatus", lang_code);
            }catch (Exception){ }
            // LANG CHANGE NOTIFICATION
            TSGetLangs software_lang = new TSGetLangs(lang_path);
            DialogResult lang_change_message = TS_MessageBoxEngine.TS_MessageBox(this, 5, string.Format(TS_String_Encoder(software_lang.TSReadLangs("LangChange", "lang_change_notification")), "\n\n", "\n\n"));
            if (lang_change_message == DialogResult.Yes) { Application.Restart(); }
        }
        private void lang_engine(string lang_type, string lang_code){
            try{
                lang_path = lang_type;
                lang = lang_code;
                // GLOBAL ENGINE
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                // CONTEXT MENU
                showAppToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("ContextMenu", "cm_show"));
                softwareUpdateCheckToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("ContextMenu", "cm_s_u_check"));
                exitToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("ContextMenu", "cm_exit"));
                // SETTINGS
                settingsToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderMenu", "header_menu_settings"));
                // THEMES
                themeToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderMenu", "header_menu_theme"));
                lightThemeToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderThemes", "theme_light"));
                darkThemeToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderThemes", "theme_dark"));
                // LANGS
                languageToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderMenu", "header_menu_language"));
                englishToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderLangs", "lang_en"));
                turkishToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderLangs", "lang_tr"));
                // INITIAL VIEW
                initialViewToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderMenu", "header_menu_start"));
                windowedToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderViewMode", "header_viev_mode_windowed"));
                fullScreenToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderViewMode", "header_viev_mode_full_screen"));
                // WINDOW BEHAVIOR
                windowBehaviorToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderMenu", "header_menu_window_behavior"));
                iconStatusToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderWindowBehavior", "hwb_icon_mode"));
                closeSoftwareToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderWindowBehavior", "hwb_exit"));
                // UPDATE NOTIFICATIONS
                updateNotificationsToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderMenu", "header_menu_update_notifications"));
                notificationOnToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderActiveMode", "header_a_on"));
                notificationOffToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderActiveMode", "header_a_off"));
                // UPDATE NOTIFICATIONS
                architectureModeToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderMenu", "header_menu_architecture"));
                x64BitToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderArchitectureMode", "ham_x64"));
                aRM64ToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderArchitectureMode", "ham_arm64"));
                manualArchitectureToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderArchitectureMode", "ham_manual"));
                // UPDATE
                checkForUpdateToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderMenu", "header_menu_update"));
                // CHECK FOR SOFTWARE UPDATE
                checkForSoftwareUpdateToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("ContextMenu", "cm_s_u_check"));
                // ABOUT
                aboutToolStripMenuItem.Text = TS_String_Encoder(software_lang.TSReadLangs("HeaderMenu", "header_menu_about"));
                //
                HeaderText.Text = string.Format(TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_header_welcome")), Environment.UserName.Trim());
                //
                TS_SoftwareInfo ts_s_info = new TS_SoftwareInfo();
                for (int i = 0; i < ts_s_info.TSSoftwareList.Count; i++){
                    var s_name = ts_s_info.TSSoftwareList[i]._name;
                    switch (i){
                        case 0: MPanelAstelHeadText.Text = s_name; break;
                        case 1: MPanelGlowHeadText.Text = s_name; break;
                        case 2: MPanelVimeraHeadText.Text = s_name; break;
                        case 3: MPanelYamiraHeadText.Text = s_name; break;
                    }
                }
                //
                MPanelAstelLinkText.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_d_text"));
                MPanelGlowLinkText.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_d_text"));
                MPanelVimeraLinkText.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_d_text"));
                MPanelYamiraLinkText.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_d_text"));
                //
                MPanelAstelRemoveBtn.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_delete"));
                MPanelGlowRemoveBtn.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_delete"));
                MPanelVimeraRemoveBtn.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_delete"));
                MPanelYamiraRemoveBtn.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_delete"));
                //
                MainToolTip.SetToolTip(MPanelAstelShortcutBtn, TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_sc_text")));
                MainToolTip.SetToolTip(MPanelGlowShortcutBtn, TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_sc_text")));
                MainToolTip.SetToolTip(MPanelVimeraShortcutBtn, TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_sc_text")));
                MainToolTip.SetToolTip(MPanelYamiraShortcutBtn, TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_sc_text")));
                //
                MainToolTip.SetToolTip(MLWeb, TS_String_Encoder(software_lang.TSReadLangs("SoftwareAbout", "sa_website_page")));
                MainToolTip.SetToolTip(MLX, TS_String_Encoder(software_lang.TSReadLangs("SoftwareAbout", "sa_twitter_page")));
                MainToolTip.SetToolTip(MLInstagram, TS_String_Encoder(software_lang.TSReadLangs("SoftwareAbout", "sa_instagram_page")));
                MainToolTip.SetToolTip(MLGitHub, TS_String_Encoder(software_lang.TSReadLangs("SoftwareAbout", "sa_github_page")));
                // OTHER PAGE DYNAMIC UI
                software_other_page_preloader();
            }catch (Exception){ }
        }
        // INITIAL SETINGS
        // ======================================================================================================
        private void select_initial_mode_active(object target_initial_mode){
            ToolStripMenuItem selected_initial_mode = null;
            select_initial_mode_deactive();
            if (target_initial_mode != null){
                if (selected_initial_mode != (ToolStripMenuItem)target_initial_mode){
                    selected_initial_mode = (ToolStripMenuItem)target_initial_mode;
                    selected_initial_mode.Checked = true;
                }
            }
        }
        private void select_initial_mode_deactive(){
            foreach (ToolStripMenuItem disabled_initial in initialViewToolStripMenuItem.DropDownItems){
                disabled_initial.Checked = false;
            }
        }
        private void windowedToolStripMenuItem_Click(object sender, EventArgs e){
            if (initial_status != 0){ initial_status = 0; initial_mode_settings("0"); select_initial_mode_active(sender); }
        }
        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e){
            if (initial_status != 1){ initial_status = 1; initial_mode_settings("1"); select_initial_mode_active(sender); }
        }
        private void initial_mode_settings(string get_inital_value){
            try{
                TSSettingsSave software_setting_save = new TSSettingsSave(ts_sf);
                software_setting_save.TSWriteSettings(ts_settings_container, "InitialStatus", get_inital_value);
            }catch (Exception){ }
        }
        // WINDOW BEHAVIOR MODE
        // ======================================================================================================
        private void select_window_behavior_mode_active(object target_window_behavior_mode){
            ToolStripMenuItem selected_window_behavior_mode = null;
            select_window_behavior_mode_deactive();
            if (target_window_behavior_mode != null){
                if (selected_window_behavior_mode != (ToolStripMenuItem)target_window_behavior_mode){
                    selected_window_behavior_mode = (ToolStripMenuItem)target_window_behavior_mode;
                    selected_window_behavior_mode.Checked = true;
                }
            }
        }
        private void select_window_behavior_mode_deactive(){
            foreach (ToolStripMenuItem disabled_window_behavior_startup in windowBehaviorToolStripMenuItem.DropDownItems){
                disabled_window_behavior_startup.Checked = false;
            }
        }
        private void iconStatusToolStripMenuItem_Click(object sender, EventArgs e){
            if (behavior_mode_status != 1){ behavior_mode_status = 1; window_behavior_mode_settings("1"); select_window_behavior_mode_active(sender); }
        }
        private void closeSoftwareToolStripMenuItem_Click(object sender, EventArgs e){
            if (behavior_mode_status != 0){ behavior_mode_status = 0; window_behavior_mode_settings("0"); select_window_behavior_mode_active(sender); }
        }
        private void window_behavior_mode_settings(string get_window_behavior_value){
            try{
                TSSettingsSave software_setting_save = new TSSettingsSave(ts_sf);
                software_setting_save.TSWriteSettings(ts_settings_container, "WindowBehavior", get_window_behavior_value);
            }catch (Exception){ }
        }
        // UPDATE NOTIFICATIONS MODE
        // ======================================================================================================
        private void select_update_notifications_mode_active(object target_update_notifications_mode){
            ToolStripMenuItem selected_update_notifications_mode = null;
            select_update_notifications_mode_deactive();
            if (target_update_notifications_mode != null){
                if (selected_update_notifications_mode != (ToolStripMenuItem)target_update_notifications_mode){
                    selected_update_notifications_mode = (ToolStripMenuItem)target_update_notifications_mode;
                    selected_update_notifications_mode.Checked = true;
                }
            }
        }
        private void select_update_notifications_mode_deactive(){
            foreach (ToolStripMenuItem disabled_update_notifications in updateNotificationsToolStripMenuItem.DropDownItems){
                disabled_update_notifications.Checked = false;
            }
        }
        private void notificationOnToolStripMenuItem_Click(object sender, EventArgs e){
            if (update_notifications_status != 1){ update_notifications_status = 1; update_notifications_mode_settings("1"); select_update_notifications_mode_active(sender); }
        }
        private void notificationOffToolStripMenuItem_Click(object sender, EventArgs e){
            if (update_notifications_status != 0){ update_notifications_status = 0; update_notifications_mode_settings("0"); select_update_notifications_mode_active(sender); }
        }
        private void update_notifications_mode_settings(string get_update_notifications_value){
            try{
                TSSettingsSave software_setting_save = new TSSettingsSave(ts_sf);
                software_setting_save.TSWriteSettings(ts_settings_container, "UpdateNotifications", get_update_notifications_value);
            }catch (Exception){ }
        }
        // ARCHITECTURE MODE
        // ======================================================================================================
        private void select_architecture_mode_active(object target_architecture_mode){
            ToolStripMenuItem selected_architecture_mode = null;
            select_architecture_mode_deactive();
            if (target_architecture_mode != null){
                if (selected_architecture_mode != (ToolStripMenuItem)target_architecture_mode){
                    selected_architecture_mode = (ToolStripMenuItem)target_architecture_mode;
                    selected_architecture_mode.Checked = true;
                }
            }
        }
        private void select_architecture_mode_deactive(){
            foreach (ToolStripMenuItem disabled_architecture in architectureModeToolStripMenuItem.DropDownItems){
                disabled_architecture.Checked = false;
            }
        }
        private void x64BitToolStripMenuItem_Click(object sender, EventArgs e){
            if (architecture_mode_status != 0){ architecture_mode_status = 0; update_architecture_mode_settings("0"); select_architecture_mode_active(sender); }
        }
        private void aRM64ToolStripMenuItem_Click(object sender, EventArgs e){
            if (architecture_mode_status != 1){ architecture_mode_status = 1; update_architecture_mode_settings("1"); select_architecture_mode_active(sender); }
        }
        private void manualArchitectureToolStripMenuItem_Click(object sender, EventArgs e){
            if (architecture_mode_status != 2){ architecture_mode_status = 2; update_architecture_mode_settings("2"); select_architecture_mode_active(sender); }
        }
        private void update_architecture_mode_settings(string get_architecture_value){
            try{
                TSSettingsSave software_setting_save = new TSSettingsSave(ts_sf);
                software_setting_save.TSWriteSettings(ts_settings_container, "ArchitectureMode", get_architecture_value);
            }catch (Exception){ }
        }
        // CHECK UPDATE
        // ======================================================================================================
        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e){
            software_update_check(1);
        }
        private void software_update_check(int _check_update_ui){
            try{
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                if (!IsNetworkCheck()){
                    if (_check_update_ui == 1){
                        TS_MessageBoxEngine.TS_MessageBox(this, 2, string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareUpdate", "su_not_connection")), "\n\n"), string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareUpdate", "su_title")), Application.ProductName));
                    }
                    return;
                }
                using (WebClient webClient = new WebClient()){
                    string client_version = TS_VersionEngine.TS_SofwareVersion(2, Program.ts_version_mode).Trim();
                    int client_num_version = Convert.ToInt32(client_version.Replace(".", string.Empty));
                    //
                    string[] version_content = webClient.DownloadString(TS_LinkSystem.github_link_lt).Split('=');
                    string last_version = version_content[1].Trim();
                    int last_num_version = Convert.ToInt32(last_version.Replace(".", string.Empty));
                    //
                    if (client_num_version < last_num_version){
                        // Update available
                        DialogResult info_update = TS_MessageBoxEngine.TS_MessageBox(this, 5, string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareUpdate", "su_available")), Application.ProductName, "\n\n", client_version, "\n", last_version, "\n\n", "\n\n"), string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareUpdate", "su_title")), Application.ProductName));
                        if (info_update == DialogResult.Yes){
                            Process.Start(new ProcessStartInfo { FileName = updater_exe_name, WorkingDirectory = Path.GetDirectoryName(updater_exe_name) });
                            Application.Exit();
                        }
                    }else if (_check_update_ui == 1 && client_num_version == last_num_version){
                        // No update available
                        TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareUpdate", "su_not_available")), Application.ProductName, "\n", client_version), string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareUpdate", "su_title")), Application.ProductName));
                    }else if (_check_update_ui == 1 && client_num_version > last_num_version){
                        // Access before public use
                        TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareUpdate", "su_newer")), "\n\n", string.Format("v{0}", client_version)), string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareUpdate", "su_title")), Application.ProductName));
                    }
                }
            }catch (Exception ex){
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                TS_MessageBoxEngine.TS_MessageBox(this, 3, string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareUpdate", "su_error")), "\n\n", ex.Message), string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareUpdate", "su_title")), Application.ProductName));
            }
        }
        // CHECK FOR SOFTWARE UPDATE
        // ======================================================================================================
        private async void checkForSoftwareUpdateToolStripMenuItem_Click(object sender, EventArgs e){
            await Task.Run(() => { ts_w_start_module(false); });
            dynamic_notification_ui();
        }
        private void dynamic_notification_ui(){
            int[] install_status = new int[]{
                __astel_i_status,
                __glow_i_status,
                __vimera_i_status,
                __yamira_i_status
            };
            bool[] update_status = new bool[]{
                __astel_u_status,
                __glow_u_status,
                __vimera_u_status,
                __yamira_u_status
            };
            //
            if (!install_status.Any(i_status => i_status != 0)){
                TSGetLangs software_lang = new TSGetLangs(lang_path);
                TS_MessageBoxEngine.TS_MessageBox(this, 1, TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_u_no_installed_software")));
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
                TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(TS_String_Encoder(softwareLang.TSReadLangs("TSWizardUI", "s_u_available_updates")), "\n\n", string.Join(Environment.NewLine, update_list)));
            }else{
                TS_MessageBoxEngine.TS_MessageBox(this, 1, TS_String_Encoder(softwareLang.TSReadLangs("TSWizardUI", "s_u_no_available_updates")));
            }
            //
            update_list.Clear();
        }
        // ABOUT
        // ======================================================================================================
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e){
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
                    TS_MessageBoxEngine.TS_MessageBox(this, 1, string.Format(TS_String_Encoder(software_lang.TSReadLangs("HeaderHelp", "header_help_info_notification")), TS_String_Encoder(software_lang.TSReadLangs("HeaderMenu", "header_menu_about"))));
                }
            }catch (Exception){ }
        }
        // MEDIA LINKS
        // ======================================================================================================
        private void MLWeb_Click(object sender, EventArgs e){
            try{
                Process.Start(new ProcessStartInfo(TS_LinkSystem.website_link){ UseShellExecute = true });
            }catch (Exception){ }
        }
        private void MLGitHub_Click(object sender, EventArgs e){
            try{
                Process.Start(new ProcessStartInfo(TS_LinkSystem.twitter_x_link){ UseShellExecute = true });
            }catch (Exception){ }
        }
        private void MLInstagram_Click(object sender, EventArgs e){
            try{
                Process.Start(new ProcessStartInfo(TS_LinkSystem.instagram_link){ UseShellExecute = true });
            }catch (Exception){ }
        }
        private void MLX_Click(object sender, EventArgs e){
            try{
                Process.Start(new ProcessStartInfo(TS_LinkSystem.github_link){ UseShellExecute = true });
            }catch (Exception){ }
        }
        // CONTEXT MENU MODES
        // ======================================================================================================
        private void NotifyMode_BalloonTipClicked(object sender, EventArgs e){ context_show_software(); }
        private void context_show_software(){
            this.Show();
            if (WindowState == FormWindowState.Minimized){
                this.WindowState = FormWindowState.Normal;
            }
            this.BringToFront();
        }
        private void showAppToolStripMenuItem_Click(object sender, EventArgs e){
            context_show_software();
        }
        private void softwareUpdateCheckToolStripMenuItem_Click(object sender, EventArgs e){
            context_show_software();
            Task __run_reload_module = Task.Run(() => { ts_w_start_module(false); });
            dynamic_notification_ui();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e){
            software_exit();
        }
        // SOFTWARE EXIT
        // ======================================================================================================
        private void software_hide(FormClosingEventArgs e){ e.Cancel = true; this.Hide(); }
        private void software_exit(){ exit_mode = true; loop_status = false; Application.Exit(); }
        private void Main_FormClosing(object sender, FormClosingEventArgs e){ if (behavior_mode_status == 1){ if (!exit_mode) { software_hide(e); } }else{ software_exit(); } }
    }
}