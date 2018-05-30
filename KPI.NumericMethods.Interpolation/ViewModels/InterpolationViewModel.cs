using Caliburn.Micro;
using KPI.NumericMethods.Interpolation.Algorithms;
using KPI.NumericMethods.Interpolation.Extensions;
using KPI.NumericMethods.Interpolation.Models;
using System;
using System.Linq;

namespace KPI.NumericMethods.Interpolation
{
    public class InterpolationViewModel : Screen
    {
        private BindableCollection<Node> _nodes;
        private Node _selected;
        private double _interpolated;
        private SeriesViewModel _seriesViewModel;

        public BindableCollection<Node> Nodes
        {
            get => _nodes;
            set
            {
                if (_nodes != value)
                {
                    _nodes = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public Node SelectedNode
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public double Interpolated
        {
            get { return _interpolated; }
            set
            {
                if (_interpolated != value)
                {
                    _interpolated = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public SeriesViewModel SeriesViewModel
        {
            get => _seriesViewModel;
            set
            {
                _seriesViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        public InterpolationViewModel()
        {
            Reset();
        }

        public void Reset()
        {
            Nodes = new BindableCollection<Node>();
            SeriesViewModel = new SeriesViewModel(Enumerable.Empty<Node>());
        }

        public void ImportPointsFromTask1()
        {
            (double, double)[] tuples = new[] { (-2.987, -16.593), (-2.966, -16.449), (-2.259, -12.305), (-2.132, -11.656), (-1.808, -10.039), (-1.614, -9.053), (-1.597, -8.970), (-1.575, -8.854), (-1.251, -7.089), (-1.108, -6.250), (-0.793, -4.243), (-0.788, -4.209), (-0.564, -2.641), (-0.427, -1.626), (-0.399, -1.410), (-0.319, -0.800), (-0.024, 1.556), (0.604, 6.821), (0.760, 8.109), (0.902, 9.257), (0.939, 9.553), (1.036, 10.320), (1.249, 11.929), (1.294, 12.261), (1.827, 15.796), (1.951, 16.524), (2.238, 18.097), (2.989, 21.863), (4.064, 28.948), (4.851, 37.111), (5.334, 43.209), (6.011, 51.996), (6.394, 56.453) };
            Nodes = Node.From(tuples).ToSortedBindableCollection();
            SeriesViewModel = new SeriesViewModel(Nodes);
        }

        public bool CanAddPoint(string point, string value)
        {
            return double.TryParse(point, out _) && double.TryParse(value, out _);
        }

        public void AddPoint(string point, string value)
        {
            var parsedPoint = double.Parse(point);
            var parsedValue = double.Parse(value);
            if (Nodes.Any(n => n.X == parsedPoint))
                throw new ArgumentException("The same point exists");

            var node = Node.From((parsedPoint, parsedValue));
            Nodes.InsertSorted(node);
            SeriesViewModel.Add(node);
        }

        public void RemoveItem()
        {
            var node = SelectedNode;
            Nodes.Remove(node);
            SeriesViewModel.Remove(node);
        }

        public bool CanInterpolatePoint(string interpolateFrom)
        {
            return double.TryParse(interpolateFrom, out _);
        }

        public void InterpolatePoint(string interpolateFrom)
        {
            Interpolated = SecondNewton.InterpolateFrom(Nodes, double.Parse(interpolateFrom)).Result;
        }
    }
}
