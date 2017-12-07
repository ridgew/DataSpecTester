using System;
using System.Diagnostics;

namespace DataSpecTester
{

    class TraceOut : TraceListener
    {
        public TraceOut(LogWriter logger)
        {
            _writer = logger;
        }

        private LogWriter _writer = null;

        /// <summary>
        /// 在派生类中被重写时，向在该派生类中所创建的侦听器写入指定消息。
        /// </summary>
        /// <param name="message">要写入的消息。</param>
        public override void Write(string message)
        {
            if (_writer != null)
            {
                _writer("{0}", message);
            }
        }

        /// <summary>
        /// 在派生类中被重写时，向在该派生类中所创建的侦听器写入消息，后跟行结束符。
        /// </summary>
        /// <param name="message">要写入的消息。</param>
        public override void WriteLine(string message)
        {
            Write(message + Environment.NewLine);
        }
    }
}
