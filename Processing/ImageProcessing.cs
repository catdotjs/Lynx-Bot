using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using ImageChartsLib;
using Newtonsoft.Json.Linq;
using ScottPlot;
using ScottPlot.Drawing;
using ScottPlot.Plottable;
using Colour = System.Drawing.ColorTranslator;

namespace Lynx_Bot.Processing {
    static class ImageProcessing {
        public static string DoughnutPieChart(DoughnutPieData PieData) {
            Plot plot = new Plot();
            // Put amount of votes next to value
            for(int i = 0;i<PieData.Data.Count;i++) {
                PieData.Labels[i]+=$" ({PieData.Data[i]})";
            }

            PiePlot pie = plot.AddPie(PieData.Data.ToArray());
            // Legend makes life so much easier fr fr
            pie.SliceLabels = PieData.Labels.ToArray();
            plot.Legend(location: Alignment.UpperRight);

            // Styling
            plot.Style(Style.Gray1);
            plot.Palette=Palette.OneHalfDark;

            // Show data
            pie.Explode=true;
            pie.ShowLabels=true;
            pie.ShowPercentages=true;
            pie.Size=0.9;

            pie.SliceFont.Bold=true;
            pie.SliceFont.Size=24;

            // Other stuff
            pie.DonutSize=0.45;
            double total = PieData.Data.Aggregate((x, y) => x+y);
            pie.DonutLabel=$"{total} {PieData.DataName}{(total>1?"s":"")}";
            pie.CenterFont.Color=Colour.FromHtml("#FFFFFF");
            
            return plot.SaveFig(PieData.FileInfo.PathAndName, lowQuality:!PieData.HighQuality);
        }

        public static Color RandomColour() {
            return new Color((uint)Program.rand.Next(0x808080, 0xFFFFFF));
        }
    }
    struct DoughnutPieData {
        public List<double> Data;
        public List<string> Labels;
        public (string Name, string PathAndName) FileInfo;
        // Optional
        public string DataName;
        public bool HighQuality;
        public DoughnutPieData(List<double> Data, List<string> Labels, string FileName, string DataName="",bool HighQuality=false) {
            this.Data=Data;
            this.Labels=Labels;
            string temp = FileName.Replace("/", "_")+".png";
            FileInfo = (temp,$"Renders/{temp}");
            
            // Optional
            this.DataName=DataName; 
            this.HighQuality=HighQuality;
        }
    }
}