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
    public class SendMessageController : ApiController
    {
        public int Post()
        {
            var request = HttpContext.Current.Request;

            try
            {

                var chatInfo = new LiveChatMessageInfo(request);

                //Log.Info("Barrage", "Barrage", chatInfo.UserName + "_" + chatInfo.ChatMessage);

                if (!chatInfo.ParametersIsValid)
                {
                    Log.Info("Barrage", "Barrage", "參數校驗失敗");
                    //return BadRequest("LIVEAPI_參數校驗失敗");
                    return -1;
                }
                else if (!chatInfo.GameIsExistsAndAvailable)
                {
                    Log.Info("Barrage", "Barrage", "Game Not Exists or Unavailable.");
                    //return BadRequest("LIVEAPI_直播不存在或停播中");
                    return -2;
                }
                //else if (chatInfo.IsSystemDefaultChatMessage && !chatInfo.SystemDefaultChatMessageIsExistsAndAvailable)
                //{
                //    Log.Info("Barrage", "Barrage", "SystemDefaultChatMessage Not Exists or Unavailable.");
                //    //return BadRequest("LIVEAPI_系統彈幕ID不存在或已停用");
                //    return -3;
                //}
                else
                {
                    Log.Info("SendBarrage", "SendBarrage", chatInfo.UserName + "_" + chatInfo.ChatMessage);

                    using (var client = new NoKeepAlivesWebClient() { Encoding = Encoding.UTF8 })
                    {
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        var message = new Message()
                        {
                            Target = chatInfo.Target,
                            GameID = chatInfo.GameID,
                            Data = new MessageData()
                            {
                                Level = chatInfo.UserGroup,
                                Message = chatInfo.ChatMessage,
                                NickName = (!string.IsNullOrEmpty(chatInfo.UserNickName) && SensitiveReplace.IsSafeContent(chatInfo.UserNickName))
                                    ? chatInfo.UserNickName
                                    : SameMethod.FuzzyName(chatInfo.UserName),
                                Type = "Barrage"
                            }
                        };
                        var data = "content=" + JsonConvert.SerializeObject(message);//.Replace("\"","");

                        var resultg = client.UploadString(Conf.WSUrl, "POST", data);
                        //var resultg = client.UploadString("", "POST", data);
                        Result result = JsonConvert.DeserializeObject<Result>(resultg.ToString());

                        Log.Info("Barrage", "Barrage 發給tony的結果", result.code.ToString());


                        if (result.code == 1)
                        {
                            //return Ok();
                            return 1;
                        }
                        else
                        {
                            //return BadRequest("LIVE_廣播失敗");
                            return 0;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Log.Info("Barrage", "Barrage 出錯", e.Message);
                //return BadRequest("LIVE_" + e.Message);
                return -10;
            }
        }
    }

}
