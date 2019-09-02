
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLCM.Models
{
    public class LiveStatusModel
    {
        public string LiveId { get; set; }

        public int Status { get; set; }

        public string CloseTitle { get; set; }

        public string CloseContent { get; set; }
    }
}