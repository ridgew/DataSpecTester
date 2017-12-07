namespace DataSpecTester.Html
{
    using ProtocolEx;
    using System;

    public class HtmlDataProvider : IHtmlDataProvider, IGenericProtocolDataProvider
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlDataProvider"/> class.
        /// </summary>
        public HtmlDataProvider()
        {

        }

        private byte[] _rawBytes = null;

        /// <summary>
        /// Gets or sets the raw bytes.
        /// </summary>
        /// <value>The raw bytes.</value>
        public byte[] RawBytes
        {
            get { return _rawBytes; }
            set { _rawBytes = value; }
        }

        #region IHtmlDataProvider 成员

        /// <summary>
        /// Gets the HTML.
        /// </summary>
        /// <value>The HTML.</value>
        public string Html
        {
            get 
            {
                string str = string.Empty;
                if (RawBytes != null && RawBytes.Length > 0)
                {
                    str = System.Text.Encoding.Default.GetString(RawBytes);
                    //str = str.Replace("\r\n", "<br />\r\n");
                    //str = "<img src='tcp://127.0.0.1:12345/test.gif'>Hello World! " + DateTime.Now.ToString();
                    str = string.Format("<xmp>{0}</xmp>", str);
                }
                return str; 
            }
        }

        /// <summary>
        /// Gets the protocols.
        /// </summary>
        /// <value>The protocols.</value>
        public string[] Protocols
        {
            get { return new string[] { "tcp", "cid" }; }
        }

        #endregion

        #region IGenericProtocolDataProvider 成员

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public byte[] GetData(string url)
        {
            //System.Diagnostics.Trace.WriteLine("GetData of " +url);
            return null;
        }

        #endregion
    }
}

