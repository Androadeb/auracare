using ApexCharts;

namespace ynex.Pages.Admin.Dashboard
{
    public partial class Dashboard
    {


        private List<AppointmentData> AppointmentList = new();
        private List<StatusData> StatusList = new();
        private List<PerformanceData> PerformanceList = new();

        private ApexChartOptions<AppointmentData> barChartOptions = new();
        private ApexChartOptions<StatusData> radialOptions = new();
        private ApexChartOptions<PerformanceData> performanceOptions = new();
        private ApexChartOptions<PerformanceData> paymentOptions = new();

        protected override void OnInitialized()
        {

            LoadMockData();
            ConfigureAllCharts();
        }

        private void LoadMockData()
        {
            AppointmentList = new() {
            new ("Sat", 12, 2), new ("Sun", 8, 3), new ("Mon", 10, 4),
            new ("Tue", 12, 4), new ("Wed", 10, 5), new ("Thu", 17, 6), new ("Fri", 9, 4)
        };

            StatusList = new() {
            new ("Completed", 120, 85, "#26bf94"),
            new ("Pending", 120, 65, "#f5b849"),
            new ("Cancelled", 120, 45, "#8c9097")
        };

            PerformanceList = new() {
            new ("Jan", 10, 15), new ("Feb", 12, 18), new ("Mar", 25, 30),
            new ("Apr", 45, 55), new ("May", 48, 60), new ("Jun", 50, 65),
            new ("Jul", 50, 65), new ("Aug", 55, 68), new ("Sep", 75, 82),
            new ("Oct", 80, 85), new ("Nov", 65, 70), new ("Dec", 35, 45)
        };
        }

        private void ConfigureAllCharts()
        {
            // 1. Appointments Bar Chart
            barChartOptions.Chart = new Chart { Toolbar = new Toolbar { Show = false }, FontFamily = "Inter" };
            barChartOptions.PlotOptions = new PlotOptions { Bar = new PlotOptionsBar { ColumnWidth = "15%", BorderRadius = 4 } };
            barChartOptions.Stroke = new Stroke { Curve = Curve.Smooth, Width = new List<double> { 0, 2 } };

            // 2. Radial Chart (The one you wanted like "Image 2")
            radialOptions.Chart = new Chart { Toolbar = new Toolbar { Show = false } };
            radialOptions.PlotOptions = new PlotOptions
            {
                RadialBar = new PlotOptionsRadialBar
                {
                    Hollow = new Hollow { Size = "45%" },
                    Track = new Track { Margin = 8, Background = "#f2f2f2" },
                    DataLabels = new RadialBarDataLabels
                    {
                        Name = new RadialBarDataLabelsName { Show = true, OffsetY = -10, FontSize = "12px", Color = "#8c9097" },
                        Value = new RadialBarDataLabelsValue { Show = true, FontSize = "24px", FontWeight = "700", Formatter = "function(w){return '186'}" },
                        Total = new RadialBarDataLabelsTotal { Show = true, Label = "Total Cases", Color = "#8c9097" }
                    }
                }
            };
            radialOptions.Stroke = new Stroke { LineCap = LineCap.Round }; // Rounded edges like the image
            radialOptions.Colors = StatusList.Select(x => x.Color).ToList();
            radialOptions.Labels = StatusList.Select(x => x.Label).ToList();

            // 3. Middle Performance Chart
            performanceOptions.Chart = new Chart { Toolbar = new Toolbar { Show = false } };
            performanceOptions.Stroke = new Stroke { Curve = Curve.Smooth, Width = 3 };
            performanceOptions.Grid = new Grid { Show = false };
            performanceOptions.Xaxis = new XAxis { Labels = new XAxisLabels { Show = false } };
            performanceOptions.Yaxis = new List<YAxis> { new YAxis { Show = false } };

            // 4. Orders and Payments Area/Line Chart
            paymentOptions.Chart = new Chart { Toolbar = new Toolbar { Show = false } };
            paymentOptions.Stroke = new Stroke { Curve = Curve.Smooth, Width = 3 };
            paymentOptions.Legend = new Legend { Position = LegendPosition.Top, HorizontalAlign = Align.Right };
            paymentOptions.Grid = new Grid { BorderColor = "#f1f1f1", StrokeDashArray = 3 };
        }

        private List<StatCard> GetTopStats() => new() {
        new() {TitleKey = "Today Sessions",Value = "2.7k",Percentage = "19.4",IconClass = "ri-calendar-check-line", BgColorOpacity = "bg-[#F1A68E]/10",TextColor = "text-[#F1A68E]",GlowColor = "#F1A68E"
        },
        new() {
        TitleKey = "Active Cases",Value = "118",Percentage = "8",IconClass = "ri-pulse-line",BgColorOpacity = "bg-blue-500/10",TextColor = "text-blue-500",GlowColor = "#3b82f6"
        },
        new() {TitleKey = "Completed Cases",Value = "43.7k",Percentage = "15.6",IconClass = "ri-checkbox-circle-line", BgColorOpacity = "bg-emerald-500/10",TextColor = "text-emerald-500",GlowColor = "#10b981"}  };

        // --- Data Models ---
        public record AppointmentData(string Day, int Count, int TrendValue);
        public class StatusData
        {
            public string Label { get; set; }
            public int Value { get; set; }
            public double Percentage { get; set; }
            public string Color { get; set; }
            public StatusData(string l, int v, double p, string c) { Label = l; Value = v; Percentage = p; Color = c; }
        }
        public record PerformanceData(string Month, int Tests, int Orders);
        public class StatCard
        {
            public string TitleKey { get; set; } = "";
            public string Value { get; set; } = "";
            public string Percentage { get; set; } = "";
            public string IconClass { get; set; } = "";
            public string BgColorOpacity { get; set; } = "";
            public string TextColor { get; set; } = "";
            public string GlowColor { get; set; } = "";
        }

    }
}
