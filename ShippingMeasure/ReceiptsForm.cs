using ShippingMeasure.Common;
using ShippingMeasure.Core;
using ShippingMeasure.Core.Models;
using ShippingMeasure.Db;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShippingMeasure
{
    public partial class ReceiptsForm : Form
    {
        public enum For
        {
            Search,
            Load,
            Save
        }

        public For FormFor { get; private set; }
        public Receipt Receipt { get; private set; }
        public ReceiptDb ReceiptDb { get; private set; }
        public ReceiptType ReceiptType { get; private set; }

        public ReceiptsForm(For formFor, ReceiptDb receiptDb, ReceiptType type)
        {
            InitializeComponent();

            this.FormFor = formFor;
            this.ReceiptDb = receiptDb;
            this.ReceiptType = type;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.btnSearch.Click += (s, a) => this.Search();
            this.btnOk.Click += (s, a) => this.Ok();
            this.txtKeyword.KeyUp += (s, a) =>
            {
                if (!a.Alt && !a.Control && !a.Shift && a.KeyCode == Keys.Enter)
                {
                    this.btnSearch.Focus();
                    this.btnSearch.PerformClick();
                    a.Handled = true;
                }
            };
            this.list.SelectedIndexChanged += (s, a) => this.btnOk.Enabled = this.list.SelectedItems.Count > 0;
            this.list.KeyUp += (s, a) =>
            {
                if (this.FormFor == For.Load || this.FormFor == For.Save)
                {
                    if (!a.Alt && !a.Control && !a.Shift && a.KeyCode == Keys.Enter)
                    {
                        if (this.list.SelectedItems.Count > 0)
                        {
                            this.btnOk.Focus();
                            this.btnOk.PerformClick();
                            a.Handled = true;
                        }
                    }
                }
            };
            this.list.DoubleClick += (s, a) =>
            {
                if (this.FormFor == For.Load || this.FormFor == For.Save)
                {
                    if (this.list.SelectedItems.Count > 0)
                    {
                        this.btnOk.Focus();
                        this.btnOk.PerformClick();
                    }
                }
            };

            this.InitForm();
            this.InitAutoComplete();
            this.LoadTop10();
        }

        private void LoadTop10()
        {
            this.ShowList(this.ReceiptDb.GetAll()
                .Where(r => r.ReceiptType == this.ReceiptType || r.ReceiptType == ReceiptType.MassOfOil)
                .Take(10)
                .OrderByDescending(r => r.TimeSaved));
        }

        private void LoadReceipt()
        {
            if (this.list.SelectedItems.Count < 1)
            {
                MessageBox.Show(this, Strings.MsgErrorSelectReceiptToLoad, Strings.MsgError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.Receipt = this.ReceiptDb.Get(((Receipt)this.list.SelectedItems[0].Tag).No);
            this.DialogResult = DialogResult.OK;
        }

        private void SaveReceipt()
        {
            // todo: ... i am changing the logic that i will save receipt directly from the form, update the save status in form status bar
        }

        private void Ok()
        {
            switch (this.FormFor)
            {
                case For.Load:
                    this.LoadReceipt();
                    break;
                case For.Save:
                    this.SaveReceipt();
                    break;
                default:
                    break;
            }
        }

        private void InitAutoComplete()
        {
            this.ReceiptDb.GetAll()
                .Where(r => r.ReceiptType == this.ReceiptType || r.ReceiptType == ReceiptType.MassOfOil).Each(r =>
                {
                    this.txtKeyword.AutoCompleteCustomSource.Add(r.No);
                    if (!String.IsNullOrEmpty(r.PortOfShipment))
                    {
                        this.txtKeyword.AutoCompleteCustomSource.Add(r.PortOfShipment);
                    }
                    if (!String.IsNullOrEmpty(r.PortOfDestination))
                    {
                        this.txtKeyword.AutoCompleteCustomSource.Add(r.PortOfDestination);
                    }
                    if (r.KindOfGoods != null)
                    {
                        this.txtKeyword.AutoCompleteCustomSource.Add(r.KindOfGoods.Name);
                    }
                    if (!String.IsNullOrEmpty(r.OperaterName))
                    {
                        this.txtKeyword.AutoCompleteCustomSource.Add(r.OperaterName);
                    }
                    if (!String.IsNullOrEmpty(r.ConsigneeName))
                    {
                        this.txtKeyword.AutoCompleteCustomSource.Add(r.ConsigneeName);
                    }
                });

            this.txtKeyword.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        private void Search()
        {
            var keyword = txtKeyword.Text;
            this.list.Items.Clear();

            this.ShowList(this.ReceiptDb.GetAll()
                .Where(r => r.ReceiptType == this.ReceiptType || r.ReceiptType == ReceiptType.MassOfOil)
                .Where(r =>
                    r.No.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
                    || r.PortOfShipment != null && r.PortOfShipment.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
                    || r.PortOfDestination != null && r.PortOfDestination.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
                    || (r.KindOfGoods != null && r.KindOfGoods.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    || r.OperaterName != null && r.OperaterName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
                    || r.ConsigneeName != null && r.ConsigneeName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
                    )
                .OrderByDescending(r => r.TimeSaved));

            this.txtKeyword.Focus();
        }

        private void ShowList(IEnumerable<Receipt> receipts)
        {
            receipts.Each(r =>
            {
                var item = new ListViewItem(
                    new[] { r.No, r.Time.Value.ToString("yyyy-MM-dd"), r.ReceiptFor, r.PortOfShipment, r.PortOfDestination, r.TimeSaved.ToString("yyyy-MM-dd") }, "Receipt.ico"
                    )
                { Tag = r };
                this.list.Items.Add(item);
            });
        }

        private void InitForm()
        {
            switch (this.FormFor)
            {
                case For.Load:
                    this.btnOk.Text = Strings.FormTitleLoad;
                    break;
                case For.Save:
                    this.btnOk.Text = Strings.FormTitleSave;
                    break;
                default:
                    this.btnOk.Visible = false;
                    this.btnCancel.Text = Strings.ButtonClose;
                    break;
            }
        }
    }
}
