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
    [Table("EvernoteUsers")]
    public class EvernoteUser : MyEntityBase
    {
        [DisplayName("Ad"),
            StringLength(25,ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Name { get; set; }

        [DisplayName("Soyad"),
            StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Surname { get; set; }

        [DisplayName("Kull. Adı"), 
            Required(ErrorMessage = "{0} alanı gereklidir."),StringLength(25,ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Username { get; set; }

        [DisplayName("Email"), 
            Required(ErrorMessage = "{0} alanı gereklidir."), StringLength(70,ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Email { get; set; }

        [DisplayName("Şifre"), 
            Required(ErrorMessage = "{0} alanı gereklidir."), StringLength(250, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Password { get; set; }

        [DisplayName("Resim"),StringLength(30), ScaffoldColumn(false)]
        public string ProfileImageFileName { get; set; }

        [DisplayName("Aktif")]
        public bool IsActive  { get; set; }

        [DisplayName("Silinmiş Mi")]
        public bool IsDelete { get; set; }

        [Required,ScaffoldColumn(false)]
        public Guid ActiveGuid { get; set; }

        [DisplayName("Admin")]
        public bool IsAdmin { get; set; }

        public virtual List<Note> Notes { get; set; } //bir User'nin birden fazla notu olabilir..
        public virtual List<Comment> Comments { get; set; } //bir user'nin birden fazla commenti olabilir..
        public virtual List<Liked> Likes { get; set; } // bir user'ın birden fazla likes'ı vardır.
    }
}
