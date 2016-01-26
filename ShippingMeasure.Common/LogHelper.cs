using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Common
{
    public class LogHelper
    {
        public static void WriteInformation(string information)
        {
            Write(information);
        }

        public static void WriteWarning(string warning)
        {
#if DEBUG
            var sb = new StringBuilder();
            sb.AppendLine(GetTitle("Warning"));
            sb.AppendLine(warning);
            Trace.TraceWarning(sb.ToString());
#endif
        }

        public static void WriteError(Exception exception)
        {
            WriteError(exception.ToString());
        }

        public static void WriteError(string error)
        {
#if DEBUG
            var sb = new StringBuilder();
            sb.AppendLine(GetTitle("Error"));
            sb.AppendLine(error);
            Trace.TraceError(sb.ToString());
#endif
        }

        public static void Write(Exception exception)
        {
            WriteError(exception.ToString());
        }

        public static void Write(string information)
        {
#if DEBUG
            var sb = new StringBuilder();
            sb.AppendLine(GetTitle("Information"));
            sb.AppendLine(information);
            Trace.TraceInformation(sb.ToString());
#endif
        }

        private static string GetTitle(string title)
        {
            return String.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff"), " ", title);
        }
    }
}
