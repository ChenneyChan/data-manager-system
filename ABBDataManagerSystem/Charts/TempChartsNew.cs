using OxyPlot.Axes;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Legends;
using OxyPlot.Series;
using LineStyle = OxyPlot.LineStyle;
using OxyPlot.Wpf;

namespace ABBDataManagerSystem.Charts
{
    public class TempChartsNew
    {
        private readonly PlotView plot;

        private PlotModel? _myPlotModel;
        private DateTimeAxis? _dateAxis;
        private LinearAxis? _valueAxis;
        private List<int> EnabledSlots;
        private int MinutesRange = -1;

        private static int MAX_RECORD_COUNT = 10000;

        public TempChartsNew(PlotView plot, List<int> enabledSlots)
        {
            this.plot = plot;
            this.EnabledSlots = enabledSlots;
        }

        private Random rand = new Random();

        public void InitChart()
        {
            _myPlotModel = new PlotModel()
            {
                //Title = "多槽位温度监测",
                Legends =
                {
                    new Legend()
                    {
                        LegendOrientation = LegendOrientation.Vertical,
                        LegendPlacement = LegendPlacement.Outside,
                        LegendPosition = LegendPosition.LeftMiddle,
                        LegendBackground = OxyColor.FromAColor(200, OxyColors.Beige),
                        LegendBorder = OxyColors.Black
                    }
                }
            };

            InitAxis();
            //InitAnnotation();
            InitSeries();
            plot.Model = _myPlotModel;
        }

        private void InitAxis()
        {
            //X轴
            _dateAxis = new DateTimeAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IntervalLength = 80,
                //IsZoomEnabled = false,
                //IsPanEnabled = false,
            };
            _myPlotModel.Axes.Add(_dateAxis);

            //Y轴
            _valueAxis = new LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IntervalLength = 80,
                Angle = 60,
                IsZoomEnabled = false,
                IsPanEnabled = false,
                //Maximum = 200,
                //Minimum = -1
            };
            _myPlotModel.Axes.Add(_valueAxis);
        }

        private void InitSeries()
        {
            #region demo
            //添加两条曲线
            //var series = new LineSeries()
            //{
            //    Color = OxyColors.Green,
            //    StrokeThickness = 2,
            //    MarkerSize = 3,
            //    MarkerStroke = OxyColors.DarkGreen,
            //    MarkerType = MarkerType.Diamond,
            //    Title = "Temp",
            //};
            //_myPlotModel.Series.Add(series);
            //series = new LineSeries()
            //{
            //    Color = OxyColors.Blue,
            //    StrokeThickness = 2,
            //    MarkerSize = 3,
            //    MarkerStroke = OxyColors.BlueViolet,
            //    MarkerType = MarkerType.Star,
            //    Title = "Humi",
            //};
            //_myPlotModel.Series.Add(series);
            #endregion

            foreach (var slot in EnabledSlots)
            {
                var series = new LineSeries()
                {
                    StrokeThickness = 1,
                    MarkerSize = 2,
                    MarkerType = MarkerType.Diamond,
                    Title = "Slot-" + slot,
                    IsVisible = true,
                };
                _myPlotModel.Series.Add(series);
            }
        }

        public void AddSeries(List<string>seriesList)
        {
            foreach(var s in seriesList)
            {
                var series = new LineSeries()
                {
                    StrokeThickness = 1,
                    MarkerSize = 2,
                    MarkerType = MarkerType.Diamond,
                    Title = s,
                    IsVisible = true,
                };
                _myPlotModel.Series.Add(series);
            }
        }

        private void InitAnnotation()
        {
            //添加标注线，温度上下限和湿度上下限
            var lineTempMaxAnnotation = new LineAnnotation()
            {
                Type = LineAnnotationType.Horizontal,
                Color = OxyColors.Red,
                LineStyle = LineStyle.Solid,
                Y = 10,
                Text = "Temp MAX:10"
            };
            _myPlotModel.Annotations.Add(lineTempMaxAnnotation);

            var lineTempMinAnnotation = new LineAnnotation()
            {
                Type = LineAnnotationType.Horizontal,
                Y = 30,
                Text = "Temp Min:30",
                Color = OxyColors.Red,
                LineStyle = LineStyle.Solid
            };
            _myPlotModel.Annotations.Add(lineTempMinAnnotation);

            var lineHumiMaxAnnotation = new LineAnnotation()
            {
                Type = LineAnnotationType.Horizontal,
                Color = OxyColors.Red,
                LineStyle = LineStyle.Solid,
                //lineMaxAnnotation.MaximumX = 0.8;
                Y = 75,
                Text = "Humi MAX:75"
            };
            _myPlotModel.Annotations.Add(lineHumiMaxAnnotation);

            var lineHumiMinAnnotation = new LineAnnotation()
            {
                Type = LineAnnotationType.Horizontal,
                Y = 35,
                Text = "Humi Min:35",
                Color = OxyColors.Red,
                LineStyle = LineStyle.Solid
            };
            _myPlotModel.Annotations.Add(lineHumiMinAnnotation);
        }

        public void AddRecords(float[] values, float[]? extentionValues = null)
        {
            if (values.Length == 0 || _myPlotModel == null)
            {
                return;
            }
            var date = DateTime.Now;
            _myPlotModel.Axes[0].Maximum = DateTimeAxis.ToDouble(date.AddSeconds(1));
            SetDateTimeAxisRange(MinutesRange, false);

            int extesionSize = extentionValues != null ? extentionValues.Length : 0;
            for (int i = 0; i < values.Length + extesionSize && i < _myPlotModel.Series.Count; i++)
            {
                LineSeries? lineSer = plot.Model.Series[i] as LineSeries;
                if (lineSer == null)
                {
                    return;
                }

                float? value = null;
                if (i < values.Length)
                {
                    value = values[i];
                }
                else if (extentionValues != null)
                {
                    value = extentionValues[i - values.Length];
                }

                lineSer.Points.Add(new DataPoint(DateTimeAxis.ToDouble(date), value ?? 0));
                if (lineSer.Points.Count > MAX_RECORD_COUNT)
                {
                    lineSer.Points.RemoveAt(0);
                }
            }

            plot.Dispatcher.Invoke(new Action(() => { _myPlotModel.InvalidatePlot(true); }));
        }

        public void ClearRecords()
        {
            for (int i = 0; i < _myPlotModel.Series.Count; i++)
            {
                LineSeries? lineSer = plot.Model.Series[i] as LineSeries;
                if (lineSer == null)
                {
                    return;
                }
                lineSer.Points.Clear();
            }
            plot.Dispatcher.Invoke(new Action(() => { _myPlotModel.InvalidatePlot(true); }));
        }

        public void ExportToPng(string filePath)
        {
            var pngExporter = new PngExporter { Width = 1200, Height = 680 };
            pngExporter.ExportToFile(_myPlotModel, filePath);
        }

        public void ToogleLegends()
        {
            foreach (var item in _myPlotModel.Legends)
            {
                item.IsLegendVisible = !item.IsLegendVisible;
            }
            _myPlotModel.InvalidatePlot(true);
        }

        private void RandomRecords()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var date = DateTime.Now;
                    _myPlotModel.Axes[0].Maximum = DateTimeAxis.ToDouble(date.AddSeconds(1));

                    var lineSer = plot.Model.Series[0] as LineSeries;
                    lineSer.Points.Add(new DataPoint(DateTimeAxis.ToDouble(date), rand.Next(100, 300) / 10.0));
                    if (lineSer.Points.Count > 100)
                    {
                        lineSer.Points.RemoveAt(0);
                    }

                    lineSer = plot.Model.Series[1] as LineSeries;
                    lineSer.Points.Add(new DataPoint(DateTimeAxis.ToDouble(date), rand.Next(350, 750) / 10.0));
                    if (lineSer.Points.Count > 100)
                    {
                        lineSer.Points.RemoveAt(0);
                    }

                    lineSer = plot.Model.Series[2] as LineSeries;
                    lineSer.Points.Add(new DataPoint(DateTimeAxis.ToDouble(date), rand.Next(350, 750) / 10.0));
                    if (lineSer.Points.Count > 100)
                    {
                        lineSer.Points.RemoveAt(0);
                    }

                    lineSer = plot.Model.Series[3] as LineSeries;
                    lineSer.Points.Add(new DataPoint(DateTimeAxis.ToDouble(date), rand.Next(350, 750) / 10.0));
                    if (lineSer.Points.Count > 100)
                    {
                        lineSer.Points.RemoveAt(0);
                    }
                    _myPlotModel.InvalidatePlot(true);

                    Thread.Sleep(1000);
                }
            });
        }

        public void SetDateTimeAxisRange(int minutes, bool needInvalidate = true)
        {
            if (_myPlotModel.Axes.Count > 0 && _myPlotModel.Axes[0] is DateTimeAxis dateTimeAxis)
            {
                if (minutes < 0)
                {
                    dateTimeAxis.Minimum = double.NaN;
                }
                else
                {
                    dateTimeAxis.Minimum = DateTimeAxis.ToDouble(DateTimeAxis.ToDateTime(dateTimeAxis.Maximum).Subtract(TimeSpan.FromMinutes(minutes)));
                }
                if (needInvalidate)
                {
                    _myPlotModel.InvalidatePlot(false);
                }
            }
        }

        // 将double类型的时间值转换为DateTime类型
        public static DateTime DoubleToDate(double value)
        {
            DateTime dtBase = new DateTime(1900, 1, 1); //基准时间
            TimeSpan ts = TimeSpan.FromDays(value); // 根据double值创建时间间隔

            return dtBase.Date + ts; // 返回相对于基准时间的真实DateTime
        }

        public void HideAllLines()
        {
            for (int i = 0; i < _myPlotModel.Series.Count; i++)
            {
                _myPlotModel.Series[i].IsVisible = false;
            }
            _myPlotModel.InvalidatePlot(false);
        }

        public void ResuneAllLines()
        {
            for (int i = 0; i < _myPlotModel.Series.Count && i < EnabledSlots.Count; i++)
            {
                _myPlotModel.Series[i].IsVisible = true;
            }
            _myPlotModel.InvalidatePlot(false);
        }
    }
}
