using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Common
{
    public static class Extensions
    {
        public static void ExecuteReader(this QueryContext query, Action<OleDbDataReader> readRecord)
        {
            var conn = query.Connection;
            var newConnection = false;
            if (conn == null)
            {
                conn = new OleDbConnection(query.ConnectionString);
                newConnection = true;
            }

            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = query.Transaction;
                    cmd.CommandText = query.CommandText;
                    cmd.Parameters.AddRange(query.Parameters.ToArray());

                    using (var reader = cmd.ExecuteReader())
                    {
                        for (bool read = reader.Read(); read; read = reader.Read())
                        {
                            readRecord(reader);
                        }
                    }
                }
            }
            finally
            {
                if (newConnection)
                {
                    conn.Dispose();
                }
            }
        }

        public static int ExecuteNonQuery(this QueryContext query)
        {
            var conn = query.Connection;
            var newConnection = false;
            if (conn == null)
            {
                conn = new OleDbConnection(query.ConnectionString);
                newConnection = true;
            }

            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = query.Transaction;
                    cmd.CommandText = query.CommandText;
                    cmd.Parameters.AddRange(query.Parameters.ToArray());

                    return cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (newConnection)
                {
                    conn.Dispose();
                }
            }
        }

        public static OleDbParameter AddWithValue(this List<OleDbParameter> source, object value)
        {
            var parameter = new OleDbParameter("?", value ?? DBNull.Value);
            source.Add(parameter);
            return parameter;
        }

        public static decimal TryToDecimal(this object source)
        {
            if (source == null || source == DBNull.Value)
            {
                return 0m;
            }
            decimal d;
            if (Decimal.TryParse(source.ToString(), out d))
            {
                return d;
            }
            return 0m;
        }

        public static decimal? TryToNullableDecimal(this object source)
        {
            if (source == null || source == DBNull.Value)
            {
                return null;
            }
            decimal d;
            if (Decimal.TryParse(source.ToString(), out d))
            {
                return d;
            }
            return null;
        }

        public static DateTime TryToDateTime(this object source)
        {
            if (source == null || source == DBNull.Value)
            {
                return DateTime.MinValue;
            }
            DateTime d;
            if (DateTime.TryParse(source.ToString(), out d))
            {
                return d;
            }
            return DateTime.MinValue;
        }

        public static DateTime? TryToNullableDateTime(this object source)
        {
            if (source == null || source == DBNull.Value)
            {
                return null;
            }
            DateTime d;
            if (DateTime.TryParse(source.ToString(), out d))
            {
                return d;
            }
            return null;
        }

        public static int ToInt32(this object source)
        {
            return Convert.ToInt32(source);
        }

        public static decimal ToDecimal(this object source)
        {
            return Convert.ToDecimal(source);
        }

        public static bool ToBoolean(this object source)
        {
            return Convert.ToBoolean(source);
        }

        public static string ToStringOrNull(this object source)
        {
            if (source == null || source == DBNull.Value)
            {
                return null;
            }
            return source.ToString();
        }

        public static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static decimal MathRound(this decimal source)
        {
            return source.MathRound(3);
        }

        public static decimal MathRound(this decimal source, int decimals)
        {
            return Math.Round(source, decimals);
        }
    }
}
