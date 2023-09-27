using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpSpider.Pub;

namespace WpSpider.Spider
{
    public class Daohang : ISpider
    {
        Microsoft.Extensions.Configuration.IConfiguration configuration;
        PubHelper postHelper = new PubHelper();

        public Daohang()
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Path.Combine(AppContext.BaseDirectory))
              .AddJsonFile("Config/Daohang.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();

            
        }
        public async Task Go()
        {
            var crawlurl = configuration.GetSection("crawlurl").Value;
            var res = ReqHelper.GetHtml(crawlurl);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HtmlParser parser = new HtmlParser();
                var doc = await parser.ParseDocumentAsync(res.Html);
                var alist = doc.QuerySelectorAll("div.url-body>a.card[data-url]").ToList();
                Dictionary<string, string> metas = new Dictionary<string, string>();
                var postType = configuration.GetSection("postType").Value;
                var author = long.Parse(configuration.GetSection("author").Value);
                var cate = long.Parse(configuration.GetSection("category").Value);
                var tags = configuration.GetSection("tags").Value.Split(",").ToList();
                
                foreach (var a in alist)
                {
                    try
                    {
                        var href = a.GetAttribute("data-url");
                        Console.WriteLine(href);
                        var div = a.QuerySelector("div.card-body");
                        var img = div.QuerySelector("div.url-img>img");
                        var img_src = img.GetAttribute("data-src");
                        var title = div.QuerySelector("div.url-info>div.text-sm>strong").TextContent;
                        var desc = div.QuerySelector("div.url-info>p").TextContent;
                        //metas.Add("_edit_last", "1");
                        metas.Add("_sites_link", href);
                        metas.Add("_sites_sescribe", desc);
                        metas.Add("_sites_order", "0");
                        metas.Add("_thumbnail", img_src);
                        metas.Add("views", "0");
                        metas.Add("_like_count", "0");
                        metas.Add("_sites_type", "sites");
                        metas.Add("_user_purview_level", "all");
                        metas.Add("_sites_country", "未知");

                        await postHelper.Post(title, string.Empty, cate, author, postType, tags, metas);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                    }
                }
            }
            else
            {
                Console.WriteLine("爬取网页失败");
            }
        }       
    }
}
