using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
//
using static TSWizard.TSModules;

namespace TSWizard{
    public partial class TS_AppLauncher : Form{
        public TS_AppLauncher(){ InitializeComponent(); CheckForIllegalCrossThreadCalls = false;}
        // LOAD
        // ======================================================================================================
        private void TS_AppLauncher_Load(object sender, EventArgs e){
            try{
                int set_attribute = TS_Wizard.theme == 1 ? 20 : 19;
                if (DwmSetWindowAttribute(Handle, set_attribute, new[] { 1 }, 4) != TS_Wizard.theme){
                    DwmSetWindowAttribute(Handle, 20, new[] { TS_Wizard.theme == 1 ? 0 : 1 }, 4);
                }
                //
                BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor2");
                HeaderLabel.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIBGColor1");
                HeaderLabel.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor2");
                //
                foreach (Control ui_buttons in BackPanel.Controls){
                    if (ui_buttons is Button launcher_btn){
                        launcher_btn.ForeColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "UIFEColor5");
                        launcher_btn.BackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "AccentColor");
                        launcher_btn.FlatAppearance.BorderColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "AccentColor");
                        launcher_btn.FlatAppearance.MouseDownBackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "AccentColor");
                        launcher_btn.FlatAppearance.MouseOverBackColor = TS_ThemeEngine.ColorMode(TS_Wizard.theme, "AccentColorHover");
                    }
                }
                //
                TSGetLangs software_lang = new TSGetLangs(TS_Wizard.lang_path);
                Text = string.Format(TS_String_Encoder(software_lang.TSReadLangs("SoftwareLauncher", "sl_title")), Application.ProductName);
                HeaderLabel.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareLauncher", "sl_tag"));
                BtnLauncherX64.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareLauncher", "sl_x64"));
                BtnLauncherARM64.Text = TS_String_Encoder(software_lang.TSReadLangs("SoftwareLauncher", "sl_arm64"));
            }catch (Exception){ }
        }
        // STARTER
        // ======================================================================================================
        private void BtnLauncherX64_Click(object sender, EventArgs e){ launcher_start(true); }
        private void BtnLauncherARM64_Click(object sender, EventArgs e){ launcher_start(false); }
        private void launcher_start(bool __s_mode){
            string launch_app = TS_Wizard.software_launcher_mode;
            string launch_app_path = Path.GetDirectoryName(launch_app);
            //
            if (!__s_mode){
                launch_app = $"{Path.GetDirectoryName(launch_app)}\\{Path.GetFileName(launch_app).Replace("x64", "arm64")}";
            }
            //
            TSGetLangs software_lang = new TSGetLangs(TS_Wizard.lang_path);
            if (!TS_Wizard.software_operation_controller(launch_app_path)){
                Process.Start(new ProcessStartInfo { FileName = launch_app, WorkingDirectory = Path.GetDirectoryName(launch_app) });
            }else{
                TS_MessageBoxEngine.TS_MessageBox(this, 1, TS_String_Encoder(software_lang.TSReadLangs("TSWizardUI", "s_currently_operational")));
            }
        }
    }
}