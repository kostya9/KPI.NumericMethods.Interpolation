﻿using Caliburn.Micro;
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
        private SeriesCollection _series;
        private ScatterSeries _scatter;

        public SeriesCollection Series
        {
            get => _series;
            set
            {
                _series = value;
                NotifyOfPropertyChange();
            }
        }

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
            _series = new SeriesCollection()
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
    }
}