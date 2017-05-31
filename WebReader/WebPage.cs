using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebReader
{
    public class WebPage
    {
        public WebPage()
        {
            web = new HtmlWeb();
        }

        public WebPage(string url)
        {
            web = new HtmlWeb();
            document = web.Load(url);
        }

        public HtmlNode GetRootNode(string url)
        {
            document = new HtmlDocument();
            document.LoadHtml(new WebClient().DownloadString(url));
            return document.DocumentNode;
        }

        public HtmlNode[] GetNodes(string xpath)
        {
            try
            { 
                return document.DocumentNode.SelectNodes(xpath).ToArray();
            }
            catch
            {
                return new HtmlNode[0];
            }
        }

        private HtmlWeb web;
        private HtmlDocument document;
    }
}
