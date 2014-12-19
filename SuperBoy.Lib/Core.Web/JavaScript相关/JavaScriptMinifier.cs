/**********************************************
 * 类作用：   客户端代码精简类
 * 建立人：   abaal
 * 建立时间： 2008-09-03 
 * Copyright (C) 2007-2008 abaal
 * All rights reserved
 * http://blog.csdn.net/abaal888
 ***********************************************/



using System;
using System.IO;
using System.Text;

namespace Core.Web
{
    public sealed class JavaScriptMinifier
    {
        private const int Eof = -1;

        private TextReader _sr;
        private TextWriter _sw;
        private int _theA;
        private int _theB;
        private int _theLookahead = Eof;

        /// <summary>
        /// 转换原始Js内容的精简版本
        /// </summary>
        /// <param name="src">原始Js文件内容</param>
        /// <returns>精简版的Js内容</returns>
        public static string JsMin(string src)
        {
            return new JavaScriptMinifier().Minify(src);
        }

        public string Minify(string src)
        {
            var dst = new StringBuilder();
            using (_sr = new StringReader(src))
            {
                using (_sw = new StringWriter(dst))
                {
                    Jsmin();
                }
            }
            return dst.ToString();
        }

        /* jsmin -- Copy the input to the output, deleting the characters which are
                insignificant to JavaScript. Comments will be removed. Tabs will be
                replaced with spaces. Carriage returns will be replaced with linefeeds.
                Most spaces and linefeeds will be removed.
        */
        private void Jsmin()
        {
            _theA = '\n';
            Action(3);
            while (_theA != Eof)
            {
                switch (_theA)
                {
                    case ' ':
                        {
                            if (IsAlphanum(_theB))
                                Action(1);
                            else
                                Action(2);
                            break;
                        }
                    case '\n':
                        {
                            switch (_theB)
                            {
                                case '{':
                                case '[':
                                case '(':
                                case '+':
                                case '-':
                                    {
                                        Action(1);
                                        break;
                                    }
                                case ' ':
                                    {
                                        Action(3);
                                        break;
                                    }
                                default:
                                    {
                                        if (IsAlphanum(_theB))
                                            Action(1);
                                        else
                                            Action(2);
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            switch (_theB)
                            {
                                case ' ':
                                    {
                                        if (IsAlphanum(_theA))
                                        {
                                            Action(1);
                                            break;
                                        }
                                        Action(3);
                                        break;
                                    }
                                case '\n':
                                    {
                                        switch (_theA)
                                        {
                                            case '}':
                                            case ']':
                                            case ')':
                                            case '+':
                                            case '-':
                                            case '"':
                                            case '\'':
                                                {
                                                    Action(1);
                                                    break;
                                                }
                                            default:
                                                {
                                                    if (IsAlphanum(_theA))
                                                        Action(1);
                                                    else
                                                        Action(3);
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        Action(1);
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
        }
        /* action -- do something! What you do is determined by the argument:
                1   Output A. Copy B to A. Get the next B.
                2   Copy B to A. Get the next B. (Delete A).
                3   Get the next B. (Delete B).
           action treats a string as a single character. Wow!
           action recognizes a regular expression if it is preceded by ( or , or =.
        */
        private void Action(int d)
        {
            if (d <= 1)
                Put(_theA);
            if (d <= 2)
            {
                _theA = _theB;
                if (_theA == '\'' || _theA == '"')
                {
                    for (; ; )
                    {
                        Put(_theA);
                        _theA = Get();
                        if (_theA == _theB)
                            break;
                        if (_theA <= '\n')
                            throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}\n", _theA));
                        if (_theA == '\\')
                        {
                            Put(_theA);
                            _theA = Get();
                        }
                    }
                }
            }
            if (d <= 3)
            {
                _theB = Next();
                if (_theB == '/' && (_theA == '(' || _theA == ',' || _theA == '=' ||
                                    _theA == '[' || _theA == '!' || _theA == ':' ||
                                    _theA == '&' || _theA == '|' || _theA == '?' ||
                                    _theA == '{' || _theA == '}' || _theA == ';' ||
                                    _theA == '\n'))
                {
                    Put(_theA);
                    Put(_theB);
                    for (; ; )
                    {
                        _theA = Get();
                        if (_theA == '/')
                            break;
                        else if (_theA == '\\')
                        {
                            Put(_theA);
                            _theA = Get();
                        }
                        else if (_theA <= '\n')
                            throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}.\n", _theA));
                        Put(_theA);
                    }
                    _theB = Next();
                }
            }
        }
        /* next -- get the next character, excluding comments. peek() is used to see
                if a '/' is followed by a '/' or '*'.
        */
        private int Next()
        {
            var c = Get();
            if (c == '/')
            {
                switch (Peek())
                {
                    case '/':
                        {
                            for (; ; )
                            {
                                c = Get();
                                if (c <= '\n')
                                    return c;
                            }
                        }
                    case '*':
                        {
                            Get();
                            for (; ; )
                            {
                                switch (Get())
                                {
                                    case '*':
                                        {
                                            if (Peek() == '/')
                                            {
                                                Get();
                                                return ' ';
                                            }
                                            break;
                                        }
                                    case Eof:
                                        {
                                            throw new Exception("Error: JSMIN Unterminated comment.\n");
                                        }
                                }
                            }
                        }
                    default:
                        {
                            return c;
                        }
                }
            }
            return c;
        }
        /* peek -- get the next character without getting it.
        */
        private int Peek()
        {
            _theLookahead = Get();
            return _theLookahead;
        }
        /* get -- return the next character from stdin. Watch out for lookahead. If
                the character is a control character, translate it to a space or
                linefeed.
        */
        private int Get()
        {
            var c = _theLookahead;
            _theLookahead = Eof;
            if (c == Eof)
                c = _sr.Read();
            if (c >= ' ' || c == '\n' || c == Eof)
                return c;
            if (c == '\r')
                return '\n';
            return ' ';
        }
        private void Put(int c)
        {
            _sw.Write((char)c);
        }
        /* isAlphanum -- return true if the character is a letter, digit, underscore,
                dollar sign, or non-ASCII character.
        */
        private bool IsAlphanum(int c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
                    (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' ||
                    c > 126);
        }
    }
}
