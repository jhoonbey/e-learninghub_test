using app.domain;
using app.domain.Exceptions;
using app.domain.Languages;
using app.domain.Utilities;
using System.Text.RegularExpressions;

namespace app.service.Validations
{
    public partial class Validator
    {
        public static void ModelIsNull(BaseModel model)
        {
            if (model == null) throw new BusinessException("Model is empty");
        }


        public static void ValidateText(Regex regex, string regexInfo, string textValue, string textLabel, int minLength, int maxLength, bool valdiateLength)
        {
            if (valdiateLength)
            {
                if (string.IsNullOrEmpty(textValue) || textValue.Length < minLength || textValue.Length > maxLength || !regex.IsMatch(textValue))
                    throw new BusinessException($"{ textLabel + Lang.ErrorIsIncorrectText + Lang.ErrorUseOnlyText + regexInfo + Lang.ErrorIsIncorrectSymbolText }" +
                                                $"{ Lang.ErrorMinimumLengthText + minLength.ToString() + Lang.ErrorMaximumLengthText + maxLength}");
            }
            else
            {
                if (string.IsNullOrEmpty(textValue) || !regex.IsMatch(textValue))
                    throw new BusinessException($"{ textLabel + Lang.ErrorIsIncorrectText + Lang.ErrorUseOnlyText + regexInfo + Lang.ErrorIsIncorrectSymbolText }");
            }
        }

        public static void ValidatePasswordText(Regex regex, string regexInfo, string textValue, string textLabel, int minLength, int maxLength, bool valdiateLength)
        {
            if (valdiateLength)
            {
                if (string.IsNullOrEmpty(textValue) || textValue.Length < minLength || textValue.Length > maxLength || !regex.IsMatch(textValue))
                    throw new BusinessException($"{ textLabel + Lang.ErrorIsIncorrectText + Lang.ErrorUseOnlyText + regexInfo + Lang.ErrorIsIncorrectSymbolText }" +
                                                $"{ Lang.ErrorMinimumLengthText + minLength.ToString() + Lang.ErrorMaximumLengthText + maxLength + Lang.ErrorPasswordMinimumRequirment}");
            }
            else
            {
                if (string.IsNullOrEmpty(textValue) || !regex.IsMatch(textValue))
                    throw new BusinessException($"{ textLabel + Lang.ErrorIsIncorrectText + Lang.ErrorUseOnlyText + regexInfo + Lang.ErrorIsIncorrectSymbolText + Lang.ErrorPasswordMinimumRequirment }");
            }
        }


        public static void ValidateText(Regex regex, string textValue, string textLabel)
        {
            if (string.IsNullOrEmpty(textValue) || !regex.IsMatch(textValue))
                throw new BusinessException(textLabel + Lang.ErrorIsIncorrectText);
        }

        public static void ValidateText(string textValue, string textLabel, int minLength, int maxLength, bool checkEmpty)
        {
            if (checkEmpty)
            {
                if (string.IsNullOrEmpty(textValue) || Common.IsOnlySpace(textValue))
                {
                    throw new BusinessException($"{ textLabel + Lang.ErrorIsIncorrectText + Lang.ErrorMinimumLengthText + minLength.ToString() + Lang.ErrorMaximumLengthText + maxLength}");
                }
            }
            else
            if (!string.IsNullOrEmpty(textValue) && !Common.IsOnlySpace(textValue))
            {
                if (textValue.Length < minLength || textValue.Length > maxLength)
                    throw new BusinessException($"{ textLabel + Lang.ErrorIsIncorrectText + Lang.ErrorMinimumLengthText + minLength.ToString() + Lang.ErrorMaximumLengthText + maxLength}");
            }
        }

        public static void ValidateText(string textValue, string textLabel, int maxLength)
        {
            if (!string.IsNullOrEmpty(textValue))
            {
                if (textValue.Length > maxLength)
                    throw new BusinessException($"{  textLabel + Lang.ErrorIsIncorrectText + Lang.ErrorMaximumLengthText + maxLength}");
            }
        }

        public static void ValidateIntPositiveIsIncorrect(int value, string textLabel)
        {
            if (value < 1)
            {
                throw new BusinessException(textLabel + Lang.ErrorIsIncorrectText);
            }
        }

        public static void ValidateDoublePositiveIsIncorrect(double value, string textLabel)
        {
            if (value <= 0)
            {
                throw new BusinessException(textLabel + Lang.ErrorIsIncorrectText);
            }
        }

        public static void ValidateIntPositiveIsNotSelected(int intValue, string textLabel)
        {
            if (intValue < 1)
            {
                throw new BusinessException(textLabel + Lang.ErrorIsNotSelected);
            }
        }

        //public static void Password(string password, int minLength, int maxLength)
        //{
        //    Regex rx = new Regex(@"^[a-zA-Z0-9_?!-&]+$");

        //    if (string.IsNullOrEmpty(password) || password.Length < minLength || password.Length > maxLength || !rx.IsMatch(password))
        //        throw new BusinessException($"Incorrect password. Use a-z, A-Z, 0-9 and _ ? ! - & symbols only. Minimum length is { minLength }, maximum { maxLength} ");
        //}

        //public static void Mail(string mail)
        //{
        //    Regex rx = new Regex(@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        //                 + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
        //    				                                [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        //                 + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
        //    				                                [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        //                 + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$");

        //    if (string.IsNullOrEmpty(mail) || Common.IsOnlySpace(mail) || !rx.IsMatch(mail))
        //        throw new BusinessException("Email is empty or incorrect");
        //}


        //public static void Mobile(string textValue, string textLabel, int minLength, int maxLength)
        //{
        //    Regex rx = new Regex(@"^\+(?:[0-9]●?){6,14}[0-9]$");  //^(\+|\d)[0-9]{7,16}$

        //    if (string.IsNullOrEmpty(textValue) || Common.IsOnlySpace(textValue) || textValue.Length < minLength || textValue.Length > maxLength || !rx.IsMatch(textValue))
        //        throw new BusinessException($"{ textLabel + Lang.ErrorIsIncorrectText + Lang.ErrorUseOnlyText} 0-9, +, '' " + Lang.ErrorIsIncorrectSymbolText +
        //                                    $"{ Lang.ErrorMinimumLengthText + minLength.ToString() + Lang.ErrorMinimumLengthText + maxLength}");
        //}



    }
}
