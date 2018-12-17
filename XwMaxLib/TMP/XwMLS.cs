using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using XwMaxLib.Extensions;

namespace NailGun.Objects
{
    //*******************************************************************************************************************
    [Serializable]
    public class XwMLS
    {
        private int initialStringLengh = 0;
        private Dictionary<string, string> Values = new Dictionary<string, string>();
        private static Dictionary<string, string[]> mlsFallBack = new Dictionary<string, string[]>();
        private static Regex mlsRegex = new Regex(@"(?isx)\[(?<START>[A-Z]{2}(?:[-_][A-Z]{2})?)\](?<VALUE>.*?)\[/\1\]", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        
        public bool DisableFallBack = false;
        
        //************************************************************************************************
        public XwMLS() : base()
        {
            BuildFallBackList();
        }

        //************************************************************************************************
        public XwMLS(string value)
        {
            BuildFallBackList();
            ParseValue(value);
            initialStringLengh = value.Length;
        }
        
        //************************************************************************************************
        public static implicit operator string(XwMLS value)
        {
            return value.ToString();
        }
        
        //************************************************************************************************
        private void ParseValue(string values)
        {
            //get out now
            if (values == "")
                return;
            
            MatchCollection ms = mlsRegex.Matches(values);
            foreach (Match m in ms)
            {
                string language = m.Groups["START"].Value;
                string value = m.Groups["VALUE"].Value;
                Values.Add(LCID(false, language), value);
            }
        }

        //************************************************************************************************
        public override string ToString()
        {
            StringBuilder ret = new StringBuilder(initialStringLengh);
            foreach (var value in Values)
            {
                ret.Append($"[{value.Key}]{value.Value}[/{value.Key}]");
            }
            
            return ret.ToString();
        }

        //*************************************************************************************************
        public string GetTranslation(string language)
        {
            string lcid = LCID(false, language);
            string value = "";
            
            if (Values.ContainsKey(language))
                value = Values[language];
            
            if (DisableFallBack == false)
            {
                if (value.IsEmpty())
                {
                    if (mlsFallBack.ContainsKey(lcid))
                    {
                        string[] alternative = mlsFallBack[lcid];
                        for (int i = 0; i< alternative.Length; i++)
                        {
                            string alternativeValue = "";
                            if (Values.ContainsKey(alternative[i]))
                                alternativeValue = Values[alternative[i]];
                            if (!alternativeValue.IsEmpty())
                                return alternativeValue;
                        }
                    }
                }
            }
            
            return value;
        }

        //*************************************************************************************************
        public void SetTranslation(string language, string value)
        {
            string lcid = LCID(false, language);
            Values[lcid] = value;
        }

        //*************************************************************************************************
        public void RemoveTranslation(string language)
        {
            string lcid = LCID(false, language);
            Values.Remove(lcid);
        }

        //************************************************************************************************
        private void BuildFallBackList()
        {
            if (XwMLS.mlsFallBack.Count == 0)
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
                mlsFallBack["ES-NI"] = new string[] { "ES-ES" }; 

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
            set { SetTranslation("PT-PT", value.ToString()); }
            get { return GetTranslation("PT-PT"); }
        }

        public string PTBR
        {
            set { SetTranslation("PT-BR", value.ToString()); }
            get { return GetTranslation("PT-BR"); }
        }

        public string ENGB
        {
            set { SetTranslation("EN-GB", value.ToString()); }
            get { return GetTranslation("EN-GB"); }
        }

        public string FRFR
        {
            set { SetTranslation("FR-FR", value.ToString()); }
            get { return GetTranslation("FR-FR"); }
        }

        public string DEDE
        {
            set { SetTranslation("DE-DE", value.ToString()); }
            get { return GetTranslation("DE-DE"); }
        }

        public string ESES
        {
            set { SetTranslation("ES-ES", value.ToString()); }
            get { return GetTranslation("ES-ES"); }
        }

        public string ESMX
        {
            set { SetTranslation("ES-MX", value.ToString()); }
            get { return GetTranslation("ES-MX"); }
        }

        public string ESPE
        {
            set { SetTranslation("ES-PE", value.ToString()); }
            get { return GetTranslation("ES-PE"); }
        }

        public string ESCO
        {
            set { SetTranslation("ES-CO", value.ToString()); }
            get { return GetTranslation("ES-CO"); }
        }

        public string ESDO
        {
            set { SetTranslation("ES-DO", value.ToString()); }
            get { return GetTranslation("ES-DO"); }
        }

        public string ESAR
        {
            set { SetTranslation("ES-AR", value.ToString()); }
            get { return GetTranslation("ES-AR"); }
        }
        
        public string CAES
        {
            set { SetTranslation("CA-ES", value.ToString()); }
            get { return GetTranslation("CA-ES"); }
        }
        public string RURU
        {
            set { SetTranslation("RU-RU", value.ToString()); }
            get { return GetTranslation("RU-RU"); }
        }
        public string ZHCN
        {
            set { SetTranslation("ZH-CN", value.ToString()); }
            get { return GetTranslation("ZH-CN"); }
        }

        //************************************************************************************************
        //compatibility
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
            StringBuilder ret = new StringBuilder(initialStringLengh);
            foreach (string language in languages)
            {
                string lcid = LCID(shortCodes, language);
                bool disableFB = DisableFallBack;
                DisableFallBack = true;
                string value = GetTranslation(lcid).Trim();
                DisableFallBack = disableFB;
                if (!removeEmptyTags || value.Length > 0)
                    ret.Append($"[{lcid}]{value}[/{lcid}]");
            }

            return ret.ToString();
        }

       

        //************************************************************************************************
        public void TruncateTo(int size)
        {
            /*
            foreach (string lang in Languages)
            {
                string text = GetTranslation(lang);
                if (text.Length > size)
                    text = text.Substring(0, size);
                SetTranslation(lang, text);
            }
            */
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
                throw new Exception($"Invalid Language Token: {lang}\n\n{ToString()}");

            if (shortCode)
            {
                string[] comps = lang.Split('-');
                if (comps.Length >= 1)
                    return comps[0];
            }

            return lang;
        }

     

        //************************************************************************************************
        public void Sanitize()
        {
            /*
            foreach (string lang in Languages)
            {
                SetTranslation(lang, SanitizeString(base.GetTranslation(lang)));
            }
            */
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
            /*
            foreach (string lang in Languages)
            {
                SetTranslation(lang, base.GetTranslation(lang).Replace(find, replace));
            }
            */
        }

        //*************************************************************************************************
        public void RemoveHtml()
        {
            /*
            foreach (string lang in Languages)
            {
                SetTranslation(lang, RemoveHtmlString(base.GetTranslation(lang)));
            }
            */
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
            /*
            foreach (string lang in Languages)
            {
                SetTranslation(lang, RemoveContactsString(lang, base.GetTranslation(lang)));
            }
            */
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
