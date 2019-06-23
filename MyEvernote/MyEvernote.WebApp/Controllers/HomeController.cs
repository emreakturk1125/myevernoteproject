using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Common.Helpers;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjects;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Helper;
using MyEvernote.WebApp.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MyEvernote.WebApp.Controllers
{
    [Except]
    public class HomeController : Controller
    {
        private NoteManager nm = new NoteManager();
        private CategoryManager cm = new CategoryManager();
        private EvernoteUserManager eum = new EvernoteUserManager();

        public ActionResult Index()
        {

            #region TempData ile veri gelseydi eğer
            //CategoryController'dan TempData ile veri gönderilseydi eğer..

            //if (TempData["mm"] != null)
            //{
            //    return View(TempData["mm"] as List<Note>);
            //} 
            #endregion

            var list = nm.ListQueryable();

            return View(nm.ListQueryable().Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifieOn).ToList());
        }

        public ActionResult ByCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            //Category cat = cm.Find(x => x.Id == id.Value);

            //if (cat == null)
            //{
            //    return HttpNotFound();
            //}

            //List<Note> notes = cat.Notes.Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifieOn).ToList(); 

            List<Note> notes = nm.ListQueryable().Where(x => x.IsDraft == false && x.CategoryId == id).OrderByDescending(x => x.ModifieOn).ToList();

            return View("Index", notes);
        }

        public ActionResult MostLiked()
        {
            return View("Index", nm.ListQueryable().OrderByDescending(x => x.LikeCount).ToList());
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpPost]
        public ActionResult About(MailModel2 model)
        {
            if (ModelState.IsValid)
            {
                var body = new StringBuilder();
                body.AppendLine("İsim: " + model.Name);
                body.AppendLine("Eposta: " + model.Mail);
                body.AppendLine("Konu: " + model.Text);
                MailHelper.SendMailForAbout(body.ToString());
                ViewBag.Success = true;
            }
            return View();
        }
        [Auth]
        public ActionResult ShowProfile()
        {
            BusinessLayerResult<EvernoteUser> res = eum.GetUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotify = new ErrorViewModel()
                {
                    Title = "Hata Oluştu",
                    Items = res.Errors
                };
                return View("Error", errorNotify);
            }
            return View(res.Result);
        }

        [Auth]
        [HttpGet]
        public ActionResult EditProfile()
        {

            BusinessLayerResult<EvernoteUser> res = eum.GetUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotify = new ErrorViewModel()
                {
                    Title = "Hata Oluştu",
                    Items = res.Errors
                };
                return View("Error", errorNotify);
            }
            return View(res.Result);
        }

        [Auth]
        [HttpPost]
        public ActionResult EditProfile(EvernoteUser model, HttpPostedFileBase ProfileImage,string txtNewPass)
        {
            ModelState.Remove("ModifiedUsername"); // bunun ile alakalı bir validation kontrolu yapılmasını istemiyorsan 
            if (ModelState.IsValid)
            {
                if (ProfileImage != null &&
                    (ProfileImage.ContentType == "image/jpeg" ||
                    ProfileImage.ContentType == "image/jpg" ||
                    ProfileImage.ContentType == "image/png"))
                {
                    string filename = $"user_{model.Id}.{ProfileImage.ContentType.Split('/')[1]}";

                    ProfileImage.SaveAs(Server.MapPath($"~/Images/{filename}"));
                    model.ProfileImageFileName = filename;
                }
                model.Password = MD5Encryption.MD5Sifrele(txtNewPass.ToString());
                BusinessLayerResult<EvernoteUser> res = eum.UpdateProfile(model);

                if (res.Errors.Count > 0)
                {
                    ErrorViewModel errorNotify = new ErrorViewModel()
                    {
                        Title = "Profile Güncellenemedi.",
                        Items = res.Errors,
                        RedirectingUrl = "/Home/EditProfile"
                    };
                    return View("Error", errorNotify);
                }
                CurrentSession.Set<EvernoteUser>("login", res.Result);// Profil güncellendiği için session güncellendi..
                return RedirectToAction("ShowProfile");
            }

            return View(model);
        }

        [Auth]
        public ActionResult DeleteProfile(EvernoteUser user)
        {
            // BusinessLayerResult<EvernoteUser> res = eum.RemoveUserById(CurrentSession.User.Id);

            BusinessLayerResult<EvernoteUser> res1 = eum.GetUserById(CurrentSession.User.Id);
            res1.Result.IsDelete = true;
            BusinessLayerResult<EvernoteUser> res2 = eum.Update(res1.Result);


            if (res2.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Items = res2.Errors,
                    Title = "Profil Silinemedi.",
                    RedirectingUrl = "/Home/ShowProfile"
                };

                return View("Error", errorNotifyObj);
            }

            Session.Clear();

            return RedirectToAction("Index");
        }

        public ActionResult TestNotify()
        {
            //InfoViewModel model = new InfoViewModel()
            //{
            //    Header = "Yönlendirme..",
            //    Title = "Ok test",
            //    RedirectingTimeout = 3000,
            //    Items = new List<string>() {"Test Başarılı 1","Test Başarılı 2"}
            //};
            //return View("Info",model);

            ErrorViewModel model = new ErrorViewModel()
            {
                Header = "Yönlendirme..",
                Title = "Ok test",
                RedirectingTimeout = 3000,
                Items = new List<ErrorMessageObj>() {
                    new ErrorMessageObj() { Message ="'Test başarılı 1" },
                    new ErrorMessageObj() { Message = "Test başarılı 2"} }
            };
            return View("Error", model);

        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Password = MD5Encryption.MD5Sifrele(model.Password);
                BusinessLayerResult<EvernoteUser> res = eum.LoginUser(model);

                if (res.Errors.Count > 0)
                {

                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                    return View(model);
                }

                CurrentSession.Set<EvernoteUser>("login", res.Result);

                //var response = Request["g-recaptcha-response"];
                //const string secret = "6LcjInAUAAAAADx99wOz1kr3QVnSrgYrX61f0eU0";

                //var client = new WebClient();
                //var reply =
                //    client.DownloadString(
                //        string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

                //var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

                //if (!captchaResponse.Success)
                //    TempData["Message"] = "Lütfen güvenliği doğrulayınız.";
                //else
                    return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {


            if (ModelState.IsValid)
            {
                model.Password = MD5Encryption.MD5Sifrele(model.Password);
                BusinessLayerResult<EvernoteUser> res = eum.RegisterUser(model);
                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }

                OkViewModel notify = new OkViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl = "/Home/Login",

                };
                notify.Items.Add("Lütfen e-posta  adresine gönderilen mail'i tıklayarak hesabınızı aktive ediniz. Hesabınızı aktive etmeden not ekleyemez ve begenme yapamazsınız..");
                return View("Ok", notify);
            }
            return View(model);
        }


        public ActionResult UserActivate(Guid id)
        {
            BusinessLayerResult<EvernoteUser> res = eum.ActivateUser(id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotify = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    Items = res.Errors
                };
                return View("Error", errorNotify);
            }
            OkViewModel okNotify = new OkViewModel()
            {
                Title = "Hesap Aktifleştirildi.",
                RedirectingUrl = "/Home/Login"
            };
            okNotify.Items.Add("Hesap Aktifleştirildi.Artık not paylaşabilir ve begenme yapabilirsiniz.");
            return View("Ok", okNotify);
        }

        public ActionResult UserActivateOK()
        {
            return View();
        }

        public ActionResult UserActivateCancel()
        {
            List<ErrorMessageObj> errors = null;
            if (TempData["errors"] != null)
            {
                errors = TempData["errors"] as List<ErrorMessageObj>;
            }
            return View(errors);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }


        public ActionResult HasError()
        {
            return View();
        }

        public ActionResult ornek()
        {
            return View();
        }

        public ActionResult MailSend(MailModel model)
        {
            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.EnableSsl = true;
                WebMail.UserName = "makalemre";
                WebMail.Password = "makalemre1125makalemre1125"; // gerçek dışı
                WebMail.SmtpPort = 587;
                WebMail.Send(
                        "makalemre1125@gmail.com",
                        model.Subject,
                        model.Text,
                        model.Mail
                    );

                OkViewModel notify = new OkViewModel()
                {
                    Title = "Mail Gönderimi Başarılı..",
                    RedirectingUrl = "/Home/Login",

                };
            }
            catch (Exception ex)
            {
                ViewData.ModelState.AddModelError("_HATA", ex.Message);
            }

            return View("Index");
        }

        public class CaptchaResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }

        [HttpGet]
        public ActionResult PasswordForget()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasswordForget(MailModel2 model)
        {

            string sifre = RastgeleUret();
            string sifreMD5 = MD5Encryption.MD5Sifrele(sifre);
             
            BusinessLayerResult<EvernoteUser> res = eum.GetUserByEmail(model);

            //var response = Request["g-recaptcha-response"];
            //const string secret = "6LcjInAUAAAAADx99wOz1kr3QVnSrgYrX61f0eU0";

            //var client = new WebClient();
            //var reply =
            //    client.DownloadString(
            //        string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            //var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            //if (!captchaResponse.Success)
            //    TempData["Message"] = "Lütfen güvenliği doğrulayınız.";
            //else
            //{

                if (res.Errors.Count > 0)
                {

                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                    return View(model);
                }

                StringBuilder body = new StringBuilder();
                body.AppendLine("Merhaba, " + res.Result.Username);
                body.AppendLine("\nYeni Şifreniz : " + sifre);
                MailHelper.SendMail(body.ToString(), model.Mail, "Makalemre.com Sitesi Yeni Şifre");
                ViewBag.Success = true;

                res.Result.Password = sifreMD5;
                BusinessLayerResult<EvernoteUser> res2 = eum.Update(res.Result); 
            //}
            return View();

        }


        public string RastgeleUret()
        {
            Random rnd = new System.Random(unchecked((int)DateTime.Now.Ticks));
            string ret = "";
            for (int i = 0; i < 7; i++)
            {
                if (i == 0 || i == 5 || i == 6)
                    ret += randkh(rnd);
                if (i == 1 || i == 4)
                    ret += randsayi(rnd);
                if (i == 2)
                    ret += randbh(rnd);
                if (i == 3)
                    ret += randnok(rnd);
            }
            return ret;
        }

        #region şifre üret

        const string sayi = "0123456789";
        char randsayi(Random rnd)
        {
            return sayi[rnd.Next(sayi.Length)];
        }

        const string bh = "ABCDEFGHIJKLMNOPRSTUVYZ";
        char randbh(Random rnd)
        {
            return bh[rnd.Next(bh.Length)];
        }

        const string kh = "abcdefghijklmnoprstuvyz";
        char randkh(Random rnd)
        {
            return kh[rnd.Next(nok.Length)];
        }

        const string nok = "%&+@?!$#";
        char randnok(Random rnd)
        {
            return nok[rnd.Next(nok.Length)];
        }
        #endregion
    }
}