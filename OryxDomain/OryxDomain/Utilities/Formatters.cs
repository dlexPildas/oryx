using OryxDomain.Models.Oryx;
using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace OryxDomain.Utilities
{
    public static class Formatters
    {
        private static readonly char[] s_Diacritics = GetDiacritics();
        private static char[] GetDiacritics()
        {
            char[] accents = new char[256];

            for (int i = 0; i < 256; i++)
                accents[i] = (char)i;

            accents[(byte)'á'] = accents[(byte)'à'] = accents[(byte)'ã'] = accents[(byte)'â'] = accents[(byte)'ä'] = 'a';
            accents[(byte)'Á'] = accents[(byte)'À'] = accents[(byte)'Ã'] = accents[(byte)'Â'] = accents[(byte)'Ä'] = 'A';

            accents[(byte)'é'] = accents[(byte)'è'] = accents[(byte)'ê'] = accents[(byte)'ë'] = 'e';
            accents[(byte)'É'] = accents[(byte)'È'] = accents[(byte)'Ê'] = accents[(byte)'Ë'] = 'E';

            accents[(byte)'í'] = accents[(byte)'ì'] = accents[(byte)'î'] = accents[(byte)'ï'] = 'i';
            accents[(byte)'Í'] = accents[(byte)'Ì'] = accents[(byte)'Î'] = accents[(byte)'Ï'] = 'I';

            accents[(byte)'ó'] = accents[(byte)'ò'] = accents[(byte)'ô'] = accents[(byte)'õ'] = accents[(byte)'ö'] = 'o';
            accents[(byte)'Ó'] = accents[(byte)'Ò'] = accents[(byte)'Ô'] = accents[(byte)'Õ'] = accents[(byte)'Ö'] = 'O';

            accents[(byte)'ú'] = accents[(byte)'ù'] = accents[(byte)'û'] = accents[(byte)'ü'] = 'u';
            accents[(byte)'Ú'] = accents[(byte)'Ù'] = accents[(byte)'Û'] = accents[(byte)'Ü'] = 'U';

            accents[(byte)'ç'] = 'c';
            accents[(byte)'Ç'] = 'C';

            accents[(byte)'ñ'] = 'n';
            accents[(byte)'Ñ'] = 'N';

            accents[(byte)'ÿ'] = accents[(byte)'ý'] = 'y';
            accents[(byte)'Ý'] = 'Y';

            return accents;
        }

        public static string FormatField(string campo, int limit)
        {
            int length = campo.Length;
            return campo.Substring(0, length <= limit ? length : limit);
        }

        public static string EncriptPass(string cf1senha)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt(workFactor: 11);
            return BCrypt.Net.BCrypt.HashPassword(cf1senha, salt, true);
        }

        public static string GetEnumDescription<T>(this T enumValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return description;
        }

        public static string FormatId(string value, int length)
        {
            return value.PadLeft(length, '0');
        }

        public static string DecriptPassOryx(string senha)
        {
            if (senha.Equals(""))
            {
                return senha;
            }
            string pass = null;
            int length = senha.Length;
            for (int i = 0; i < length; i++)
            {
                char a = senha[i];
                if (pass == null)
                {
                    pass = ((int)a).ToString("X");
                }
                else
                {
                    pass += ((int)a).ToString("X");
                }
            }

            return pass;
        }

        public static string EncriptPassOryx(string senha)
        {
            return senha;
            //char[] charArray = senha.ToCharArray();
            //string aaa = null;
            //foreach (char ch in charArray)
            //{
            //    if (aaa == null)
            //    {
            //        aaa = "\\u" + ((int)ch).ToString("X4");
            //    }
            //    else
            //    {
            //        aaa += ("\\u" + ((int)ch).ToString("X4"));
            //    }
            //}

            //// Create two different encodings.
            //Encoding ascii = Encoding.UTF8;
            //Encoding unicode = Encoding.Unicode;

            //// Convert the string into a byte[].
            //byte[] unicodeBytes = unicode.GetBytes(senha);

            //// Perform the conversion from one encoding to the other.
            //byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

            //// Convert the new byte[] into a char[] and then into a string.
            //// This is a slightly different approach to converting to illustrate
            //// the use of GetCharCount/GetChars.
            //char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            //ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            //string asciiString = new string(asciiChars);
            //return asciiString;
        }

        public static string RemoveSpecialCharacterByNames(string value)
        {
            return Regex.Replace(value, "[']+", " ");
        }

        public static string OnlyNumbers(this string value)
        {
            return new Regex(@"[^\d]").Replace(value, "");
        }

        public static string Capitalize(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException("empty string");
            }
            char[] arr = str.ToCharArray();
            arr[0] = char.ToUpper(arr[0]);
            return new string(arr);
        }

        public static string FormatCep(string cf0cepcob)
        {
            return string.Format(@"{0:00000\-000}", cf0cepcob);
        }

        public static string FormatAddress(CF1 cf1)
        {
            string number = string.IsNullOrWhiteSpace(cf1.Cf1numero.Trim()) ? string.Empty : ", "+cf1.Cf1numero.Trim();
            string district = string.IsNullOrWhiteSpace(cf1.Cf1bairro) ? string.Empty : cf1.Cf1bairro.Trim();
            return string.Format("{0} {1} {2} {3}", cf1.Cf1ender1.Trim(), number, cf1.Cf1compl.Trim(), district);
        }

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] > 255)
                    sb.Append(text[i]);
                else
                    sb.Append(s_Diacritics[text[i]]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Formatar uma string CNPJ
        /// </summary>
        /// <param name="CNPJ">string CNPJ sem formatacao</param>
        /// <returns>string CNPJ formatada</returns>
        /// <example>Recebe '99999999999999' Devolve '99.999.999/9999-99'</example>
        public static string FormatCNPJ(string CNPJ)
        {
            return Convert.ToUInt64(CNPJ).ToString(@"00\.000\.000\/0000\-00");
        }

        /// <summary>
        /// Formatar uma string CPF
        /// </summary>
        /// <param name="CPF">string CPF sem formatacao</param>
        /// <returns>string CPF formatada</returns>
        /// <example>Recebe '99999999999' Devolve '999.999.999-99'</example>
        public static string FormatCPF(string CPF)
        {
            return Convert.ToUInt64(CPF).ToString(@"000\.000\.000\-00");
        }
    }
}
