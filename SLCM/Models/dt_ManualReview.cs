//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SLCM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class dt_ManualReview
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string SensitiveWords { get; set; }
        public string UserName { get; set; }
        public System.DateTime SendTime { get; set; }
        public System.DateTime AddTime { get; set; }
        public int ManagerID { get; set; }
        public byte State { get; set; }
        public string UserNickName { get; set; }
        public string Target { get; set; }
        public string GameID { get; set; }
        public Nullable<int> UserLevel { get; set; }
        public string IdentityId { get; set; }
    }
}
