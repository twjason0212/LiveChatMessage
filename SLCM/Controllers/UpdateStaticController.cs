using Newtonsoft.Json;
using SLCM.Controllers.Common;
using SLCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SLCM.Controllers
{
    public class UpdateStaticController : ApiController
    {
        public bool Post()
        {
            bool flog = false;
            var request = HttpContext.Current.Request;

            try
            {
                String Msg = request.Form["msg"] ?? "";
                int o_count = 0;
                string o_json = "";
                int n_count = 0;
                string n_json = "";

                using (var db = new livecloudEntities())
                {
                    switch (Msg)
                    {
                        case "dt_SensitiveSentences":
                            o_count = StaticTables.sdt_SensitiveSentences.Count();
                            db.Configuration.LazyLoadingEnabled = false;
                            StaticTables.sdt_SensitiveSentences = db.dt_SensitiveSentences.AsNoTracking().ToList()
                                .OrderByDescending(s => s.id).ToList();
                            n_count = StaticTables.sdt_SensitiveSentences.Count();
                            break;

                        case "dt_SensitiveWords":
                            o_count = StaticTables.sdt_SensitiveWords.Count();
                            db.Configuration.LazyLoadingEnabled = false;
                            StaticTables.sdt_SensitiveWords = db.dt_SensitiveWords.AsNoTracking().ToList()
                                .OrderByDescending(p => p.id).ToList();
                            n_count = StaticTables.sdt_SensitiveWords.Count();
                            break;

                        case "dt_UserBarrageNoSpeak":
                            o_count = StaticTables.sdt_UserBarrageNoSpeak.Count();
                            db.Configuration.LazyLoadingEnabled = false;
                            StaticTables.sdt_UserBarrageNoSpeak = db.dt_UserBarrageNoSpeak.AsNoTracking().ToList()
                                .OrderByDescending(p => p.id).ToList();
                            n_count = StaticTables.sdt_UserBarrageNoSpeak.Count();
                            break;

                        case "dt_BlackWords":
                            o_count = StaticTables.sdt_BlackWords.Count();
                            db.Configuration.LazyLoadingEnabled = false;
                            StaticTables.sdt_BlackWords = db.dt_BlackWords.AsNoTracking().ToList()
                                .OrderByDescending(p => p.id).ToList();
                            n_count = StaticTables.sdt_BlackWords.Count();
                            break;
                    }

                    Log.CreateErrorLogTxt("RedisSubscribeUpdateStatic", Msg, "RedisSubscribeGetMsg Time: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Msg + "\r\n old:" + o_count + " \r\n\r\n new:" + n_count + "");

                }
                flog = true;
            }
            catch(Exception ex)
            {
                flog = false;
                Log.Error("UpdateStaticController", "UpdateStaticController.Post", ex.ToString());
            }
            return flog;

        }
    }
}
