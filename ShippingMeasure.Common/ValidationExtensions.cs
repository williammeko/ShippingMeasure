using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Common
{
    public static class ValidationExtensions
    {
        public static string ValidateNotEmpty(this object obj, string field, bool trim)
        {
            if (obj == null)
            {
                return String.Format(CommonStrings.MsgValidationErrorCannotBeEmpty, field);
            }

            var text = obj.ToString();
            if (String.IsNullOrEmpty(text) || (trim && text.Trim().Length < 1))
            {
                return String.Format(CommonStrings.MsgValidationErrorCannotBeEmpty, field);
            }
            return null;
        }

        public static string ValidateDecimal(this object obj, string field)
        {
            if (obj == null)
            {
                return String.Format(CommonStrings.MsgValidationErrorInvalidDecimal, field);
            }

            var text = obj.ToString();
            decimal tmp;
            if (!Decimal.TryParse(text, out tmp))
            {
                return String.Format(CommonStrings.MsgValidationErrorInvalidDecimalWithValue, field, text);
            }
            return null;
        }

        public static string ValidateDate(this object obj, string field)
        {
            if (obj == null)
            {
                return String.Format(CommonStrings.MsgValidationErrorInvalidDateTime, field);
            }

            var text = obj.ToString();
            DateTime tmp;
            if (String.IsNullOrEmpty(text) || text.Trim().Length < 1 || !DateTime.TryParse(text, out tmp))
            {
                return String.Format(CommonStrings.MsgValidationErrorInvalidDateTimeWithValue, field, text);
            }
            return null;
        }
    }
}
