using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using SLCM.Models;
using SLCM.Controllers.Common;
using TestAPI20171114.Models;
using Newtonsoft.Json;

namespace SLCM
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            using (var db = new livecloudEntities())
            {
                // var SSS = (from dd in db.dt_liveList select dd);
                StaticTables.sdt_SensitiveSentences = (from p in db.dt_SensitiveSentences select p).ToList();
                StaticTables.sdt_SensitiveWords = (from p in db.dt_SensitiveWords select p).ToList();
                StaticTables.sdt_UserBarrageNoSpeak = (from p in db.dt_UserBarrageNoSpeak select p).ToList();
                StaticTables.sdt_BlackWords = (from p in db.dt_BlackWords select p).ToList();
            }

            ProcessBlockLogEmptyData();
        }

        private void ProcessBlockLogEmptyData()
        {

            using (var db = new livecloudEntities())
            {
                var blockLog1 = db.ChatMessageBlockLog.OrderBy(o => o.Id).FirstOrDefault();
                if (blockLog1 != null && string.IsNullOrEmpty(blockLog1.BlockWords))
                {
                    foreach (var blockLog in db.ChatMessageBlockLog)
                    {
                        if (string.IsNullOrEmpty(blockLog.BlockWords))
                            blockLog.BlockWords = GetBlockWords(db, blockLog.Content);
                    }

                    db.SaveChanges();
                }
            }
        }

        private string GetBlockWords(livecloudEntities db, string fullContent)
        {
            var sWords = db.dt_SensitiveWords
                        .Where(o => fullContent.Contains(o.content) && o.state == 0)
                        .FirstOrDefault();

            if (sWords != null)
                return sWords.content;

            var black = db.dt_SensitiveSentences
                            .Where(o => fullContent == o.content && o.state == 0)
                            .FirstOrDefault();

            if (black != null)
                return black.content;

            sWords = db.dt_SensitiveWords
                        .Where(o => fullContent.Contains(o.content) && o.state == 1)
                        .FirstOrDefault();

            if (sWords != null)
                return sWords.content;

            return "";
        }
    }
}
