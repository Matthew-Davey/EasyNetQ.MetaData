namespace EasyNetQ.MetaData.Bindings {
    using System;
    using System.Reflection;
    using EasyNetQ.MetaData.Abstractions;

    class DeliveryModeBinding : IMetaDataBinding {
        const Byte DefaultDeliveryMode = 2;

        public PropertyInfo BoundProperty { get; set; }

        public void ToMessageMetaData(Object source, MessageProperties destination) {
            var propertyValue = BoundProperty.GetValue(source);
            var deliveryMode = Convert.ToByte(propertyValue);

            if (deliveryMode == default(Byte))
                deliveryMode = DefaultDeliveryMode;

            destination.DeliveryModePresent = true;
            destination.DeliveryMode = deliveryMode;
        }

        public void FromMessageMetaData(MessageProperties source, Object destination) {
            if (source.DeliveryModePresent) {
                var deliveryMode = source.DeliveryMode;

                object propertyValue;
                if (BoundProperty.PropertyType == typeof(DeliveryMode)) {
                    propertyValue = (DeliveryMode)deliveryMode;
                } else {
                    propertyValue = Convert.ChangeType(deliveryMode, BoundProperty.PropertyType);
                }

                BoundProperty.SetValue(destination, propertyValue);
            }
            else {
                BoundProperty.SetValue(destination, DefaultDeliveryMode);
            }
        }
    }
}