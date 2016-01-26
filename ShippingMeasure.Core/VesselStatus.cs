using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ShippingMeasure.Core
{
    public class VesselStatus
    {
        private static Regex regex = new Regex(@"^\s*([PS])\s*:\s*([-\+]?\d?\.?\d+)\s*/\s*([-\+]?\d?\.?\d+)\s*-\s*([-\+]?\d?\.?\d+)\s*=\s*([-\+]?\d?\.?\d+)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public HInclinationStatus HStatus { get; set; }

        public decimal HValue { get; set; }

        public decimal VValue { get; set; }

        public decimal AftDraft { get; set; }

        public decimal ForeDraft { get; set; }

        public bool IsEmpty { get; set; } = false;

        public static VesselStatus Empty
        {
            get
            {
                return new VesselStatus { IsEmpty = true };
            }
        }
        public VesselStatus(HInclinationStatus hStatus, decimal hValue, decimal aftDraft, decimal foreDraft)
        {
            this.HStatus = hStatus;
            this.HValue = hValue;
            this.VValue = aftDraft - foreDraft;
            this.AftDraft = aftDraft;
            this.ForeDraft = foreDraft;
            this.IsEmpty = false;
        }

        private VesselStatus()
        {
        }

        public override string ToString()
        {
            if (this.IsEmpty)
            {
                return String.Empty;
            }
            return String.Format("{0}: {1} / {2} - {3} = {4}", this.HStatus.ToString(), this.HValue.ToString(), this.AftDraft.ToString(), this.ForeDraft.ToString(), this.VValue.ToString());
        }

        public static VesselStatus Parse(string s)
        {
            if (String.IsNullOrEmpty(s) || s.Trim().Length < 1)
            {
                return VesselStatus.Empty;
            }

            var match = VesselStatus.regex.Match(s);
            if (!match.Success)
            {
                throw new FormatException(String.Format("invalid status string to parse, argument: s = {0}", s));
            }

            var hStatus = (HInclinationStatus)Enum.Parse(typeof(HInclinationStatus), match.Groups[1].Value, true);
            var hValue = Convert.ToDecimal(match.Groups[2].Value);
            var aftDraft = Convert.ToDecimal(match.Groups[3].Value);
            var foreDraft = Convert.ToDecimal(match.Groups[4].Value);
            var vValue = Convert.ToDecimal(match.Groups[5].Value);

            if (vValue != (aftDraft - foreDraft))
            {
                throw new FormatException(String.Format("illegal equation, argument: s = {0}", s));
            }

            return new VesselStatus(hStatus, hValue, aftDraft, foreDraft);
        }

        public static VesselStatus Create(string hStatus, string hValue, string aftDraft, string foreDraft, string vValue)
        {
            return VesselStatus.Parse(String.Format("{0}: {1} / {2} - {3} = {4}", hStatus, hValue, aftDraft, foreDraft, vValue));
        }

        public static bool Validate(string hStatus, string hValue, string aftDraft, string foreDraft, string vValue)
        {
            try
            {
                VesselStatus.Create(hStatus, hValue, aftDraft, foreDraft, vValue);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public enum HInclinationStatus
    {
        P,
        S
    }
}
