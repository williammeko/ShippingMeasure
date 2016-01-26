using ShippingMeasure.Common;
using ShippingMeasure.Core;
using ShippingMeasure.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Db
{
    public class ReceiptDb : DbBase
    {
        private List<Receipt> all = null;
        private List<KindOfGoods> allKindsOfGoods = null;

        public IEnumerable<Receipt> GetAll()
        {
            if (this.all == null)
            {
                var items = new List<Receipt>();

                new QueryContext(this.ConnectionString, "SELECT r.*, k.[Name] AS KindOfGoodsName FROM [Receipts] AS r LEFT OUTER JOIN [KindsOfGoods] AS k ON r.KindOfGoodsUId = k.[UId]")
                    .ExecuteReader(r => items.Add(this.ReadReceipt(r)));

                this.all = items;
            }

            return this.all;
        }

        public IEnumerable<ReceiptTankDetail> GetReceiptTankDetails(string receiptNo)
        {
            var items = new List<ReceiptTankDetail>();

            var query = new QueryContext(this.ConnectionString, "SELECT * FROM [ReceiptTankDetails] WHERE [ReceiptNo] = ?");
            query.Parameters.AddWithValue(receiptNo);
            query.ExecuteReader(r => items.Add(new ReceiptTankDetail
            {
                TankName = r["TankName"].ToStringOrNull(),
                VolumeByHeight = r["VolumeByHeight"].ToBoolean(),
                Height = r["Height"].TryToNullableDecimal(),
                TemperatureOfTank = r["TemperatureOfTank"].TryToNullableDecimal(),
                HeightOfWater = r["HeightOfWater"].TryToNullableDecimal(),
                TemperatureMeasured = r["TemperatureMeasured"].TryToNullableDecimal(),
                DensityMeasured = r["DensityMeasured"].TryToNullableDecimal(),
                DensityOfStandard = r["DensityOfStandard"].TryToNullableDecimal(),
                Vcf20 = r["Vcf20"].TryToNullableDecimal(),
                Volume = r["Volume"].TryToNullableDecimal(),
                VolumeOfWater = r["VolumeOfWater"].TryToNullableDecimal(),
                VolumeOfStandard = r["VolumeOfStandard"].TryToNullableDecimal(),
                Mass = r["Mass"].TryToNullableDecimal(),
            }));

            return items;
        }

        public IEnumerable<KindOfGoods> GetAllKindsOfGoods()
        {
            if (this.allKindsOfGoods == null)
            {
                var items = new List<KindOfGoods>();

                new QueryContext(this.ConnectionString, "SELECT * FROM [KindsOfGoods]")
                    .ExecuteReader(r => items.Add(new KindOfGoods { UId = r["UId"].ToStringOrNull(), Name = r["Name"].ToStringOrNull(), Customized = true }));

                this.allKindsOfGoods = items;
            }

            return this.allKindsOfGoods;
        }

        public Receipt Get(string receiptNo)
        {
            Receipt receipt = null;

            var query = new QueryContext(this.ConnectionString, "SELECT r.*, k.[Name] AS KindOfGoodsName FROM [Receipts] AS r LEFT OUTER JOIN [KindsOfGoods] AS k ON r.KindOfGoodsUId = k.[UId] WHERE r.[No] = ?");
            query.Parameters.AddWithValue(receiptNo);
            query.ExecuteReader(r => receipt = this.ReadReceipt(r));

            if (receipt == null)
            {
                throw new KeyNotFoundException(String.Format("Receipt No.: \"{0}\" not found", receiptNo));
            }

            receipt.ReceiptTankDetails.AddRange(this.GetReceiptTankDetails(receiptNo));
            return receipt;
        }

        public void Save(Receipt receipt)
        {
            this.ValidateRequiredFields(receipt);
            this.ValidateFields(receipt.ReceiptTankDetails);

            using (var conn = new OleDbConnection(this.ConnectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        if (this.Exists(tran, receipt.No))
                        {
                            this.Update(tran, receipt);
                        }
                        else
                        {
                            this.Add(tran, receipt);
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public void Save(KindOfGoods kindOfGoods)
        {
            this.ValidateFields(kindOfGoods);

            using (var conn = new OleDbConnection(this.ConnectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        if (this.ExistsKindOfGoods(tran, kindOfGoods.UId))
                        {
                            this.Update(tran, kindOfGoods);
                        }
                        else
                        {
                            this.Add(tran, kindOfGoods);
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public bool Exists(string receiptNo)
        {
            return this.Exists(null, receiptNo);
        }

        public void Remove(string receiptNo)
        {
            using (var conn = new OleDbConnection(this.ConnectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        this.RemoveReceiptTankDetails(tran, receiptNo);

                        var query = new QueryContext(null, "DELETE * FROM [Receipts] WHERE [No] = ?") { Connection = conn, Transaction = tran };
                        query.Parameters.AddWithValue(receiptNo);
                        query.ExecuteNonQuery();

                        tran.Commit();

                        if (this.all != null)
                        {
                            this.all.RemoveAll(r => r.No.Equals(receiptNo));
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public void RemoveKindOfGoods(string uid)
        {
            using (var conn = new OleDbConnection(this.ConnectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        if (this.IsKindOfGoodsInUsed(tran, uid))
                        {
                            throw new InvalidOperationException(String.Format("The kind [{0}] is currently in used, cannot be removed", uid));
                        }

                        var query = new QueryContext(null, "DELETE * FROM [KindsOfGoods] WHERE [UId] = ?") { Connection = conn, Transaction = tran };
                        query.Parameters.AddWithValue(uid);
                        query.ExecuteNonQuery();

                        tran.Commit();

                        if (this.allKindsOfGoods != null)
                        {
                            this.allKindsOfGoods.RemoveAll(k => k.UId.Equals(uid));
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }

        private void Update(OleDbTransaction tran, Receipt receipt)
        {
            OleDbConnection conn;
            var newTran = (tran == null);
            if (newTran)
            {
                conn = new OleDbConnection(this.ConnectionString);
                conn.Open();
                tran = conn.BeginTransaction();
            }
            else
            {
                conn = tran.Connection;
            }

            try
            {
                var query = new QueryContext(null, @"UPDATE [Receipts] SET
                        [VesselName] = ?, [Time] = ?, [ReceiptFor] = ?, [PortOfShipment] = ?,
                        [PortOfDestination] = ?, [KindOfGoodsUId] = ?, [VesselStatus] = ?, [TotalOfVolumeOfStandard] = ?,
                        [TotalOfVolume] = ?, [TotalOfVolumeOfWater] = ?, [TotalOfMass] = ?, [TotalOfVolumeOfPipes] = ?,
                        [OperaterName] = ?, [AgentName] = ?, [ShipperName] = ?, [ConsignerName] = ?,
                        [ConsigneeName] = ?, [TimeSaved] = ?, [ReceiptType] = ?
                    WHERE [No] = ?") { Connection = conn, Transaction = tran };
                query.Parameters.AddWithValue(receipt.VesselName);
                query.Parameters.AddWithValue(receipt.Time).OleDbType = OleDbType.Date;
                query.Parameters.AddWithValue(receipt.ReceiptFor);
                query.Parameters.AddWithValue(receipt.PortOfShipment);
                query.Parameters.AddWithValue(receipt.PortOfDestination);
                query.Parameters.AddWithValue(receipt.KindOfGoods != null ? receipt.KindOfGoods.UId : null);
                query.Parameters.AddWithValue(receipt.VesselStatus != null ? receipt.VesselStatus.ToString() : VesselStatus.Empty.ToString());
                query.Parameters.AddWithValue(receipt.TotalOfVolumeOfStandard);
                query.Parameters.AddWithValue(receipt.TotalOfVolume);
                query.Parameters.AddWithValue(receipt.TotalOfVolumeOfWater);
                query.Parameters.AddWithValue(receipt.TotalOfMass);
                query.Parameters.AddWithValue(receipt.TotalOfVolumeOfPipes);
                query.Parameters.AddWithValue(receipt.OperaterName);
                query.Parameters.AddWithValue(receipt.AgentName);
                query.Parameters.AddWithValue(receipt.ShipperName);
                query.Parameters.AddWithValue(receipt.ConsignerName);
                query.Parameters.AddWithValue(receipt.ConsigneeName);
                query.Parameters.AddWithValue(DateTime.Now).OleDbType = OleDbType.Date;
                query.Parameters.AddWithValue(receipt.ReceiptType.ToString());
                query.Parameters.AddWithValue(receipt.No);
                query.ExecuteNonQuery();

                this.RemoveReceiptTankDetails(tran, receipt.No);
                this.Add(tran, receipt.No, receipt.ReceiptTankDetails);

                if (newTran)
                {
                    tran.Commit();
                }

                if (this.all != null)
                {
                    this.all.RemoveAll(r => r.No.Equals(receipt.No));
                    this.all.Add(receipt);
                }
            }
            catch (Exception ex)
            {
                if (newTran)
                {
                    tran.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (newTran)
                {
                    tran.Dispose();
                    conn.Dispose();
                }
            }
        }

        private void Update(OleDbTransaction tran, KindOfGoods kindOfGoods)
        {
            OleDbConnection conn;
            var newTran = (tran == null);
            if (newTran)
            {
                conn = new OleDbConnection(this.ConnectionString);
                conn.Open();
                tran = conn.BeginTransaction();
            }
            else
            {
                conn = tran.Connection;
            }

            try
            {
                var query = new QueryContext(null, @"UPDATE [KindsOfGoods] SET [Name] = ? WHERE [UId] = ?") { Connection = conn, Transaction = tran };
                query.Parameters.AddWithValue(kindOfGoods.Name);
                query.Parameters.AddWithValue(kindOfGoods.UId);
                query.ExecuteNonQuery();

                if (newTran)
                {
                    tran.Commit();
                }

                if (this.allKindsOfGoods != null)
                {
                    this.allKindsOfGoods.RemoveAll(k => k.UId.Equals(kindOfGoods.UId));
                    this.allKindsOfGoods.Add(kindOfGoods);
                }
            }
            catch (Exception ex)
            {
                if (newTran)
                {
                    tran.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (newTran)
                {
                    tran.Dispose();
                    conn.Dispose();
                }
            }
        }

        private void Add(OleDbTransaction tran, Receipt receipt)
        {
            OleDbConnection conn;
            var newTran = (tran == null);
            if (newTran)
            {
                conn = new OleDbConnection(this.ConnectionString);
                conn.Open();
                tran = conn.BeginTransaction();
            }
            else
            {
                conn = tran.Connection;
            }

            try
            {
                var query = new QueryContext(null, @"INSERT INTO [Receipts]
                    (
                        [No],
                        [VesselName], [Time], [ReceiptFor], [PortOfShipment],
                        [PortOfDestination], [KindOfGoodsUId], [VesselStatus], [TotalOfVolumeOfStandard],
                        [TotalOfVolume], [TotalOfVolumeOfWater], [TotalOfMass], [TotalOfVolumeOfPipes],
                        [OperaterName], [AgentName], [ShipperName], [ConsignerName],
                        [ConsigneeName], [TimeSaved], [ReceiptType]
                    )
                    VALUES
                    (
                        ?,
                        ?, ?, ?, ?,
                        ?, ?, ?, ?,
                        ?, ?, ?, ?,
                        ?, ?, ?, ?,
                        ?, ?, ?
                    )") { Connection = conn, Transaction = tran };
                query.Parameters.AddWithValue(receipt.No);
                query.Parameters.AddWithValue(receipt.VesselName);
                query.Parameters.AddWithValue(receipt.Time).OleDbType = OleDbType.Date;
                query.Parameters.AddWithValue(receipt.ReceiptFor);
                query.Parameters.AddWithValue(receipt.PortOfShipment);
                query.Parameters.AddWithValue(receipt.PortOfDestination);
                query.Parameters.AddWithValue(receipt.KindOfGoods != null ? receipt.KindOfGoods.UId : null);
                query.Parameters.AddWithValue(receipt.VesselStatus != null ? receipt.VesselStatus.ToString() : VesselStatus.Empty.ToString());
                query.Parameters.AddWithValue(receipt.TotalOfVolumeOfStandard);
                query.Parameters.AddWithValue(receipt.TotalOfVolume);
                query.Parameters.AddWithValue(receipt.TotalOfVolumeOfWater);
                query.Parameters.AddWithValue(receipt.TotalOfMass);
                query.Parameters.AddWithValue(receipt.TotalOfVolumeOfPipes);
                query.Parameters.AddWithValue(receipt.OperaterName);
                query.Parameters.AddWithValue(receipt.AgentName);
                query.Parameters.AddWithValue(receipt.ShipperName);
                query.Parameters.AddWithValue(receipt.ConsignerName);
                query.Parameters.AddWithValue(receipt.ConsigneeName);
                query.Parameters.AddWithValue(DateTime.Now).OleDbType = OleDbType.Date;
                query.Parameters.AddWithValue(receipt.ReceiptType.ToString());
                query.ExecuteNonQuery();

                this.Add(tran, receipt.No, receipt.ReceiptTankDetails);

                if (newTran)
                {
                    tran.Commit();
                }

                if (this.all != null)
                {
                    this.all.RemoveAll(r => r.No.Equals(receipt.No));
                    this.all.Add(receipt);
                }
            }
            catch (Exception ex)
            {
                if (newTran)
                {
                    tran.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (newTran)
                {
                    tran.Dispose();
                    conn.Dispose();
                }
            }
        }

        private void Add(OleDbTransaction tran, KindOfGoods kindOfGoods)
        {
            OleDbConnection conn;
            var newTran = (tran == null);
            if (newTran)
            {
                conn = new OleDbConnection(this.ConnectionString);
                conn.Open();
                tran = conn.BeginTransaction();
            }
            else
            {
                conn = tran.Connection;
            }

            try
            {
                var query = new QueryContext(null, @"INSERT INTO [KindsOfGoods] ([UId], [Name]) VALUES (?, ?)") { Connection = conn, Transaction = tran };
                query.Parameters.AddWithValue(kindOfGoods.UId);
                query.Parameters.AddWithValue(kindOfGoods.Name);
                query.ExecuteNonQuery();

                if (newTran)
                {
                    tran.Commit();
                }

                if (this.allKindsOfGoods != null)
                {
                    this.allKindsOfGoods.RemoveAll(k => k.UId.Equals(kindOfGoods.UId));
                    this.allKindsOfGoods.Add(kindOfGoods);
                }
            }
            catch (Exception ex)
            {
                if (newTran)
                {
                    tran.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (newTran)
                {
                    tran.Dispose();
                    conn.Dispose();
                }
            }
        }

        private void RemoveReceiptTankDetails(OleDbTransaction tran, string receiptNo)
        {
            OleDbConnection conn;
            if (tran != null)
            {
                conn = tran.Connection;
            }
            else
            {
                conn = new OleDbConnection(this.ConnectionString);
                conn.Open();
            }

            try
            {
                var query = new QueryContext(null, "DELETE * FROM [ReceiptTankDetails] WHERE [ReceiptNo] = ?") { Connection = conn, Transaction = tran };
                query.Parameters.AddWithValue(receiptNo);
                query.ExecuteNonQuery();
            }
            finally
            {
                if (tran == null)
                {
                    conn.Dispose();
                }
            }
        }

        private void Add(OleDbTransaction tran, string receiptNo, IEnumerable<ReceiptTankDetail> receiptTankDetails)
        {
            OleDbConnection conn;
            if (tran != null)
            {
                conn = tran.Connection;
            }
            else
            {
                conn = new OleDbConnection(this.ConnectionString);
                conn.Open();
            }

            try
            {
                var query = new QueryContext(null, @"INSERT INTO ReceiptTankDetails
                    (
                        ReceiptNo,
                        TankName, VolumeByHeight, Height, TemperatureOfTank,
                        HeightOfWater, TemperatureMeasured, DensityMeasured, DensityOfStandard,
                        Vcf20, Volume, VolumeOfWater, VolumeOfStandard,
                        Mass
                    )
                    VALUES
                    (
                        ?,
                        ?, ?, ?, ?,
                        ?, ?, ?, ?,
                        ?, ?, ?, ?,
                        ?
                    )") { Connection = conn, Transaction = tran };

                receiptTankDetails.Each(d =>
                {
                    query.Parameters.Clear();
                    query.Parameters.AddWithValue(receiptNo);
                    query.Parameters.AddWithValue(d.TankName);
                    query.Parameters.AddWithValue(d.VolumeByHeight);
                    query.Parameters.AddWithValue(d.Height);
                    query.Parameters.AddWithValue(d.TemperatureOfTank);
                    query.Parameters.AddWithValue(d.HeightOfWater);
                    query.Parameters.AddWithValue(d.TemperatureMeasured);
                    query.Parameters.AddWithValue(d.DensityMeasured);
                    query.Parameters.AddWithValue(d.DensityOfStandard);
                    query.Parameters.AddWithValue(d.Vcf20);
                    query.Parameters.AddWithValue(d.Volume);
                    query.Parameters.AddWithValue(d.VolumeOfWater);
                    query.Parameters.AddWithValue(d.VolumeOfStandard);
                    query.Parameters.AddWithValue(d.Mass);
                    query.ExecuteNonQuery();
                });
            }
            finally
            {
                if (tran == null)
                {
                    conn.Dispose();
                }
            }
        }

        private bool Exists(OleDbTransaction tran, string receiptNo)
        {
            OleDbConnection conn;
            if (tran != null)
            {
                conn = tran.Connection;
            }
            else
            {
                conn = new OleDbConnection(this.ConnectionString);
                conn.Open();
            }

            try
            {
                bool existed = false;
                var query = new QueryContext(null, "SELECT COUNT(*) FROM [Receipts] WHERE [No] = ?") { Connection = conn, Transaction = tran };
                query.Parameters.AddWithValue(receiptNo);
                query.ExecuteReader(r => existed = r[0].ToBoolean());
                return existed;
            }
            finally
            {
                if (tran == null)
                {
                    conn.Dispose();
                }
            }
        }

        private bool ExistsKindOfGoods(OleDbTransaction tran, string uid)
        {
            OleDbConnection conn;
            if (tran != null)
            {
                conn = tran.Connection;
            }
            else
            {
                conn = new OleDbConnection(this.ConnectionString);
                conn.Open();
            }

            try
            {
                bool existed = false;
                var query = new QueryContext(null, "SELECT COUNT(*) FROM [KindsOfGoods] WHERE [UId] = ?") { Connection = conn, Transaction = tran };
                query.Parameters.AddWithValue(uid);
                query.ExecuteReader(r => existed = r[0].ToBoolean());
                return existed;
            }
            finally
            {
                if (tran == null)
                {
                    conn.Dispose();
                }
            }
        }

        private bool IsKindOfGoodsInUsed(OleDbTransaction tran, string uid)
        {
            OleDbConnection conn;
            if (tran != null)
            {
                conn = tran.Connection;
            }
            else
            {
                conn = new OleDbConnection(this.ConnectionString);
                conn.Open();
            }

            try
            {
                bool existed = false;
                var query = new QueryContext(null, "SELECT TOP 1 1 FROM [Receipts] WHERE [KindOfGoodsUId] = ?") { Connection = conn, Transaction = tran };
                query.Parameters.AddWithValue(uid);
                query.ExecuteReader(r => existed = r[0].ToBoolean());
                return existed;
            }
            finally
            {
                if (tran == null)
                {
                    conn.Dispose();
                }
            }
        }

        private void ValidateRequiredFields(Receipt receipt)
        {
            if (String.IsNullOrEmpty(receipt.No) || receipt.No.Trim().Length < 1)
            {
                throw new ArgumentException("Receipt No. cannot be empty");
            }
        }

        private void ValidateFields(IEnumerable<ReceiptTankDetail> receiptTankDetails)
        {
            receiptTankDetails.Each(d => this.ValidateFields(d));
        }

        private void ValidateFields(ReceiptTankDetail receiptTankDetail)
        {
            if (String.IsNullOrEmpty(receiptTankDetail.TankName) || receiptTankDetail.TankName.Trim().Length < 1)
            {
                throw new ArgumentException("Receipt tank detail: TankName cannot be empty");
            }
        }

        private void ValidateFields(KindOfGoods kindOfGoods)
        {
            if (String.IsNullOrEmpty(kindOfGoods.UId) || kindOfGoods.UId.Trim().Length < 1 || String.IsNullOrEmpty(kindOfGoods.Name) || kindOfGoods.Name.Trim().Length < 1)
            {
                throw new ArgumentException("Kind name/id cannot be empty");
            }
            if (Const.BuiltInKindsOfGoods.Any(k => kindOfGoods.UId.Equals(k.UId)))
            {
                throw new ArgumentException(String.Format("Built-in kind \"{0}\" cannot be saved", Const.BuiltInKindsOfGoods.First(k => k.UId.Equals(kindOfGoods.UId)).Name));
            }
            if (kindOfGoods.BuiltIn)
            {
                throw new ArgumentException("Built-in kind cannot be saved");
            }
            if (!kindOfGoods.Customized)
            {
                throw new ArgumentException("Only customized kinds can be saved");
            }
        }

        private Receipt ReadReceipt(OleDbDataReader reader)
        {
            var readKindOfGoods = new Func<KindOfGoods>(() =>
            {
                var uid = reader["KindOfGoodsUId"].ToStringOrNull();
                if (uid == null)
                {
                    return null;
                }

                var kind = new KindOfGoods { UId = uid, Name = reader["KindOfGoodsName"].ToStringOrNull(), Customized = true };
                if (String.IsNullOrEmpty(kind.Name))
                {
                    var builtInKind = Const.BuiltInKindsOfGoods.FirstOrDefaultRecursively(k => k.UId.Equals(kind.UId));
                    if (builtInKind != null)
                    {
                        kind = builtInKind;
                    }
                }

                return kind;
            });

            return new Receipt
            {
                No = reader["No"].ToStringOrNull(),
                VesselName = reader["VesselName"].ToStringOrNull(),
                Time = reader["Time"].TryToNullableDateTime(),
                ReceiptFor = reader["ReceiptFor"].ToStringOrNull(),
                PortOfShipment = reader["PortOfShipment"].ToStringOrNull(),
                PortOfDestination = reader["PortOfDestination"].ToStringOrNull(),
                KindOfGoods = readKindOfGoods(),
                VesselStatus = VesselStatus.Parse(reader["VesselStatus"].ToStringOrNull()),
                TotalOfVolumeOfStandard = reader["TotalOfVolumeOfStandard"].TryToNullableDecimal(),
                TotalOfVolume = reader["TotalOfVolume"].TryToNullableDecimal(),
                TotalOfVolumeOfWater = reader["TotalOfVolumeOfWater"].TryToNullableDecimal(),
                TotalOfMass = reader["TotalOfMass"].TryToNullableDecimal(),
                TotalOfVolumeOfPipes = reader["TotalOfVolumeOfPipes"].TryToNullableDecimal(),
                OperaterName = reader["OperaterName"].ToStringOrNull(),
                AgentName = reader["AgentName"].ToStringOrNull(),
                ShipperName = reader["ShipperName"].ToStringOrNull(),
                ConsignerName = reader["ConsignerName"].ToStringOrNull(),
                ConsigneeName = reader["ConsigneeName"].ToStringOrNull(),
                TimeSaved = reader["TimeSaved"].TryToDateTime(),
                ReceiptType = reader["ReceiptType"].ToReceiptType()
            };
        }
    }
}
