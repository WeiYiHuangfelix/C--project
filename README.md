# C# Project (電商/購物車系統專題)

## 第一步：定義你的專案架構 (The Big Picture)

> 「這是一個基於 **ASP.NET Core MVC** 架構開發的電商/購物車系統專題。
> 在前端，我使用 **Bootstrap** 來進行響應式網頁設計 (RWD)。
> 在後端，我嚴格遵守 **MVC** 的設計模式。資料庫存取層我導入了 **Entity Framework Core (EF Core)** 作為 ORM 工具，並連接 **SQL Server**，取代了傳統的 ADO.NET (DataSet/DataTable) 寫法，讓資料庫操作完全『物件化』。」

---

## 第二步：講解亮點 —— 展示你的「資深思維」

### 亮點 1：為什麼要用 ViewModel？(這題必考！)

在設計「訂單明細」這個功能時，我沒有直接把資料庫的實體模型 (`Data Model`) 丟給前端畫面。我特別設計了視圖模型 (`ViewModel`)。因為我認為：

- **資安考量：** 原生 Model 可能包含敏感資訊，`ViewModel` 讓我只暴露畫面需要的欄位。
- **解耦與彈性：** 畫面需要合併「銷售主檔」與「銷售明細」的資料，我透過 `Controller` 將兩張表的資料整理好，扁平化之後再裝進 `ViewModel` 傳給 `View`。這樣即使未來資料庫結構改變，前端的 HTML 也不用大改，達到**前後端分離**的精神。

### 亮點 2：如何從資料庫撈取關聯資料？

在撈取訂單資料時，我使用了 EF Core 的 **Eager Loading (預先載入)**。
我利用 LINQ 的 `.Include()` 和 `.ThenInclude()` 語法，在一次資料庫查詢中，就把「訂單主檔」、「客戶資料」、「訂單明細」和「產品名稱」全部 `JOIN` 出來。這樣可以避免產生 `N+1` 查詢效能問題，也比手寫繁雜的 SQL `JOIN` 語句更好維護。

### 亮點 3：資料庫連線的安全性設計

關於系統的基礎建設，我將資料庫連線字串統一寫在 `appsettings.json` 中統一管理。並且在 `Program.cs` 裡，透過 ASP.NET Core 內建的**依賴注入 (Dependency Injection, DI)** 將 `ApplicationDbContext` 註冊為 `Scoped` 服務。這讓我的 `Controller` 不需要自己去建立資料庫連線，降低了程式的耦合度。

---

## 第三步：把「犯錯」變成你的「最強武器」

在建立測試資料時，我遇到了 SQL Server 報出 **547 錯誤 (Foreign Key 條件約束衝突)**。當下系統不讓我把資料 `INSERT` 進去。我去查明原因後，深刻體會到關聯式資料庫中**「參考完整性 (Referential Integrity)」**的重要性。

我發現是因為我試圖在「銷售明細表」新增一筆訂單明細，但當時「銷售主檔」裡還沒有對應的 `OrderId`。系統為了保護資料不變成孤兒資料 (Orphan Data)，所以阻擋了我。

> **我的解決方式是：**
> 重新梳理了建立資料的相依性順序。我寫了一段 T-SQL 腳本，嚴格要求必須**先建立 Customer (客戶) 與 Product (產品)，接著建立 Order (主檔)，最後才能 INSERT OrderDetail (明細)**。這個經驗讓我對資料庫的關聯設計有了更扎實的理解。

---

## 實作紀錄：建立測試資料的 T-SQL 腳本

> **核心觀念：** 與刪除相反，新增資料時，必須先建立被依賴的「父表」，才能建立依賴人的「子表」。

### 步驟 1：新增 Customer (客戶)
*(獨立資料表，無依賴)*

```sql
-- 步驟 1：新增 Customer (獨立資料表，無依賴)
SET IDENTITY_INSERT Customers ON;

INSERT INTO Customers (Id, Name, Region, PaymentMethods)
VALUES (1, N'王小明', N'北', N'信用卡');

SET IDENTITY_INSERT Customers OFF;
📝 語法解釋：

SET IDENTITY_INSERT Customers ON;：IDENTITY_INSERT 是一個非常重要的開關。通常資料庫的 Id 欄位會設定為「自動遞增 (Identity)」，也就是資料庫會自己給它 1, 2, 3...，不允許我們手動輸入。將其設為 ON，代表我們強行開啟手動寫入 ID 的權限，這樣後續的關聯才不會因為 ID 亂跳而對不上。

Id：指定為 1 (因為前面開啟了權限才能這樣做)。

N'王小明'：這裡的字母 N 代表 Unicode 字串 (NVARCHAR)。加上 N 可以確保中文字不會在存入資料庫時變成亂碼。

SET IDENTITY_INSERT Customers OFF;：資料新增完畢，立刻關閉手動寫入 ID 的權限，把控制權還給資料庫。這是非常好的安全習慣。

步驟 2：新增 Product (商品)
(獨立資料表，無依賴)

SQL
-- 步驟 2：新增 Product (獨立資料表，無依賴)
SET IDENTITY_INSERT Products ON;

INSERT INTO Products (Id, Name, Price, ImageUrl)
VALUES 
    (1, N'高階電競筆電', 45000, N'/images/laptop.jpg'),
    (2, N'人體工學滑鼠', 1500, N'/images/mouse.jpg');

SET IDENTITY_INSERT Products OFF;
📝 語法解釋：

開啟 Products 資料表的手動寫入 ID 權限。

一次新增兩筆商品資料。Id 分別強制指定為 1 (筆電) 和 2 (滑鼠)。

關閉 Products 的手動寫入 ID 權限。

步驟 3：新增 Order (訂單)
(依賴 Customer)

SQL
-- 步驟 3：新增 Order (依賴 Customer)
-- 這裡的 CustomerId 必須是剛才建立的 1
SET IDENTITY_INSERT Orders ON;

INSERT INTO Orders (OrderId, OrderDate, CustomerId, TotalAmount)
VALUES (1, GETDATE(), 1, 46500); 

SET IDENTITY_INSERT Orders OFF;
📝 語法解釋：

開啟 Orders 資料表的手動寫入 ID 權限。

OrderId：強制指定為 1。

GETDATE()：這是 SQL 內建的函數，會自動抓取當下系統的時間與日期填入。

CustomerId：填入 1，代表這筆訂單是屬於剛剛建立的客戶「王小明」。(如果填入不存在的數字，資料庫會報錯阻擋)。

關閉 Orders 的手動寫入 ID 權限。

步驟 4：新增 OrderDetail (訂單明細)
(依賴 Order 和 Product)

SQL
-- 步驟 4：新增 OrderDetail (依賴 Order 和 Product)
-- 這裡的 OrderId 必須是 1，ProductId 必須是剛才建立的 1 和 2
SET IDENTITY_INSERT OrderDetails ON;

INSERT INTO OrderDetails (OrderDetailId, OrderId, ProductId, Quantity, UnitPrice, SubTotal)
VALUES 
    (1, 1, 1, 1, 45000, 45000), -- 買了一台筆電
    (2, 1, 2, 1, 1500, 1500);   -- 買了一隻滑鼠

SET IDENTITY_INSERT OrderDetails OFF;
📝 語法解釋：

開啟 OrderDetails 資料表的手動寫入 ID 權限。

一次新增兩筆訂單明細：

第一筆： OrderDetailId 為 1，掛在 OrderId 1 之下，購買了 ProductId 1 (筆電)，數量 1，單價 45000，小計 45000。

第二筆： OrderDetailId 為 2，同樣掛在 OrderId 1 之下，購買了 ProductId 2 (滑鼠)，數量 1，單價 1500，小計 1500。

關閉 OrderDetails 的手動寫入 ID 權限。完成所有資料建置。