using MilanoFashion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.ViewModels
{
    public class AboutViewModel
    {
        public Dictionary<string,string> Abouts { get; set; }
        public List<Team> Teams { get; set; }

    }
}
