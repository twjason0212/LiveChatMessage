using SLCM.Controllers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLCM.Models
{
    public class RewardGiftInfo
    {
        public RewardGiftInfo(HttpRequest request)
        {
            //_RewardVerify = -1;
            try
            {
                int anchorid = int.TryParse(request.Form["AnchorId"], out anchorid) ? anchorid : 0;
                int combocount = int.TryParse(request.Form["ComboCount"], out combocount) ? combocount : 0;
                decimal giftprice = decimal.TryParse(request.Form["GiftPrice"], out giftprice) ? giftprice : 0;
                int userid = int.TryParse(request.Form["UserId"], out userid) ? userid : 0;
                int userlevel = int.TryParse(request.Form["UserLevel"], out userlevel) ? userlevel : 0;

                GameId = request.Form["GameId"] ?? "";
                AnchorId = anchorid;
                AnchorName = request.Form["AnchorName"] ?? "";
                ComboCount = combocount;
                GiftName = request.Form["GiftName"] ?? "";
                GiftPrice = giftprice;
                UserId = userid;
                UserName = request.Form["UserName"] ?? "";
                UserNickName = ChineseConverter.ToSimplified(request.Form["UserNickName"] ?? "");
                UserLevel = userlevel;
                UserAvatar = request.Form["UserAvatar"] ?? "";
                Identityid = request.Form["Identityid"] ?? "";
                ClientSource = request.Form["ClientSource"] ?? "";

            }
            catch
            { }
        }

        public string GameId { get; set; }

        public int AnchorId { get; set; }

        public string AnchorName { get; set; }

        public int ComboCount { get; set; }

        public string GiftName { get; set; }

        public decimal GiftPrice { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserNickName { get; set; }

        public int UserLevel { get; set; }

        public string UserAvatar { get; set; }

        public string Identityid { get; set; }

        public string ClientSource { get; set; }

        //private int _RewardVerify { get; set; }
        //public bool RewardVerify
        //{
        //    //後續需要進行快取

        //    //優先順序
        //    //1禁用句
        //    //2禁用詞
        //    //3白名單句
        //    //4敏感詞
        //    get
        //    {
        //        if (_RewardVerify >= 0)
        //            return _RewardVerify > 0;


        //        //句子整句match
        //        var sSentences = StaticTables.sdt_SensitiveSentences
        //            .Where(s => (UserNickName.Contains(s.content)))
        //            .FirstOrDefault();

        //        if (sSentences != null && sSentences.state != 1)
        //        {
        //            //SensitiveSentences State = 1(白名單)/0(黑名單)
        //            BlockWords = sSentences.content;
        //            _RewardVerify = 0;

        //            return _RewardVerify > 0;
        //        }

        //        var sWordsList = StaticTables.sdt_SensitiveWords
        //            .Where(s => (UserNickName.Contains(s.content)))
        //            .OrderByDescending(s => s.state).ThenByDescending(s => s.updatetime)
        //            .ToList();

        //        if (sWordsList != null && sWordsList.Count > 0)
        //        {
        //            foreach (var sword in sWordsList.Where(w => w.state == 0))//禁用
        //            {
        //                _RewardVerify = 0;
        //                BlockWords = sword.content;

        //            }
        //            return _RewardVerify > 0;
        //        }

        //        if (sSentences != null && sSentences.state == 1)
        //        {
        //            _RewardVerify = 1;
        //            return _RewardVerify > 0;
        //        }

        //        if (sWordsList != null && sWordsList.Count > 0)
        //        {
        //            foreach (var sword in sWordsList.Where(w => w.state == 1))
        //            {
        //                _RewardVerify = 0;
        //                BlockWords = sword.content;

        //            }
        //            return _RewardVerify > 0;
        //        }

        //        _RewardVerify = 1;  // 沒有命中任何 敏感句子 / 敏感字


        //        return _RewardVerify > 0;
        //    }
        //}

        //public string BlockWords { get; set; }

        public bool ParametersIsValid
        {
            get
            {
                var pass = GiftPrice > 0 && UserLevel > -2 &&
                        AnchorId > 0 && ComboCount >= 0 &&
                        !string.IsNullOrEmpty(AnchorName) &&
                        !string.IsNullOrEmpty(GameId)
                         && !string.IsNullOrEmpty(GiftName);

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
                    var game = db.dt_liveList.Where(o => o.liveId == GameId && o.state == 1).FirstOrDefault();

                    return game != null;
                }
            }
        }
    }
}