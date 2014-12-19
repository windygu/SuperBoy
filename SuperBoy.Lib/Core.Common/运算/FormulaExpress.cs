using System;

namespace Core.Common
{
    /// <summary>
    ///     EnumFormula
    /// </summary>
    public enum EnumFormula
    {
        Add, //加号
        Dec, //减号
        Mul, //乘号
        Div, //除号
        Sin, //正玄
        Cos, //余玄
        Tan, //正切
        ATan, //余切
        Sqrt, //平方根
        Pow, //求幂
        None //无
    }

    /// <summary>
    ///     FormulaDeal
    /// </summary>
    public class FormulaDeal
    {
        private double CalculateExpress(string strExpression)
        {
            var strTemp = "";
            var strTempB = "";
            var strOne = "";
            var strTwo = "";
            double replaceValue = 0;
            while (strExpression.IndexOf("+") != -1 || strExpression.IndexOf("-") != -1
                   || strExpression.IndexOf("*") != -1 || strExpression.IndexOf("/") != -1)
            {
                if (strExpression.IndexOf("*") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("*") + 1,
                        strExpression.Length - strExpression.IndexOf("*") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("*"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);

                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    replaceValue = Convert.ToDouble(GetExpType(strOne))*Convert.ToDouble(GetExpType(strTwo));
                    strExpression = strExpression.Replace(strOne + "*" + strTwo, replaceValue.ToString());
                }
                else if (strExpression.IndexOf("/") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("/") + 1,
                        strExpression.Length - strExpression.IndexOf("/") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("/"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);


                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    replaceValue = Convert.ToDouble(GetExpType(strOne))/Convert.ToDouble(GetExpType(strTwo));
                    strExpression = strExpression.Replace(strOne + "/" + strTwo, replaceValue.ToString());
                }
                else if (strExpression.IndexOf("+") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("+") + 1,
                        strExpression.Length - strExpression.IndexOf("+") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("+"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);

                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    replaceValue = Convert.ToDouble(GetExpType(strOne)) + Convert.ToDouble(GetExpType(strTwo));
                    strExpression = strExpression.Replace(strOne + "+" + strTwo, replaceValue.ToString());
                }
                else if (strExpression.IndexOf("-") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("-") + 1,
                        strExpression.Length - strExpression.IndexOf("-") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("-"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);


                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    replaceValue = Convert.ToDouble(GetExpType(strOne)) - Convert.ToDouble(GetExpType(strTwo));
                    strExpression = strExpression.Replace(strOne + "-" + strTwo, replaceValue.ToString());
                }
            }
            return Convert.ToDouble(strExpression);
        }

        private double CalculateExExpress(string strExpression, EnumFormula expressType)
        {
            double retValue = 0;
            switch (expressType)
            {
                case EnumFormula.Sin:
                    retValue = Math.Sin(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Cos:
                    retValue = Math.Cos(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Tan:
                    retValue = Math.Tan(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.ATan:
                    retValue = Math.Atan(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Sqrt:
                    retValue = Math.Sqrt(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Pow:
                    retValue = Math.Pow(Convert.ToDouble(strExpression), 2);
                    break;
            }
            if (retValue == 0) return Convert.ToDouble(strExpression);
            return retValue;
        }

        private int GetNextPos(string strExpression)
        {
            var expPos = new int[4];
            expPos[0] = strExpression.IndexOf("+");
            expPos[1] = strExpression.IndexOf("-");
            expPos[2] = strExpression.IndexOf("*");
            expPos[3] = strExpression.IndexOf("/");
            var tmpMin = strExpression.Length;
            for (var count = 1; count <= expPos.Length; count++)
            {
                if (tmpMin > expPos[count - 1] && expPos[count - 1] != -1)
                {
                    tmpMin = expPos[count - 1];
                }
            }
            return tmpMin;
        }

        private int GetPrivorPos(string strExpression)
        {
            var expPos = new int[4];
            expPos[0] = strExpression.LastIndexOf("+");
            expPos[1] = strExpression.LastIndexOf("-");
            expPos[2] = strExpression.LastIndexOf("*");
            expPos[3] = strExpression.LastIndexOf("/");
            var tmpMax = -1;
            for (var count = 1; count <= expPos.Length; count++)
            {
                if (tmpMax < expPos[count - 1] && expPos[count - 1] != -1)
                {
                    tmpMax = expPos[count - 1];
                }
            }
            return tmpMax;
        }

        public string SpiltExpression(string strExpression)
        {
            var strTemp = "";
            var strExp = "";
            while (strExpression.IndexOf("(") != -1)
            {
                strTemp = strExpression.Substring(strExpression.LastIndexOf("(") + 1,
                    strExpression.Length - strExpression.LastIndexOf("(") - 1);
                strExp = strTemp.Substring(0, strTemp.IndexOf(")"));
                strExpression = strExpression.Replace("(" + strExp + ")", CalculateExpress(strExp).ToString());
            }
            if (strExpression.IndexOf("+") != -1 || strExpression.IndexOf("-") != -1
                || strExpression.IndexOf("*") != -1 || strExpression.IndexOf("/") != -1)
            {
                strExpression = CalculateExpress(strExpression).ToString();
            }
            return strExpression;
        }

        private string GetExpType(string strExpression)
        {
            strExpression = strExpression.ToUpper();
            if (strExpression.IndexOf("SIN") != -1)
            {
                return
                    CalculateExExpress(
                        strExpression.Substring(strExpression.IndexOf("N") + 1,
                            strExpression.Length - 1 - strExpression.IndexOf("N")), EnumFormula.Sin).ToString();
            }
            if (strExpression.IndexOf("COS") != -1)
            {
                return
                    CalculateExExpress(
                        strExpression.Substring(strExpression.IndexOf("S") + 1,
                            strExpression.Length - 1 - strExpression.IndexOf("S")), EnumFormula.Cos).ToString();
            }
            if (strExpression.IndexOf("TAN") != -1)
            {
                return
                    CalculateExExpress(
                        strExpression.Substring(strExpression.IndexOf("N") + 1,
                            strExpression.Length - 1 - strExpression.IndexOf("N")), EnumFormula.Tan).ToString();
            }
            if (strExpression.IndexOf("ATAN") != -1)
            {
                return
                    CalculateExExpress(
                        strExpression.Substring(strExpression.IndexOf("N") + 1,
                            strExpression.Length - 1 - strExpression.IndexOf("N")), EnumFormula.ATan).ToString();
            }
            if (strExpression.IndexOf("SQRT") != -1)
            {
                return
                    CalculateExExpress(
                        strExpression.Substring(strExpression.IndexOf("T") + 1,
                            strExpression.Length - 1 - strExpression.IndexOf("T")), EnumFormula.Sqrt).ToString();
            }
            if (strExpression.IndexOf("POW") != -1)
            {
                return
                    CalculateExExpress(
                        strExpression.Substring(strExpression.IndexOf("W") + 1,
                            strExpression.Length - 1 - strExpression.IndexOf("W")), EnumFormula.Pow).ToString();
            }
            return strExpression;
        }
    }
}