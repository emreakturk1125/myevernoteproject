using MyEvernote.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    //MultiThread uygulamalar için bile DatabaseContext'in bir defa oluşmasını sağlar
    //Bu da singleton disagn pattern dır
    public class RepositoryBase
    {
        protected static DatabaseContext context;        // bu class new' lenemez sadece miras alan new'ler...
        private static object _lockSync = new object();

        protected RepositoryBase()                     // bu class new' lenemez sadece miras alan new'ler...
        {
             CreateContext();
        }
        
        // databaseContext kontrol edicek varsa yani daha önde oluşturulmuş ise var olanı göndericek yoksa new'leyip yaniden oluşturcak

        private static void CreateContext()  
        {
            if (context == null)
            {
                lock (_lockSync)   // lock bir iş bitmeden diğerrini çalıştımaz
                {
                    if (context == null)
                    {
                        context = new DatabaseContext();
                    }
                }
                
            }
 
        }
    }
}
