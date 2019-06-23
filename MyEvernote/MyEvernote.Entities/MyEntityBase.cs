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
    public class MyEntityBase
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Kayıt Tar."), Required]
        public DateTime CreatedOn { get; set; }
        [DisplayName("Değiş. Tar."), Required]
        public DateTime ModifieOn { get; set; }
        [DisplayName("Değiş. Kull."), Required, StringLength(30)]
        public string ModifiedUsername { get; set; }
    }
}
