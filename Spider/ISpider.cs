using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpSpider.Spider
{
    public interface ISpider
    {
        Task Go();
    }
}
