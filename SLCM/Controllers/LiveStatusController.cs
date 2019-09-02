using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SLCM.Models;
using SLCM.Controllers.Common;
using System.Text;

namespace SLCM.Controllers
{
    public class LiveStatusController : ApiController
    {
        public IHttpActionResult Post()
        {
            var request = HttpContext.Current.Request;

            try
            {
                var liveId = request.Form["LiveId"] ?? "";

                if (string.IsNullOrEmpty(liveId))
                    return BadRequest();

                using (var db = new livecloudEntities())
                {
                    var live = db.dt_liveList.Where(s => s.liveId == liveId).FirstOrDefault();

                    if (live == null)
                        return BadRequest();

                    var status = new LiveStatusModel();

                    status.LiveId = liveId;
                    status.Status = live.state;
                    status.CloseContent = (live.state == (byte)1) ? "" : live.CloseContent;
                    status.CloseTitle = (live.state == (byte)1) ? "" : live.CloseTitle;

                    return Ok(status);
                }
            }
            catch (Exception e)
            {
                Log.Error("LiveStatus", "LiveStatus 出錯", e.Message);
                //return BadRequest("LIVE_" + e.Message);
                return InternalServerError();
            }
        }
    }
}
