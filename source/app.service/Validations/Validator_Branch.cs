using app.Model.Entities;
using JhoonHelper;
using System;

namespace app.database.Validations
{
    public partial class Validator
    {
        public static void BranchCreateValidation(Branch model)
        {
            //model
            if (model == null) throw new Exception("Fill form correct");

            //if (string.IsNullOrEmpty(model.Name) || Common.IsOnlySpace(model.Name)) throw new Exception("Name is empty");

            ////Description
            //if (string.IsNullOrEmpty(model.Description) || Common.IsOnlySpace(model.Description)) throw new Exception("Description is empty");
        }

        public static void BranchEditValidation(Branch model)
        {
            //model
            if (model == null) throw new Exception("Fill form correct");

            //if (string.IsNullOrEmpty(model.Name) || Common.IsOnlySpace(model.Name)) throw new Exception("Name is empty");

            ////Description
            //if (string.IsNullOrEmpty(model.Description) || Common.IsOnlySpace(model.Description)) throw new Exception("Description is empty");
        }
    }
}
