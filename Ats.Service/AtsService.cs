using Ats.Data.UnitOfWork;
using Ats.Model.Models;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Ats.Service
{
    public class AtsService : IDisposable
    {
        ChromeDriver driver;
        bool isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public AtsService()
        {
            LogHelper.WriteLine("Phantom Başlatılıyor!");
            driver = new ChromeDriver();
            driver.Manage().Window.Size = new Size(1600, 900);
            LogHelper.WriteLine("Phantom Başlatıldı!");
        }

        ~AtsService()
        {
            Dispose(false);

            LogHelper.WriteLine("Destructor - Phantom Durduruluyor!", ConsoleColor.White);

            if (driver != null)
            {
                driver.Dispose();
            }

            LogHelper.WriteLine("Destructor - Phantom Durduruldu!", ConsoleColor.White);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                LogHelper.WriteLine("Dispose - Phantom Durduruluyor!", ConsoleColor.White);

                if (driver != null)
                {
                    driver.Dispose();
                }

                LogHelper.WriteLine("Dispose - Phantom Durduruldu!", ConsoleColor.White);
            }

            isDisposed = true;
        }


        // Publics
        public void GetNewDefinitionsAndSendMail()
        {
            var allAnnouncements = new List<Announcement>();

            foreach (var item in GetDefinitions())
            {
                try
                {
                    if (driver.SessionId == null)
                    {
                        break;
                    }

                    if (!driver.Url.Equals(item.Url))
                    {
                        LogHelper.WriteLine($"{item.Description} sayfasına gidiliyor. Lütfen bekleyiniz!");

                        driver.Navigate().GoToUrl(item.Url);

                        LogHelper.WriteLine($"{item.Url} sayfası açıldı!");
                    }

                    var list = GetAnnouncements(driver, item.TypeId, item.RowCssSelector, item.ClickCssSelector);

                    if (list == null || list.Count.Equals(0))
                    {
                        LogHelper.WriteLine($"Sayfada kaydedilecek yeni veri bulunamadı!");

                        continue;
                    }

                    LogHelper.WriteLine($"Bu sayfada {list.Count} bildirim bulundu!");

                    allAnnouncements.AddRange(list);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLine(ex, "Genel döngü hatası!");
                }
            }

            LogHelper.WriteLine($"Tüm sayfalarda bulunan toplam bildirim sayısı: {allAnnouncements.Count}!");

            if (allAnnouncements.Count > 0)
            {
                using (var db = new UnitOfWork())
                {
                    var emails = db.Emails.GetAll();
                    var mailResult = SendMailForNewSavedAnnounces(allAnnouncements, emails);

                    if (mailResult)
                    {
                        try
                        {
                            db.Announcements.Insert(allAnnouncements);
                            db.Commit();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLine(ex, "Kayıt hatası!");

                            db.Rollback();
                        }
                    }
                }
            }
        }


        // Privates
        private bool SendMailForNewSavedAnnounces(List<Announcement> newEntities, List<Email> emails)
        {
            try
            {
                var body = string.Empty;

                foreach (var item in newEntities)
                {
                    body += $"<a href='{item.Url}'>{item.Text}</a></br>";
                }

                var templateFileFullName = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, "email_template.html");

                if (File.Exists(templateFileFullName))
                {
                    //body = File.ReadAllText(templateFileFullName);
                }

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("info@trxtarget.com"),
                    Subject = $"Gop Üniversitesinden ({newEntities.Count}) Yeni Bildirim",
                    Body = body,
                    IsBodyHtml = true,
                };

                foreach (var email in emails)
                {
                    mailMessage.To.Clear();
                    mailMessage.To.Add(email.EmailAddress);

                    using (var smtpClient = new SmtpClient("smtp.yandex.com.tr", 587)
                    {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("info@trxtarget.com", "6157850x"),
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        EnableSsl = true
                    })
                    {
                        smtpClient.Send(mailMessage);
                    };
                }

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLine(ex, "Mail gönderme hatası!");

                return false;
            }
        }

        private List<Announcement> GetAnnouncements(RemoteWebDriver driver, byte typeId, string rowsCssSelector, string clickCssSelector)
        {
            try
            {
                if (clickCssSelector != null)
                {
                    try
                    {
                        LogHelper.WriteLine($"Tıklanacak element bulundu: {clickCssSelector}!");

                        driver.FindElement(By.CssSelector(clickCssSelector)).Click();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLine(ex, "Tıklama hatası!");
                    }
                }

                var rows = new List<IWebElement>();

                try
                {
                    rows = driver.FindElements(By.CssSelector(rowsCssSelector)).Where(x => x != null && x.Displayed).ToList();
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLine(ex, "Satırları alma hatası!");
                    rows = null;
                }

                if (rows == null)
                {
                    return default;
                }

                LogHelper.WriteLine($"Tablodaki kayıtlar inceleniyor! Toplam {rows.Count} satır veri bulundu!");

                var target = new List<Announcement>();

                using (var db = new UnitOfWork())
                {
                    for (var i = 0; i < rows.Count; i++)
                    {
                        var row = rows[i];

                        try
                        {
                            var columns = row.FindElements(By.XPath("td")).ToList();
                            var url = columns.FirstOrDefault().FindElement(By.TagName("a")).GetAttribute("href");
                            var splittedDateValues = columns[1].Text.Split(' ');
                            int? monthId;

                            switch (splittedDateValues[1])
                            {
                                case "Ocak": monthId = 1; break;
                                case "Şubat": monthId = 2; break;
                                case "Mart": monthId = 3; break;
                                case "Nisan": monthId = 4; break;
                                case "Mayıs": monthId = 5; break;
                                case "Haziran": monthId = 6; break;
                                case "Temmuz": monthId = 7; break;
                                case "Ağustos": monthId = 8; break;
                                case "Eylül": monthId = 9; break;
                                case "Ekim": monthId = 10; break;
                                case "Kasım": monthId = 11; break;
                                case "Aralık": monthId = 12; break;
                                default: monthId = null; break;
                            }

                            if (!monthId.HasValue || !int.TryParse(splittedDateValues[2], out int year) || !int.TryParse(splittedDateValues[0], out int day))
                            {
                                continue;
                            }

                            var text = columns[0].Text;
                            var date = new DateTime(year, monthId.Value, day);

                            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(url) || db.Announcements.Any(x => x.Text == text && x.Date == date && x.Url == url))
                            {
                                //Console.Write($"Tablodaki {i + 1}. kayıt zaten mevcut!");

                                continue;
                            }

                            if (text.Length > 3999)
                            {
                                text = text.Substring(0, 3999);
                            }

                            if (url.Length > 499)
                            {
                                url = url.Substring(0, 499);
                            }

                            target.Add(new Announcement
                            {
                                Text = text,
                                Url = url,
                                Date = date,
                                TypeId = typeId,
                            });
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLine(ex, "Satırlar arasında gezerken oluşan bir hata!");
                        }
                    }
                }

                LogHelper.WriteLine($"Tablodaki kayıt incelemeleri tamamlandı!");

                return target;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLine(ex, "GetAnnouncements hatası!");

                return default;
            }
        }

        private List<AnnouncementDefinition> GetDefinitions()
        {
            using (var db = new UnitOfWork())
            {
                return db.AnnouncementDefinitions.GetAll();
            }
        }
    }
}