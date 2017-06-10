using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyToolbox
{
    public class WebPage
    {
        public HtmlDocument Document
        {
            get { return document; }
        }

        public HtmlNode RootNode
        {
            get { return root_node; }
        }

        public WebPage()
        {
            Initialize();
        }

        public WebPage(string url)
        {
            Initialize();
            LoadPage(url);
        }

        public bool LoadPage(string url)
        {
            try
            {
                document.LoadHtml(new WebClient().DownloadString(url));
                root_node = document.DocumentNode;

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("LoadPage({0})", url));
                Console.WriteLine(ex);
            }

            return false;
        }

        //Depricated - to be removed
        public HtmlNode GetRootNode
        {
            get { return root_node; }
        }

        public IEnumerable<HtmlNode> GetNodes(string name)
        {
            return GetNodes(root_node, name, String.Empty, String.Empty);
        }

        public IEnumerable<HtmlNode> GetNodes(HtmlNode node, string name)
        {
            return GetNodes(node, name, String.Empty, String.Empty);
        }

        public IEnumerable<HtmlNode> GetNodes(string name, string attribute, string value)
        {
            return GetNodes(root_node, name, attribute, value);
        }

        public IEnumerable<HtmlNode> GetNodes(HtmlNode node, string name, string attribute, string value)
        {
            if ((String.IsNullOrEmpty(attribute)) && (String.IsNullOrEmpty(value)))
            {
                return node.Descendants(name);
            }
            return node.Descendants(name).Where(n => n.GetAttributeValue(attribute, String.Empty).Equals(value));
        }

        public HtmlNode GetSingleNode(string name)
        {
            return GetSingleNode(root_node, name, String.Empty, String.Empty);
        }

        public HtmlNode GetSingleNode(HtmlNode node, string name)
        {
            return GetSingleNode(node, name, String.Empty, String.Empty);
        }

        public HtmlNode GetSingleNode(string name, string attribute, string value)
        {
            return GetSingleNode(root_node, name, attribute, value);
        }

        public HtmlNode GetSingleNode(HtmlNode node, string name, string attribute, string value)
        {
            IEnumerable<HtmlNode> nodes = GetNodes(node, name, attribute, value);
            return (nodes.Count() == 1) ? nodes.Single() : null;
        }

        private HtmlWeb web;
        private HtmlDocument document;
        private HtmlNode root_node;

        private void Initialize()
        {
            web = new HtmlWeb();
            document = new HtmlDocument();
        }
    }
}
