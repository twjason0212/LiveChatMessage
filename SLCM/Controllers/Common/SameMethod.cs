using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace SLCM.Controllers.Common
{
    public class SameMethod
    {
        public static string FuzzyName(string userName)
        {
            var fuzzyName = "";

            fuzzyName = userName.Substring(0, 2) + "***";

            if (userName.Length > 4)
                fuzzyName += userName.Substring(userName.Length - 1, 1);

            return fuzzyName;
        }

        //public static string FuzzySensitive(string userName, string sensitive)
        //{
        //    var fuzzyName = userName;

        //    foreach (var word in sensitive)
        //    {
        //        fuzzyName = fuzzyName.Replace(word, '*');
        //    }

        //    return fuzzyName;
        //}
    }
}