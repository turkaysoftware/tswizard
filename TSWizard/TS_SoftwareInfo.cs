using System.Collections.Generic;

namespace TSWizard{
    public class TS_S_Detail{
        public string Name { get; set; }
        public string Developer { get; set; }
        public string Copyright { get; set; }
        public string Platform { get; set; }
        public string Architectural { get; set; }
        public string Language { get; set; }
        public string Req {  get; set; }
        public string Git_link { get; set; }
        public string Pn_link { get; set; }
        public string Ts_website { get; set; }
    }
    public class TS_SoftwareInfo{
        public List<TS_S_Detail> TSSoftwareList { get; private set; }
        public TS_SoftwareInfo(){
            TSSoftwareList = new List<TS_S_Detail>{
                // Astel
                new TS_S_Detail {
                    Name = "Astel",
                    Developer = "Türkay Software",
                    Copyright = "© 2024-2025, Eray Türkay.",
                    Platform = "Windows / 10 - 11",
                    Architectural = "x64 (64 Bit) / ARM64",
                    Language = "EN, IT, TR",
                    Req = " Windows 10 x64\nRAM: 50 MB\n.NET: .NET Framework 4.8.1",
                    Git_link = "https://github.com/turkaysoftware/astel",
                    Pn_link = "https://www.turkaysoftware.com/astel-pn",
                    Ts_website = "https://www.turkaysoftware.com",
                },
                // Encryphix
                new TS_S_Detail {
                    Name = "Encryphix",
                    Developer = "Türkay Software",
                    Copyright = "© 2025, Eray Türkay.",
                    Platform = "Windows / 10 - 11",
                    Architectural = "x64 (64 Bit) / ARM64",
                    Language = "EN, TR",
                    Req = " Windows 10 x64\nRAM: 75 MB\n.NET: .NET Framework 4.8.1",
                    Git_link = "https://github.com/turkaysoftware/encryphix",
                    Pn_link = "https://www.turkaysoftware.com/encryphix-pn",
                    Ts_website = "https://www.turkaysoftware.com",
                },
                // Glow
                new TS_S_Detail {
                    Name = "Glow",
                    Developer = "Türkay Software",
                    Copyright = "© 2019-2025, Eray Türkay.",
                    Platform = "Windows / 10 - 11",
                    Architectural = "x64 (64 Bit) / ARM64",
                    Language = "AR, ZH, EN, FR, DE, HI, IT, JA, KO, PT, RU, ES, TR",
                    Req = " Windows 10 x64 22H2\nRAM: 100 MB\n.NET: .NET Framework 4.8.1",
                    Git_link = "https://github.com/turkaysoftware/glow",
                    Pn_link = "https://www.turkaysoftware.com/glow-pn",
                    Ts_website = "https://www.turkaysoftware.com",
                },
                // VCardix
                new TS_S_Detail {
                    Name = "VCardix",
                    Developer = "Türkay Software",
                    Copyright = "© 2025, Eray Türkay.",
                    Platform = "Windows / 10 - 11",
                    Architectural = "x64 (64 Bit) / ARM64",
                    Language = "EN, TR",
                    Req = " Windows 10 x64\nRAM: 150 MB\n.NET: .NET Framework 4.8.1",
                    Git_link = "https://github.com/turkaysoftware/vcardix",
                    Pn_link = "https://www.turkaysoftware.com/vcardix-pn",
                    Ts_website = "https://www.turkaysoftware.com",
                },
                // Vimera
                new TS_S_Detail {
                    Name = "Vimera",
                    Developer = "Türkay Software",
                    Copyright = "© 2023-2025, Eray Türkay.",
                    Platform = "Windows / 10 - 11",
                    Architectural = "x64 (64 Bit) / ARM64",
                    Language = "EN, TR",
                    Req = " Windows 10 x64\nRAM: 75 MB\n.NET: .NET Framework 4.8.1",
                    Git_link = "https://github.com/turkaysoftware/vimera",
                    Pn_link = "https://www.turkaysoftware.com/vimera-pn",
                    Ts_website = "https://www.turkaysoftware.com",
                },
                // Yamira
                new TS_S_Detail {
                    Name = "Yamira",
                    Developer = "Türkay Software",
                    Copyright = "© 2024-2025, Eray Türkay.",
                    Platform = "Windows / 10 - 11",
                    Architectural = "x64 (64 Bit) / ARM64",
                    Language = "EN, TR",
                    Req = " Windows 10 x64\nRAM: 75 MB\n.NET: .NET Framework 4.8.1",
                    Git_link = "https://github.com/turkaysoftware/yamira",
                    Pn_link = "https://www.turkaysoftware.com/yamira-pn",
                    Ts_website = "https://www.turkaysoftware.com",
                }
            };
        }
    }
}