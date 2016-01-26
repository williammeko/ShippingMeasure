using ShippingMeasure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace ShippingMeasure.Db
{
    public class UserDb : DbBase
    {
        public bool Authorize(string username, SecureString password)
        {
            bool verified = false;

            var query = new QueryContext(this.ConnectionString, "SELECT * FROM [Users] WHERE [Username] = ?");
            query.Parameters.AddWithValue(username.Trim());
            query.ExecuteReader(r =>
            {
                verified = r["password"].ToString().Equals(CommonHelper.SecureStringToMD5(password), StringComparison.Ordinal);
            });

            return verified;
        }

        public void Save(string username, SecureString password)
        {
            if (this.Exists(username))
            {
                this.Update(username, password);
            }
            else
            {
                this.Add(username, password);
            }
        }

        private bool Exists(string username)
        {
            bool existed = false;
            var query = new QueryContext(this.ConnectionString, "SELECT TOP 1 1 FROM [Users] WHERE [Username] = ?");
            query.Parameters.AddWithValue(username.Trim());
            query.ExecuteReader(r => existed = r[0].ToBoolean());
            return existed;
        }

        private void Update(string username, SecureString password)
        {
            var query = new QueryContext(this.ConnectionString, "UPDATE [Users] SET [Password] = ? WHERE [Username] = ?");
            query.Parameters.AddWithValue(CommonHelper.SecureStringToMD5(password));
            query.Parameters.AddWithValue(username.Trim());
            query.ExecuteNonQuery();
        }

        private void Add(string username, SecureString password)
        {
            var query = new QueryContext(this.ConnectionString, "INSERT INTO [Users] ([Username], [Password]) VALUES (?, ?)");
            query.Parameters.AddWithValue(username.Trim());
            query.Parameters.AddWithValue(CommonHelper.SecureStringToMD5(password));
            query.ExecuteNonQuery();
        }
    }
}
