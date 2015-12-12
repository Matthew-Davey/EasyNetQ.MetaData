namespace EasyNetQ.MetaData {
    using System;

    static class DateTimeExtensions {
        static public DateTime Epoch = DateTime.Parse("1970-01-01 00:00:00");

        static public Int64 ToUnixTimestamp(this DateTime value) {
            var period = value.Subtract(Epoch);

            return Convert.ToInt64(period.TotalSeconds);
        }

        static public DateTime FromUnixTimestamp(this Int64 value) {
            var period = TimeSpan.FromSeconds(value);

            return Epoch.Add(period);
        }
    }
}