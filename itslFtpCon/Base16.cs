using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itslFtpCon
{
    internal static class Base16
    {
        static string alphaDigits = "0123456789ABCDEF";

        public static string from16(string b16)
        {
            string r = "";
            char bufC = '\0';
            bool Odd = true;
            foreach (char c in b16.ToCharArray())
            {
                bufC = (char)(((bufC << 4) | alphaDigits.IndexOfAny(new char[] { char.ToUpper(c) })) & 255);
                if (!Odd)
                {
                    r += bufC.ToString();
                }
                Odd = !Odd;
            }
            return r;
        }
    }
}