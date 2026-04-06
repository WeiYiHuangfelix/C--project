using ShoppingCart.Controllers;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace ShoppingCart
{
    public partial class MainForm : Form
    {
        private readonly ProductController _productController;
        private readonly CartController _cartController;

        public MainForm()
        {
            InitializeComponent();
            _productController = new ProductController();
            _cartController = new CartController();
        }

        #region 🌟 1. 系統初始化與 UI 設定
        private void Form1_Load(object sender, EventArgs e)
        {
            AppData.InitializeData();
            SetupCartGrid();

            cmbSort.Items.AddRange(new string[] { "按編號排序", "按價格排序 (從低到高)", "按庫存排序 (從少到多)" });
            cmbSort.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSort.SelectedIndex = 0;

            // 🔓 關鍵修改：解開整個表格的唯讀限制，讓 CheckBox 可以被點擊！
            // (不用擔心，我們在後面會把其他資料欄位單獨鎖起來)
            dgvProducts.ReadOnly = false;
            dgvCart.ReadOnly = false;

            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            numPrice.Maximum = 1000000m;
            numPrice.DecimalPlaces = 2;
            numStock.Maximum = 10000m;

            // ⚡ 綁定標題列點擊事件 (用來觸發「全選」)
            dgvProducts.ColumnHeaderMouseClick += DgvProducts_ColumnHeaderMouseClick;
            dgvCart.ColumnHeaderMouseClick += DgvCart_ColumnHeaderMouseClick;

            RefreshProductGrid();
            RefreshCartGrid();
        }

        private void SetupCartGrid()
        {
            dgvCart.AutoGenerateColumns = false;
            dgvCart.Columns.Clear();

            // 🌟 追加：CheckBox 選擇欄位 (放在第 0 欄，且設定 ReadOnly = false)
            dgvCart.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Select", HeaderText = "全選", Width = 50, ReadOnly = false });

            // 原本的欄位 (加上 ReadOnly = true，保護資料不被亂改)
            dgvCart.Columns.Add(new DataGridViewImageColumn { Name = "ProductImage", HeaderText = "圖片預覽", ImageLayout = DataGridViewImageCellLayout.Zoom, Width = 60, ReadOnly = true });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName", HeaderText = "商品名稱", Name = "ProductName", Width = 150, ReadOnly = true });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "單價", Name = "UnitPrice", Width = 80, ReadOnly = true });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "數量", Name = "Quantity", Width = 80, ReadOnly = true });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SubTotal", HeaderText = "小計", Name = "SubTotal", Width = 100, ReadOnly = true });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductId", HeaderText = "ID", Name = "ProductId", Visible = false, ReadOnly = true });

            dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCart.RowHeadersVisible = false;
            dgvCart.AllowUserToAddRows = false;
            dgvCart.RowTemplate.Height = 60;
        }

        private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedIndex == 0) RefreshProductGrid();
            else if (tabControlMain.SelectedIndex == 1)
            {
                RefreshCartGrid();
                if (dgvProducts.CurrentRow?.DataBoundItem is Product product && lblSelectedInfo != null)
                {
                    lblSelectedInfo.Text = $"目前選取：{product.Name} (庫存: {product.Stock})";
                }
            }
        }
        #endregion

        #region 🌟 2. 畫面共用更新邏輯 (終極脫鉤法)
        private void BindProductData(List<Product> data)
        {
            string selectedId = (dgvProducts.CurrentRow?.DataBoundItem as Product)?.Id;
            dgvProducts.SelectionChanged -= dgvProducts_SelectionChanged;

            var safeList = new List<Product>(data);

            dgvProducts.CurrentCell = null;
            dgvProducts.DataSource = null;
            dgvProducts.Rows.Clear();
            dgvProducts.DataSource = safeList;

            dgvProducts.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            foreach (DataGridViewRow row in dgvProducts.Rows) row.Height = 80;

            if (dgvProducts.Columns["ImagePath"] != null) dgvProducts.Columns["ImagePath"].Visible = false;

            // 🌟 追加：確保第一欄是 CheckBox
            if (dgvProducts.Columns["Select"] == null)
            {
                dgvProducts.Columns.Insert(0, new DataGridViewCheckBoxColumn { Name = "Select", HeaderText = "全選", Width = 50, ReadOnly = false });
            }

            // 🌟 確保第二欄是圖片預覽
            if (dgvProducts.Columns["ProductImage"] == null)
            {
                dgvProducts.Columns.Insert(1, new DataGridViewImageColumn { Name = "ProductImage", HeaderText = "圖片預覽", ImageLayout = DataGridViewImageCellLayout.Zoom, Width = 80, ReadOnly = true });
            }

            // 🛡️ 防呆：將除了 Select (CheckBox) 以外的所有資料欄位強制鎖上唯讀
            foreach (DataGridViewColumn col in dgvProducts.Columns)
            {
                if (col.Name != "Select") col.ReadOnly = true;
            }

            if (safeList.Count > 0)
            {
                int targetIndex = selectedId != null ? safeList.FindIndex(p => p.Id == selectedId) : 0;
                targetIndex = targetIndex == -1 ? 0 : targetIndex;

                if (targetIndex < dgvProducts.Rows.Count)
                {
                    // 因為第 0 欄變成 CheckBox 了，我們把焦點放在第 1 欄(圖片或資料)避免自動勾選
                    dgvProducts.CurrentCell = dgvProducts.Rows[targetIndex].Cells[1];
                    dgvProducts.Rows[targetIndex].Selected = true;
                }
            }

            dgvProducts.SelectionChanged += dgvProducts_SelectionChanged;
            if (dgvProducts.Rows.Count > 0) dgvProducts_SelectionChanged(this, EventArgs.Empty);
        }

        private void RefreshProductGrid() => BindProductData(_productController.GetAllProducts());

        private void RefreshCartGrid()
        {
            dgvCart.SelectionChanged -= dgvCart_SelectionChanged;

            var safeList = new List<CartItem>(_cartController.GetCartItems());

            dgvCart.CurrentCell = null;
            dgvCart.DataSource = null;
            dgvCart.Rows.Clear();
            dgvCart.DataSource = safeList;

            foreach (DataGridViewRow row in dgvCart.Rows) row.Height = 60;

            var summary = _cartController.GetCheckoutSummary();
            if (lblItemCount != null) lblItemCount.Text = $"品項數：{summary.TotalItems}";
            if (lblTotalQty != null) lblTotalQty.Text = $"總數量：{summary.TotalQuantity}";
            if (lblTotalAmount != null) lblTotalAmount.Text = $"總金額：NT$ {summary.TotalAmount}";

            dgvCart.SelectionChanged += dgvCart_SelectionChanged;
            if (dgvCart.Rows.Count > 0) dgvCart_SelectionChanged(this, EventArgs.Empty);
        }
        #endregion

        #region 🌟 6. 全選/取消全選 CheckBox 處理邏輯

        // 點擊商品列表標題列時
        private void DgvProducts_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex >= 0 && dgvProducts.Columns[e.ColumnIndex].Name == "Select")
            {
                ToggleSelectAll(dgvProducts);
            }
        }

        // 點擊購物車標題列時
        private void DgvCart_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex >= 0 && dgvCart.Columns[e.ColumnIndex].Name == "Select")
            {
                ToggleSelectAll(dgvCart);
            }
        }

        // 共用的全選/反選邏輯核心
        private void ToggleSelectAll(DataGridView dgv)
        {
            if (dgv.Rows.Count == 0) return;

            dgv.EndEdit(); // 結束目前使用者的編輯狀態，確保資料寫入

            // 1. 先檢查是不是「已經全部打勾」了
            bool allChecked = true;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (Convert.ToBoolean(row.Cells["Select"].Value) == false)
                {
                    allChecked = false;
                    break;
                }
            }

            // 2. 如果全勾了就「全部取消」，反之則「全部打勾」
            bool targetState = !allChecked;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                row.Cells["Select"].Value = targetState;
            }

            dgv.EndEdit(); // 套用所有變更
        }
        #endregion

        #region 🌟 3. 商品維護操作
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var newProduct = new Product(txtProductId.Text, txtProductName.Text, decimal.Parse(numPrice.Text), (int)numStock.Value, txtImagePath.Text);
                var (success, message) = _productController.AddProduct(newProduct);

                MessageBox.Show(message, success ? "成功" : "錯誤", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                if (success) RefreshProductGrid();
            }
            catch { MessageBox.Show("請確認輸入格式正確！", "格式錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductId.Text)) { MessageBox.Show("請先從下方列表中點選要修改的商品！", "提示"); return; }
            try
            {
                var updatedProduct = new Product(txtProductId.Text, txtProductName.Text, decimal.Parse(numPrice.Text), (int)numStock.Value, txtImagePath.Text);
                var (success, message) = _productController.UpdateProduct(updatedProduct);

                MessageBox.Show(message, success ? "成功" : "錯誤", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                if (success) { RefreshProductGrid(); RefreshCartGrid(); }
            }
            catch { MessageBox.Show("請確認輸入格式正確！", "格式錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow?.DataBoundItem is Product product)
            {
                if (MessageBox.Show($"確定要刪除商品 [{product.Id}] 嗎？", "刪除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var (success, message) = _productController.DeleteProduct(product.Id);
                    MessageBox.Show(message, success ? "成功" : "刪除失敗", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                    if (success) { RefreshProductGrid(); RefreshCartGrid(); }
                }
            }
            else MessageBox.Show("請先選擇要刪除的商品！", "提示");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var searchResults = _productController.SearchProducts(txtSearch.Text.Trim());
            BindProductData(searchResults);
            if (searchResults.Count == 0) MessageBox.Show("找不到符合關鍵字的商品！", "搜尋結果");
        }

        private void cmbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSort.SelectedItem == null) return;
            string txt = cmbSort.SelectedItem.ToString();
            string sortKey = txt.Contains("價格") ? "price" : txt.Contains("庫存") ? "stock" : "id";

            BindProductData(_productController.SortProducts(sortKey, true));
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "圖片檔案|*.jpg;*.jpeg;*.png;*.bmp", Title = "請選擇商品圖片" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtImagePath.Text = ofd.FileName;
                    picProduct.ImageLocation = ofd.FileName;
                    picProduct.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow?.DataBoundItem is Product product)
            {
                txtProductId.Text = product.Id;
                txtProductName.Text = product.Name;
                numPrice.Value = product.Price;
                numStock.Value = product.Stock;
                txtImagePath.Text = product.ImagePath;
                picProduct.ImageLocation = (!string.IsNullOrEmpty(product.ImagePath) && File.Exists(product.ImagePath)) ? product.ImagePath : null;
            }
        }

        private void dgvProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvProducts.Rows.Count && dgvProducts.Columns[e.ColumnIndex].Name == "ProductImage")
            {
                if (dgvProducts.Rows[e.RowIndex].DataBoundItem is Product product && File.Exists(product.ImagePath))
                {
                    try { e.Value = Image.FromFile(product.ImagePath); } catch { e.Value = null; }
                }
            }
        }
        #endregion

        #region 🌟 4. 購物車與結帳操作
        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            btnAddToCart.Enabled = false;
            try
            {
                if (!(dgvProducts.CurrentRow?.DataBoundItem is Product product)) { MessageBox.Show("請先選擇商品！", "提示"); return; }

                int buyQty = (int)numBuyQty.Value;
                if (buyQty <= 0) { MessageBox.Show("購買數量必須大於 0！", "錯誤"); return; }
                if (buyQty > product.Stock) { MessageBox.Show($"庫存不足！目前僅剩：{product.Stock}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                var (success, message) = _cartController.AddToCart(product.Id, buyQty);
                MessageBox.Show(message, success ? "提示" : "錯誤", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                if (success) RefreshCartGrid();
            }
            catch (Exception ex) { MessageBox.Show($"系統錯誤：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { btnAddToCart.Enabled = true; }
        }

        private void btnUpdateCartQty_Click(object sender, EventArgs e)
        {
            if (dgvCart.CurrentRow?.DataBoundItem is CartItem cartItem)
            {
                var (success, message) = _cartController.UpdateCart(cartItem.ProductId, (int)numBuyQty.Value);
                MessageBox.Show(success ? "數量已更新！" : message, success ? "提示" : "修改失敗", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                if (success) RefreshCartGrid();
            }
            else MessageBox.Show("請先點選購物車中的商品！", "提示");
        }

        private void btnRemoveFromCart_Click(object sender, EventArgs e)
        {
            if (dgvCart.CurrentRow?.DataBoundItem is CartItem cartItem)
            {
                var (success, _) = _cartController.RemoveFromCart(cartItem.ProductId);
                if (success) { RefreshCartGrid(); MessageBox.Show("項目已移除！", "提示"); }
            }
            else MessageBox.Show("請先選擇要刪除的項目！", "提示");
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            if (_cartController.GetCartItems().Count == 0) { MessageBox.Show("購物車是空的！", "提示"); return; }

            var summary = _cartController.GetCheckoutSummary();
            if (MessageBox.Show($"共 {summary.TotalItems} 項，總金額 NT$ {summary.TotalAmount}\n確定結帳？", "結帳", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                var result = _cartController.Checkout();
                if (result.Success)
                {
                    MessageBox.Show(result.Message, "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshCartGrid();
                    RefreshProductGrid();
                }
                else
                {
                    MessageBox.Show(result.Message, "結帳失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void dgvCart_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCart.CurrentRow?.DataBoundItem is CartItem cartItem)
            {
                var product = _productController.GetAllProducts().Find(p => p.Id == cartItem.ProductId);
                int currentStock = product?.Stock ?? 0;

                if (lblSelectedInfo != null) lblSelectedInfo.Text = $"目前選取：{cartItem.ProductName} (庫存: {currentStock})";
                if (cartItem.Quantity <= numBuyQty.Maximum && cartItem.Quantity >= numBuyQty.Minimum) numBuyQty.Value = cartItem.Quantity;
            }
        }

        private void dgvCart_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvCart.Rows.Count && dgvCart.Columns[e.ColumnIndex].Name == "ProductImage")
            {
                if (dgvCart.Rows[e.RowIndex].DataBoundItem is CartItem item && File.Exists(item.ImagePath))
                {
                    try { e.Value = Image.FromFile(item.ImagePath); } catch { e.Value = null; }
                }
            }
        }
        #endregion

        #region 🖨️ 5. 報表與收據列印
        private void btnPrintList_Click(object sender, EventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += PrintDocument_PrintPage;
            var preview = new PrintPreviewDialog { Document = pd, Width = 800, Height = 600, Text = "商品清單預覽", WindowState = FormWindowState.Maximized };
            preview.ShowDialog();
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font tFont = new Font("微軟正黑體", 18, FontStyle.Bold), hFont = new Font("微軟正黑體", 12, FontStyle.Bold), cFont = new Font("微軟正黑體", 12);
            Pen line = new Pen(Color.Black, 2);
            int y = 80, x1 = 80, x2 = 230, x3 = 480, x4 = 630;

            g.DrawString("商店商品清單總表", tFont, Brushes.Black, x1, y); y += 50;
            g.DrawString("商品編號", hFont, Brushes.Black, x1, y); g.DrawString("商品名稱", hFont, Brushes.Black, x2, y); g.DrawString("單價", hFont, Brushes.Black, x3, y); g.DrawString("庫存", hFont, Brushes.Black, x4, y);
            y += 30; g.DrawLine(line, x1, y, x1 + 650, y); y += 15;

            var products = _productController.GetAllProducts();
            foreach (var p in products)
            {
                g.DrawString(p.Id, cFont, Brushes.Black, x1, y); g.DrawString(p.Name, cFont, Brushes.Black, x2, y); g.DrawString($"NT$ {p.Price}", cFont, Brushes.Black, x3, y); g.DrawString(p.Stock.ToString(), cFont, Brushes.Black, x4, y);
                y += 30;
            }
            y += 10; g.DrawLine(line, x1, y, x1 + 650, y); y += 10;
            g.DrawString($"★ 總計：共 {products.Count} 項商品", hFont, Brushes.Black, x1, y);
        }

        private void btnPrintReceipt_Click(object sender, EventArgs e)
        {
            if (_cartController.GetCartItems().Count == 0) { MessageBox.Show("購物車是空的！", "提示"); return; }
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += PrintReceipt_PrintPage;
            new PrintPreviewDialog { Document = pd, Width = 500, Height = 700, Text = "收據預覽" }.ShowDialog();
        }

        private void PrintReceipt_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font tFont = new Font("微軟正黑體", 16, FontStyle.Bold), hFont = new Font("微軟正黑體", 11, FontStyle.Bold), cFont = new Font("微軟正黑體", 11);
            Pen line = new Pen(Color.Black, 2), dash = new Pen(Color.Black, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            int y = 50, x1 = 50, x2 = 230, x3 = 310, x4 = 370;

            g.DrawString("★ 購物結帳明細 ★", tFont, Brushes.Black, x1 + 70, y); y += 40;
            g.DrawString($"時間：{DateTime.Now:yyyy/MM/dd HH:mm:ss}", cFont, Brushes.Black, x1, y); y += 30;

            g.DrawLine(line, x1, y, 450, y); y += 10;
            g.DrawString("商品名稱", hFont, Brushes.Black, x1, y); g.DrawString("單價", hFont, Brushes.Black, x2, y); g.DrawString("數量", hFont, Brushes.Black, x3, y); g.DrawString("小計", hFont, Brushes.Black, x4, y);
            y += 25; g.DrawLine(line, x1, y, 450, y); y += 15;

            foreach (var item in _cartController.GetCartItems())
            {
                g.DrawString(item.ProductName, cFont, Brushes.Black, x1, y); g.DrawString(item.UnitPrice.ToString("0"), cFont, Brushes.Black, x2, y); g.DrawString(item.Quantity.ToString(), cFont, Brushes.Black, x3, y); g.DrawString(item.SubTotal.ToString("0"), cFont, Brushes.Black, x4, y);
                y += 30;
            }

            g.DrawLine(dash, x1, y, 450, y); y += 15;
            var summary = _cartController.GetCheckoutSummary();
            g.DrawString($"總品項：{summary.TotalItems} 項", cFont, Brushes.Black, x1, y); y += 25;
            g.DrawString($"總件數：{summary.TotalQuantity} 件", cFont, Brushes.Black, x1, y); y += 40;
            g.DrawString($"應收總額：NT$ {summary.TotalAmount}", tFont, Brushes.Black, x1 + 100, y);
        }
        #endregion
    }
}