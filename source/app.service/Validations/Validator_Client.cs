using app.Model.Entities;
using JhoonHelper;
using System;
using System.Web;

namespace app.database.Validations
{
    public partial class Validator
    {
        public static void ClientCreateValidation(Client model, HttpPostedFileBase postedFile)
        {
            //model
            if (model == null) throw new Exception("Fill form correct");

            if (postedFile == null)
            {
                throw new Exception("Use image please");
            }

            //format
            string[] allowedExtensions = new string[] { ".JPEG", ".JPG", ".PNG", ".GIF" };
            if (!MediaHelper.IsValidFile(postedFile.FileName, allowedExtensions)) throw new Exception("Only use .JPEG, .JPG and .PNG formats");

            //length
            // 1 kb = 1024,       1 mb = 1048576  byte,   50 mb = 52428800
            if (!MediaHelper.IsValidLength(postedFile.ContentLength, 10, 6291456)) throw new Exception("Use image in minimum 10 byte, maximum 6 MB");


            ////Dimensions
            //if (!MediaHelper.IsValidImageByDimensions(postedFile, 70, 70, 170, 170))
            //    throw new Exception("Use image in 170px width and 70px height");
        }
    }
}
