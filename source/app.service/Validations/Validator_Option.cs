using app.Model.Entities;
using System;

namespace app.database.Validations
{
    public partial class Validator
    {
        public static void OptionEditValidation(Option model)
        {
            //model
            if (string.IsNullOrEmpty(model.Sec)) throw new Exception("Sec is null");

            //val length
            if (!string.IsNullOrEmpty(model.Val))
            {
                if (model.Val.Length > 4000)
                {
                    throw new Exception("Value must be 4000 symbols maximum");
                }
            }


        }
    }
}
