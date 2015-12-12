namespace EasyNetQ.MetaData.Bindings {
    using System;
    using System.Reflection;

    class DeliveryModeBinding : IMetaDataBinding {
        public PropertyInfo BoundProperty { get; set; }

        public void ToMessageMetaData(Object source, MessageProperties destination) {
            var propertyValue = BoundProperty.GetValue(source);
            var deliveryMode = Convert.ToByte(propertyValue);

            destination.DeliveryModePresent = true;
            destination.DeliveryMode = deliveryMode;
        }

        public void FromMessageMetaData(MessageProperties source, Object destination) {
            var deliveryMode = source.DeliveryMode;
            var propertyValue = Convert.ChangeType(deliveryMode, BoundProperty.PropertyType);

            BoundProperty.SetValue(destination, propertyValue);
        }
    }
}