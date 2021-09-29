using System;
using System.Reflection;
using System.Text;

namespace app.domain.Model.Entities
{
    public class EntityBaseModel : BaseModel
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsDeleted { get; set; }

        public override string ToString()
        {
            if (this == null)
            {
                return "Model is null";
            }

            StringBuilder stringBuilder = new StringBuilder();

            PropertyInfo[] properties = GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.PropertyType.FullName.StartsWith("System"))
                {
                    stringBuilder.Append(prop.Name + " - " + prop.GetValue(this, null) + Environment.NewLine);
                }
                else
                {
                    object propertyInstance = prop.GetValue(this, null);
                    if (propertyInstance != null)
                    {
                        PropertyInfo[] subProperties = prop.PropertyType.GetProperties();
                        foreach (var subProp in subProperties)
                        {
                            stringBuilder.Append(prop.Name + "." + subProp.Name + " - " + subProp.GetValue(propertyInstance, null) + Environment.NewLine);
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
}
