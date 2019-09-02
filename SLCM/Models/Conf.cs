using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLCM.Models
{
    public class Conf
    {
        public static string WSUrl
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WSUrl"] ?? "";
            }
        }
    }
}