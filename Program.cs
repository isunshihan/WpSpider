using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using WpSpider.Spider;

namespace WpSpider
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(AppContext.BaseDirectory))
            .AddJsonFile("Config/Main.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build(); ;
            var classNames = configuration.GetSection("excutor").Value;
            //string[] temp = classNames.Split("|", StringSplitOptions.RemoveEmptyEntries);
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            ISpider spider = (ISpider)assembly.CreateInstance("WpSpider.Spider." + classNames); // 类的完全限定名（即包括命名空间）
            await spider.Go();
            //ParallelLoopResult result = Parallel.ForEach<string>(temp, (str, state, i) =>
            //{

            //});
            //Console.WriteLine("是否完成:{0}", result.IsCompleted);
            //Console.WriteLine("最低迭代:{0}", result.LowestBreakIteration);
        }
    }
}
