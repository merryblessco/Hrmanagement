using System.Text;

namespace LinkOrgNet.Models
{
    public class SecurityClass
    {
        private static string _key = "THEREISNOOTHERNAMELIKETHENAMEOFJESUSHEISWORTHOFGLORYANDHONOURANDPRAISE";

        private static int Asc(int value)
        {
            return Convert.ToInt32(Encoding.ASCII.GetBytes(value.ToString()).ToString());
        }

        private static int Asc(string value)
        {
            return Convert.ToInt32(Encoding.ASCII.GetBytes(value)[0]);
        }

        public static string FCODE(string aword)
        {
            string str = null;
            string str2 = null;
            try
            {
                int length = aword.Length;
                int denomenator = key.Length;
                for (int i = 0; i <= length; i++)
                {
                    str2 = key.Substring(mod(i - 1, denomenator) + 1, 1);
                    int num3 = mod(Asc(aword.Substring(i, 1)) + Asc(str2), 120) + 1;
                    str = str + ((char)num3);
                }
            }
            catch (Exception)
            {
            }
            return str;
        }

        public static string FDECODE(string apass)
        {
            int num4 = 0;
            string str = null;
            string str2 = null;
            try
            {
                int length = apass.Length;
                int denomenator = key.Length;
                for (int i = 0; i <= length; i++)
                {
                    str2 = key.Substring(mod(i - 1, denomenator) + 1, 1);
                    num4 = (Asc(apass.Substring(i, 1)) - Asc(str2)) - 1;
                    if (num4 <= 0)
                    {
                        num4 += 120;
                        str = str + ((char)num4);
                    }
                    else
                    {
                        str = str + ((char)num4);
                    }
                }
            }
            catch (Exception)
            {
            }
            return str;
        }

        private static int mod(int numerator, int denomenator)
        {
            return (numerator % denomenator);
        }

        public static string key
        {
            get
            {
                return _key;
            }
        }
    }
}
