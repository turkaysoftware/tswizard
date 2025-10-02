using System;
using System.Reflection;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
//
using static TSWizard.TSModules;

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
        private void TS_SoftwareDetails_Load(object sender, EventArgs e){ Preload_software_info(); }
        // PRELOADER
        // ======================================================================================================
        public void Preload_software_info(){
            // COLOR SETTINGS
            try{
                bool isLight = TSWizardMain.theme == 1;
                TSImageRenderer(TLinkWebSite, isLight ? Properties.Resources.ctb_website_light : Properties.Resources.ctb_website_dark, 0, ContentAlignment.MiddleCenter);
                TSImageRenderer(TLinkGitHub, isLight ? Properties.Resources.ctb_github_light : Properties.Resources.ctb_github_dark, 0, ContentAlignment.MiddleCenter);
                TSImageRenderer(TLinkBmac, isLight ? Properties.Resources.ct_bmac_light : Properties.Resources.ct_bmac_dark, 0, ContentAlignment.MiddleCenter);
            }catch (Exception){ }
            //
            BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor2");
            // TOOLTIP
            MainToolTip.ForeColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "HeaderFEColor");
            MainToolTip.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "HeaderBGColor");
            // HEAD
            TLinkWebSitePanel.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor1");
            TLinkGitHubPanel.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor1");
            TLinkBmacPanel.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor1");
            SInfo.ForeColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor2");
            SInfoDetail.ForeColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor2");
            // CONTENT
            InPanel.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor1");
            SPanel1.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor2");
            SPanel2.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor2");
            SPanel3.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor2");
            SPanel4.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor2");
            SPanel5.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor2");
            SPanel6.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor2");
            SPanel7.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor2");
            SPanel8.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor2");
            SPanel9.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor2");
            SPanel10.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor2");
            SPanel11.BackColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIBGColor2");
            //
            SName.Text = TSWizardMain.ts_softwares_list[TSWizardMain.ts_active_software];
            //
            switch (TSWizardMain.ts_active_software){
                case 0:
                    Load_software_info_ui(0, TSWizardMain.__astel_version);
                    break;
                case 1:
                    Load_software_info_ui(1, TSWizardMain.__encryphix_version);
                    break;
                case 2:
                    Load_software_info_ui(2, TSWizardMain.__glow_version);
                    break;
                case 3:
                    Load_software_info_ui(3, TSWizardMain.__vcardix_version);
                    break;
                case 4:
                    Load_software_info_ui(4, TSWizardMain.__vimera_version);
                    break;
                case 5:
                    Load_software_info_ui(5, TSWizardMain.__yamira_version);
                    break;
            }
        }
        // PRELOADER UI
        // ======================================================================================================
        private void Load_software_info_ui(int s_mode, string s_version){
            try{
                TSGetLangs software_lang = new TSGetLangs(TSWizardMain.lang_path);
                //
                LVersion_V.Text = s_version != null ? "v" + s_version : software_lang.TSReadLangs("SoftwareDetail", "s_u_no_version_info");
                //
                TS_SoftwareInfo ts_s_info = new TS_SoftwareInfo();
                //
                if (s_mode < 0 || s_mode >= ts_s_info.TSSoftwareList.Count)
                    return;
                //
                TS_S_Detail selectedSoftware = ts_s_info.TSSoftwareList[s_mode];
                Text_title_color_ui(s_mode);
                Text_content_color_ui();
                //
                switch (s_mode){
                    case 0:
                        Text = string.Format(software_lang.TSReadLangs("SoftwareDetail", "sd_title"), Application.ProductName, TSWizardMain.ts_softwares_list[0]);
                        TSImageRenderer(SLogo, Properties.Resources.astel_logo, 0, ContentAlignment.MiddleCenter);
                        SInfo.Text = software_lang.TSReadLangs("TSWizardUI", "s_t_astel");
                        SInfoDetail.Text = software_lang.TSReadLangs("TSWizardUI", "s_d_astel");
                        break;
                    case 1:
                        Text = string.Format(software_lang.TSReadLangs("SoftwareDetail", "sd_title"), Application.ProductName, TSWizardMain.ts_softwares_list[1]);
                        TSImageRenderer(SLogo, Properties.Resources.encryphix_logo, 0, ContentAlignment.MiddleCenter);
                        SInfo.Text = software_lang.TSReadLangs("TSWizardUI", "s_t_encryphix");
                        SInfoDetail.Text = software_lang.TSReadLangs("TSWizardUI", "s_d_encryphix");
                        break;
                    case 2:
                        Text = string.Format(software_lang.TSReadLangs("SoftwareDetail", "sd_title"), Application.ProductName, TSWizardMain.ts_softwares_list[2]);
                        TSImageRenderer(SLogo, Properties.Resources.glow_logo, 0, ContentAlignment.MiddleCenter);
                        SInfo.Text = software_lang.TSReadLangs("TSWizardUI", "s_t_glow");
                        SInfoDetail.Text = software_lang.TSReadLangs("TSWizardUI", "s_d_glow");
                        break;
                    case 3:
                        Text = string.Format(software_lang.TSReadLangs("SoftwareDetail", "sd_title"), Application.ProductName, TSWizardMain.ts_softwares_list[3]);
                        TSImageRenderer(SLogo, Properties.Resources.vcardix_logo, 0, ContentAlignment.MiddleCenter);
                        SInfo.Text = software_lang.TSReadLangs("TSWizardUI", "s_t_vcardix");
                        SInfoDetail.Text = software_lang.TSReadLangs("TSWizardUI", "s_d_vcardix");
                        break;
                    case 4:
                        Text = string.Format(software_lang.TSReadLangs("SoftwareDetail", "sd_title"), Application.ProductName, TSWizardMain.ts_softwares_list[4]);
                        TSImageRenderer(SLogo, Properties.Resources.vimera_logo, 0, ContentAlignment.MiddleCenter);
                        SInfo.Text = software_lang.TSReadLangs("TSWizardUI", "s_t_vimera");
                        SInfoDetail.Text = software_lang.TSReadLangs("TSWizardUI", "s_d_vimera");
                        break;
                    case 5:
                        Text = string.Format(software_lang.TSReadLangs("SoftwareDetail", "sd_title"), Application.ProductName, TSWizardMain.ts_softwares_list[5]);
                        TSImageRenderer(SLogo, Properties.Resources.yamira_logo, 0, ContentAlignment.MiddleCenter);
                        SInfo.Text = software_lang.TSReadLangs("TSWizardUI", "s_t_yamira");
                        SInfoDetail.Text = software_lang.TSReadLangs("TSWizardUI", "s_d_yamira");
                        break;
                }
                //
                LDeveloper.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_developer");
                LCopyright.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_copyright");
                LVersion.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_version");
                LPlatform.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_platform");
                LArch.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_architectural");
                LSuppLanguage.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_sup_language");
                LSuppThemes.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_supp_themes");
                LCodeTypeLicense.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_c_and_license_type");
                LDependence.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_dependence");
                LSystemReq.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_system_req");
                LLink.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_links"); ;
                //
                LDeveloper_V.Text = selectedSoftware.Developer;
                LCopyright_V.Text = selectedSoftware.Copyright;
                LPlatform_V.Text = selectedSoftware.Platform;
                LArch_V.Text = selectedSoftware.Architectural;
                LSuppLanguage_V.Text = selectedSoftware.Language;
                LSuppThemes_V.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_theme_light") + ", " + software_lang.TSReadLangs("SoftwareDetail", "sd_theme_dark");
                LCodeTypeLicense_V.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_c_type_and_license");
                LDependence_V.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_dependence_independent");
                LSystemReq_V.Text = software_lang.TSReadLangs("SoftwareDetail", "sd_system_req_os") + selectedSoftware.Req;
                //
                BSLinkGitHub.Text = string.Format(software_lang.TSReadLangs("SoftwareDetail", "sd_github"), TSWizardMain.ts_softwares_list[TSWizardMain.ts_active_software]);
                BSLinkPN.Text = string.Format(software_lang.TSReadLangs("SoftwareDetail", "sd_pn"), TSWizardMain.ts_softwares_list[TSWizardMain.ts_active_software]);
                BSLinkTS.Text = string.Format(software_lang.TSReadLangs("SoftwareDetail", "sd_ts_website"), Application.CompanyName);
                //
                MainToolTip.SetToolTip(TLinkWebSite, software_lang.TSReadLangs("SoftwareAbout", "sa_website_page"));
                MainToolTip.SetToolTip(TLinkGitHub, software_lang.TSReadLangs("SoftwareAbout", "sa_github_page"));
                MainToolTip.SetToolTip(TLinkBmac, software_lang.TSReadLangs("SoftwareAbout", "sa_bmac_page"));
                //
                _rotater_link_github = selectedSoftware.Git_link;
                _rotater_link_pn = selectedSoftware.Pn_link;
                _rotater_link_ts_website = selectedSoftware.Ts_website;
            }catch (Exception){ }
        }
        // DYNAMIC COLOR UI TITLE
        // ======================================================================================================
        private void Text_title_color_ui(int s_mode){
            string[] colorModes = { "AstelFE", "EncryphixFE", "GlowFE", "VCardixFE", "VimeraFE", "YamiraFE" };
            //
            if (s_mode < 0 || s_mode >= colorModes.Length)
                return;
            //
            string colorMode = colorModes[s_mode];
            var color = TS_ThemeEngine.ColorMode(TSWizardMain.theme, colorMode);
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
            BSLinkGitHub.LinkColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor3");
            BSLinkPN.LinkColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor3");
            BSLinkTS.LinkColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor3");
            //
            BSLinkGitHub.ActiveLinkColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor4");
            BSLinkPN.ActiveLinkColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor4");
            BSLinkTS.ActiveLinkColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor4");
        }
        // DYNAMIC COLOR UI CONTENT
        // ======================================================================================================
        private void Text_content_color_ui(){
            LDeveloper_V.ForeColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor2");
            LCopyright_V.ForeColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor2");
            LVersion_V.ForeColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor2");
            LPlatform_V.ForeColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor2");
            LArch_V.ForeColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor2");
            LSuppLanguage_V.ForeColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor2");
            LSuppThemes_V.ForeColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor2");
            LCodeTypeLicense_V.ForeColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor2");
            LDependence_V.ForeColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor2");
            LSystemReq_V.ForeColor = TS_ThemeEngine.ColorMode(TSWizardMain.theme, "UIFEColor2");
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
        private void TLinkGitHub_Click(object sender, EventArgs e){
            try{
                Process.Start(new ProcessStartInfo(TS_LinkSystem.github_link){ UseShellExecute = true });
            }catch (Exception){ }
        }
        private void TLinkBmac_Click(object sender, EventArgs e){
            try{
                Process.Start(new ProcessStartInfo(TS_LinkSystem.ts_bmac){ UseShellExecute = true });
            }catch (Exception){ }
        }
    }
}