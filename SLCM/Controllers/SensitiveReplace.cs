using SLCM.Controllers.Common;
using SLCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLCM.Controllers
{
    public static class SensitiveReplace
    {
        //public static string Sensitive(string key)
        //{
        //    string result = key;
        //    string resultTraditional = SameMethod.ToSimplified(result); // 訊息內容轉繁體

        //    var sSentences = StaticTables.sdt_SensitiveSentences
        //           .Where(s => (result.Contains(s.content)) || resultTraditional.Contains(s.content))
        //           .FirstOrDefault();

        //    if (sSentences != null && sSentences.state != 1)
        //    {
        //        result = SameMethod.FuzzySensitive(key, sSentences.content);
        //    }

        //    var sWordsList = StaticTables.sdt_SensitiveWords
        //            .Where(s => (result.Contains(s.content)) || resultTraditional.Contains(s.content))
        //            .OrderByDescending(s => s.state).ThenByDescending(s => s.updatetime)
        //            .ToList();

        //    if (sWordsList != null || sWordsList.Count > 0)
        //    {
        //        foreach (var sword in sWordsList)
        //        {
        //            result = SameMethod.FuzzySensitive(result, sword.content);
        //        }
        //    }

        //    return result;
        //}

        
        public static bool IsSafeContent(string content)
        {
            //var sSentences = StaticTables.sdt_SensitiveSentences
            //       .Where(s => content == s.content)
            //       .FirstOrDefault();

            //if (sSentences != null && sSentences.state != 1)
            //{
            //    return false;
            //}

            var sWordsList = StaticTables.sdt_SensitiveWords
                    .Where(s => (content.Contains(s.content) && s.state == 0))
                    .OrderByDescending(s => s.state).ThenByDescending(s => s.updatetime)
                    .ToList();

            if (sWordsList.Count > 0 )
            {
                return false;
            }

            return true;
        }
    }
}