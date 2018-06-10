using Caliburn.Micro;
using KPI.NumericMethods.Interpolation.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Linq;

namespace KPI.NumericMethods.Interpolation
{
    public class SeriesViewModel : PropertyChangedBase
    {
        private ScatterSeries _scatter;
        private LineSeries _graph;

        public SeriesCollection Series { get;  }

        public SeriesViewModel(IEnumerable<Node> initial)
        {
            _scatter = new ScatterSeries
            {
                Title = "Points",
                Values = new ChartValues<ScatterPoint>(initial.Select(n => new ScatterPoint()
                {
                    X = n.X,
                    Y = n.Y
                }))
            };

            Series = new SeriesCollection()
            {
                _scatter
            };
        }

        public void Add(Node n)
        {
            _scatter.Values.Add(new ScatterPoint()
            {
                X = n.X,
                Y = n.Y
            });
        }

        public void Remove(Node n)
        {
            var found = ((IEnumerable<object>)_scatter.Values).Where(v => (v is ScatterPoint p) && p.X == n.X).First();
            _scatter.Values.Remove(found);
        }

        public void AddGraph(IEnumerable<Node> nodes)
        {
            _graph = new LineSeries()
            {
                Values = new ChartValues<ObservablePoint>(nodes.Select(n => new ObservablePoint(n.X, n.Y)))
            };
            Series.Add(_graph);
        }

        public void RemoveGraph()
        {
            if (_graph != null)
            {
                Series.Remove(_graph);
                _graph = null;
            }

        }
    }
}
