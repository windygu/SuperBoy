using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core.Common
{
    public class BarCodeToHtml
    {
        public static string Get39(string s, int width, int height)
        {
            var ht = new Hashtable();

            #region 39码 12位
            ht.Add('A', "110101001011");
            ht.Add('B', "101101001011");
            ht.Add('C', "110110100101");
            ht.Add('D', "101011001011");
            ht.Add('E', "110101100101");
            ht.Add('F', "101101100101");
            ht.Add('G', "101010011011");
            ht.Add('H', "110101001101");
            ht.Add('I', "101101001101");
            ht.Add('J', "101011001101");
            ht.Add('K', "110101010011");
            ht.Add('L', "101101010011");
            ht.Add('M', "110110101001");
            ht.Add('N', "101011010011");
            ht.Add('O', "110101101001");
            ht.Add('P', "101101101001");
            ht.Add('Q', "101010110011");
            ht.Add('R', "110101011001");
            ht.Add('S', "101101011001");
            ht.Add('T', "101011011001");
            ht.Add('U', "110010101011");
            ht.Add('V', "100110101011");
            ht.Add('W', "110011010101");
            ht.Add('X', "100101101011");
            ht.Add('Y', "110010110101");
            ht.Add('Z', "100110110101");
            ht.Add('0', "101001101101");
            ht.Add('1', "110100101011");
            ht.Add('2', "101100101011");
            ht.Add('3', "110110010101");
            ht.Add('4', "101001101011");
            ht.Add('5', "110100110101");
            ht.Add('6', "101100110101");
            ht.Add('7', "101001011011");
            ht.Add('8', "110100101101");
            ht.Add('9', "101100101101");
            ht.Add('+', "100101001001");
            ht.Add('-', "100101011011");
            ht.Add('*', "100101101101");
            ht.Add('/', "100100101001");
            ht.Add('%', "101001001001");
            ht.Add('$', "100100100101");
            ht.Add('.', "110010101101");
            ht.Add(' ', "100110101101");
            #endregion

            #region 39码 9位
            //ht.Add('0', "000110100");
            //ht.Add('1', "100100001");
            //ht.Add('2', "001100001");
            //ht.Add('3', "101100000");
            //ht.Add('4', "000110001");
            //ht.Add('5', "100110000");
            //ht.Add('6', "001110000");
            //ht.Add('7', "000100101");
            //ht.Add('8', "100100100");
            //ht.Add('9', "001100100");
            //ht.Add('A', "100001001");
            //ht.Add('B', "001001001");
            //ht.Add('C', "101001000");
            //ht.Add('D', "000011001");
            //ht.Add('E', "100011000");
            //ht.Add('F', "001011000");
            //ht.Add('G', "000001101");
            //ht.Add('H', "100001100");
            //ht.Add('I', "001001100");
            //ht.Add('J', "000011100");
            //ht.Add('K', "100000011");
            //ht.Add('L', "001000011");
            //ht.Add('M', "101000010");
            //ht.Add('N', "000010011");
            //ht.Add('O', "100010010");
            //ht.Add('P', "001010010");
            //ht.Add('Q', "000000111");
            //ht.Add('R', "100000110");
            //ht.Add('S', "001000110");
            //ht.Add('T', "000010110");
            //ht.Add('U', "110000001");
            //ht.Add('V', "011000001");
            //ht.Add('W', "111000000");
            //ht.Add('X', "010010001");
            //ht.Add('Y', "110010000");
            //ht.Add('Z', "011010000");
            //ht.Add('-', "010000101");
            //ht.Add('.', "110000100");
            //ht.Add(' ', "011000100");
            //ht.Add('*', "010010100");
            //ht.Add('$', "010101000");
            //ht.Add('/', "010100010");
            //ht.Add('+', "010001010");
            //ht.Add('%', "000101010");
            #endregion

            s = "*" + s.ToUpper() + "*";

            var resultBin = "";//二进制串

            try
            {
                foreach (var ch in s)
                {
                    resultBin += ht[ch].ToString();
                    resultBin += "0";//间隔，与一个单位的线条宽度相等
                }
            }
            catch { return "存在不允许的字符！"; }

            var resultHtml = resultBin.Select(c => c == '0' ? "#FFFFFF" : "#000000").Aggregate("", (current, color) => current + ("<div style=\"width:" + width + "px;height:" + height + "px;float:left;background:" + color + ";\"></div>")); //HTML代码
            resultHtml += "<div style=\"clear:both\"></div>";

            var len = ht['*'].ToString().Length;
            resultHtml = s.Aggregate(resultHtml, (current, c) => current + ("<div style=\"width:" + (width*(len + 1)) + "px;float:left;color:#000000;text-align:center;\">" + c + "</div>"));
            resultHtml += "<div style=\"clear:both\"></div>";

            return "<div style=\"background:#FFFFFF;padding:5px;font-size:" + (width * 10) + "px;font-family:'楷体';\">" + resultHtml + "</div>";
        }

        public static string GetEan13(string s, int width, int height)
        {
            var checkcodeInput = -1;//输入的校验码
            if (!Regex.IsMatch(s, @"^\d{12}$"))
            {
                if (!Regex.IsMatch(s, @"^\d{13}$"))
                {
                    return "存在不允许的字符！";
                }
                else
                {
                    checkcodeInput = int.Parse(s[12].ToString());
                    s = s.Substring(0, 12);
                }
            }
            
            var sumEven = 0;//偶数位之和
            var sumOdd = 0; //奇数位之和

            for (var i = 0; i < 12; i++)
            {
                if (i % 2 == 0)
                {
                    sumOdd += int.Parse(s[i].ToString());
                }
                else
                {
                    sumEven += int.Parse(s[i].ToString());
                }
            }

            var checkcode = (10 - (sumEven * 3 + sumOdd) % 10) % 10;//校验码

            if (checkcodeInput > 0 && checkcodeInput != checkcode)
            {
                return "输入的校验码错误！";
            }

            s += checkcode;//变成13位

            // 000000000101左侧42个01010右侧35个校验7个101000000000
            // 6        101左侧6位 01010右侧5位校验1位101000000000

            var resultBin = "";//二进制串
            resultBin += "000000000101";

            var type = Ean13Type(s[0]);
            for (var i = 1; i < 7; i++)
            {
                resultBin += Ean13(s[i], type[i - 1]);
            }
            resultBin += "01010";
            for (var i = 7; i < 13; i++)
            {
                resultBin += Ean13(s[i], 'C');
            }
            resultBin += "101000000000";

            var heightBottom = width * 5;
            var resultHtml = resultBin.Select(c => c == '0' ? "#FFFFFF" : "#000000").Aggregate("", (current, color) => current + ("<div style=\"width:" + width + "px;height:" + height + "px;float:left;background:" + color + ";\"></div>")); //HTML代码
            resultHtml += "<div style=\"clear:both\"></div>";

            resultHtml += "<div style=\"float:left;color:#000000;width:" + (width * 9) + "px;text-align:center;\">" + s[0] + "</div>";
            resultHtml += "<div style=\"float:left;width:" + width + "px;height:" + heightBottom + "px;background:#000000;\"></div>";
            resultHtml += "<div style=\"float:left;width:" + width + "px;height:" + heightBottom + "px;background:#FFFFFF;\"></div>";
            resultHtml += "<div style=\"float:left;width:" + width + "px;height:" + heightBottom + "px;background:#000000;\"></div>";
            for (var i = 1; i < 7; i++)
            {
                resultHtml += "<div style=\"float:left;width:" + (width * 7) + "px;color:#000000;text-align:center;\">" + s[i] + "</div>";
            }
            resultHtml += "<div style=\"float:left;width:" + width + "px;height:" + heightBottom + "px;background:#FFFFFF;\"></div>";
            resultHtml += "<div style=\"float:left;width:" + width + "px;height:" + heightBottom + "px;background:#000000;\"></div>";
            resultHtml += "<div style=\"float:left;width:" + width + "px;height:" + heightBottom + "px;background:#FFFFFF;\"></div>";
            resultHtml += "<div style=\"float:left;width:" + width + "px;height:" + heightBottom + "px;background:#000000;\"></div>";
            resultHtml += "<div style=\"float:left;width:" + width + "px;height:" + heightBottom + "px;background:#FFFFFF;\"></div>";
            for (var i = 7; i < 13; i++)
            {
                resultHtml += "<div style=\"float:left;width:" + (width * 7) + "px;color:#000000;text-align:center;\">" + s[i] + "</div>";
            }
            resultHtml += "<div style=\"float:left;width:" + width + "px;height:" + heightBottom + "px;background:#000000;\"></div>";
            resultHtml += "<div style=\"float:left;width:" + width + "px;height:" + heightBottom + "px;background:#FFFFFF;\"></div>";
            resultHtml += "<div style=\"float:left;width:" + width + "px;height:" + heightBottom + "px;background:#000000;\"></div>";
            resultHtml += "<div style=\"float:left;color:#000000;width:" + (width * 9) + "px;\"></div>";
            resultHtml += "<div style=\"clear:both\"></div>";

            return "<div style=\"background:#FFFFFF;padding:0px;font-size:" + (width * 10) + "px;font-family:'楷体';\">" + resultHtml + "</div>";
        }

        private static string Ean13(char c, char type)
        {
            switch (type)
            {
                case 'A':
                    {
                        switch (c)
                        {
                            case '0': return "0001101";
                            case '1': return "0011001";
                            case '2': return "0010011";
                            case '3': return "0111101";//011101
                            case '4': return "0100011";
                            case '5': return "0110001";
                            case '6': return "0101111";
                            case '7': return "0111011";
                            case '8': return "0110111";
                            case '9': return "0001011";
                            default: return "Error!";
                        }
                    }
                case 'B':
                    {
                        switch (c)
                        {
                            case '0': return "0100111";
                            case '1': return "0110011";
                            case '2': return "0011011";
                            case '3': return "0100001";
                            case '4': return "0011101";
                            case '5': return "0111001";
                            case '6': return "0000101";//000101
                            case '7': return "0010001";
                            case '8': return "0001001";
                            case '9': return "0010111";
                            default: return "Error!";
                        }
                    }
                case 'C':
                    {
                        switch (c)
                        {
                            case '0': return "1110010";
                            case '1': return "1100110";
                            case '2': return "1101100";
                            case '3': return "1000010";
                            case '4': return "1011100";
                            case '5': return "1001110";
                            case '6': return "1010000";
                            case '7': return "1000100";
                            case '8': return "1001000";
                            case '9': return "1110100";
                            default: return "Error!";
                        }
                    }
                default: return "Error!";
            }
        }

        private static string Ean13Type(char c)
        {
            switch (c)
            {
                case '0': return "AAAAAA";
                case '1': return "AABABB";
                case '2': return "AABBAB";
                case '3': return "AABBBA";
                case '4': return "ABAABB";
                case '5': return "ABBAAB";
                case '6': return "ABBBAA";//中国
                case '7': return "ABABAB";
                case '8': return "ABABBA";
                case '9': return "ABBABA";
                default: return "Error!";
            }
        }
    }
}