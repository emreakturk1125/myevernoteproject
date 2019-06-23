using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyEvernote.WebApp.ViewModels
{
    public class WarningViewModel : NotitfyViewModelBase<string>
    {
        public WarningViewModel()
        {
            Title = "Uyarı!";
        }  
    }
}