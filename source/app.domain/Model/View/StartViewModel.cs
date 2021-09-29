using app.domain.Languages;
using app.domain.Static;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace app.domain.Model.View
{
    public class StartViewModel : ViewBaseModel
    {
        [Name]
        public string Name { get; set; }

        [Surname]
        public string Surname { get; set; }

        [Mobile]
        public string Mobile { get; set; }

        [Email]
        public string Email { get; set; }

        [Password]
        public string Password { get; set; }

        [ConfirmPassword("Password")]
        public string ConfirmPassword { get; set; }

        [Position]
        public string Position { get; set; }

        [Workplace]
        public string Workplace { get; set; }

        [Rolename]
        public string Rolename { get; set; }

        [Category("Rolename")]
        public int CategoryId { get; set; }

        [IsAgreeTerms]
        public bool IsAgreeTerms { get; set; }

        //login
        [Email]
        public string LoginEmail { get; set; }

        [Password]
        public string LoginPassword { get; set; }

        public bool LoginRememberMe { get; set; }

        //reset
        [Email]
        public string ResetEmail { get; set; }
    }

    public class NameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string textValue = Convert.ToString(value);

            int minLength = 2;
            int maxLength = 100;

            if (string.IsNullOrEmpty(textValue) || textValue.Length < minLength || textValue.Length > maxLength || !new Regex(StaticValues.RegexNameUnicode).IsMatch(textValue))
                return new ValidationResult($"{ Lang.NameText + Lang.ErrorIsIncorrectText + Lang.ErrorUseOnlyText + StaticValues.RegexNameUnicode_Info + Lang.ErrorIsIncorrectSymbolText }" +
                                           $"{ Lang.ErrorMinimumLengthText + minLength + Lang.ErrorMaximumLengthText + maxLength }");

            return ValidationResult.Success;
        }
    }

    public class SurnameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string textValue = Convert.ToString(value);

            int minLength = 2;
            int maxLength = 100;

            if (string.IsNullOrEmpty(textValue) || textValue.Length < minLength || textValue.Length > maxLength || !new Regex(StaticValues.RegexNameUnicode).IsMatch(textValue))
                return new ValidationResult($"{ Lang.SurnameText + Lang.ErrorIsIncorrectText + Lang.ErrorUseOnlyText + StaticValues.RegexNameUnicode_Info + Lang.ErrorIsIncorrectSymbolText }" +
                                           $"{ Lang.ErrorMinimumLengthText + minLength + Lang.ErrorMaximumLengthText + maxLength }");

            return ValidationResult.Success;
        }
    }

    public class MobileAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string textValue = Convert.ToString(value);

            if (string.IsNullOrEmpty(textValue) || !new Regex(StaticValues.RegexMobile).IsMatch(textValue))
                return new ValidationResult($"{ Lang.MobileText + Lang.ErrorIsIncorrectText + Lang.ErrorUseOnlyText + StaticValues.RegexMobile_Info + Lang.ErrorIsIncorrectSymbolText }");

            return ValidationResult.Success;
        }
    }

    public class EmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string textValue = Convert.ToString(value);

            if (string.IsNullOrEmpty(textValue) || !new Regex(StaticValues.RegexEmail).IsMatch(textValue))
                return new ValidationResult(Lang.EmailText + Lang.ErrorIsIncorrectText);

            return ValidationResult.Success;
        }
    }

    public class PasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string textValue = Convert.ToString(value);

            int minLength = 5;
            int maxLength = 50;

            if (string.IsNullOrEmpty(textValue) || textValue.Length < minLength || textValue.Length > maxLength || !new Regex(StaticValues.RegexPassword).IsMatch(textValue))
                return new ValidationResult($"{ Lang.PasswordText + Lang.ErrorIsIncorrectText + Lang.ErrorUseOnlyText + StaticValues.RegexPassword_Info + Lang.ErrorIsIncorrectSymbolText }" +
                                            $"{ Lang.ErrorMinimumLengthText + minLength + Lang.ErrorMaximumLengthText + maxLength + Lang.ErrorPasswordMinimumRequirment}");

            return ValidationResult.Success;
        }
    }

    public class ConfirmPasswordAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        public ConfirmPasswordAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string textValue = Convert.ToString(value);

            int minLength = 5;
            int maxLength = 50;

            if (string.IsNullOrEmpty(textValue) || textValue.Length < minLength || textValue.Length > maxLength || !new Regex(StaticValues.RegexPassword).IsMatch(textValue))
                return new ValidationResult($"{ Lang.ConfirmPasswordText + Lang.ErrorIsIncorrectText + Lang.ErrorUseOnlyText + StaticValues.RegexPassword_Info + Lang.ErrorIsIncorrectSymbolText }" +
                                            $"{ Lang.ErrorMinimumLengthText + minLength + Lang.ErrorMaximumLengthText + maxLength + Lang.ErrorPasswordMinimumRequirment}");

            //compare
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (string)property.GetValue(validationContext.ObjectInstance);

            if (textValue != comparisonValue)
                return new ValidationResult(Lang.ErrorPasswordsNotSameText);

            return ValidationResult.Success;
        }
    }

    //public class SamePasswordAttribute : ValidationAttribute
    //{
    //    private readonly string _comparisonProperty;
    //    public SamePasswordAttribute(string comparisonProperty)
    //    {
    //        _comparisonProperty = comparisonProperty;
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        string textValue = Convert.ToString(value);

    //        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

    //        if (property == null)
    //            throw new ArgumentException("Property with this name not found");

    //        var comparisonValue = (string)property.GetValue(validationContext.ObjectInstance);

    //        if (textValue != comparisonValue)
    //            return new ValidationResult(Lang.ErrorPasswordsNotSameText);

    //        return ValidationResult.Success;
    //    }
    //}


    public class PositionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string textValue = Convert.ToString(value);

            if (!string.IsNullOrEmpty(textValue))
            {
                int maxLength = 250;

                if (textValue.Length > maxLength)
                    return new ValidationResult($"{ Lang.ErrorMaximumLengthText + maxLength}");
            }

            return ValidationResult.Success;
        }
    }


    public class WorkplaceAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string textValue = Convert.ToString(value);

            if (!string.IsNullOrEmpty(textValue))
            {
                int maxLength = 250;

                if (textValue.Length > maxLength)
                    return new ValidationResult($"{ Lang.ErrorMaximumLengthText + maxLength}");
            }

            return ValidationResult.Success;
        }
    }


    public class RolenameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string textValue = Convert.ToString(value);

            if (string.IsNullOrEmpty(textValue) && textValue != "instructor" && textValue != "learner")
            {
                return new ValidationResult(Lang.ErrorRoleIsIncorrectText);
            }

            return ValidationResult.Success;
        }
    }

    public class CategoryAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        public CategoryAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int intValue = Convert.ToInt32(value);

            //compare
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (string)property.GetValue(validationContext.ObjectInstance);
            if (string.IsNullOrEmpty(comparisonValue))
            {
                return new ValidationResult(Lang.ErrorRoleIsIncorrectText);
            }
            if (comparisonValue == "instructor")
            {
                if (intValue < 1)
                {
                    return new ValidationResult(Lang.CategoryText + Lang.ErrorIsIncorrectText);
                }
            }

            return ValidationResult.Success;
        }
    }

    public class IsAgreeTermsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool boolValue = Convert.ToBoolean(value);

            if (!boolValue)
            {
                return new ValidationResult(Lang.ErrorIsAgreeTerms);
            }

            return ValidationResult.Success;
        }
    }
}
