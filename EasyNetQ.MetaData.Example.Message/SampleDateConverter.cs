using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyNetQ.MetaData.Example.Message
{
    public class SampleDateConverter : TypeConverter
    {
        private const String dateFormat = @"yyyy-mm-dd'T'hh:mm:ss.fff";

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return typeof(string) == sourceType;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return typeof(System.DateTime) == destinationType;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            DateTime newDate = default(System.DateTime);
            DateTime.TryParseExact((String)value, dateFormat, null, DateTimeStyles.AssumeLocal, out newDate);
            return newDate;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            DateTime oldDate = (System.DateTime)value;
            return oldDate.ToString(dateFormat);
        }
    }
}
