namespace EasyNetQ.MetaData.Bindings {
    using System;
    using System.Reflection;

    class PriorityBinding : IMetaDataBinding {
        public PropertyInfo BoundProperty { get; set; }

        public void ToMessageMetaData(Object source, MessageProperties destination) {
            var propertyValue = BoundProperty.GetValue(source);
            var priority = Convert.ToByte(propertyValue);

            if (priority != default(Byte)) {
                destination.PriorityPresent = true;
                destination.Priority = priority;
            }
        }

        public void FromMessageMetaData(MessageProperties source, Object destination) {
            if (source.PriorityPresent) {
                var priority = source.Priority;
                var propertyValue = Convert.ChangeType(priority, BoundProperty.PropertyType);

                BoundProperty.SetValue(destination, propertyValue);
            }
        }
    }
}