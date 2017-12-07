using ProtocolEx;
using System;
using System.Windows.Forms;

namespace DataSpecTester.Html
{
    public class HtmlBrowser : WebBrowser
    {
        public HtmlBrowser()
        {
            base.Navigate("about:blank");
        }

        public bool Navigate(IHtmlDataProvider html)
        {
            this.RegisterProtocols(html, true);
            bool flag = this.ShowHtml(html.Html);
            this.RegisterProtocols(html, false);
            return flag;
        }

        private void RegisterProtocols(IHtmlDataProvider html, bool register)
        {
            string[] protocols = html.Protocols;
            if (protocols != null)
            {
                for (int i = 0; i < protocols.Length; i++)
                {
                    if (register)
                    {
                        GenericProtocolManager.RegisterProtocol(protocols[i], html);
                    }
                    else
                    {
                        GenericProtocolManager.UnRegisterProtocol(protocols[i]);
                    }
                }
            }
        }

        private bool ShowHtml(string html)
        {
            //System.Diagnostics.Trace.WriteLine("showHtml");

            HtmlDocument document = base.Document;
            if (document == null)
            {
                return false;
            }
            document.OpenNew(false);
            document.Write(html);
            return true;
        }

    }
}
