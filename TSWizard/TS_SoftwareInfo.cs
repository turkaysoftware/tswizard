using System.Collections.Generic;

namespace TSWizard{
    public class TS_S_Detail{
        public string _name { get; set; }
        public string _developer { get; set; }
        public string _copyright { get; set; }
        public string _platform { get; set; }
        public string _architectural{ get; set; }
        public string _s_language { get; set; }
        public string _s_req {  get; set; }
        public string _git_link { get; set; }
        public string _pn_link { get; set; }
        public string _ts_website { get; set; }
    }
    public class TS_SoftwareInfo{
        public List<TS_S_Detail> TSSoftwareList { get; private set; }
        public TS_SoftwareInfo(){
            TSSoftwareList = new List<TS_S_Detail>{
                // Astel
                new TS_S_Detail {
                    _name = "Astel",
                    _developer = "Türkay Software",
                    _copyright = "© 2024-2025, Eray Türkay.",
                    _platform = "Windows / 10 - 11",
                    _architectural = "x64 (64 Bit) / ARM64",
                    _s_language = "TR, EN",
                    _s_req = " Windows 10 x64\nRAM: 50 MB\n.NET: .NET Framework 4.8.1",
                    _git_link = "https://github.com/turkaysoftware/astel",
                    _pn_link = "https://www.turkaysoftware.com/astel-pn",
                    _ts_website = "https://www.turkaysoftware.com",
                },
                // Glow
                new TS_S_Detail {
                    _name = "Glow",
                    _developer = "Türkay Software",
                    _copyright = "© 2019-2025, Eray Türkay.",
                    _platform = "Windows / 10 - 11",
                    _architectural = "x64 (64 Bit) / ARM64",
                    _s_language = "ZH, EN, FR, DE, IT, KO, PT, RU, ES, TR",
                    _s_req = " Windows 10 x64 22H2\nRAM: 100 MB\n.NET: .NET Framework 4.8.1",
                    _git_link = "https://github.com/turkaysoftware/glow",
                    _pn_link = "https://www.turkaysoftware.com/glow-pn",
                    _ts_website = "https://www.turkaysoftware.com",
                },
                // Vimera
                new TS_S_Detail {
                    _name = "Vimera",
                    _developer = "Türkay Software",
                    _copyright = "© 2023-2025, Eray Türkay.",
                    _platform = "Windows / 10 - 11",
                    _architectural = "x64 (64 Bit) / ARM64",
                    _s_language = "TR, EN",
                    _s_req = " Windows 10 x64\nRAM: 75 MB\n.NET: .NET Framework 4.8.1",
                    _git_link = "https://github.com/turkaysoftware/vimera",
                    _pn_link = "https://www.turkaysoftware.com/vimera-pn",
                    _ts_website = "https://www.turkaysoftware.com",
                },
                // Yamira
                new TS_S_Detail {
                    _name = "Yamira",
                    _developer = "Türkay Software",
                    _copyright = "© 2024-2025, Eray Türkay.",
                    _platform = "Windows / 10 - 11",
                    _architectural = "x64 (64 Bit) / ARM64",
                    _s_language = "TR, EN",
                    _s_req = " Windows 10 x64\nRAM: 75 MB\n.NET: .NET Framework 4.8.1",
                    _git_link = "https://github.com/turkaysoftware/yamira",
                    _pn_link = "https://www.turkaysoftware.com/yamira-pn",
                    _ts_website = "https://www.turkaysoftware.com",
                }
            };
        }
    }
}