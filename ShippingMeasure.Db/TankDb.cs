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
        private List<OilVolume> allOilVolumeItems = null;
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

        public void ClearTanks()
        {
            new QueryContext(this.ConnectionString, "DELETE * FROM Tanks").ExecuteNonQuery();

            if (this.allTanks != null)
            {
                this.allTanks.Clear();
            }
        }

        public void ClearOilVolumeItems()
        {
            new QueryContext(this.ConnectionString, "DELETE * FROM OilVolume").ExecuteNonQuery();

            if (this.allOilVolumeItems != null)
            {
                this.allOilVolumeItems.Clear();
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
