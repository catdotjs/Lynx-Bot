using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using ImageChartsLib;
using Newtonsoft.Json.Linq;

namespace Lynx_Bot.Processing {
    static class ImageProcessing {
        public static string DoughnutPieChart(string Data,string Labels,string Total) {

            ImageCharts pie = new ImageCharts()
                .cht("pd")
                .chd(Data)
                .chco("DA4478,7833DA")
                .chl(Labels)
                .chli(Total)
                .chf("bg,s,AAAAAA00")
                .chlps("font.size,30")
                .chs("512x512");
            return pie.toURL();
        }

        public static Color RandomColour() {
            return new Color((uint)Program.rand.Next(0x808080, 0xFFFFFF));
        }
    }
}
