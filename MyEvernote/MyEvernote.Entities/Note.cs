using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Entities
{
    [Table("Notes")]
    public class Note : MyEntityBase
    {
        [DisplayName("Not Başlığı"),Required, StringLength(100, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Title { get; set; }

        [DisplayName("Not Metni"), Required, MaxLength]
        public string Text { get; set; }

        [DisplayName("Not Açıklaması"), MaxLength]
        public string Description { get; set; }

        [DisplayName("Taslak mı?"),]
        public bool IsDraft { get; set; }

        [DisplayName("Begenilme")]
        public int LikeCount { get; set; }

        [DisplayName("Kategori")]
        public int CategoryId { get; set; }

        public virtual EvernoteUser Owner { get; set; } //bir notun sahibi vardır
        public virtual List<Comment> Comments { get; set; } //bir notun birden fazla yorumu vardır..
        public virtual Category Category { get; set; } // bir notun category'si vardır.
        public virtual List<Liked> Likes { get; set; } // bir notun birden fazla like'ı vardır

        public Note()
        {
            Comments = new List<Comment>();
            Likes = new List<Liked>();
        }
    }
}
