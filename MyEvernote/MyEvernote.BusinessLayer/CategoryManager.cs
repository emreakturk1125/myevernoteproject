using MyEvernote.BusinessLayer.Abstract; 
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
    public class CategoryManager : ManagerBase<Category>
    {
        //veritabanında ilişkileri cascade yaptıgımız, ilişkili tablolardaki bilgileri otomatik sildi.

        #region İlişkili tabloları silme  örnek
        //public override int Delete(Category obj)
        //{
        //    NoteManager nm = new NoteManager();
        //    LikedManager lm = new LikedManager();
        //    CommentManager cm = new CommentManager();

        //    //Kategori ile ilişkili notların silinmesi
        //    foreach (Note note in obj.Notes.ToList())
        //    {
        //        //Note ile ilişikili like'ların silinmesi

        //        foreach (Liked like in note.Likes.ToList())
        //        {
        //            lm.Delete(like);
        //        }

        //        //Note ile ilişkli comment'lerin silinmesi
        //        foreach (Comment comment in note.Comments.ToList())
        //        {
        //            cm.Delete(comment);
        //        }

        //        //Note'nin kendisi silinir..
        //        nm.Delete(note);

        //    }
        //    return base.Delete(obj);
        //} 
        #endregion


    }
}
