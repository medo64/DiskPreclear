using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace DiskPreclear {
    internal sealed class ProgressObjectState {

        public ProgressObjectState(Stopwatch operationStopwatch, int current, int maximum, int okCount, int nokCount, int blockSize, double writeBytesPerSecond, double readBytesPerSecond) {
            Current = current;
            Maximum = maximum;
            OkCount = okCount;
            NokCount = nokCount;
            BlockSize = blockSize;

            var timeTaken = operationStopwatch.Elapsed.TotalSeconds;
            var progressDone = (double)Current / Maximum;
            var progressRemaining = 1.0 - progressDone;

            var timeLeft = (timeTaken / progressDone) * progressRemaining;

            TimeUsed = operationStopwatch.Elapsed;
            EstimatedRemaining = TimeSpan.FromSeconds(timeLeft);

            WriteSpeed = writeBytesPerSecond / 1024 / 1024;
            ReadSpeed = readBytesPerSecond / 1024 / 1024;
        }

        /// <summary>
        /// Gets current value.
        /// </summary>
        public int Current { get; }

        /// <summary>
        /// Gets maximum value.
        /// </summary>
        public int Maximum { get; }

        /// <summary>
        /// Gets number of validated blocks.
        /// </summary>
        public int OkCount { get; }

        /// <summary>
        /// Gets number of blocks with errors.
        /// </summary>
        public int NokCount { get; }

        /// <summary>
        /// Gets block size in MB.
        /// </summary>
        public int BlockSize { get; }

        /// <summary>
        /// Gets currently used time.
        /// </summary>
        public TimeSpan TimeUsed { get; }

        /// <summary>
        /// Gets remaining time as text.
        /// </summary>
        public string TimeUsedAsString {
            get { return GetTimeSpanText(TimeUsed, 1); }
        }

        /// <summary>
        /// Gets remaining time to completion.
        /// </summary>
        public TimeSpan EstimatedRemaining { get; }

        /// <summary>
        /// Gets remaining time as text.
        /// </summary>
        public string EstimatedRemainingAsString {
            get { return GetTimeSpanText(EstimatedRemaining); }
        }

        /// <summary>
        /// Gets percentage of completion.
        /// </summary>
        public double Percents => Current * 100.0 / Maximum;

        /// <summary>
        /// Gets permille as integer value.
        /// </summary>
        public int Permilles => (int)((long)Current * 1000 / Maximum);

        /// <summary>
        /// Gets write speed in MB/s
        /// </summary>
        public double WriteSpeed { get; init; }

        /// <summary>
        /// Gets read speed in MB/s.
        /// </summary>
        public double ReadSpeed { get; init; }


        private static string GetTimeSpanText(TimeSpan time, int extra = 0) {
            var sb = new StringBuilder();
            var remaining = time;
            var days = remaining.Days;
            var hours = remaining.Hours;
            var minutes = remaining.Minutes;
            var seconds = remaining.Seconds;
            if (days > 0) {  // give result in days and hours
                sb.AppendFormat(CultureInfo.CurrentCulture, "{0} {1}", days, days != 1 ? "days" : "day");
                hours += extra;
                if (hours > 0) {
                    sb.AppendFormat(CultureInfo.CurrentCulture, " {0} {1}", hours, hours != 1 ? "hours" : "hour");
                }
            } else if (hours > 0) {  // give hours and minutes
                sb.AppendFormat(CultureInfo.CurrentCulture, "{0} {1}", hours, hours != 1 ? "hours" : "hour");
                minutes += extra;
                if (minutes > 0) {
                    sb.AppendFormat(CultureInfo.CurrentCulture, " {0} {1}", minutes, minutes != 1 ? "minutes" : "minute");
                }
            } else if (minutes > 0) {  // give minutes and seconds
                sb.AppendFormat(CultureInfo.CurrentCulture, "{0} {1}", minutes, minutes != 1 ? "minutes" : "minute");
                seconds += extra;
                if (seconds > 0) {
                    sb.AppendFormat(CultureInfo.CurrentCulture, " {0} {1}", seconds, seconds != 1 ? "seconds" : "second");
                }
            } else {  // give seconds
                seconds += extra;
                sb.AppendFormat(CultureInfo.CurrentCulture, "{0} {1}", seconds, seconds != 1 ? "seconds" : "second");
            }
            return sb.ToString();
        }

    }
}
