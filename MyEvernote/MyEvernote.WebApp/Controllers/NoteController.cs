using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Helper;

namespace MyEvernote.WebApp.Controllers
{
    [Except]
    public class NoteController : Controller
    {
       private  NoteManager nm = new NoteManager();
       private  CategoryManager cm = new CategoryManager();
       private LikedManager lm = new LikedManager();

        [Auth]
        public ActionResult Index()
        {
            // tarihe göre tersten sırala...
            // Include getirmesini istediğmiz ilişkili tabloları getirmek için (Note class'ı içinde ilişikili abloları  EvernoteUser(Owner)) 
            var notes = nm.ListQueryable().Include("Category").Include("Owner").Where(x => x.Owner.Id == CurrentSession.User.Id).OrderByDescending(x => x.ModifieOn);
            return View(notes.ToList());
        }

        [Auth]
        public ActionResult MyLikedNotes()
        {
            var notes = lm.ListQueryable().
                Include("LikedUser").
                Include("Note").
                Where(x => x.LikedUser.Id == CurrentSession.User.Id).
                Select(x => x.Note).
                Include("Category").
                Include("Owner").
                OrderByDescending(x => x.ModifieOn);

            var dsf = notes.Count();
             
            return View("Index",notes.ToList());
        }

        [Auth]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = nm.Find(x => x.Id == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        [Auth]
        public ActionResult Create()
        {
            //ViewBag.CategoryId = new SelectList(cm.List(), "Id", "Title");
            //Kategorileri çok değişmediği için cache'den çektik
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title");
            return View();
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Note note)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifieOn");
            ModelState.Remove("ModifiedUserName");
            ModelState.Remove("IsDraft");

            if (ModelState.IsValid)
            {
                note.Owner = CurrentSession.User;
                note.IsDraft = true;
                nm.Insert(note); 
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }

        [Auth]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = nm.Find(x => x.Id == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Note note)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifieOn");
            ModelState.Remove("ModifiedUserName");

            if (ModelState.IsValid)
            {
                Note db_note = nm.Find(x => x.Id == note.Id);
                db_note.IsDraft = note.IsDraft;
                db_note.CategoryId = note.CategoryId;
                db_note.Text = note.Text;
                db_note.Title = note.Title;

                nm.Update(db_note);

                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }

        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = nm.Find(x => x.Id == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        [Auth]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = nm.Find(x => x.Id == id);
            nm.Delete(note);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {
            //o sayfadaki not id'lerin içinden o anki kullanıcının beğendiği notları geri dönderir.
            if (CurrentSession.User != null)
            {
                List<int> likedNoteIds = lm.List(
                    x => x.LikedUser.Id == CurrentSession.User.Id && ids.Contains(x.Note.Id)).Select(
                    x => x.Note.Id).ToList();

                return Json(new { result = likedNoteIds });
            }
            else
            {
                return Json(new { result = new List<int>() });
            }
        }

        [HttpPost]
        public ActionResult SetLikeState(int noteid,bool liked)
        {
            int res = 0;

            if (CurrentSession.User == null)
                return Json(new { hasError = true, errorMessage = "Beğenme ve Yorum işlemleri için giriş yapmalısınız.", result = 0 });

            Liked like = lm.Find(x => x.Note.Id == noteid && x.LikedUser.Id == CurrentSession.User.Id);
            Note note = nm.Find(x => x.Id == noteid);

            if (like != null && liked == false)
            {
                res = lm.Delete(like);
            }
            else if(like == null && liked == true)
            {
                res = lm.Insert(new Liked() {

                    LikedUser = CurrentSession.User,
                    Note = note

                });
            }

            if (res > 0)
            {
                if (liked)
                {
                    note.LikeCount++;
                }
                else
                {
                    note.LikeCount--;
                }

               res =  nm.Update(note);

                return Json(new { hasError = false, errorMessage = string.Empty, result = note.LikeCount });
            }

            return Json(new { hasError = true,errorMessage = "Beğenme işlemi gerçekleştirilemedi.",result = note.LikeCount});
        }

        public ActionResult GetNoteText(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = nm.Find(x => x.Id == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PartialNoteText",note);
        }

        [HttpGet]
        public ActionResult DetailNote(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = nm.Find(x => x.Id == Id);
            if (note == null)
            {
                return HttpNotFound();
            } 
            return View(note);
        }


    }
}
