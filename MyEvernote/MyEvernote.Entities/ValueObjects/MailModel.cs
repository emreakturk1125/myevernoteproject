using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Entities.ValueObjects
{
   public class MailModel
    {
        public string Mail { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
    }

    public class MailModel2
    {
        public string Name { get; set; }
           [DisplayName("E-posta"),
           Required(ErrorMessage = "{0} alanı boş geçilemez."),
           StringLength(70, ErrorMessage = "{0} max. {1} karakter olmalı."),
           EmailAddress(ErrorMessage = "{0} alanı için geçerli bir e-posta giriniz.")]
        public string Mail { get; set; }
        
        public string Text { get; set; }
    }
}
