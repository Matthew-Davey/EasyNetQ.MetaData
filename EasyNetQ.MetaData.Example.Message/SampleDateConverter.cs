using System;
using System.ComponentModel;
using System.Globalization;

namespace EasyNetQ.MetaData.Example.Message {
    public class SampleDateConverter : TypeConverter {
        private const String dateFormat = @"yyyy-mm-dd'T'hh:mm:ss.fff";

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return typeof(string) == sourceType;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return typeof(DateTime) == destinationType;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            DateTime newDate = default(DateTime);
            DateTime.TryParseExact((String)value, dateFormat, null, DateTimeStyles.AssumeLocal, out newDate);
            return newDate;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            DateTime oldDate = (DateTime)value;
            return oldDate.ToString(dateFormat);
        }
    }
}
