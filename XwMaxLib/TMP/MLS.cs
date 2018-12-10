using XwMaxLib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace NailGun.Objects
{
    //*******************************************************************************************************************
    //*******************************************************************************************************************
    //*******************************************************************************************************************
    //*******************************************************************************************************************
    [Serializable]
    public class MLS : MultiLanguageString
    {
        public static Dictionary<string, string[]> mlsFallBack = new Dictionary<string, string[]>();
        public static Regex _MlsCompatibilityRegex = new Regex(@"(?isx)(?<START>\[(\w{2}(?:[-_]\w{2})?)\]).*?(?<END>\[/\1\])", RegexOptions.Compiled | RegexOptions.CultureInvariant| RegexOptions.RightToLeft);
        
        public bool DisableFallBack = false;
        public bool DisableCompatibility = false;

        //************************************************************************************************
        public MLS() : base()
        {
            BuildFallBackList();
        }

        //************************************************************************************************
        public MLS(string value, bool compatibility = true)
            : base(Compatibility(value, compatibility))
        {
            BuildFallBackList();
        }

        //************************************************************************************************
        //para poder tirra a compatibilidade não posso ter este operador
        //public static implicit operator MLS(string sValue)
        //{
        //    return new MLS(sValue);
        //}

        //************************************************************************************************
        public static implicit operator string(MLS oValue)
        {
            return oValue.ToString();
        }

        //************************************************************************************************
        public override string ToString()
        {
            StringBuilder ret = new StringBuilder(1024);

            foreach (string lang in Languages)
            {
                ret.AppendFormat("[{0}]{1}[/{0}]", lang, GetTranslation(lang));
            }

            return ret.ToString();
        }
        
        //************************************************************************************************
        private void BuildFallBackList()
        {
            if (MLS.mlsFallBack.Count == 0)
            {
                mlsFallBack["PT-PT"] = new string[] { "PT-BR" };
                mlsFallBack["PT-BR"] = new string[] { "PT-PT" };
                mlsFallBack["PT-AO"] = new string[] { "PT-PT" };
                mlsFallBack["PT-CV"] = new string[] { "PT-PT" };
                mlsFallBack["PT-MZ"] = new string[] { "PT-PT" };

                mlsFallBack["CA-ES"] = new string[] { "ES-ES" };
                mlsFallBack["ES-ES"] = new string[] { "CA-ES", "ES-VE" };
                mlsFallBack["ES-MX"] = new string[] { "ES-PE", "ES-ES" };
                mlsFallBack["ES-PE"] = new string[] { "ES-MX", "ES-ES" };
                mlsFallBack["ES-UY"] = new string[] { "ES-MX", "ES-ES" };
                mlsFallBack["ES-CO"] = new string[] { "ES-MX", "ES-ES" };
                mlsFallBack["ES-AR"] = new string[] { "ES-MX", "ES-ES" };
                mlsFallBack["ES-DO"] = new string[] { "ES-MX", "ES-ES" };
                mlsFallBack["ES-PY"] = new string[] { "ES-MX", "ES-ES" };
                mlsFallBack["ES-CL"] = new string[] { "ES-MX", "ES-ES" };
                mlsFallBack["ES-VE"] = new string[] { "ES-MX", "ES-ES" };
                mlsFallBack["ES-EC"] = new string[] { "ES-MX", "ES-ES" };
                mlsFallBack["ES-CU"] = new string[] { "ES-MX", "ES-ES" };
                mlsFallBack["ES-CR"] = new string[] { "ES-MX", "ES-ES" };
                mlsFallBack["ES-PA"] = new string[] { "ES-MX", "ES-ES" };
                mlsFallBack["ES-HN"] = new string[] { "ES-CO", "ES-MX", "ES-ES" };
                mlsFallBack["ES-NI"] = new string[] { "ES-ES" }; //foi o Justo que disse

                mlsFallBack["EN-US"] = new string[] { "EN-GB" };
                mlsFallBack["EN-GB"] = new string[] { "EN-US" };
                mlsFallBack["FR-FR"] = new string[] { "EN-GB" };
                mlsFallBack["DE-DE"] = new string[] { "EN-GB" };
                mlsFallBack["RU-RU"] = new string[] { "EN-GB" };
                mlsFallBack["IT-IT"] = new string[] { "EN-GB" };
                mlsFallBack["AF-ZA"] = new string[] { "EN-GB" };
                mlsFallBack["NB-NO"] = new string[] { "EN-GB" };
                mlsFallBack["SV-SE"] = new string[] { "EN-GB" };
                mlsFallBack["SQ-AL"] = new string[] { "EN-GB" };
                mlsFallBack["ZH-HK"] = new string[] { "ZH-CH", "EN-GB" };
                mlsFallBack["ZH-CH"] = new string[] { "ZH-HK", "EN-GB" };
                mlsFallBack["NL-NL"] = new string[] { "ZH-HK", "DE-DE", "EN-GB" };
            }
        }
                
        //*************************************************************************************************
        public string PTPT
        {
            set { this.SetTranslation("PT-PT", value.ToString()); }
            get { return this.GetTranslation("PT-PT"); }
        }

        public string PTBR
        {
            set { this.SetTranslation("PT-BR", value.ToString()); }
            get { return this.GetTranslation("PT-BR"); }
        }

        public string ENGB
        {
            set { this.SetTranslation("EN-GB", value.ToString()); }
            get { return this.GetTranslation("EN-GB"); }
        }

        public string FRFR
        {
            set { this.SetTranslation("FR-FR", value.ToString()); }
            get { return this.GetTranslation("FR-FR"); }
        }

        public string DEDE
        {
            set { this.SetTranslation("DE-DE", value.ToString()); }
            get { return this.GetTranslation("DE-DE"); }
        }

        public string ESES
        {
            set { this.SetTranslation("ES-ES", value.ToString()); }
            get { return this.GetTranslation("ES-ES"); }
        }

        public string ESMX
        {
            set { this.SetTranslation("ES-MX", value.ToString()); }
            get { return this.GetTranslation("ES-MX"); }
        }

        public string ESPE
        {
            set { this.SetTranslation("ES-PE", value.ToString()); }
            get { return this.GetTranslation("ES-PE"); }
        }

        public string ESCO
        {
            set { this.SetTranslation("ES-CO", value.ToString()); }
            get { return this.GetTranslation("ES-CO"); }
        }

        public string ESDO
        {
            set { this.SetTranslation("ES-DO", value.ToString()); }
            get { return this.GetTranslation("ES-DO"); }
        }

        public string ESAR
        {
            set { this.SetTranslation("ES-AR", value.ToString()); }
            get { return this.GetTranslation("ES-AR"); }
        }
        
        public string CAES
        {
            set { this.SetTranslation("CA-ES", value.ToString()); }
            get { return this.GetTranslation("CA-ES"); }
        }
        public string RURU
        {
            set { this.SetTranslation("RU-RU", value.ToString()); }
            get { return this.GetTranslation("RU-RU"); }
        }
        public string ZHCN
        {
            set { this.SetTranslation("ZH-CN", value.ToString()); }
            get { return this.GetTranslation("ZH-CN"); }
        }

        //************************************************************************************************
        public static MLS MakeMLS(string pt, string en, string fr, string de, string es, string ca)
        {
            MLS mls = new MLS();
            mls.SetTranslation("PT-PT", pt);
            mls.SetTranslation("EN-GB", en);
            mls.SetTranslation("FR-FR", fr);
            mls.SetTranslation("DE-DE", de);
            mls.SetTranslation("ES-ES", es);
            mls.SetTranslation("CA-ES", ca);
            return mls;
        }

        //************************************************************************************************
        public string GetMultiLanguageString(bool shortCodes, bool removeEmptyTags, params string[] languages)
        {
            StringBuilder ret = new StringBuilder(1024);

            foreach (string language in languages)
            {
                string lang = LCID(shortCodes, language);
                bool disableFB = DisableFallBack;
                DisableFallBack = true;
                string value = GetTranslation(lang).Trim();
                DisableFallBack = disableFB;
                if (!removeEmptyTags || value.Length > 0)
                    ret.AppendFormat("[{0}]{1}[/{0}]", lang, value);
            }

            return ret.ToString();
        }

        //*************************************************************************************************
        public override string GetTranslation(string sLang)
        {
            string lcid = LCID(false, sLang);
            
            string value = base.GetTranslation(lcid);
            
            if (DisableFallBack == false)
            {
                if (value.IsEmpty())
                {
                    if (mlsFallBack.ContainsKey(lcid))
                    {
                        string[] alternative = mlsFallBack[lcid];
                        for (int i = 0; i< alternative.Length; i++)
                        {
                            string alternativeValue = base.GetTranslation(alternative[i]);
                            if (!alternativeValue.IsEmpty())
                                return alternativeValue;
                        }
                    }
                }
            }
            
            return value;
        }

        //*************************************************************************************************
        public override void SetTranslation(string sLang, string sLocaltext)
        {
            string lcid = LCID(false, sLang);
            base.SetTranslation(lcid, sLocaltext);
        }

        //*************************************************************************************************
        public override void RemoveTranslation(string sLang)
        {
            string lcid = LCID(false, sLang);
            base.RemoveTranslation(lcid);
        }

        //************************************************************************************************
        public void TruncateTo(int size)
        {
            foreach (string lang in Languages)
            {
                string text = GetTranslation(lang);
                if (text.Length > size)
                    text = text.Substring(0, size);
                SetTranslation(lang, text);
            }
        }
        
        //*************************************************************************************************
        private string LCID(bool shortCode, string language)
        {
            string lang = language.ToUpper();

            switch (lang)
            {
                case "":
                case "PT":
                case "PTPT":
                    lang = "PT-PT";
                    break;
                case "EN":
                case "ENGB":
                    lang = "EN-GB";
                    break;
                case "FR":
                case "FRFR":
                    lang = "FR-FR";
                    break;
                case "ES":
                case "ESES":
                    lang = "ES-ES";
                    break;
                case "DE":
                case "DEDE":
                    lang = "DE-DE";
                    break;
                case "RU":
                case "RURU":
                    lang = "RU-RU";
                    break;
                case "BR":
                case "PTBR":
                    lang = "PT-BR";
                    break;
                case "CA":
                case "CAES":
                case "ESCA":
                case "ES-CA":
                    lang = "CA-ES";
                    break;
                case "PE":
                case "ESPE":
                    lang = "ES-PE";
                    break;
                case "MX":
                case "ESMX":
                    lang = "ES-MX";
                    break;
                case "AR":
                case "ESAR":
                    lang = "ES-AR";
                    break;
            }

            if (lang.Length == 4)
                lang = lang.Insert(2, "-");

            if (lang.Length > 5 || (lang.Length == 5 && !lang.Contains("-")))
                throw new Exception(String.Format("Invalid Language Token: {0}\n\n{1}", lang, this.ToString()));

            if (shortCode)
            {
                string[] comps = lang.Split('-');
                if (comps.Length >= 1)
                    return comps[0];
            }

            return lang;
        }

        //************************************************************************************************
        private static string Compatibility(string value, bool Compatibility)
        {
            if (!Compatibility)
                return value;

            if (value == string.Empty)
                return string.Empty;

            MatchCollection ms = _MlsCompatibilityRegex.Matches(value);

            int cnt = ms.Count;
            for (int i = 0; i < cnt; i++)
            {
                string start = ms[i].Result("${START}");
                string end = ms[i].Result("${END}");
                value = value.Replace(start, start.ToUpper());
                value = value.Replace(end, end.ToUpper());
            }

            value = value.Replace("[PT]", "[PT-PT]");
            value = value.Replace("[EN]", "[EN-GB]");
            value = value.Replace("[FR]", "[FR-FR]");
            value = value.Replace("[DE]", "[DE-DE]");
            value = value.Replace("[ES]", "[ES-ES]");
            value = value.Replace("[CA]", "[CA-ES]");
            value = value.Replace("[BR]", "[PT-BR]");
            value = value.Replace("[PE]", "[ES-PE]");
            value = value.Replace("[MX]", "[ES-MX]");
            value = value.Replace("[RU]", "[RU-RU]");
            value = value.Replace("[CN]", "[ZH-CN]");
            value = value.Replace("[AR]", "[ES-AR]");

            value = value.Replace("[/PT]", "[/PT-PT]");
            value = value.Replace("[/EN]", "[/EN-GB]");
            value = value.Replace("[/FR]", "[/FR-FR]");
            value = value.Replace("[/DE]", "[/DE-DE]");
            value = value.Replace("[/ES]", "[/ES-ES]");
            value = value.Replace("[/CA]", "[/CA-ES]");
            value = value.Replace("[/BR]", "[/PT-BR]");
            value = value.Replace("[/PE]", "[/ES-PE]");
            value = value.Replace("[/MX]", "[/ES-MX]");
            value = value.Replace("[/RU]", "[/RU-RU]");
            value = value.Replace("[/CN]", "[/ZH-CN]");
            value = value.Replace("[/AR]", "[/ES-AR]");
            
            return value;
        }

        //************************************************************************************************
        public void Sanitize()
        {
            foreach (string lang in Languages)
            {
                SetTranslation(lang, SanitizeString(base.GetTranslation(lang)));
            }
        }

        //************************************************************************************************
        private static readonly Regex StrangeChars = new Regex(@"[\x00-\x1F\x7F\xFF-[\t\r\n]]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static string SanitizeString(string text)
        {
            text = text.Replace("‘", "'"); 
            text = text.Replace("’", "'");
            text = text.Replace("\"", "'");
            text = text.Replace("“", "'");
            text = text.Replace("”", "'");
            //text = text.Replace("|", "-"); //commented because some client wanted | on the page
            text = text.Replace("\v", "\n");
            text = StrangeChars.Replace(text, "");
            return text;
        }

        //************************************************************************************************
        public void Replace(string find, string replace)
        {
            foreach (string lang in Languages)
            {
                SetTranslation(lang, base.GetTranslation(lang).Replace(find, replace));
            }
        }

        //*************************************************************************************************
        public void RemoveHtml()
        {
            foreach (string lang in Languages)
            {
                SetTranslation(lang, RemoveHtmlString(base.GetTranslation(lang)));
            }
        }

        //*************************************************************************************************
        private static readonly Regex DetectHtml = new Regex(@"</*?(?![^>]*?\b(br)\b)[^>[]*?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DetectBR = new Regex(@"<(\s)?(/)?(\s)?br(\s)?(/)?(\s)?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static string RemoveHtmlString(string text)
        {
            text = HttpUtility.HtmlDecode(text);
            if (DetectBR.IsMatch(text))
            {
                text = text.Replace("\n", "");
                text = DetectBR.Replace(text, "\n");
            }
            text = DetectHtml.Replace(text, "");
            return text;
        }

        //*************************************************************************************************
        public void RemoveContacts()
        {
            foreach (string lang in Languages)
            {
                SetTranslation(lang, RemoveContactsString(lang, base.GetTranslation(lang)));
            }
        }

        //*************************************************************************************************
        /* By Ego */
        //private static Regex DetectPhone = new Regex(@"((?!(.*\.\d+\,\d+)|(.*\,\d+\.\d+))((?:\d[\ \-\)\,\.]*){9,}))(?![\ \-\(\)\,\.\d]*(Kz|Kwanzas|R\$|Reais|€|Euros|\$|US\$|Dólares|£|Kwanzas|CV\$|Escudos|MT|Meticais))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private static Regex DetectEmail = new Regex(@"([\w\.\-_]+)?\w+@[\w-_]+(\.\w+){1,}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private static Regex DetectUrl = new Regex(@"([a-zA-Z]+[:\/]+)?(([a-zA-Z]+)(\.)){2,}([a-zA-Z]+)(\/)?|([a-zA-Z:]+)?(\/\/)+.[^\s]+|(([a-zA-Z]+)(\.))([a-zA-Z]+)(\/)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /* By Ze */
        private static readonly Regex DetectPhone = new Regex(@"(?is)(?<!\#ref:)((?!(.*\.\d+\,\d+)|(.*\,\d+\.\d+))((?:\d[\ \-\(\)\,\.]*){9,}))(?![\ \-\(\)\,\.\d]*(Kz|Kwanzas|R\$|Reais|€|Euros|\$|US\$|Dólares|£|Kwanzas|CV\$|Escudos|MT|Meticais))", RegexOptions.Compiled);
        private static readonly Regex DetectEmail = new Regex(@"([\w\.\-_]+)?\w+@[\w-_]+(\.\w+){1,}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DetectUrl = new Regex(@"((?:(?:ftp|file|https?):\/\/|\/\/|www.)[-a-z0-9:%._\+~#=]{2,256}\.[a-z]{2,6}\b[-a-z0-9:%_\+.~#?&\/\/=]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static string RemoveContactsString(string language, string text)
        {
            text = DetectPhone.Replace(text, GetSanitizeReplacementString("PHONE", language));
            text = DetectEmail.Replace(text, GetSanitizeReplacementString("EMAIL", language));
            text = DetectUrl.Replace(text, GetSanitizeReplacementString("URL", language));
            return text;
        }

        //*************************************************************************************************
        private static string GetSanitizeReplacementString(string data, string language)
        {
            switch (data)
            {
                case "PHONE":
                    {
                        if (language.StartsWith("CA-ES"))
                            return " (telèfon ocult) ";
                        if (language.StartsWith("ES-"))
                            return " (teléfono oculto) ";
                        if (language.StartsWith("EN-"))
                            return " (phone hidden) ";
                        if (language.StartsWith("PT-"))
                            return " (telefone) ";
                        if (language.StartsWith("FR-"))
                            return " (téléphone caché) ";
                    }
                    break;
                case "EMAIL":
                    {
                        if (language.StartsWith("CA-ES"))
                            return " (email ocult) ";
                        if (language.StartsWith("ES-"))
                            return " (email oculto) ";
                        if (language.StartsWith("EN-"))
                            return " (email hidden) ";
                        if (language.StartsWith("PT-"))
                            return " (email) ";
                        if (language.StartsWith("FR-"))
                            return " (email caché) ";
                    }
                    break;
                case "URL":
                    {
                        if (language.StartsWith("CA-ES"))
                            return " (url ocult) ";
                        if (language.StartsWith("ES-"))
                            return " (url oculto) ";
                        if (language.StartsWith("EN-"))
                            return " (url hidden) ";
                        if (language.StartsWith("PT-"))
                            return " (url) ";
                        if (language.StartsWith("FR-"))
                            return " (url caché) ";
                    }
                    break;
            }

            return " (---) ";
        }
    }
}
