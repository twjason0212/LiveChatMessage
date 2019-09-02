using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace SLCM.Controllers.Common
{
    public class ChineseConverter
    {
        private const int LocaleSystemDefault = 0x0800;
        private const int LcmapSimplifiedChinese = 0x02000000;
        private const int LcmapTraditionalChinese = 0x04000000;

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LCMapString(int locale, int dwMapFlags, string lpSrcStr, int cchSrc,
                                              [Out] string lpDestStr, int cchDest);

        // 繁體轉成簡體
        public static string ToSimplified(string argSource)
        {
            var t = new String(' ', argSource.Length);
            LCMapString(LocaleSystemDefault, LcmapSimplifiedChinese, argSource, argSource.Length, t, argSource.Length);
            return t;
        }

        // 簡體轉成繁體
        public static string ToTraditional(string argSource)
        {
            var t = new String(' ', argSource.Length);
            LCMapString(LocaleSystemDefault, LcmapTraditionalChinese, argSource, argSource.Length, t, argSource.Length);
            return t;
        }
    }
}