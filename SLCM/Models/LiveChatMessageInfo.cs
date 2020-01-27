using SLCM.Controllers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Runtime.Caching;

namespace SLCM.Models
{
    public class LiveChatMessageInfo
    {
        public LiveChatMessageInfo(HttpRequest request)
        {
            _SensitiveWordsVerify = -1;
            try
            {
                Identityid = request.Form["Identityid"] ?? "";
                Target = request.Form["Target"] ?? "";
                GameID = request.Form["GameID"] ?? "";
                Type = request.Form["Type"] ?? "";
                ChatMessage = Regex.Replace(ChineseConverter.ToSimplified(request.Form["ChatMessage"] ?? ""), @"[,， \t]+", "");
                int userGroup = int.TryParse(request.Form["UserGroup"], out userGroup) ? userGroup : -32768;
                int minSendInterval = int.TryParse(request.Form["MinSendInterval"], out minSendInterval) ? minSendInterval : -32768;
                MinSendInterval = minSendInterval;
                UserGroup = userGroup;
                AllowUserGroups = request.Form["AllowUserGroups"] ?? "";
                UserName = request.Form["UserName"] ?? "";
                UserNickName = ChineseConverter.ToSimplified(request.Form["UserNickName"] ?? "");
                UserAvatar = request.Form["UserAvatar"] ?? "";
                CountryName = request.Form["CountryName"] ?? "";
                RegionName = request.Form["RegionName"] ?? "";
                CityName = request.Form["CityName"] ?? "";
                ClientIp = request.Form["ClientIp"] ?? "";
                IspDomain = request.Form["IspDomain"] ?? "";

                Log.Info("request", "request", request.Form.ToString());

            }
            catch
            { }
        }

        public string Identityid { get; set; }

        public int MinSendInterval { get; set; }

        public string GameID { get; set; }

        public string Target { get; set; }

        public string Type { get; set; }

        public string ChatMessage { get; set; }

        public int UserGroup { get; set; }

        public string AllowUserGroups { get; set; }

        public string UserName { get; set; }

        public string UserNickName { get; set; }

        public string UserAvatar { get; set; }
        public string CountryName { get; set; }
        public string RegionName { get; set; }
        public string CityName { get; set; }
        public string ClientIp { get; set; }
        public string IspDomain { get; set; }

        public bool ParametersIsValid
        {
            get
            {
                var pass = UserGroup > -2 &&
                        MinSendInterval >= 0 &&
                        !string.IsNullOrEmpty(ChatMessage) &&
                        !string.IsNullOrEmpty(GameID) &&
                        !string.IsNullOrEmpty(UserName);

                return pass;
            }
        }

        public bool GameIsExistsAndAvailable
        {
            //確認gameid存在且直播中
            get
            {
                using (var db = new livecloudEntities())
                {
                    var game = db.dt_liveList.Where(o => o.liveId == GameID && o.state == 1).FirstOrDefault();

                    return game != null;
                }
            }
        }

        public bool IsSystemDefaultChatMessage
        {
            get
            {
                return new Regex(@"^\#{2}[\d]{1,}\#{2}$").IsMatch(ChatMessage);
            }
        }

        public bool SystemDefaultChatMessageIsExistsAndAvailable
        {
            get
            {
                try
                {
                    using (var db = new livecloudEntities())
                    {
                        var reqSysBarrageId = int.Parse(ChatMessage.Replace("##", ""));

                        var sysBarrage = db.dt_SystemBarrage
                            .Where(s => s.id == reqSysBarrageId && s.state == 1)
                            .FirstOrDefault();

                        return sysBarrage != null;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// 用戶系統彈幕內容記錄至DB
        /// </summary>
        public void SystemBarrageLog()
        {
            using (var db = new livecloudEntities())
            {
                var sysbarrageId = int.Parse(ChatMessage.Replace("##", ""));

                var systembarrage = new dt_SystemBarrageLog
                {
                    barrageId = sysbarrageId,
                    operUser = UserName,
                    add_time = System.DateTime.Now,
                    signal = 0
                };

                db.dt_SystemBarrageLog.Add(systembarrage);

                var sysBarrageTimes = db.dt_SystemBarrageTimes.Find(sysbarrageId);

                if (sysBarrageTimes == null)
                {
                    sysBarrageTimes = new dt_SystemBarrageTimes()
                    {
                        systembarrageid = sysbarrageId,
                        times = 1,
                        updatetime = DateTime.Now
                    };

                    db.dt_SystemBarrageTimes.Add(sysBarrageTimes);
                }
                else
                {
                    var now = DateTime.Now;
                    var addDays = ((int)now.DayOfWeek == 0) //週日
                                     ? 6
                                     : (int)now.DayOfWeek - 1;
                    var startTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(-addDays); //該週周一凌晨12點

                    sysBarrageTimes.times = (sysBarrageTimes.updatetime >= startTime)
                        ? sysBarrageTimes.times + 1
                        : 1;

                    sysBarrageTimes.updatetime = now;
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 自由發言阻擋紀錄
        /// </summary>
        /// <param name="blockType">0:黑名单｜1:禁用词 | 2:人工審核</param>
        public void ChatBlockLog(int blockType, string fuckWords = "")
        {
            using (var db = new livecloudEntities())
            {
                var blockLog = new ChatMessageBlockLog
                {
                    UserName = UserName,
                    UserNickName = UserNickName ?? "",
                    State = blockType,
                    Content = ChatMessage,
                    BlockWords = fuckWords,
                    Time = DateTime.Now
                };

                db.ChatMessageBlockLog.Add(blockLog);

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 用戶自由聊天內容記錄至DB
        /// </summary>
        public void UserBarrageLog()
        {
            // 用戶彈幕內容記錄至DB
            using (var db = new livecloudEntities())
            {
                dt_UserBarrageLog userBarrageLog = new dt_UserBarrageLog
                {
                    UserName = this.UserName,
                    UserNickName = this.UserNickName,
                    ChatMessage = this.ChatMessage,
                    AddTime = DateTime.Now
                };
                db.dt_UserBarrageLog.Add(userBarrageLog);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 確認用戶是否禁言
        /// </summary>
        /// <returns>-7:无法自由发言;-8:无法发送弹幕;-9:无法发布信息</returns>
        public int BlockSpeakDays()
        {
            //Duration  时长值，1(一天)，7(一周)，0(永久)
            //type：0(全部)，1(自由发言)，2(系统弹幕)
            var blockSpeak = StaticTables.sdt_UserBarrageNoSpeak
                                                .FindAll(o => o.UserName == UserName)
                                                .OrderByDescending(o => o.id)
                                                .FirstOrDefault();

            //if (blockSpeak != null && (blockSpeak.AddTime.Value.AddDays(blockSpeak.Duration.Value) > DateTime.Now || blockSpeak.Duration == 0) && blockSpeak.Type == 1 && !SystemDefaultChatMessageIsExistsAndAvailable)
            //{
            //    result = -7; // 您的账户暂时无法自由发言！
            //}
            //else if (blockSpeak != null && (blockSpeak.AddTime.Value.AddDays(blockSpeak.Duration.Value) > DateTime.Now || blockSpeak.Duration == 0) && blockSpeak.Type == 2 && SystemDefaultChatMessageIsExistsAndAvailable)
            //{
            //    result = -8; // 您的账户暂时无法发送弹幕！
            //}
            //else if (blockSpeak != null && (blockSpeak.AddTime.Value.AddDays(blockSpeak.Duration.Value) > DateTime.Now || blockSpeak.Duration == 0) && blockSpeak.Type == 0)
            //{
            //    result = -9; //您的账户暂时无法发布信息！
            //}

            if (blockSpeak != null)
            {
                if (blockSpeak.Duration.Value == 0)
                    return -1;

                var unblockDT = blockSpeak.AddTime.Value.AddDays(blockSpeak.Duration.Value);

                if (unblockDT > DateTime.Now)
                {
                    //var needWaitDays = Math.Ceiling((unblockDT - DateTime.Now).TotalDays);
                    //return Convert.ToInt32(needWaitDays);
                    return blockSpeak.Duration.Value;
                }
            }

            return 0;
        }

        public int AddNoSpeak(int days)
        {
            int result = 0;

            using (livecloudEntities db = new livecloudEntities())
            {
                dt_UserBarrageNoSpeak dt_UserBarrageNoSpeak = new dt_UserBarrageNoSpeak()
                {
                    identityid = Identityid,
                    UserName = UserName,
                    UserNickName = string.IsNullOrEmpty(UserNickName) ? "" : UserNickName,
                    Remark = ChatMessage,
                    Type = 0,
                    Duration = days,
                    AddTime = DateTime.Now,
                    OperateUser = "UUSystem"
                };

                db.dt_UserBarrageNoSpeak.Add(dt_UserBarrageNoSpeak);
                db.SaveChanges();
                //更新NoSpeak緩存
                StaticTables.sdt_UserBarrageNoSpeak = (from p in db.dt_UserBarrageNoSpeak select p).OrderByDescending(p => p.lockid).ToList();
            }

            return result;
        }

        private bool _BlackWordsVerify { get; set; }
        public bool BlackWords
        {
            get
            {
                string content = ChatMessage;
                var ss = StaticTables.sdt_BlackWords.Where(o => o.content == content).ToList();

                if (ss.Count > 0)
                    _BlackWordsVerify = true;
                else
                    _BlackWordsVerify = false;

                return _BlackWordsVerify;
            }
        }

        private int _SensitiveWordsVerify { get; set; }
        public bool SensitiveWordsVerify
        {
            //後續需要進行快取

            //優先順序
            //1禁用句
            //2禁用詞
            //3白名單句
            //4敏感詞
            get
            {
                if (_SensitiveWordsVerify >= 0)
                    return _SensitiveWordsVerify > 0;

                using (var db = new livecloudEntities())
                {
                    string content = ChatMessage;

                    //句子整句match
                    var sSentences = StaticTables.sdt_SensitiveSentences
                        .Where(s => s.content == content)
                        .FirstOrDefault();

                    if (sSentences != null && sSentences.state != 1)
                    {
                        //SensitiveSentences State = 1(白名單)/0(黑名單)
                        WaitToManualReview = false;
                        BlockWords = sSentences.content;
                        _SensitiveWordsVerify = 0;

                        ChatBlockLog(0, sSentences.content);

                        return _SensitiveWordsVerify > 0;
                    }

                    var sWordsList = StaticTables.sdt_SensitiveWords
                        .Where(s => content.Contains(s.content))
                        .OrderByDescending(s => s.state).ThenByDescending(s => s.updatetime)
                        .ToList();

                    if (sWordsList != null && sWordsList.Count > 0)
                    {
                        foreach (var sword in sWordsList.Where(w => w.state == 0))//禁用
                        {
                            WaitToManualReview = false;
                            _SensitiveWordsVerify = 0;
                            BlockWords = sword.content;

                            ChatBlockLog(1, sword.content);

                            return _SensitiveWordsVerify > 0;
                        }
                    }

                    if (sSentences != null && sSentences.state == 1)
                    {
                        _SensitiveWordsVerify = 1;
                        return _SensitiveWordsVerify > 0;
                    }

                    if (sWordsList != null && sWordsList.Count > 0)
                    {
                        foreach (var sword in sWordsList.Where(w => w.state == 1))//敏感, 進入人工審核流程
                        {

                            WaitToManualReview = true;
                            _SensitiveWordsVerify = 0;
                            BlockWords = sword.content;

                            var manualReview = new dt_ManualReview()
                            {
                                ManagerID = 0,
                                AddTime = DateTime.Now,
                                SendTime = DateTime.Now,
                                Content = ChatMessage,
                                SensitiveWords = sword.content,
                                State = 0,
                                UserName = UserName,
                                UserNickName = UserNickName,
                                UserLevel = UserGroup,
                                GameID = GameID,
                                Target = Target,
                                IdentityId = Identityid,
                                Area = string.Format($"{CountryName} {RegionName} {CityName}"),
                                Ip = ClientIp,
                                IspDomain = IspDomain,
                            };
                            db.dt_ManualReview.Add(manualReview);

                            db.SaveChanges();

                            ChatBlockLog(2, sword.content);

                            return _SensitiveWordsVerify > 0;
                        }
                    }

                    _SensitiveWordsVerify = 1;  // 沒有命中任何 敏感句子 / 敏感字
                }

                return _SensitiveWordsVerify > 0;
            }
        }

        private static MemoryCache cacheMinSendInterval = new MemoryCache("IsMinSendInterval");
        public bool InMinSendInterval
        {
            get
            {
                string cacheKey = UserName + UserNickName;
                if (!cacheMinSendInterval.Add(cacheKey, ChatMessage, DateTimeOffset.Now.AddSeconds(MinSendInterval)))
                {
                    return false;
                }
                return true;
            }
        }

        public bool WaitToManualReview { get; set; }

        public string BlockWords { get; set; }

    }
}