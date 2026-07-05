using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace LogMonitor
{
    class Program
    {
        // رشته اتصال به دیتابیس - مقادیر رو با توجه به سیستم خودت تغییر بده
        static string emailFrom = "logmonitor1405@gmail.com";
        static string emailPassword = "sdky dgaz okgj heyz";
        static string emailTo = "reyhanehsangtarash85@gmail.com";  // 🔁 این را به ایمیل واقعی ادمین تغییر دهید
        static string smtpServer = "smtp.gmail.com";
        static int smtpPort = 587;
        static int sentAlerts = 0;


        static string connectionString = "Server=localhost;Database=logmonitordb;Integrated Security=True;";
        static void Main(string[] args)
        {
            Console.WriteLine("===== windows log monitor =====");
            Console.WriteLine("1. read Application logs ");
            Console.WriteLine("2. read System logs ");
            Console.WriteLine("3. read Security log  (run as admin)");
            Console.Write("choice ");

            string choice = Console.ReadLine();
            string logName = "";

            switch (choice)
            {
                case "1": logName = "Application"; break;
                case "2": logName = "System"; break;
                case "3": logName = "Security"; break;
                default: return;
            }

            Console.WriteLine($"\nreading log {logName}...");
            ReadAndSaveLogs(logName);

            Console.WriteLine("\ndone ! press any key to finish...");
            Console.ReadKey();
          
        }

        static void ReadAndSaveLogs(string logName)
        {
            try
            {
                // دسترسی به لاگ ویندوز
                EventLog eventLog = new EventLog(logName);

                int savedCount = 0;
                int sensitiveCount = 0;

                // فقط آخرین 100 لاگ رو می‌خونیم (برای اینکه زیاد طول نکشه)
                for (int i = 0; i < Math.Min(100, eventLog.Entries.Count); i++)
                {
                    EventLogEntry entry = eventLog.Entries[i];

                    // بررسی حساس بودن لاگ
                    bool isSensitive = IsSensitiveLog(entry);

                    if (isSensitive)
                        sensitiveCount++;

                    // ذخیره در دیتابیس
                    SaveToDatabase(logName, entry, isSensitive);
                    savedCount++;

                    // نمایش پیشرفت
                    if (i % 10 == 0)
                        Console.Write($".");
                }

                Console.WriteLine($"\n\n✅ {savedCount} log saved.");
                Console.WriteLine($"⚠️ {sensitiveCount} known sensitive log .");
                Console.WriteLine("💡 go to the database if you want to see logs.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ error: {ex.Message}");
                if (ex.Message.Contains("Access"))
                    Console.WriteLine("if you want to read security logs you need to run as admin.");
            }
        }
        static bool IsSensitiveLog(EventLogEntry entry)
        {
            string message = entry.Message?.ToLower() ?? "";
            bool result = false;
            string reason = "";

            // بررسی کلمات کلیدی
            string[] sensitiveKeywords = { "fail", "error", "critical", "security", "attack", "unauthorized", "access denied", "virus", "malware", "account locked", "password", "breach" };
            foreach (string keyword in sensitiveKeywords)
            {
                if (message.Contains(keyword))
                {
                    result = true;
                    reason = $"word '{keyword}' in message";
                    break;
                }
            }

            // بررسی نوع رویداد
            if (!result && (entry.EntryType == EventLogEntryType.Error || entry.EntryType == EventLogEntryType.FailureAudit))
            {
                result = true;
                reason = $"EventType = {entry.EntryType}";
            }

            // بررسی EventIdهای حساس
            if (!result)
            {
                int[] sensitiveEventIds = { 4625, 4648, 4720, 4723, 4724 };
                if (Array.IndexOf(sensitiveEventIds, entry.InstanceId) >= 0)
                {
                    result = true;
                    reason = $"EventId = {entry.InstanceId}";
                }
            }

            // چاپ نتیجه در کنسول
            if (result)
                Console.WriteLine($"✅ sensitive: {reason} | EventId={entry.InstanceId} | Source={entry.Source}");
            else
                Console.WriteLine($"❌ unsensitive: Type={entry.EntryType}, EventId={entry.InstanceId}, MessagePreview={message.Substring(0, Math.Min(80, message.Length))}");

            return result;
        }

        
        static string GetSensitiveReason(EventLogEntry entry)
        {
            if (entry.EntryType == EventLogEntryType.Error)
                return "Error event";
            if (entry.EntryType == EventLogEntryType.FailureAudit)
                return "Security failure";
            if (entry.Message?.ToLower().Contains("password") == true)
                return "Password related";
            return "Sensitive log detected";
        }




        static void SendEmailAlert(EventLogEntry entry, string reason, string logName)
        {
            Console.WriteLine("EMAIL FUNCTION CALLED");
            try
            {
                string subject = $"🚨 هشدار امنیتی - لاگ حساس در {logName} شناسایی شد";

                // ساخت بدنه ایمیل (HTML)
                string body = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; direction: rtl; }}
                .alert {{ background-color: #ff4444; color: white; padding: 10px; border-radius: 5px; }}
                .info {{ background-color: #f0f0f0; padding: 10px; margin: 10px 0; border-radius: 5px; }}
                .details {{ margin: 10px 0; }}
                .label {{ font-weight: bold; color: #333; }}
                .important {{ color: #ff0000; font-weight: bold; }}
            </style>
        </head>
        <body>
            <div class='alert'>
                <h2>🚨 هشدار فوری امنیتی 🚨</h2>
            </div>
            
            <div class='info'>
                <h3>جزئیات رویداد حساس:</h3>
                <div class='details'>
                    <p><span class='label'>📋 نام لاگ:</span> {logName}</p>
                    <p><span class='label'>🆔 کد رویداد:</span> <span class='important'>{entry.InstanceId}</span></p>
                    <p><span class='label'>📌 منبع:</span> {entry.Source}</p>
                    <p><span class='label'>⏰ زمان وقوع:</span> {entry.TimeGenerated}</p>
                    <p><span class='label'>⚠️ دلیل حساسیت:</span> <span class='important'>{reason}</span></p>
                    <p><span class='label'>📝 سطح رویداد:</span> {entry.EntryType}</p>
                </div>
            </div>
            
            <div class='info'>
                <h3>پیام رویداد:</h3>
                <p style='background-color: #fff3cd; padding: 10px; border-left: 4px solid #ffc107;'>
                    {entry.Message?.Replace("\n", "<br/>") ?? "بدون پیام"}
                </p>
            </div>
            
            <div class='info' style='background-color: #d4edda;'>
                <p><span class='label'>🕐 زمان ارسال هشدار:</span> {DateTime.Now}</p>
                <p><span class='label'>💻 نام کامپیوتر:</span> {Environment.MachineName}</p>
                <p><span class='label'>👤 کاربر جاری:</span> {Environment.UserName}</p>
            </div>
            
            <hr/>
            <p style='color: #666; font-size: 12px;'>
                این ایمیل به طور خودکار توسط سیستم مانیتورینگ لاگ‌های ویندوز ارسال شده است.
                لطفاً در اسرع وقت وضعیت سیستم را بررسی کنید.
            </p>
        </body>
        </html>
        ";

                // ساخت و ارسال ایمیل
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom, "Windows Log Monitor");
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    mail.Priority = MailPriority.High;

                    using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, emailPassword);
                        smtp.EnableSsl = true;
                        smtp.Timeout = 10000; // 10 ثانیه تایم‌اوت

                        smtp.Send(mail);
                        Console.WriteLine($"📧 ایمیل هشدار به {emailTo} ارسال شد.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در ارسال ایمیل: {ex.Message}");
                // خطا را در یک فایل لاگ ذخیره کن تا بعداً بررسی شود
                LogEmailError(ex.Message, entry);
            }
        }

        static void LogEmailError(string errorMessage, EventLogEntry entry)
        {
            try
            {
                string logPath = @"C:\LogMonitor\EmailErrors.log";
                string directory = Path.GetDirectoryName(logPath);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string errorText = $"[{DateTime.Now}] ERROR: {errorMessage} | EventId:{entry.InstanceId} | Source:{entry.Source}";
                File.AppendAllText(logPath, errorText + Environment.NewLine);
            }
            catch { Console.WriteLine("error"); }
        }

        static void SaveToDatabase(string logName, EventLogEntry entry, bool isSensitive)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    
                    conn.Open();
                    string query = @"INSERT INTO WindowsLogs 
                                    (LogName, EventId, Source, Message, TimeGenerated, EntryType, IsSensitive)
                                    VALUES (@LogName, @EventId, @Source, @Message, @TimeGenerated, @EntryType, @IsSensitive)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        Console.WriteLine("Before SQL");
                        // ... پارامترها ...
                        cmd.Parameters.AddWithValue("@LogName", logName);

                        // ✅ فقط اگر لاگ حساس است، ایمیل بفرست
                        if (isSensitive && sentAlerts < 5)
                        {
                            Console.WriteLine("Sensitive log found");
                            sentAlerts++;

                            string reason = GetSensitiveReason(entry);
                            SendEmailAlert(entry, reason, logName);

                        }
                        Console.WriteLine("After SQL");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n⚠️ error to save logs: {ex.Message}");
            }

        }

    }
}
