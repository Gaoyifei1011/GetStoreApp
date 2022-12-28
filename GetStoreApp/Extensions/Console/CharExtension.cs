using GetStoreApp.Properties;

namespace GetStoreApp.Extensions.Console
{
    /// <summary>
    /// 字符类型扩展
    /// </summary>
    public static class CharExtension
    {
        private static byte[] Lengths { get; } = Resources.Lengths;

        /// <summary>
        /// 判断该字符在控制台显示的实际宽度是否大于1
        /// </summary>
        public static bool IsWideDisplayChar(char c)
        {
            return GetCharDisplayLength(c) > 1;
        }

        /// <summary>
        /// 获取该字符在控制台显示的实际宽度
        /// </summary>
        public static int GetCharDisplayLength(char c)
        {
            int index = c;
            if (index < 8800)
            {
                if (index < 8246)
                {
                    if (index < 913)
                    {
                        if (index < 183)
                        {
                            if (index < 166)
                            {
                                if (index < 11)
                                {
                                    if (index < 9)
                                    {
                                        if (index < 7)
                                            return 1;
                                        else
                                            return 0;
                                    }
                                    else
                                    {
                                        if (index < 10)
                                            return 8;
                                        else
                                            return 0;
                                    }
                                }
                                else
                                {
                                    if (index < 14)
                                    {
                                        if (index < 13)
                                            return 1;
                                        else
                                            return 0;
                                    }
                                    else
                                    {
                                        if (index < 162)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 178)
                                {
                                    if (index < 169)
                                    {
                                        if (index < 167)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 175)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 182)
                                    {
                                        if (index < 180)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (index < 450)
                            {
                                if (index < 247)
                                {
                                    if (index < 215)
                                    {
                                        if (index < 184)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 216)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 449)
                                    {
                                        if (index < 248)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 716)
                                {
                                    if (index < 712)
                                    {
                                        if (index < 711)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 713)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 730)
                                    {
                                        if (index < 729)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (index < 8209)
                        {
                            if (index < 1025)
                            {
                                if (index < 945)
                                {
                                    if (index < 931)
                                    {
                                        if (index < 930)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 938)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 963)
                                    {
                                        if (index < 962)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 970)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 1105)
                                {
                                    if (index < 1040)
                                    {
                                        if (index < 1026)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 1104)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 8208)
                                    {
                                        if (index < 1106)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (index < 8229)
                            {
                                if (index < 8218)
                                {
                                    if (index < 8215)
                                    {
                                        if (index < 8211)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 8216)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 8222)
                                    {
                                        if (index < 8220)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 8242)
                                {
                                    if (index < 8240)
                                    {
                                        if (index < 8231)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 8241)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 8245)
                                    {
                                        if (index < 8244)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (index < 8721)
                    {
                        if (index < 8481)
                        {
                            if (index < 8452)
                            {
                                if (index < 8255)
                                {
                                    if (index < 8252)
                                    {
                                        if (index < 8251)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 8254)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 8365)
                                    {
                                        if (index < 8364)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 8451)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 8458)
                                {
                                    if (index < 8454)
                                    {
                                        if (index < 8453)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 8457)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 8471)
                                    {
                                        if (index < 8470)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (index < 8596)
                            {
                                if (index < 8560)
                                {
                                    if (index < 8544)
                                    {
                                        if (index < 8482)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 8556)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 8592)
                                    {
                                        if (index < 8570)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 8713)
                                {
                                    if (index < 8602)
                                    {
                                        if (index < 8598)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 8712)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 8720)
                                    {
                                        if (index < 8719)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (index < 8743)
                        {
                            if (index < 8731)
                            {
                                if (index < 8728)
                                {
                                    if (index < 8725)
                                    {
                                        if (index < 8722)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 8726)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 8730)
                                    {
                                        if (index < 8729)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 8740)
                                {
                                    if (index < 8737)
                                    {
                                        if (index < 8733)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 8739)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 8742)
                                    {
                                        if (index < 8741)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (index < 8766)
                            {
                                if (index < 8756)
                                {
                                    if (index < 8750)
                                    {
                                        if (index < 8748)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 8751)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 8764)
                                    {
                                        if (index < 8760)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 8781)
                                {
                                    if (index < 8777)
                                    {
                                        if (index < 8776)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 8780)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 8787)
                                    {
                                        if (index < 8786)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (index < 12549)
                {
                    if (index < 9676)
                    {
                        if (index < 8979)
                        {
                            if (index < 8857)
                            {
                                if (index < 8814)
                                {
                                    if (index < 8804)
                                    {
                                        if (index < 8802)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 8808)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 8853)
                                    {
                                        if (index < 8816)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 8854)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 8895)
                                {
                                    if (index < 8869)
                                    {
                                        if (index < 8858)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 8870)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 8978)
                                    {
                                        if (index < 8896)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (index < 9650)
                            {
                                if (index < 9372)
                                {
                                    if (index < 9322)
                                    {
                                        if (index < 9312)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 9332)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 9634)
                                    {
                                        if (index < 9632)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 9670)
                                {
                                    if (index < 9660)
                                    {
                                        if (index < 9652)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 9662)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 9675)
                                    {
                                        if (index < 9672)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (index < 12293)
                        {
                            if (index < 9738)
                            {
                                if (index < 9702)
                                {
                                    if (index < 9680)
                                    {
                                        if (index < 9678)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 9698)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 9735)
                                    {
                                        if (index < 9733)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 9737)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 9795)
                                {
                                    if (index < 9793)
                                    {
                                        if (index < 9792)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 9794)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 12292)
                                    {
                                        if (index < 12288)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (index < 12436)
                            {
                                if (index < 12321)
                                {
                                    if (index < 12317)
                                    {
                                        if (index < 12312)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 12319)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 12353)
                                    {
                                        if (index < 12330)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 12535)
                                {
                                    if (index < 12447)
                                    {
                                        if (index < 12443)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 12449)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 12543)
                                    {
                                        if (index < 12540)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (index < 55297)
                    {
                        if (index < 13215)
                        {
                            if (index < 12959)
                            {
                                if (index < 12832)
                                {
                                    if (index < 12690)
                                    {
                                        if (index < 12586)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 12704)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 12928)
                                    {
                                        if (index < 12868)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 12958)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 13198)
                                {
                                    if (index < 12969)
                                    {
                                        if (index < 12964)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 12977)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 13212)
                                    {
                                        if (index < 13200)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (index < 13265)
                            {
                                if (index < 13253)
                                {
                                    if (index < 13218)
                                    {
                                        if (index < 13217)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 13252)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 13263)
                                    {
                                        if (index < 13262)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 19968)
                                {
                                    if (index < 13269)
                                    {
                                        if (index < 13267)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 13270)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 55296)
                                    {
                                        if (index < 40870)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        return 0;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (index < 65093)
                        {
                            if (index < 63733)
                            {
                                if (index < 59335)
                                {
                                    if (index < 56321)
                                    {
                                        if (index < 56320)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 57344)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 59493)
                                    {
                                        if (index < 59337)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 65072)
                                {
                                    if (index < 63744)
                                    {
                                        if (index < 63734)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 64046)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 65075)
                                    {
                                        if (index < 65074)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (index < 65128)
                            {
                                if (index < 65112)
                                {
                                    if (index < 65107)
                                    {
                                        if (index < 65097)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        if (index < 65108)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                }
                                else
                                {
                                    if (index < 65127)
                                    {
                                        if (index < 65113)
                                            return 1;
                                        else
                                            return 2;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                            else
                            {
                                if (index < 65504)
                                {
                                    if (index < 65281)
                                    {
                                        if (index < 65132)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        if (index < 65375)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                }
                                else
                                {
                                    if (index < 65536)
                                    {
                                        if (index < 65510)
                                            return 2;
                                        else
                                            return 1;
                                    }
                                    else
                                    {
                                        return 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取字符串在控制台显示的实际宽度
        /// </summary>
        public static int GetStringDisplayLength(string str)
        {
            int total = 0;
            foreach (char c in str)
            {
                total += GetCharDisplayLength(c);
            }
            return total;
        }

        /// <summary>
        /// 判断该字符在控制台显示的实际宽度是否大于1（速度更快版本）
        /// </summary>
        public static bool IsWideDisplayCharEx(char c)
        {
            int index = c;
            return (Lengths[index / 8] & (1 << (index % 8))) != 0;
        }

        /// <summary>
        /// 获取该字符在控制台显示的实际宽度（速度更快版本）
        /// </summary>
        public static int GetCharDisplayLengthEx(char c)
        {
            return IsWideDisplayCharEx(c) ? 2 : 1;
        }

        /// <summary>
        /// 获取字符串在控制台显示的实际宽度（速度更快版本）
        /// </summary>
        public static int GetStringDisplayLengthEx(string str)
        {
            int total = 0;
            foreach (char c in str)
                total += GetCharDisplayLengthEx(c);
            return total;
        }
    }
}
