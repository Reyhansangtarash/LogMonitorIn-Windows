# LogMonitorIn-Windows
reading logs in logview and save it in database .while reading if there was a sensitive log program would report to the admin with sending gmail
# 🖥️ Windows Log Monitor & Alert System

یک ابزار مانیتورینگ خودکار برای **خواندن، ذخیره‌سازی و هشدار** لاگ‌های ویندوز (Application، System، Security) با قابلیت شناسایی رویدادهای حساس و ارسال اعلان ایمیل.

## ✨ ویژگی‌ها

- 📋 **خواندن لاگ‌های ویندوز** (Application, System, Security)
- 💾 **ذخیره خودکار در SQL Server** (با جدول‌های `WindowsLogs` و `Alerts`)
- 🔍 **تشخیص لاگ‌های حساس** بر اساس:
  - کلمات کلیدی (`error`, `fail`, `password`, `attack`, ...)
  - نوع رویداد (`Error`, `FailureAudit`)
  - EventIdهای خاص (`4625`, `4648`, `4720`, ...)
- 🎨 **نمایش گرافیکی** با هایلایت (ردیف‌های حساس با رنگ قرمز و پس‌زمینه صورتی)
- 📧 **ارسال ایمیل هشدار** به ادمین (با جزئیات کامل رویداد)
- ⚠️ **هشدار تکرار رویداد** (اخطار در صورت تکرار بیش از ۱۰ بار در ۵ دقیقه)
- 📁 **ذخیره خطاهای ایمیل** در فایل لاگ (`C:\LogMonitor\EmailErrors.log`)

## 🛠️ پیش‌نیازها

- [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) یا بالاتر (با قابلیت `.NET Framework 4.7.2+`)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (یا هر نسخه دیگر)
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) (برای مدیریت دیتابیس)
- یک حساب ایمیل معتبر (مثلاً Gmail) برای ارسال هشدارها
2-تنظیم دیتابیس
  CREATE DATABASE logmonitordb;
GO

USE logmonitordb;
GO

CREATE TABLE WindowsLogs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    LogName NVARCHAR(50),
    EventId INT,
    Source NVARCHAR(200),
    Message NVARCHAR(MAX),
    TimeGenerated DATETIME,
    EntryType NVARCHAR(20),
    IsSensitive BIT DEFAULT 0,
    InsertedAt DATETIME DEFAULT GETDATE()
);
GO
3. تنظیمات رشته اتصال به دیتابیس 
static string connectionString = "Server=localhost;Database=logmonitordb;Integrated Security=True;";

4. تنظیمات ایمیل برای ارسال هشدار
static string emailFrom = "your-email@gmail.com";
static string emailPassword = "your-app-password";  // رمز برنامه (App Password)
static string emailTo = "admin@yourcompany.com";
static string smtpServer = "smtp.gmail.com";
static int smtpPort = 587;

5. اجرای برنامه 
پروژه را در Visual Studio باز کنید.

با کلید F5 یا دکمه Start اجرا کنید.

در کنسول، شماره لاگ مورد نظر را وارد کنید (۱: Application، ۲: System، ۳: Security).

نتیجه ذخیره‌سازی و هشدارها را مشاهده کنید.

📸 پیش‌نمایش

1. اجرا در کنسول
   <img width="2556" height="1382" alt="image" src="https://github.com/user-attachments/assets/31e38820-7a87-47db-845d-f94cb4547229" />

   2. ایمیل هشدار ارسال شده
  <img width="2480" height="1290" alt="image" src="https://github.com/user-attachments/assets/035835a8-1ece-40ef-9959-6d4d7fc04c16" />
ساختار پروژه
LogMonitor/
│
├── Program.cs               # کد اصلی برنامه
├── LogViewerForm.cs         # فرم نمایش و هایلایت لاگ‌ها
├── LogViewerForm.Designer.cs
├── README.md                # این فایل
تصاویر برای مستندات
