using app.Model.Entities;
using JhoonHelper;
using System;
using System.Web;

namespace app.database.Validations
{
    public partial class Validator
    {
        public static void NewsCreateValidation(News model)
        {
            //model
            if (model == null) throw new Exception("Fill form correct");

            //Name
            if (string.IsNullOrEmpty(model.Name) || Common.IsOnlySpace(model.Name)) throw new Exception("Name (AZ) is empty");
            if (string.IsNullOrEmpty(model.NameEN) || Common.IsOnlySpace(model.NameEN)) throw new Exception("Name (EN) is empty");
            if (string.IsNullOrEmpty(model.NameRU) || Common.IsOnlySpace(model.NameRU)) throw new Exception("Name (RU) is empty");
            //if (string.IsNullOrEmpty(model.NameTR) || Common.IsOnlySpace(model.NameTR)) throw new Exception("Name (TR) is empty");

            //Description
            if (string.IsNullOrEmpty(model.Description) || Common.IsOnlySpace(model.Description)) throw new Exception("Description (AZ) is empty");
            if (string.IsNullOrEmpty(model.DescriptionEN) || Common.IsOnlySpace(model.DescriptionEN)) throw new Exception("Description (EN) is empty");
            if (string.IsNullOrEmpty(model.DescriptionRU) || Common.IsOnlySpace(model.DescriptionRU)) throw new Exception("Description (RU) is empty");
            //if (string.IsNullOrEmpty(model.DescriptionTR) || Common.IsOnlySpace(model.DescriptionTR)) throw new Exception("Description (TR) is empty");
        }

        public static void NewsEditValidation(News model)
        {
            //model
            if (model == null) throw new Exception("Fill form correct");

            //Name
            if (string.IsNullOrEmpty(model.Name) || Common.IsOnlySpace(model.Name)) throw new Exception("Name (AZ) is empty");
            if (string.IsNullOrEmpty(model.NameEN) || Common.IsOnlySpace(model.NameEN)) throw new Exception("Name (EN) is empty");
            if (string.IsNullOrEmpty(model.NameRU) || Common.IsOnlySpace(model.NameRU)) throw new Exception("Name (RU) is empty");
            //if (string.IsNullOrEmpty(model.NameTR) || Common.IsOnlySpace(model.NameTR)) throw new Exception("Name (TR) is empty");

            //Description
            if (string.IsNullOrEmpty(model.Description) || Common.IsOnlySpace(model.Description)) throw new Exception("Description (AZ) is empty");
            if (string.IsNullOrEmpty(model.DescriptionEN) || Common.IsOnlySpace(model.DescriptionEN)) throw new Exception("Description (EN) is empty");
            if (string.IsNullOrEmpty(model.DescriptionRU) || Common.IsOnlySpace(model.DescriptionRU)) throw new Exception("Description (RU) is empty");
            //if (string.IsNullOrEmpty(model.DescriptionTR) || Common.IsOnlySpace(model.DescriptionTR)) throw new Exception("Description (TR) is empty");
        }

        public static void NewsUpdateImageValidation(int id, HttpPostedFileBase postedFile, int minHeight, int maxHeight, int minWidth, int maxWidth, bool watchSizes = false)
        {
            //model
            if (postedFile == null) throw new Exception("Select photo, please");
            if (id < 1) throw new Exception("Please select News");

            //format
            string[] allowedExtensions = new string[] { ".JPEG", ".JPG", ".PNG" };
            if (!MediaHelper.IsValidFile(postedFile.FileName, allowedExtensions)) throw new Exception("Only select photos in .JPEG, .JPG and .PNG formats");

            //length - 1 kb = 1024,       1 mb = 1048576  byte,   50 mb = 52428800
            if (!MediaHelper.IsValidLength(postedFile.ContentLength, 10, 6291456)) throw new Exception("Select photo in minimum 10 byte, maximum 6 MB");

            //Dimensions
            if (watchSizes)
            {
                if (!MediaHelper.IsValidImageByDimensions(postedFile, minHeight, maxHeight, minWidth, maxWidth))
                {
                    throw new Exception("Select photo in " + minWidth.ToString() + "px" + " width and " + minHeight.ToString() + "px" + " height");
                }
            }
        }

    }
}
