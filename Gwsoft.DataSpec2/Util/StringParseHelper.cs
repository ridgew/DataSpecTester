using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gwsoft.DataSpec2.Util
{
    /// <summary>
    /// 字符串解析辅助类
    /// </summary>
    public class StringParseHelper : IDisposable
    {

        StringReader reader = null;

        /// <summary>
        /// 初始化一个 <see cref="StringParseHelper"/> class 实例。
        /// </summary>
        /// <param name="strToParse">待解析的原始字符串</param>
        public StringParseHelper(string strToParse)
        {
            reader = new StringReader(strToParse);
        }

        StringBuilder buffer = new StringBuilder();

        /// <summary>
        /// 获取缓存区内所有字符
        /// </summary>
        /// <returns></returns>
        public string GetBuffer()
        {
            return buffer.ToString();
        }

        /// <summary>
        /// 清除缓冲区字符
        /// </summary>
        public void ClearBuffer()
        {
            buffer = new StringBuilder();
        }

        public void RegisterPeekHandler(int peekChar)
        {

        }

        /// <summary>
        /// 当前解析游标
        /// </summary>
        ParseCursor Cursor = new ParseCursor();

        /// <summary>
        /// 向前读取一个字符，并返回读取到的字符。
        /// </summary>
        /// <returns></returns>
        public char Read()
        {
            int rChr = reader.Read();
            if (rChr == -1) return '\0';
            Cursor.Index++;
            return (char)rChr;
        }

        /// <summary>
        /// 返回下一个可用的字符，但不使用它。
        /// </summary>
        /// <returns></returns>
        public char Peek()
        {
            int rChr = reader.Peek();
            if (rChr == -1) return '\0';
            return (char)rChr;
        }

        /// <summary>
        /// [TODO]根据设置的解析规则解析
        /// </summary>
        public void Parse()
        {
            char chr = '\0';
            while ((chr = Peek()) != '\0')
            {
                Read();
            }

            //{Exception:-1/*服务器异常*/, Success:0/*服务器处理成功*/, Updatable:1/*主程序有更新,请按照客户端更新策略下载*/}
            /*
             * when found : => getBufferAsKey -> readToChar([',','}']) -> getBufferAsValue
             * when found / => checkNextCharIs('*') -> readToChar(['*']) and peekNextChar == '/' as Key Comment
             * 
             */
            StringBuilder buffer = new StringBuilder();
            reader.Read();
        }

        #region IDisposable 成员

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            buffer = null;
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
            }
        }

        #endregion

        /// <summary>
        /// [TODO]解析游标
        /// </summary>
        internal class ParseCursor
        {
            /// <summary>
            /// 判断是否在一个区间之内，如[],(),{},'',""等之内。
            /// </summary>
            /// <returns></returns>
            public bool IsInBrace()
            {
                return false;
            }

            /// <summary>
            /// 获取当前游标所在嵌套区间内的深度，无嵌套为则深度为1。
            /// </summary>
            /// <returns></returns>
            public int GetBraceDeepth()
            {
                return 1;
            }

            /// <summary>
            /// 获取或设置字符所在位置的索引
            /// </summary>
            public int Index { get; set; }


        }
    }

    public delegate TData ParserHandlerAction<TData>(StringParseHelper parser);

    /// <summary>
    /// 获取解析当时的缓冲字符串
    /// </summary>
    /// <returns></returns>
    public delegate string GetParseBufferString();
}
