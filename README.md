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
