using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
namespace System
{
    [Serializable]
    public class MultiLanguageString
    {
        #region private fields
        private enum CursorIn
        {
            OpenTag,
            CloseTag,
            Content,
            Void,
        }
        private Dictionary<String, String> _oLocales = new Dictionary<String, String>();
        #endregion


        internal string Value
        {
            get
            {
                StringBuilder oSB = new StringBuilder();
                foreach (String sLang in _oLocales.Keys)
                {
                    String sLocaltext = _oLocales[sLang];
                    if (sLocaltext == null)
                        continue;
                    sLocaltext = sLocaltext.Replace("\\", "\\\\");
                    sLocaltext = sLocaltext.Replace("]", "\\]");
                    oSB.AppendFormat("[{0}]{1}[/{0}]", sLang, sLocaltext);
                }
                return oSB.ToString();
            }
        }


        #region public properties
        public int Length
        {
            get
            {
                return this._oLocales.Keys.Count;
            }
        }
        public string[] Languages
        {
            get
            {
                return (new List<String>(this._oLocales.Keys)).ToArray();
            }
        }
        public CultureInfo[] Cultures
        {
            get
            {
                List<CultureInfo> oCult = new List<CultureInfo>();
                foreach (String sCult in this.Languages)
                {
                    try
                    {
                        oCult.Add(new CultureInfo(sCult));
                    }
                    catch { }
                }
                return oCult.ToArray();
            }
        }
        #endregion

        #region constructors
        public MultiLanguageString()
        {

        }
        public MultiLanguageString(char[] sValue) : this(new string(sValue))
        {
        }
        private static Regex _oLngRX = new Regex(@"\[([a-z]{2}(-[a-z]{2})?)\](.*?)\[/\1\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public MultiLanguageString(string sValue)
        {
            int nStartTag = sValue.IndexOf('[');
            while (nStartTag >= 0)
            {
                int nEndTag = sValue.IndexOf(']', nStartTag);
                if (nEndTag > nStartTag)
                {
                    String sLang = sValue.Substring(nStartTag + 1, nEndTag - (nStartTag + 1));
                    Boolean bOkIso = false;
                    if (sLang.Length == 2)
                    {
                        if (Char.IsLetter(sLang[0]) && Char.IsLetter(sLang[1]))
                            bOkIso = true;
                    }
                    else if (sLang.Length == 5)
                    {
                        if (Char.IsLetter(sLang[0]) && Char.IsLetter(sLang[1]) && sLang[2] == '-' && Char.IsLetter(sLang[3]) && Char.IsLetter(sLang[4]))
                            bOkIso = true;
                    }
                    if (bOkIso)
                    {

                        int nEnd = sValue.IndexOf("[/" + sLang + "]", nEndTag);//check for language end
                        if (nEnd > nEndTag)
                        {
                            string sLocale = sValue.Substring(nEndTag + 1, nEnd - (nEndTag + 1));//extract string
                                                                                                 //unescape
                            sLocale = sLocale.Replace("\\\\", "\\");
                            sLocale = sLocale.Replace("\\]", "]");
                            _oLocales[sLang.ToUpperInvariant()] = sLocale;
                            nStartTag = sValue.IndexOf('[', nEnd + 1);
                        }
                        else
                            nStartTag = sValue.IndexOf('[', nEndTag + 1);
                    }
                    else
                        nStartTag = sValue.IndexOf('[', nEndTag + 1);
                }
                else
                    nStartTag = sValue.IndexOf('[', nStartTag + 1);
            }
            /* RX
			MatchCollection oCol = _oLngRX.Matches(sValue);
			foreach(Match oMatch in oCol)
			{

				String sTag = oMatch.Groups[1].Value;
				String sContent = oMatch.Groups[3].Value;
				sContent = sContent.Replace("\\\\", "\\");
				sContent = sContent.Replace("\\]", "]");
				_oLocales[sTag.ToUpperInvariant()] = sContent;
			}*/

            /* CHAR PARSING
			StringBuilder sTag = new StringBuilder();
			StringBuilder sCloseTag = new StringBuilder();
			StringBuilder sLocale = new StringBuilder();
			CursorIn oPlace = CursorIn.Void;
			for(int i = 0 ; i < sValue.Length ; i++)
			{
				char oChar = sValue[i];
				switch(oPlace)
				{
					case CursorIn.OpenTag:
						{
							if(oChar == '/')
							{
								oPlace = CursorIn.CloseTag;
								sCloseTag.Append("[/");
							}
							else if(oChar == ']')
								oPlace = CursorIn.Content;
							else
								sTag.Append(oChar);
						}
						break;
					case CursorIn.CloseTag:
						{
							if(oChar == ']')
							{
								String sParsedEndTag = sCloseTag.ToString();
								String sParsedStartTag = sTag.ToString();
								if(sParsedEndTag.EndsWith("\\") || sParsedEndTag != "[/" + sParsedStartTag)
								{
									sParsedEndTag += "]";
									sParsedEndTag = sParsedEndTag.Replace("\\\\", "\\");
									sParsedEndTag = sParsedEndTag.Replace("\\]", "]");
									sLocale.Append(sParsedEndTag);
									sCloseTag.Length = 0;
								}
								else
								{
									oPlace = CursorIn.Void;
									String sTagIso = sTag.ToString();
									String sContent = sLocale.ToString();
									//sContent = sContent.Replace("\\\\", "\\");
									//sContent = sContent.Replace("\\]", "]");
									_oLocales[sTagIso.ToUpperInvariant()] = sContent;
									sTag.Length = 0;
									sLocale.Length = 0;
								}
							}
							else
								sCloseTag.Append(oChar);
						}
						break;
					case CursorIn.Content:
						{
							if(oChar == '[')
								oPlace = CursorIn.OpenTag;
							else if(oChar == '\\')
							{
								if(++i < sValue.Length-1)
									sLocale.Append(sValue[i]);
							}
							else
								sLocale.Append(oChar);
						}
						break;
					case CursorIn.Void:
					default:
						{
							if(oChar == '[')
								oPlace = CursorIn.OpenTag;
						}
						break;
				}
			}
			if(oPlace == CursorIn.Void && sTag.Length != 0)
				_oLocales[sTag.ToString().ToUpperInvariant()] = sLocale.ToString();*/


        }

        #endregion

        #region public methods
        public virtual string GetTranslation(CultureInfo oCulture)
        {
            return this.GetTranslation(oCulture.Name);
        }
        public virtual string GetTranslation(string sLang)
        {
            sLang = sLang.ToUpper();
            String sValue = null;
            if (_oLocales.TryGetValue(sLang, out sValue))
                return sValue;
            else
                return String.Empty;
        }

        public virtual void RemoveTranslation(CultureInfo oCulture)
        {
            this.RemoveTranslation(oCulture.Name);
        }
        public virtual void RemoveTranslation(string sLang)
        {
            sLang = sLang.ToUpper();
            _oLocales.Remove(sLang);
        }

        public virtual void SetTranslation(string sLang, string sLocaltext)
        {
            sLang = sLang.ToUpper();

            _oLocales[sLang] = sLocaltext;
        }

        public virtual void SetTranslation(CultureInfo oCulture, string sLocaltext)
        {
            this.SetTranslation(oCulture.Name, sLocaltext);
        }

        public string ToHtml()
        {
            StringBuilder oSb = new StringBuilder();
            oSb.Append("<SELECT>");
            foreach (string sLang in this.Languages)
                oSb.AppendFormat("<OPTION VAULUE=\"{0}\">[ {0} ] {1}</OPTION>", sLang, this.GetTranslation(sLang));
            oSb.Append("</SELECT>");
            return oSb.ToString();
        }
        #endregion

        #region overrides
        public override int GetHashCode() { return this.Value.GetHashCode(); }
        public override bool Equals(object oObj)
        {
            if (oObj == null)
                return false;
            if (oObj is String)
                oObj = new MultiLanguageString(oObj as String);
            if (!(oObj is MultiLanguageString))
                return false;
            MultiLanguageString oThat = oObj as MultiLanguageString;
            String[] oLangThis = this.Languages;
            String[] oLangThat = this.Languages;
            if (oLangThis.Length != oLangThis.Length)
                return false;
            foreach (String sLangThis in oLangThis)
                if (Array.IndexOf<String>(oLangThat, sLangThis) < 0)
                    return false;
            foreach (String sLangThat in oLangThat)
                if (Array.IndexOf<String>(oLangThis, sLangThat) < 0)
                    return false;
            foreach (String sLangThis in oLangThis)
            {
                if (!this.GetTranslation(sLangThis).Equals(oThat.GetTranslation(sLangThis)))
                    return false;
            }
            return true;
        }
        public bool Equals(string sValue)
        {
            return this.Equals(new MultiLanguageString(sValue));
        }
        public bool Equals(string sValue, string sLang)
        {
            return this.GetTranslation(sLang).Equals(sValue);
        }
        public override string ToString() { return this.Value; }
        public string ToString(string sLang) { return this.GetTranslation(sLang); }
        public string ToString(IFormatProvider oProvider)
        {
            return this.GetTranslation(CultureInfo.CurrentUICulture.Name).ToString(oProvider);
        }
        public string ToString(string sLang, IFormatProvider oProvider)
        {
            return this.GetTranslation(sLang).ToString(oProvider);
        }
        #endregion

        #region casts
        public static implicit operator MultiLanguageString(string sValue)
        {
            return new MultiLanguageString(sValue);
        }
        /*
        public static implicit operator MultiLanguageString(Variant sValue)
        {
            return new MultiLanguageString(sValue.ToString());
        }
        */
        public static implicit operator string(MultiLanguageString oValue)
        {
            return oValue.GetTranslation(CultureInfo.CurrentUICulture.Name);
        }
        /*
        public static implicit operator Variant(MultiLanguageString oValue)
        {
            return new Variant(oValue.Value);
        }
        */
        #endregion

        #region Indexers
        /// <summary>
        /// Extract a translation from a multilanguage string
        /// </summary>
        /// <param name="sLang">The language to return</param>
        /// <returns></returns>
        public string this[string sLang]
        {
            get { return this.GetTranslation(sLang); }
        }
        /// <summary>
        /// Extract a translation from a multilanguage string
        /// </summary>
        /// <param name="nIndex">The index of the language(in the current string)</param>
        /// <returns></returns>
        public string this[int nIndex]
        {
            get { return this.GetTranslation(this.Languages[nIndex]); }
        }
        #endregion
    }
}