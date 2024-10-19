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
    }
}
