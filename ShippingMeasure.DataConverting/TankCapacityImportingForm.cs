using ShippingMeasure.Db;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ShippingMeasure.Common;

namespace ShippingMeasure.DataConverting
{
    public partial class TankCapacityImportingForm : Form
    {
        private BackgroundWorker worker = new BackgroundWorker();
        private Stopwatch stopwatch = new Stopwatch();

        public TankCapacityImportingForm()
        {
            InitializeComponent();

            this.menuAddRawDataFiles.Click += (sender, e) => this.AddRawDataFiles();
            this.menuSelectAccessFile.Click += (sender, e) => this.SelectAccessFile();
            this.listTxtFiles.SelectedIndexChanged += (sender, e) => this.menuRemoveSelected.Enabled = this.listTxtFiles.SelectedIndex >= 0;
            this.menuRemoveSelected.Click += (sender, e) => this.RemoveSelected();
            this.menuImport.Click += (sender, e) => this.StartImport();
            this.chkWrapOutput.CheckedChanged += (sender, e) => this.txtOutput.WordWrap = chkWrapOutput.Checked;

            this.worker.DoWork += (sender, e) =>
            {
                this.stopwatch.Start();
                this.ResetOutput();
                this.WriteOutput(TraceLevel.Info, String.Format("Start importing ({0})", DateTime.Now.ToString("HH:mm:ss")));

                // var importer = new TankCapacityImporter { AccessFile = this.txtAccessFile.Text };
                var importer = new DataImporter { AccessFile = this.txtAccessFile.Text };
                importer.WriteLog += (l, m) => this.WriteOutput(l, m);
                this.listTxtFiles.Items.OfType<string>().Each(f => importer.RawDataFiles.Add(f));
                importer.Import();
            };
            this.worker.RunWorkerCompleted += (sender, e) =>
            {
                this.menuAddRawDataFiles.Enabled = true;
                this.menuSelectAccessFile.Enabled = true;
                this.menuImport.Enabled = true;
                this.listTxtFiles.Enabled = true;

                if (e.Error == null)
                {
                    this.WriteOutput(TraceLevel.Off, "Importing finished successfully)");
                }
                else
                {
                    this.WriteOutput(TraceLevel.Error, String.Format("Importing finished with error: {0}", e.Error.Message));
                }

                this.WriteOutput(TraceLevel.Info, String.Format("Time elapsed: {0}", this.stopwatch.Elapsed.ToString()));
                this.stopwatch.Reset();
            };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.worker.IsBusy)
            {
                e.Cancel = true;
                return;
            }

            this.worker.Dispose();
            base.OnClosing(e);
        }

        private void StartImport()
        {
            this.menuAddRawDataFiles.Enabled = false;
            this.menuSelectAccessFile.Enabled = false;
            this.menuImport.Enabled = false;
            this.listTxtFiles.Enabled = false;

            this.listTxtFiles.SelectedIndex = -1;

            this.worker.RunWorkerAsync();
        }

        private void AddRawDataFiles()
        {
            using (var dialog = new OpenFileDialog { Filter = "Tank capacity raw data file (*.txt)|*.txt", Multiselect = true, Title = "Select tank capacity raw data file(s)" })
            {
                var result = dialog.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    dialog.FileNames.Each(f =>
                    {
                        if (!this.listTxtFiles.Items.Contains(f))
                        {
                            this.listTxtFiles.Items.Add(f);
                        }
                    });
                }
            }
        }

        private void SelectAccessFile()
        {
            using (var dialog = new OpenFileDialog { Filter = "Access file (*.mdb)|*.mdb", Title = "Select Access file" })
            {
                var result = dialog.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    this.txtAccessFile.Text = dialog.FileName;
                }
            }
        }

        private void RemoveSelected()
        {
            this.listTxtFiles.BeginUpdate();
            while (this.listTxtFiles.SelectedIndex >= 0)
            {
                this.listTxtFiles.Items.RemoveAt(this.listTxtFiles.SelectedIndex);
            }
            this.listTxtFiles.EndUpdate();
        }

        private void WriteOutput(TraceLevel level, string message)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("[{0} - {1}] ", DateTime.Now.ToString("HH:mm:ss"), level);
            sb.AppendLine(message);

            Color color;
            switch (level)
            {
                case TraceLevel.Error:
                    color = Color.Red;
                    break;
                case TraceLevel.Warning:
                    color = Color.Orange;
                    break;
                case TraceLevel.Off:
                    color = Color.Green;
                    break;
                default:
                    color = SystemColors.ControlText;
                    break;
            }

            //this.Invoke(new Action(() => this.txtOutput.Text = sb.ToString() + this.txtOutput.Text));
            this.Invoke(new Action(() =>
            {
                this.txtOutput.SelectionStart = txtOutput.TextLength;
                this.txtOutput.SelectionColor = color;
                this.txtOutput.AppendText(sb.ToString());
                this.txtOutput.ScrollToCaret();
            }));
        }

        private void ResetOutput()
        {
            this.Invoke(new Action(() => this.txtOutput.Text = String.Empty));
        }
    }
}
