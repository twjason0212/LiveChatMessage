using Newtonsoft.Json;
using SLCM.Controllers.Common;
using SLCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace SLCM.Controllers
{
    public class RewardController : ApiController
    {
        public IHttpActionResult Post()
        {
            var request = HttpContext.Current.Request;

            try
            {
                var RewardInfo = new RewardGiftInfo(request);

                if (!RewardInfo.ParametersIsValid)
                {
                    Log.Info("Reward", "Reward", "參數校驗失敗");
                    return BadRequest("LIVEAPI_參數校驗失敗");
                }
                else if (!RewardInfo.GameIsExistsAndAvailable)
                {
                    Log.Info("Reward", "Reward", "Game Not Exists or Unavailable.");
                    return BadRequest("LIVEAPI_直播不存在或停播中");
                }
                else
                {
                    Log.Info("Reward", "Reward", RewardInfo.UserName + "_");

                    //if (!RewardInfo.RewardVerify)
                    //{
                    //    Log.Info("Reward", "Reward", "Mark:" + RewardInfo.UserNickName);
                    //}
                    using (var client = new NoKeepAlivesWebClient() { Encoding = Encoding.UTF8 })
                    {
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        var message = new
                        {
                            Target = "dafa-" + RewardInfo.Identityid,
                            GameID = RewardInfo.GameId,
                            Data = new
                            {
                                Type = "Reward",
                                UserName = SameMethod.FuzzyName(RewardInfo.UserName),
                                NickName = (!string.IsNullOrEmpty(RewardInfo.UserNickName) && SensitiveReplace.IsSafeContent(RewardInfo.UserNickName))
                                    ? RewardInfo.UserNickName
                                    : "",
                                UserPhoto = RewardInfo.UserAvatar,
                                GiftName = RewardInfo.GiftName,
                                Combo = RewardInfo.ComboCount,
                                GiftPrice = RewardInfo.GiftPrice
                            }
                        };
                        var data = "content=" + JsonConvert.SerializeObject(message);//.Replace("\"","");

                        var resultg = client.UploadString(Conf.WSUrl, "POST", data);
                        Result result = JsonConvert.DeserializeObject<Result>(resultg.ToString());

                        Log.Info("Reward", "Reward 發給tony的結果", result.code.ToString());
                        Log.Info("Target", "Reward", "username:"+ RewardInfo.UserName +"    "+ data);

                        if (result.code == 1)
                            return Ok(1);
                        else
                            return BadRequest("LIVE_廣播失敗");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Info("Reward", "Reward 出錯", e.Message);
                return BadRequest("LIVE_" + e.Message);
            }
        }
    }
}
