﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutomateOpenGraph.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ADVANC,AOT,AWC,BAM,BBL,BDMS,BEM,BGRIM,BH,BJC,BTS,CBG,COM7,CPALL,CPF,CPN,CRC,DELTA" +
            ",DTAC,EA,EGCO,GLOBAL,GPSC,GULF,HMPRO,INTUCH,IVL,KBANK,KTB,KTC,LH,MINT,MTC,OSP,PT" +
            "T,PTTEP,PTTGC,RATCH,SAWAD,SCB,SCC,SCGP,TISCO,TMB,TOA,TOP,TRUE,TU,VGI,X-X,OR")]
        public string set50 {
            get {
                return ((string)(this["set50"]));
            }
            set {
                this["set50"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"ADVANC,AEONTS,AMATA,AOT,AP,AWC,BANPU,BPP,BBL,BCH,BCP,BCPG,BDMS,BEC,BEM,BGRIM,BH,BJC,BANPU,BTS,CBG,CENTEL,CHG,CK,CKP,COM7,CPALL,CPF,CPN,CRC,DELTA,DTAC,EA,EGCO,EPG,ERW,ESSO,GFPT,GLOBAL,GPSC,GULF,GUNKUL,HANA,HMPRO,INTUCH,IRPC,IVL,JAS,JMT,KBANK,KCE,KKP,KTB,KTC,LH,MAJOR,MEGA,MINT,MTC,ORI,OSP,PLANB,PRM,PSH,PTG,PTT,PTTEP,PTTGC,QH,RATCH,RS,SAWAD,SCB,SCC,SCGP,SGP,SPALI,SPRC,STA,STEC,SUPER,TASCO,TCAP,THANI,TISCO,TKN,TMB,TOA,TOP,TPIPP,TQM,TRUE,TU,VGI,WHA,X-X,DOHOME,AAV,ACE,PSL,JMART,BAM,MAKRO,STGT,NER,SPCG,BEAUTY,THCOM,AMANAH,WORK,BEC,AU,TVO,KEX,NCAP,RBF,DOD,OR")]
        public string set100 {
            get {
                return ((string)(this["set100"]));
            }
            set {
                this["set100"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("7")]
        public string delaysec {
            get {
                return ((string)(this["delaysec"]));
            }
            set {
                this["delaysec"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("BLAND")]
        public string customlist {
            get {
                return ((string)(this["customlist"]));
            }
            set {
                this["customlist"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SET,SET50,TFEX,MAI,^DJIA,^NIX,^HSKI,^DAX,^FTSE,^JKSE,^KOSPI")]
        public string market {
            get {
                return ((string)(this["market"]));
            }
            set {
                this["market"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\nevada\\Documents\\vs2017project\\AutomateOpenGraph\\AutomateOpenGraph\\data")]
        public string datapath {
            get {
                return ((string)(this["datapath"]));
            }
            set {
                this["datapath"] = value;
            }
        }
    }
}
