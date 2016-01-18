namespace EasyNetQ.MetaData {
    using System;
    using System.Reflection;

    interface IMetaDataBinding {
        void ToMessageMetaData(Object source, MessageProperties destination);
        void FromMessageMetaData(MessageProperties source, Object destination);
    }
}