using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json.Linq;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.SkiaSharp;

namespace Lynx_Bot.Processing {
    static class ImageProcessing {
        public static string DoughnutPieChart(DoughnutPieData PieData) {
            PlotModel model = new PlotModel {
                Title=$"{PieData.Total} {PieData.DataName}{(PieData.Total!=1 ? "s" : "")}",
                Background=OxyColor.FromRgb(44, 45, 48),
                TitleColor=OxyColor.FromRgb(242,243,245),
                TextColor=OxyColor.FromRgb(255, 255, 255),
                DefaultFontSize=24,
            };

            PieSeries series = new PieSeries {
                AngleSpan=360,
                StartAngle=0,
                RenderInLegend=true,
                FontSize=20,
                FontWeight=FontWeights.Bold,
            };

            // Put amount of votes next to value
            for(int i = 0;i<PieData.Data.Count;i++) {
                //PieData.Labels[i]+=$" ({PieData.Data[i]})";
                series.Slices.Add(new PieSlice(PieData.Labels[i], PieData.Data[i]) { IsExploded=true});
            }
            model.Series.Add(series);
            PngExporter.Export(model,PieData.FileInfo.PathAndName,512,384);
            return PieData.FileInfo.PathAndName;
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
        public double Total;
        public DoughnutPieData(List<double> Data, List<string> Labels, string FileName, string DataName="",bool HighQuality=false) {
            this.Data=Data;
            this.Labels=Labels;
            string temp = FileName.Replace("/", "_")+".png";
            FileInfo = (temp,$"Renders/{temp}");
            
            // Optional
            this.DataName=DataName; 
            this.HighQuality=HighQuality;
            Total=Data.Sum();
        }
    }
}