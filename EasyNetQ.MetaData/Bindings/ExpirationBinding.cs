namespace EasyNetQ.MetaData.Bindings {
    using System;
    using System.Reflection;

    class ExpirationBinding : IMetaDataBinding {
        public PropertyInfo BoundProperty { get; set; }

        public void ToMessageMetaData(Object source, MessageProperties destination) {
            var propertyValue = BoundProperty.GetValue(source);
            var timespan = (TimeSpan)Convert.ChangeType(propertyValue, typeof(TimeSpan));

            destination.ExpirationPresent = true;
            destination.Expiration = ((long)timespan.TotalMilliseconds).ToString();
        }

        public void FromMessageMetaData(MessageProperties source, Object destination) {
            var expiration = TimeSpan.FromMilliseconds(Int64.Parse(source.Expiration));
            var propertyValue = Convert.ChangeType(expiration, BoundProperty.PropertyType);

            BoundProperty.SetValue(destination, propertyValue);
        }
    }
}