using ShippingMeasure.Common;
using ShippingMeasure.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Db
{
    public class TankDb : DbBase
    {
        private List<Tank> allTanks = null;
        [Obsolete]
        private List<OilVolume> allOilVolumeItems = null;
        private List<ListingHeightCorrection> allListingHeightCorrectionItems = null;
        private List<TrimmingHeightCorrection> allTrimmingHeightCorrectionItems = null;
        private List<Volume> allVolumeItems = null;
        private Vessel vessel = null;

        public IEnumerable<Tank> GetAllTanks()
        {
            if (this.allTanks == null)
            {
                var items = new List<Tank>();

                new QueryContext(this.ConnectionString, "SELECT * FROM Tanks").ExecuteReader(r => items.Add(new Tank
                {
                    Name = r["Name"].ToString(),
                    Height = r["Height"].TryToDecimal()
                }));

                this.allTanks = items;
            }

            return this.allTanks;
        }

        [Obsolete]
        public IEnumerable<OilVolume> GetAllOilVolumeItems()
        {
            if (this.allOilVolumeItems == null)
            {
                var items = new List<OilVolume>();

                new QueryContext(this.ConnectionString, "SELECT * FROM OilVolume").ExecuteReader(r => items.Add(new OilVolume
                {
                    TankName = r["TankName"].ToString(),
                    HInclination = r["HInclination"].TryToDecimal(),
                    VInclination = r["VInclination"].TryToDecimal(),
                    Height = r["Height"].TryToDecimal(),
                    Volume = r["Volume"].TryToDecimal()
                }));

                this.allOilVolumeItems = items;
            }

            return this.allOilVolumeItems;
        }

        public IEnumerable<ListingHeightCorrection> GetAllListingHeightCorrectionItems()
        {
            if (this.allListingHeightCorrectionItems == null)
            {
                var items = new List<ListingHeightCorrection>();

                new QueryContext(this.ConnectionString, "SELECT * FROM ListingHeightCorrection").ExecuteReader(r => items.Add(new ListingHeightCorrection
                {
                    TankName = r["TankName"].ToString(),
                    Height = r["Height"].TryToDecimal(),
                    HInclination = r["HInclination"].TryToDecimal(),
                    Correction = r["Correction"].TryToDecimal(),
                }));

                this.allListingHeightCorrectionItems = items;
            }

            return this.allListingHeightCorrectionItems;
        }

        public IEnumerable<TrimmingHeightCorrection> GetAllTrimmingHeightCorrectionItems()
        {
            if (this.allTrimmingHeightCorrectionItems == null)
            {
                var items = new List<TrimmingHeightCorrection>();

                new QueryContext(this.ConnectionString, "SELECT * FROM TrimmingHeightCorrection").ExecuteReader(r => items.Add(new TrimmingHeightCorrection
                {
                    TankName = r["TankName"].ToString(),
                    Height = r["Height"].TryToDecimal(),
                    VInclination = r["VInclination"].TryToDecimal(),
                    Correction = r["Correction"].TryToDecimal(),
                }));

                this.allTrimmingHeightCorrectionItems = items;
            }

            return this.allTrimmingHeightCorrectionItems;
        }

        public IEnumerable<Volume> GetAllVolumeItems()
        {
            if (this.allVolumeItems == null)
            {
                var items = new List<Volume>();

                new QueryContext(this.ConnectionString, "SELECT * FROM Volumes").ExecuteReader(r => items.Add(new Volume
                {
                    TankName = r["TankName"].ToString(),
                    Height = r["Height"].TryToDecimal(),
                    Value = r["Volume"].TryToDecimal(),
                }));

                this.allVolumeItems = items;
            }

            return this.allVolumeItems;
        }

        public void Add(IEnumerable<Tank> tanks)
        {
            using (var conn = new OleDbConnection(this.ConnectionString))
            {
                var query = new QueryContext(this.ConnectionString, "INSERT INTO [Tanks] ([Name], [Height]) VALUES (?, ?)") { Connection = conn };

                tanks.Each(t =>
                {
                    query.Parameters.Clear();
                    query.Parameters.AddWithValue(t.Name);
                    query.Parameters.AddWithValue(t.Height);
                    query.ExecuteNonQuery();
                });
            }

            if (this.allTanks != null)
            {
                this.allTanks.AddRange(tanks);
            }
        }

        [Obsolete]
        public void Add(IEnumerable<OilVolume> oilVolumeItems)
        {
            using (var conn = new OleDbConnection(this.ConnectionString))
            {
                var query = new QueryContext(this.ConnectionString, "INSERT INTO OilVolume (TankName, HInclination, VInclination, Height, Volume) VALUES (?, ?, ?, ?, ?)") { Connection = conn };

                oilVolumeItems.Each(v =>
                {
                    query.Parameters.Clear();
                    query.Parameters.AddWithValue(v.TankName);
                    query.Parameters.AddWithValue(v.HInclination);
                    query.Parameters.AddWithValue(v.VInclination);
                    query.Parameters.AddWithValue(v.Height);
                    query.Parameters.AddWithValue(v.Volume);
                    query.ExecuteNonQuery();
                });
            }

            if (this.allOilVolumeItems != null)
            {
                this.allOilVolumeItems.AddRange(oilVolumeItems);
            }
        }

        public void Add(IEnumerable<TrimmingHeightCorrection> trimmingHeightCorrectionItems)
        {
            using (var conn = new OleDbConnection(this.ConnectionString))
            {
                var query = new QueryContext(this.ConnectionString, "INSERT INTO TrimmingHeightCorrection (TankName, Height, VInclination, Correction) VALUES (?, ?, ?, ?)") { Connection = conn };

                trimmingHeightCorrectionItems.Each(c =>
                {
                    query.Parameters.Clear();
                    query.Parameters.AddWithValue(c.TankName);
                    query.Parameters.AddWithValue(c.Height);
                    query.Parameters.AddWithValue(c.VInclination);
                    query.Parameters.AddWithValue(c.Correction);
                    query.ExecuteNonQuery();
                });
            }

            if (this.allTrimmingHeightCorrectionItems != null)
            {
                this.allTrimmingHeightCorrectionItems.AddRange(trimmingHeightCorrectionItems);
            }
        }

        public void Add(IEnumerable<ListingHeightCorrection> listingHeightCorrectionItems)
        {
            using (var conn = new OleDbConnection(this.ConnectionString))
            {
                var query = new QueryContext(this.ConnectionString, "INSERT INTO ListingHeightCorrection (TankName, Height, HInclination, Correction) VALUES (?, ?, ?, ?)") { Connection = conn };

                listingHeightCorrectionItems.Each(c =>
                {
                    query.Parameters.Clear();
                    query.Parameters.AddWithValue(c.TankName);
                    query.Parameters.AddWithValue(c.Height);
                    query.Parameters.AddWithValue(c.HInclination);
                    query.Parameters.AddWithValue(c.Correction);
                    query.ExecuteNonQuery();
                });
            }

            if (this.allListingHeightCorrectionItems != null)
            {
                this.allListingHeightCorrectionItems.AddRange(listingHeightCorrectionItems);
            }
        }

        public void Add(IEnumerable<Volume> volumeItems)
        {
            using (var conn = new OleDbConnection(this.ConnectionString))
            {
                var query = new QueryContext(this.ConnectionString, "INSERT INTO Volume (TankName, Height, Volume) VALUES (?, ?, ?)") { Connection = conn };

                volumeItems.Each(v =>
                {
                    query.Parameters.Clear();
                    query.Parameters.AddWithValue(v.TankName);
                    query.Parameters.AddWithValue(v.Height);
                    query.Parameters.AddWithValue(v.Value);
                    query.ExecuteNonQuery();
                });
            }

            if (this.allVolumeItems != null)
            {
                this.allVolumeItems.AddRange(volumeItems);
            }
        }

        public void ClearTanks()
        {
            new QueryContext(this.ConnectionString, "DELETE * FROM Tanks").ExecuteNonQuery();

            if (this.allTanks != null)
            {
                this.allTanks.Clear();
            }
        }

        [Obsolete]
        public void ClearOilVolumeItems()
        {
            new QueryContext(this.ConnectionString, "DELETE * FROM OilVolume").ExecuteNonQuery();

            if (this.allOilVolumeItems != null)
            {
                this.allOilVolumeItems.Clear();
            }
        }

        public void ClearTrimmingHeightCorrectionItems()
        {
            new QueryContext(this.ConnectionString, "DELETE * FROM TrimmingHeightCorrection").ExecuteNonQuery();

            if (this.allTrimmingHeightCorrectionItems != null)
            {
                this.allTrimmingHeightCorrectionItems.Clear();
            }
        }

        public void ClearListingHeightCorrectionItems()
        {
            new QueryContext(this.ConnectionString, "DELETE * FROM ListingHeightCorrection").ExecuteNonQuery();

            if (this.allListingHeightCorrectionItems != null)
            {
                this.allListingHeightCorrectionItems.Clear();
            }
        }

        public void ClearVolumeItems()
        {
            new QueryContext(this.ConnectionString, "DELETE * FROM Volumes").ExecuteNonQuery();

            if (this.allVolumeItems != null)
            {
                this.allVolumeItems.Clear();
            }
        }

        public void SaveVessel(Vessel vessel)
        {
            int count = 0;
            new QueryContext(this.ConnectionString, "SELECT COUNT(*) FROM Vessel").ExecuteReader(r => count = r[0].ToInt32());

            QueryContext query;
            if (count > 0)
            {
                query = new QueryContext(this.ConnectionString, "UPDATE Vessel SET Name = ?, CertNo = ?");
                query.Parameters.AddWithValue(vessel.Name);
                query.Parameters.AddWithValue(vessel.CertNo);
            }
            else
            {
                query = new QueryContext(this.ConnectionString, "INSERT INTO Vessel (Name, CertNo) VALUES (?, ?)");
                query.Parameters.AddWithValue(vessel.Name);
                query.Parameters.AddWithValue(vessel.CertNo);
            }

            query.ExecuteNonQuery();
            this.vessel = vessel;
        }

        public Vessel GetVessel()
        {
            if (this.vessel == null)
            {
                Vessel vessel = null;

                using (var conn = new OleDbConnection(this.ConnectionString))
                {
                    var read = false;
                    new QueryContext(null, "SELECT * FROM Vessel") { Connection = conn }.ExecuteReader(r =>
                    {
                        if (read) // only read the first record
                        {
                            return;
                        }

                        vessel = new Vessel();
                        vessel.Name = r["Name"].ToString();
                        vessel.CertNo = r["CertNo"].ToString();
                    });

                    new QueryContext(null, "SELECT * FROM Pipes") { Connection = conn }.ExecuteReader(r =>
                    {
                        vessel.Pipes.Add(new Pipe { Name = r["Name"].ToString(), Volume = r["Volume"].TryToDecimal() });
                    });
                }

                this.vessel = vessel;
            }

            return this.vessel;
        }

        public void SavePipes(IEnumerable<Pipe> pipes)
        {
            using (var conn = new OleDbConnection(this.ConnectionString))
            {
                new QueryContext(null, "DELETE * FROM Pipes") { Connection = conn }.ExecuteNonQuery();

                var query = new QueryContext(null, "INSERT INTO Pipes (Name, Volume) VALUES (?, ?)") { Connection = conn };

                pipes.Each(p =>
                {
                    query.Parameters.Clear();
                    query.Parameters.AddWithValue(p.Name);
                    query.Parameters.AddWithValue(p.Volume);
                    query.ExecuteNonQuery();
                });
            }
        }
    }
}
