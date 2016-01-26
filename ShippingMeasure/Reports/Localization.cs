using Microsoft.Reporting.WinForms;
using ShippingMeasure.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace ShippingMeasure.Reports
{
    public class Localization
    {
        public LocalReport Report { get; private set; }

        private ResourceManager res = null;
        private CultureInfo culture = null;

        public Localization(LocalReport report)
        {
            this.Report = report;
            this.culture = Thread.CurrentThread.CurrentUICulture;
            this.LoadResourceManager();
        }

        public void Localize()
        {
            var doc = XDocument.Load(this.Report.ReportPath);
            var ns = (XNamespace)"http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition";
            var rd = (XNamespace)"http://schemas.microsoft.com/SQLServer/reporting/reportdesigner";
            doc.Descendants(ns + "Value")
                .Where(e => e.Attribute(rd + "LocID") != null && e.Attribute(rd + "LocID").Value.Length > 0)
                .Each(e =>
                {
                    try
                    {
                        var locKey = e.Attribute(rd + "LocID").Value;
                        var locText = this.res.GetString(locKey, this.culture);
                        if (locText != null)
                        {
                            e.SetValue(locText);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Write(ex);
                    }
                });

            this.Report.ReportPath = null;
            using (var stream = new StringReader(doc.Root.ToString()))
            {
                this.Report.LoadReportDefinition(stream);
            }
        }

        private void LoadResourceManager()
        {
            if (res != null)
            {
                return;
            }

            var reportName = Path.GetFileNameWithoutExtension(this.Report.ReportPath);
            var type = Type.GetType(String.Format("{0}.{1}", this.GetType().Namespace, reportName));
            this.res = new ResourceManager(type.FullName, type.Assembly);
        }
    }
}
