using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace DiskPreclear {
    internal sealed class ProgressObjectState {

        public ProgressObjectState(Stopwatch operationStopwatch, long current, long maximum, double writeBytesPerSecond, double readBytesPerSecond) {
            Current = current;
            Maximum = maximum;

            var timeTaken = operationStopwatch.Elapsed.TotalSeconds;
            var progressDone = (double)Current / Maximum;
            var progressRemaining = 1.0 - progressDone;

            var timeLeft = (timeTaken / progressDone) * progressRemaining;
            EstimatedRemaining = TimeSpan.FromSeconds(timeLeft);

            WriteSpeed = writeBytesPerSecond / 1024 / 1024;
            ReadSpeed = readBytesPerSecond / 1024 / 1024;
        }

        /// <summary>
        /// Current value.
        /// </summary>
        public long Current { get; }

        /// <summary>
        /// Maximum value.
        /// </summary>
        public long Maximum { get; }

        /// <summary>
        /// Gets remaining time to completion.
        /// </summary>
        public TimeSpan EstimatedRemaining { get; }

        /// <summary>
        /// Gets remaining time as text.
        /// </summary>
        public string EstimatedRemainingAsString {
            get {
                var sb = new StringBuilder();
                var remaining = EstimatedRemaining;
                var days = remaining.Days;
                var hours = remaining.Hours;
                var minutes = remaining.Minutes;
                var seconds = remaining.Seconds;
                if (days > 0) {  // give result in days and hours
                    sb.AppendFormat(CultureInfo.CurrentCulture, "{0} {1}", days, days > 1 ? "days" : "day");
                    if (hours > 0) {
                        sb.AppendFormat(CultureInfo.CurrentCulture, " {0} {1}", hours, hours > 1 ? "hours" : "hour");
                    }
                } else if (hours > 0) {  // give hours and minutes
                    sb.AppendFormat(CultureInfo.CurrentCulture, "{0} {1}", hours, hours > 1 ? "hours" : "hour");
                    if (hours > 0) {
                        sb.AppendFormat(CultureInfo.CurrentCulture, " {0} {1}", minutes, minutes > 1 ? "minutes" : "minute");
                    }
                } else if (minutes > 0) {  // give minutes and seconds
                    sb.AppendFormat(CultureInfo.CurrentCulture, "{0} {1}", minutes, minutes > 1 ? "minutes" : "minute");
                    if (hours > 0) {
                        sb.AppendFormat(CultureInfo.CurrentCulture, " {0} {1}", seconds, seconds > 1 ? "seconds" : "second");
                    }
                } else {  // give seconds
                    sb.AppendFormat(CultureInfo.CurrentCulture, "{0} {1}", seconds, seconds > 1 ? "seconds" : "second");
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets percentage of completion.
        /// </summary>
        public double Percents => Current * 100.0 / Maximum;

        /// <summary>
        /// Gets permille as integer value.
        /// </summary>
        public int Permilles => (int)(Current * 1000 / Maximum);

        /// <summary>
        /// Gets write speed in MB/s
        /// </summary>
        public double WriteSpeed { get; init; }

        /// <summary>
        /// Gets read speed in MB/s.
        /// </summary>
        public double ReadSpeed { get; init; }

    }
}
