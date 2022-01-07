using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Diagnostics;
using Webapp_project.Models;

//Mailkit (for contact page)//
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

//HtmlSanitizer//
using Ganss.XSS;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace Webapp_project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            //throw new ArgumentException("!!This is an error message!!");
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(ContactModel contactModel)
        {
            if (ModelState.IsValid)
            {
                string contactEmail = _config["ContactEmail"];
                string contactPassword = _config["ContactPassword"];
                string domainEmail = "AutomatedEmail@domainEmail.com"; //Fake email name to identify automation

                MimeMessage mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress("Automated Contact", domainEmail));
                mimeMessage.To.Add(MailboxAddress.Parse(contactEmail));
                mimeMessage.Subject = contactModel.Subject;
                mimeMessage.Body = new TextPart("html")
                {
                    Text = $"<p><b>Email From: {contactModel.Name} ({contactModel.Email})</b></p> " +
                    $"<p>{new HtmlSanitizer().Sanitize(contactModel.Message)}</p>"
                };

                try
                {
                    SmtpClient client = new SmtpClient();
                    await client.ConnectAsync("smtp.gmail.com", 465, true); //Host, Port, EnableSsl
                    await client.AuthenticateAsync(contactEmail, contactPassword);
                    await client.SendAsync(mimeMessage);

                    return View("SendContact", contactModel);
                }
                catch
                {
                    _logger.LogInformation("Failed to send email;", mimeMessage.ToString());
                    return View(contactModel);
                }
            }
            else
            {
                return View(contactModel);
            }
        }



        // other stuff //
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            IExceptionHandlerFeature exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var statusCode = exception.Error.GetType().Name switch
            {
                "ArgumentException" => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.NotFound
            };

            _logger.LogError($"{DateTime.Now}: {statusCode}");
            ViewData["StatusCode"] = (int)statusCode;

            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            return View();
        }
    }
}
