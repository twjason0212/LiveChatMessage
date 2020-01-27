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
using System.Text.RegularExpressions;

namespace SLCM.Controllers
{
    public class SendLiveChatMessageController : ApiController
    {
        public string Post()
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
                    return "-1";
                }
                else if (!chatInfo.GameIsExistsAndAvailable)
                {
                    Log.Info("Barrage", "Barrage", "Game Not Exists or Unavailable.");
                    //return BadRequest("LIVEAPI_直播不存在或停播中");
                    return "-2";
                }
                //else if (chatInfo.IsSystemDefaultChatMessage && !chatInfo.SystemDefaultChatMessageIsExistsAndAvailable)
                //{
                //    Log.Info("Barrage", "Barrage", "SystemDefaultChatMessage Not Exists or Unavailable.");
                //    //return BadRequest("LIVEAPI_系統彈幕ID不存在或已停用");
                //    return -3;
                //}
                else if (chatInfo.BlockSpeakDays() != 0)
                {
                    //if (chatInfo.BlockSpeakDays() < 0)
                    //    return "-8,-1";

                    return "-8,"+ chatInfo.BlockSpeakDays().ToString();
                }
                else if (chatInfo.BlackWords && chatInfo.BlockSpeakDays() == 0)
                {
                    //符合黑詞且未被禁言
                    Log.Info("Barrage", "Barrage", "BlackWord:" + chatInfo.UserName);
                    chatInfo.AddNoSpeak(7);
                    return "-8,7";
                }
                else if (!chatInfo.InMinSendInterval)
                {
                    Log.Info("Barrage", "Barrage", "NotInMinSendInterval:" + chatInfo.UserName);
                    return "-6";
                }
                else if (!chatInfo.SensitiveWordsVerify)
                {
                    if (!chatInfo.WaitToManualReview)
                    {
                        Log.Info("Barrage", "Barrage", "Words block:" + chatInfo.BlockWords);
                        //return BadRequest("LIVEAPI_發言包含禁用詞:" + chatInfo.BlockWords);
                        return "-4";
                    }

                    Log.Info("Barrage", "Barrage", "Waitfor Manual Review:" + chatInfo.BlockWords);
                    //return BadRequest("LIVEAPI_發言包含敏感詞:" + chatInfo.BlockWords + ", 等待人工審核");
                    return "-5";
                }              
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
                                Type = "Barrage",
                                UserAvatar = chatInfo.UserAvatar,
                            }
                        };
                        var data = "content=" + JsonConvert.SerializeObject(message);//.Replace("\"","");

                        Log.TraceLog("target_SendBarrageToTony", data);
                        Log.Info("Target", "Reward", "username:" + chatInfo.UserName + "    " + data);

                        // 發送聊天彈幕到Tony_WS
                        var resultg = client.UploadString(Conf.WSUrl, "POST", data);

                        //var resultg = client.UploadString("", "POST", data);
                        Result result = JsonConvert.DeserializeObject<Result>(resultg.ToString());

                        Log.Info("Barrage", "Tony Response code: ", result.code.ToString());

                        if (result.code == 1)
                        {
                            //發送彈幕成功
                            if (chatInfo.IsSystemDefaultChatMessage && chatInfo.SystemDefaultChatMessageIsExistsAndAvailable)
                            {
                                chatInfo.SystemBarrageLog();
                            }
                            else
                                chatInfo.UserBarrageLog();
                            //return Ok();
                            return "1";
                        }
                        else
                        {
                            //return BadRequest("LIVE_廣播失敗");
                            return "0";
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Log.Info("Barrage", "Barrage 出錯", e.Message);
                //return BadRequest("LIVE_" + e.Message);
                return "-10";
            }
        }
    }


    public class NoKeepAlivesWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                ((HttpWebRequest)request).KeepAlive = false;
            }

            return request;
        }
    }
}
