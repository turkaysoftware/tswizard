using System;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
//
using static TSWizard.TSModules;
using System.Drawing;

namespace TSWizard{
    public partial class TS_SoftwareDetails : Form{
        public TS_SoftwareDetails(){
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, InPanel, new object[] { true });
        }
        // LOCAL VARIABLES
        // ======================================================================================================
        string _rotater_link_github = string.Empty;
        string _rotater_link_pn = string.Empty;
        string _rotater_link_ts_website = string.Empty;
        // TOOLTIP SET CONFIG
        // ======================================================================================================
        private void MainToolTip_Draw(object sender, DrawToolTipEventArgs e){ e.DrawBackground(); e.DrawBorder(); e.DrawText(); }
        // LOAD
        // ======================================================================================================
        private void TS_SoftwareDetails_Load(object sender, EventArgs e){ preload_software_info(); }
        // PRELOADER
        // ======================================================================================================
        public void preload_software_info(){
            // COLOR SETTINGS
            try{
                int set_attribute = TS_Wizard.theme == 1 ? 20 : 19;
                if (DwmSetWindowAttribute(Handle, set_attribute, new[]{ 1 }, 4) != TS_Wizard.theme){
                    DwmSetWindowAttribute(Handle, 20, new[]{ TS_Wizard.theme == 1 ? 0 : 1 }, 4);
                }
                //
                bool isLight = TS_Wizard.theme == 1;
                TSImageRenderer(TLinkWebSite, isLight ? Properties.Resources.ctb_website_light : Properties.Resources.ctb_website_dark, 0, ContentAlignment.MiddleCenter);
                TSImageRenderer(TLinkX, isLight ? Properties.Resources.ctb_x_light : Properties.Resources.ctb_x_dark, 0, ContentAlignment.MiddleCenter);
                TSImageRenderer(TLinkInstagram, isLight ? Properties.Resources.ctb_instagram_light : Properties.Resources.ctb_instagram_dark, 0, ContentAlignment.MiddleCenter);
                TSImageRenderer(TLinkGitHub, isLight ? Properties.Resources.ctb_github_light : Properties.Resources.ctb_github_dark, 0, ContentAlignment.MiddleCenter);
                TSImageRenderer(TLinkYouTube, isLight ? Properties.Resources.ctb_youtube_light : Properties.Resources.ctb_youtube_dark, 0, ContentAlignment.MiddleCenter);
            }catch (Exception){ }
            //
            BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor2");
            // TOOLTIP
            MainToolTip.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "HeaderFEColor");
            MainToolTip.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "HeaderBGColor");
            // HEAD
            TLinkWebSitePanel.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor1");
            TLinkXPanel.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor1");
            TLinkInstagramPanel.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor1");
            TLinkGitHubPanel.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor1");
            TLinkYouTubePanel.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor1");
            SInfo.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor3");
            SInfoDetail.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor2");
            // CONTENT
            InPanel.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor1");
            SPanel1.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor2");
            SPanel2.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor2");
            SPanel3.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor2");
            SPanel4.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor2");
            SPanel5.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor2");
            SPanel6.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor2");
            SPanel7.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor2");
            SPanel8.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor2");
            SPanel9.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor2");
            SPanel10.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor2");
            SPanel11.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor2");
            //
            SName.Text = TS_Wizard.ts_softwares_list[TS_Wizard.ts_active_software];
            //
            switch (TS_Wizard.ts_active_software){
                case 0:
                    load_software_info_ui(0, TS_Wizard.__astel_version);
                    break;
                case 1:
                    load_software_info_ui(1, TS_Wizard.__glow_version);
                    break;
                case 2:
                    load_software_info_ui(2, TS_Wizard.__vimera_version);
                    break;
                case 3:
                    load_software_info_ui(3, TS_Wizard.__yamira_version);
                    break;
            }
        }
        // PRELOADER UI
        // ======================================================================================================
        private void load_software_info_ui(int s_mode, string s_version){
            try{
                TSGetLangs software_lang = new TSGetLangs(TS_Wizard.lang_path);
                //
                LVersion_V.Text = s_version != null ? "v" + s_version : TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "s_u_no_version_info"));
                //
                TS_SoftwareInfo ts_s_info = new TS_SoftwareInfo();
                //
                if (s_mode < 0 || s_mode >= ts_s_info.TSSoftwareList.Count)
                    return;
                //
                TS_S_Detail selectedSoftware = ts_s_info.TSSoftwareList[s_mode];
                text_title_color_ui(s_mode);
                text_content_color_ui();
                //
                switch (s_mode){
                    case 0:
                        Text = string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_title")), Application.ProductName, TS_Wizard.ts_softwares_list[0]);
                        TSImageRenderer(SLogo, Properties.Resources.astel_logo, 0, ContentAlignment.MiddleCenter);
                        SInfo.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_t_astel"));
                        SInfoDetail.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_d_astel"));
                        break;
                    case 1:
                        Text = string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_title")), Application.ProductName, TS_Wizard.ts_softwares_list[1]);
                        TSImageRenderer(SLogo, Properties.Resources.glow_logo, 0, ContentAlignment.MiddleCenter);
                        SInfo.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_t_glow"));
                        SInfoDetail.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_d_glow"));
                        break;
                    case 2:
                        Text = string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_title")), Application.ProductName, TS_Wizard.ts_softwares_list[2]);
                        TSImageRenderer(SLogo, Properties.Resources.vimera_logo, 0, ContentAlignment.MiddleCenter);
                        SInfo.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_t_vimera"));
                        SInfoDetail.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_d_vimera"));
                        break;
                    case 3:
                        Text = string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_title")), Application.ProductName, TS_Wizard.ts_softwares_list[3]);
                        TSImageRenderer(SLogo, Properties.Resources.yamira_logo, 0, ContentAlignment.MiddleCenter);
                        SInfo.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_t_yamira"));
                        SInfoDetail.Text = TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_d_yamira"));
                        break;
                }
                //
                LDeveloper.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_developer"));
                LCopyright.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_copyright"));
                LVersion.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_version"));
                LPlatform.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_platform"));
                LArch.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_architectural"));
                LSuppLanguage.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_sup_language"));
                LSuppThemes.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_supp_themes"));
                LCodeTypeLicense.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_c_and_license_type"));
                LDependence.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_dependence"));
                LSystemReq.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_system_req"));
                LLink.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_links")); ;
                //
                LDeveloper_V.Text = selectedSoftware._developer;
                LCopyright_V.Text = selectedSoftware._copyright;
                LPlatform_V.Text = selectedSoftware._platform;
                LArch_V.Text = selectedSoftware._architectural;
                LSuppLanguage_V.Text = selectedSoftware._s_language;
                LSuppThemes_V.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_theme_light")) + ", " + TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_theme_dark"));
                LCodeTypeLicense_V.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_c_type_and_license"));
                LDependence_V.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_dependence_independent"));
                LSystemReq_V.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_system_req_os")) + selectedSoftware._s_req;
                //
                BSLinkGitHub.Text = string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_github")), TS_Wizard.ts_softwares_list[TS_Wizard.ts_active_software]);
                BSLinkPN.Text = string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_pn")), TS_Wizard.ts_softwares_list[TS_Wizard.ts_active_software]);
                BSLinkTS.Text = string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareDetail", "sd_ts_website")), Application.CompanyName);
                //
                MainToolTip.SetToolTip(TLinkWebSite, TS_String_Encoder(software_lang.TSReadLangs("SoftwareAbout", "sa_website_page")));
                MainToolTip.SetToolTip(TLinkX, TS_String_Encoder(software_lang.TSReadLangs("SoftwareAbout", "sa_twitter_page")));
                MainToolTip.SetToolTip(TLinkInstagram, TS_String_Encoder(software_lang.TSReadLangs("SoftwareAbout", "sa_instagram_page")));
                MainToolTip.SetToolTip(TLinkGitHub, TS_String_Encoder(software_lang.TSReadLangs("SoftwareAbout", "sa_github_page")));
                MainToolTip.SetToolTip(TLinkYouTube, TS_String_Encoder(software_lang.TSReadLangs("SoftwareAbout", "sa_youtube_page")));
                //
                _rotater_link_github = selectedSoftware._git_link;
                _rotater_link_pn = selectedSoftware._pn_link;
                _rotater_link_ts_website = selectedSoftware._ts_website;
            }catch (Exception){ }
        }
        // DYNAMIC COLOR UI TITLE
        // ======================================================================================================
        private void text_title_color_ui(int s_mode){
            string[] colorModes = { "AstelFE", "GlowFE", "VimeraFE", "YamiraFE" };
            //
            if (s_mode < 0 || s_mode >= colorModes.Length)
                return;
            //
            string colorMode = colorModes[s_mode];
            var color = TS_ThemeEngine.ColorMode(TS_Wizard.theme, colorMode);
            //
            var controls = new[]{
                SName, LDeveloper, LCopyright,
                LVersion, LPlatform, LArch,
                LSuppLanguage, LSuppThemes,
                LCodeTypeLicense, LDependence,
                LSystemReq, LLink
            };
            foreach (var control in controls){
                control.ForeColor = color;
            }
            //
            BSLinkGitHub.LinkColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor3");
            BSLinkPN.LinkColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor3");
            BSLinkTS.LinkColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor3");
            //
            BSLinkGitHub.ActiveLinkColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor4");
            BSLinkPN.ActiveLinkColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor4");
            BSLinkTS.ActiveLinkColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor4");
        }
        // DYNAMIC COLOR UI CONTENT
        // ======================================================================================================
        private void text_content_color_ui(){
            LDeveloper_V.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor2");
            LCopyright_V.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor2");
            LVersion_V.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor2");
            LPlatform_V.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor2");
            LArch_V.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor2");
            LSuppLanguage_V.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor2");
            LSuppThemes_V.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor2");
            LCodeTypeLicense_V.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor2");
            LDependence_V.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor2");
            LSystemReq_V.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor2");
        }
        // DYNAMIC SOFTWARE EXTERNAL LINKS
        // ======================================================================================================
        private void BSLinkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){
            try{
                Process.Start(new ProcessStartInfo(_rotater_link_github){ UseShellExecute = true });
            }catch (Exception){ }
        }
        private void BSLinkPN_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){
            try{
                Process.Start(new ProcessStartInfo(_rotater_link_pn){ UseShellExecute = true });
            }catch (Exception){ }
        }
        private void BSLinkTS_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){
            try{
                Process.Start(new ProcessStartInfo(_rotater_link_ts_website){ UseShellExecute = true });
            }catch (Exception){ }
        }
        // SOCIAL MEDIA LINKS
        // ======================================================================================================
        private void TLinkWebSite_Click(object sender, EventArgs e){
            try{
                Process.Start(new ProcessStartInfo(TS_LinkSystem.website_link){ UseShellExecute = true });
            }catch (Exception){ }
        }
        private void TLinkX_Click(object sender, EventArgs e){
            try{
                Process.Start(new ProcessStartInfo(TS_LinkSystem.twitter_x_link){ UseShellExecute = true });
            }catch (Exception){ }
        }
        private void TLinkInstagram_Click(object sender, EventArgs e){
            try{
                Process.Start(new ProcessStartInfo(TS_LinkSystem.instagram_link){ UseShellExecute = true });
            }catch (Exception){ }
        }
        private void TLinkGitHub_Click(object sender, EventArgs e){
            try{
                Process.Start(new ProcessStartInfo(TS_LinkSystem.github_link){ UseShellExecute = true });
            }catch (Exception){ }
        }
        private void TLinkYouTube_Click(object sender, EventArgs e){
            try{
                Process.Start(new ProcessStartInfo(TS_LinkSystem.youtube_link){ UseShellExecute = true });
            }catch (Exception){ }
        }
    }
}