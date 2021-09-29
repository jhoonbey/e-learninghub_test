using app.Model.Entities;
using JhoonHelper;
using System;
using System.Web;

namespace app.database.Validations
{
    public partial class Validator
    {
        public static void CategoryCreateValidation(Category model)
        {
            //model
            if (model == null) throw new Exception("Fill form correct");

            if (string.IsNullOrEmpty(model.Name) || Common.IsOnlySpace(model.Name)) throw new Exception("Name is empty");
        }

        public static void CategoryEditValidation(Category model)
        {
            //model
            if (model == null) throw new Exception("Fill form correct");

            if (string.IsNullOrEmpty(model.Name) || Common.IsOnlySpace(model.Name)) throw new Exception("Name is empty");
        }

        public static void CategoryUpdateImageValidation(int id, HttpPostedFileBase postedFile, int minHeight, int maxHeight, int minWidth, int maxWidth, bool watchSizes = false)
        {
            //model
            if (postedFile == null) throw new Exception("Select photo, please");
            if (id < 1) throw new Exception("Please select Category");

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
