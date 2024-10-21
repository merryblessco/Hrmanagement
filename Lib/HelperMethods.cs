using System.Globalization;
using System.Text.RegularExpressions;

namespace HRbackend.Lib
{
    public class HelperMethods
    {
        public static string ConvertToInternationalFormat(string phoneNumber)
        {
            // Ensure the phone number starts with '0'
            if (phoneNumber.StartsWith("0") && phoneNumber.Length == 11)
            {
                // Remove the first '0' and prepend '234' for the Nigerian country code
                return $"234{phoneNumber.Substring(1)}";
            }
            else
            {
                throw new ArgumentException("Phone number is not valid or doesn't start with '0'");
            }
        }

        public static string formatExcelDate(string dateString)
        {
            // Remove any ordinal suffixes (like 'th', 'st', 'nd', 'rd')
            string cleanedDateString = Regex.Replace(dateString, @"\b(\d{1,2})(st|nd|rd|th)\b", "$1");

            // Parse the cleaned date string
            DateTime date = DateTime.ParseExact(cleanedDateString, "d MMM, yyyy", CultureInfo.InvariantCulture);

            // Format the DateTime to use dashes instead of slashes
            string formattedDate = date.ToString("dd-MM-yyyy");

            return formattedDate;
        }
    }
}
