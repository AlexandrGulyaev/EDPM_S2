using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sem2Lab1
{
    public partial class FormChart : Form
    {
        public FormChart()
        {
            InitializeComponent();
        }

        public FormChart(
                double[] x,
                int size = -1,
                int start = 0,
                double step = 1,
                string FormName = "Chart",
                string ChartName = "Seria",
                double IntervalX = -1,
                double IntervalY = -1,
                double minX = -9999,
                double minY = -9999,
                int increment = 0
            )
        {
            InitializeComponent();
            if (minX != -9999)
                chart.ChartAreas[0].AxisX.Minimum = minX;
            if (minY != -9999)
                chart.ChartAreas[0].AxisY.Minimum = minY;
            if (IntervalX != -1)
                chart.ChartAreas[0].AxisX.Interval = IntervalX;
            if (IntervalY != -1)
                chart.ChartAreas[0].AxisY.Interval = IntervalY;
            this.Text = FormName;
            chart.Series[0].Name = ChartName;

            if (size == -1)
                size = x.Count();
            for (int i = start; i < start + size; i++)
            {
                chart.Series[0].Points.AddXY((i + increment) * step, x[i]);
            }
            this.Show();
        }

        public FormChart(Dictionary<ushort, int> histogram, string FormName = "")
        {
            InitializeComponent();
            chart.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            var dict = histogram.OrderBy(x => x.Key);
            foreach (var temp in dict)
            {
                chart.Series[0].Points.AddXY(temp.Key, temp.Value);
            }
            this.Text = "Histogram " + FormName;
            this.Show();
        }
    }
}
