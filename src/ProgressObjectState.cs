using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskPreclear {
    internal sealed class ProgressObjectState {

        public ProgressObjectState(long current, long maximum) {
            Current = current;
            Maximum = maximum;
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
        /// Gets percentage.
        /// </summary>
        public double Percentage => Current / (double)Maximum;

        /// <summary>
        /// Gets percentage as integer value.
        /// </summary>
        public int PercentageAsInt => (int)(Current * 100 / Maximum);

    }
}
