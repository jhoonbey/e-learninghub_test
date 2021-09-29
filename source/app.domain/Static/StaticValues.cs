namespace app.domain.Static
{
    public static class StaticValues
    {
        public static string RegexNameUnicode = @"^[a-zA-ZÀ-ú]+$";
        public static string RegexNameUnicode_Info = " a-z A-Z À-ú ";

        public static string RegexEmail = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                         + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
            				                                [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                         + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
            				                                [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                         + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
        public static string RegexEmail_Info = " a-z A-Z 0-9 ";


        public static string RegexMobile = @"^\+(?:[0-9]●?){6,14}[0-9]$"; //^(\+|\d)[0-9]{7,16}$
        public static string RegexMobile_Info = " + 0-9 '' ";


        public static string RegexPassword = @"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%&-_]).{5,50})";
        public static string RegexPassword_Info = "a-z A-Z 0-9 ! @ # $ % & - _ ";

        public static string RegisterKey = "2J36RHI9BgH2DSib3x3s1rum2SFRHoCx";
        public static string ResetPublicKey = "2o4qCxUMiQwMVlQ4JRh5H6X5mZQlYrcx";
        public static string ResetKey = "GBqpkjHq98lgryS235P0QQclqmhLC1Zn";
        public static string RoleKey = "ynG3apec5uJBheqw66kCQE1XwMWmSAKj";
        public static string CookieKey = "RHEifgjxsIp1Owid8rWRsNqlhfyXn7w5";


    }
}
