using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace SLCM.Models
{
    public class StaticTables
    {
        private static MemoryCache _Cache = new MemoryCache("DB_DATA_CACHE");
        private static object _lock1 = new object();
        private static object _lock2 = new object();


        /// <summary>
        /// 敏感句
        /// </summary>
        public static List<dt_SensitiveSentences> sdt_SensitiveSentences = null;

        /// <summary>
        /// 敏感字
        /// </summary>
        public static List<dt_SensitiveWords> sdt_SensitiveWords = null;

        /// <summary>
        /// 用戶禁言表
        /// </summary>
        public static List<dt_UserBarrageNoSpeak> sdt_UserBarrageNoSpeak = null;

        /// <summary>
        /// 黑詞
        /// </summary>
        public static List<dt_BlackWords> sdt_BlackWords = null;

    }
}