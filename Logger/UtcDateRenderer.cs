using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    [LayoutRenderer("utc_date")]
    public class UtcDateRenderer : LayoutRenderer
    {
        public UtcDateRenderer()
        {
            this.Format = "G";
            this.Culture = CultureInfo.InvariantCulture;
        }

        public CultureInfo Culture { get; set; }

        [DefaultParameter]
        public string Format { get; set; }

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(logEvent.TimeStamp.ToUniversalTime().ToString(this.Format, this.Culture));
        }
    }
}
