using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using WpSpider.Pub;

namespace WpSpider.Spider
{
    public class Araneid : ISpider
    {
        IConfiguration configuration;
        PubHelper pubHelper = new PubHelper();
        long category;
        List<string> tags;
        public Araneid()
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Path.Combine(AppContext.BaseDirectory))
              .AddJsonFile("Config/Araneid.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();
            
            category = long.Parse(configuration.GetSection("category").Value);
            tags = configuration.GetSection("tags").Value.Split(",").ToList();
        }
        public async Task Go()
        {
            string connStr = "server=localhost;Database=wenda;uid=wenda;pwd=i6JpJMNEGezEFehc;charset=utf8";
            var id = int.Parse(configuration.GetSection("id").Value); //初始ID
            while (true)
            {               
                var sql = $"SELECT * FROM `araneid_spider_article` where id>{id} limit 10";
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    try
                    {
                        // 打开连接
                        conn.Open();

                        // 创建 MySQL 命令对象
                        MySqlCommand cmd = new MySqlCommand(sql, conn);

                        // 执行查询，返回 MySqlDataReader 对象
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // 读取数据并输出
                            while (await reader.ReadAsync())
                            {
                                var title = reader["title"].ToString();
                                var context = reader["context"].ToString();
                                await WriteData(title, context);
                               
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                id = id + 10; //成功读取一条,id加1
                Thread.Sleep(1000 * 60 * 6);
            }
            
        }

        private async Task WriteData(string title, string context)
        {
            Console.WriteLine("读取问答条" + title);
            await pubHelper.Post(title, context, category, 1, "post", tags, null);
            Console.WriteLine("成功发布文章" + title);
        }
    }
}
