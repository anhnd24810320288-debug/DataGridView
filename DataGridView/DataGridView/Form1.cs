using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DataGridView
{
    public partial class Form1 : Form
    {
        private readonly List<Product> products = new();
        private readonly BindingSource bsProducts = new BindingSource();

        public Form1()
        {
            InitializeComponent();

            // Setup binding
            bsProducts.DataSource = products;
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.DataSource = bsProducts;

            // Add some sample columns mapped to Product properties
            // (Designer also creates these columns; keep this in case designer is regenerated)

            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.MultiSelect = false;

            // Wire events
            dgvProducts.CellClick += DgvProducts_CellClick;
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSearch.Click += BtnSearch_Click;
            txtSearch.KeyDown += TxtSearch_KeyDown;
        }

        private void TxtSearch_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnSearch_Click(this, EventArgs.Empty);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (!TryCreateProductFromInputs(out var p))
                return;

            products.Add(p);
            bsProducts.ResetBindings(false);
            ClearInputs();
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (bsProducts.Current is not Product current)
            {
                MessageBox.Show("Please select a product to edit.", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!TryCreateProductFromInputs(out var updated))
                return;

            // Update properties
            current.ProductId = updated.ProductId;
            current.ProductName = updated.ProductName;
            current.UnitPrice = updated.UnitPrice;
            current.Quantity = updated.Quantity;
            current.Category = updated.Category;

            bsProducts.ResetBindings(false);
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (bsProducts.Current is not Product current)
            {
                MessageBox.Show("Please select a product to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var res = MessageBox.Show($"Delete product '{current.ProductName}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                products.Remove(current);
                bsProducts.ResetBindings(false);
                ClearInputs();
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            var term = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(term))
            {
                bsProducts.DataSource = products;
            }
            else
            {
                var filtered = products.Where(p => p.ProductName != null && p.ProductName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                bsProducts.DataSource = filtered;
            }

            bsProducts.ResetBindings(false);
        }

        private void DgvProducts_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgvProducts.Rows[e.RowIndex].DataBoundItem is Product p)
            {
                PopulateInputs(p);
            }
        }

        private bool TryCreateProductFromInputs(out Product product)
        {
            product = new Product();

            product.ProductId = txtProductId.Text.Trim();
            product.ProductName = txtProductName.Text.Trim();

            if (!decimal.TryParse(txtUnitPrice.Text.Trim(), out var price))
            {
                MessageBox.Show("Invalid unit price.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out var qty))
            {
                MessageBox.Show("Invalid quantity.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            product.UnitPrice = price;
            product.Quantity = qty;
            product.Category = txtCategory.Text.Trim();

            return true;
        }

        private void PopulateInputs(Product p)
        {
            txtProductId.Text = p.ProductId;
            txtProductName.Text = p.ProductName;
            txtUnitPrice.Text = p.UnitPrice.ToString();
            txtQuantity.Text = p.Quantity.ToString();
            txtCategory.Text = p.Category;
        }

        private void ClearInputs()
        {
            txtProductId.Clear();
            txtProductName.Clear();
            txtUnitPrice.Clear();
            txtQuantity.Clear();
            txtCategory.Clear();
        }
    }
}
