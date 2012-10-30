using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Carcass.Common.Utility;
using Carcass.Common.Resources;

namespace Carcass.Common.MVC.Validation
{
    /// <summary>
    /// Custom validator to register localized Carcass validation messages.
    /// Some info about <c>ModelValidator</c>:
    /// http://msdn.microsoft.com/en-us/library/system.web.mvc.modelvalidator%28v=vs.108%29.aspx
    /// http://stackoverflow.com/questions/3611166/asp-net-mvc-2-problem-with-custom-modelvalidator
    /// </summary>
    public class DataTypeModelValidator : ModelValidator
    {
        public DataTypeModelValidator(ModelMetadata metadata, ControllerContext context)
            : base(metadata, context)
        {
            IsNumber = false;
            DataType = DataType.Text;
        }
                
        public DataType DataType { get; set; }

        public bool IsNumber { get; set; }

        private Func<ModelMetadata, string> ErrorMessage { get; set; }

        private Func<string, ModelMetadata, bool> Validator { get; set; }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            if (Metadata.Model == null)
                yield break;

            string formattedValue = null;
            if (Metadata.Model != null)
            {
                if (!String.IsNullOrEmpty(Metadata.EditFormatString))
                    formattedValue = String.Format(CultureInfo.CurrentCulture, Metadata.EditFormatString, Metadata.Model);
                else
                    formattedValue = Metadata.Model.ToString();
            }

            if (Validator != null && !Validator(formattedValue, Metadata))
            {
                yield return new ModelValidationResult { Message = ErrorMessage(Metadata) };
            }
        }

        internal static void Configure(DataTypeModelValidator validator)
        {
            Throw.IfNullArgument(validator, "validator");
            Throw.IfNullArgument(validator.Metadata, "metadata");
            LoadDataType(validator);

            // TODO: Refactor and implement server-side validors for Url, Credit Card, E-mail, and others

            if(validator.DataType == DataType.DateTime) 
            {
                validator.ErrorMessage = metadata => ValidationResources.DateTime;
                validator.Validator = (val, metadata) =>
                {
                    DateTime dt;
                    return DateTime.TryParse(val, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out dt);
                };
            }
            else if(validator.DataType == DataType.Date) 
            {
                validator.ErrorMessage = metadata => ValidationResources.Date;
                validator.Validator = (val, metadata) => 
                {   
                    DateTime dt;
                    return DateTime.TryParse(val, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out dt);
                };
            }
            else if(validator.DataType == DataType.Time) 
            {
                validator.ErrorMessage = metadata => ValidationResources.Time;
                validator.Validator = (val, metadata) => 
                {   
                    DateTime dt;
                    return DateTime.TryParse(val, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out dt);
                };
            }
        }

        private static void LoadDataType(DataTypeModelValidator validator)
        {
            if (!String.IsNullOrEmpty(validator.Metadata.DataTypeName))
            {
                validator.DataType = (DataType)Enum.Parse(typeof(DataType), validator.Metadata.DataTypeName);
            }
            else
            {
                var typeName = validator.Metadata.TemplateHint ?? validator.Metadata.ModelType.Name;
                if (Nullable.GetUnderlyingType(validator.Metadata.ModelType) != null
                       && validator.Metadata.ModelType.GenericTypeArguments.Length > 0)
                {
                    typeName = validator.Metadata.ModelType.GenericTypeArguments[0].Name;
                }

                var dataType = DataType.Text;
                var isNumber = false;

                switch (typeName)
                {
                    case "MultilineText": dataType = DataType.MultilineText; break;
                    case "Password": dataType = DataType.Password; break;
                    case "Text": dataType = DataType.Text; break;
                    case "CreditCard": dataType = DataType.CreditCard; break;
                    case "Currency": dataType = DataType.Currency; break;
                    case "Html": dataType = DataType.Html; break;
                    case "Duration": dataType = DataType.Duration; break;

                    case "PhoneNumber": dataType = DataType.PhoneNumber; break;
                    case "Url": dataType = DataType.Url; break;
                    case "ImageUrl": dataType = DataType.Url; break;
                    case "EmailAddress": dataType = DataType.EmailAddress; break;
                    case "PostalCode": dataType = DataType.PostalCode; break;
                    case "DateTime": dataType = DataType.DateTime; break;
                    case "Date": dataType = DataType.Date; break;
                    case "Time": dataType = DataType.Time; break;

                    case /* typeof(string).Name */ "String":
                        dataType = DataType.Text; break;

                    case /* typeof(sbyte).Name*/ "SByte":
                    case /* typeof(int).Name*/ "Int32":
                    case /* typeof(short).Name*/ "Int16":
                    case /* typeof(long).Name*/ "Int64":
                    case /* typeof(byte).Name*/ "Byte":
                    case /* typeof(ushort).Name*/ "UInt16":
                    case /* typeof(uint).Name*/ "UInt32":
                    case /* typeof(ulong).Name*/ "UInt64":
                    case /* typeof(decimal).Name*/ "Decimal":
                    case /* typeof(float).Name*/ "Single":
                    case /* typeof(double).Name*/ "Double":
                    case /* typeof(bool).Name*/ "Boolean":
                        dataType = DataType.Text;
                        isNumber = true;
                        break;
                 };

                validator.DataType = dataType;
                validator.IsNumber = isNumber;
            }
        }
    }
}
