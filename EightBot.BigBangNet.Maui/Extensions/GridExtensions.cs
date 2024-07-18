using System;
using System.Collections.Generic;
using Microsoft.Maui;
using System.Linq;
using System.Data.Common;

namespace EightBot.BigBang.Maui
{
    public static class GridExtensions
    {
        public static GridBuilder CreateBuilder(this Grid grid)
        {
            return new GridBuilder(grid);
        }
    }

    public class GridBuilder
    {
        Grid _grid;

        readonly List<GridCommand> _gridCommands = new List<GridCommand>();

        public GridBuilder(Grid grid)
        {
            _grid = grid;
        }

        public GridBuilder AddRow(params View[] views)
        {
            if (views?.Any() ?? false)
                return this;

            var maxRow = _gridCommands.Max(x => x.Row) + 1;

            for (int i = 0; i < views.Count(); i++)
            {
                var view = views[i];

                if (view != null)
                    _gridCommands.Add(
                        new GridCommand()
                        {
                            Column = i,
                            Row = maxRow
                        });
            }

            return this;
        }

        public GridBuilder AddRow(View view, int column = 0, bool spanAllColumns = false)
        {
            var maxRow = _gridCommands.Max(x => x.Row) + 1;

            _gridCommands.Add(
                    new GridCommand(view)
                    {
                        Column = column,
                        Row = maxRow,
                        SpanAllColumns = spanAllColumns
                    });

            return this;
        }

        public GridBuilder Add(GridCommand gridCommand)
        {
            _gridCommands.Add(gridCommand);

            return this;
        }

        public GridBuilder AddToFullRow(View view, int row = 0, int rowSpan = 0)
        {
            _gridCommands.Add(new GridCommand(view) { Row = row, RowSpan = rowSpan, SpanAllColumns = true });

            return this;
        }

        public GridBuilder AddToFullColumn(View view, int column = 0, int columnSpan = 0)
        {
            _gridCommands.Add(new GridCommand(view) { Column = column, ColumnSpan = columnSpan, SpanAllRows = true });

            return this;
        }

        public GridBuilder AddToFullGrid(View view, int column = 0, int row = 0)
        {
            _gridCommands.Add(new GridCommand(view) { Column = column, Row = row, SpanAllColumnsAndRows = true });

            return this;
        }

        public GridBuilder AddToEndOfRow(View view, int row = 0)
        {
            var maxColumns = _grid.ColumnDefinitions.Count - 1;
            _gridCommands.Add(new GridCommand(view) { Column = maxColumns, Row = row, SpanAllColumnsAndRows = true });

            return this;
        }

        public GridBuilder AddToStartOfRow(View view, int row = 0)
        {
            _gridCommands.Add(new GridCommand(view) { Column = 0, Row = row, SpanAllColumnsAndRows = true });

            return this;
        }

        public GridBuilder AddToRightOf(View view, View comparisonView)
        {
            var foundView = _gridCommands.FirstOrDefault(x => x.View == comparisonView);

            if (foundView != null)
                _gridCommands.Add(new GridCommand(view) { Column = foundView.Column + 1, Row = foundView.Row });

            return this;
        }

        public GridBuilder AddToLeftOf(View view, View comparisonView)
        {
            var foundView = _gridCommands.FirstOrDefault(x => x.View == comparisonView);

            if (foundView != null)
                _gridCommands.Add(new GridCommand(view) { Column = foundView.Column - 1, Row = foundView.Row });

            return this;
        }

        public GridBuilder AddAboveOf(View view, View comparisonView)
        {
            var foundView = _gridCommands.FirstOrDefault(x => x.View == comparisonView);

            if (foundView != null)
                _gridCommands.Add(new GridCommand(view) { Column = foundView.Column, Row = foundView.Row - 1 });

            return this;
        }

        public GridBuilder AddBelowOf(View view, View comparisonView)
        {
            var foundView = _gridCommands.FirstOrDefault(x => x.View == comparisonView);

            if (foundView != null)
                _gridCommands.Add(new GridCommand(view) { Column = foundView.Column, Row = foundView.Row + 1 });

            return this;
        }

        public void Build()
        {
            _grid.Children.Clear();

            var maxColumns = _grid.ColumnDefinitions.Count;
            var maxRows = _grid.RowDefinitions.Count;

            foreach (var gridCommand in _gridCommands)
            {
                _grid.Add(gridCommand.View, gridCommand.Column, gridCommand.Row);

                if (gridCommand.SpanAllColumnsAndRows)
                {
                    Grid.SetColumnSpan(gridCommand.View, maxColumns - gridCommand.Column);
                    Grid.SetColumnSpan(gridCommand.View, maxRows - gridCommand.Row);
                }
                else
                {
                    if (gridCommand.SpanAllColumns)
                    {
                        Grid.SetColumnSpan(gridCommand.View, maxColumns - gridCommand.Column);
                    }
                    else if (gridCommand.ColumnSpan > 0)
                    {
                        Grid.SetColumnSpan(gridCommand.View, gridCommand.ColumnSpan);
                    }

                    if (gridCommand.SpanAllRows)
                    {
                        Grid.SetColumnSpan(gridCommand.View, maxRows - gridCommand.Row);
                    }
                    else if (gridCommand.RowSpan > 0)
                    {
                        Grid.SetColumnSpan(gridCommand.View, gridCommand.RowSpan);
                    }
                }

            }
        }

        public static GridBuilder operator +(GridBuilder gridBuilder, View view)
        {
            gridBuilder.AddRow(view);

            return gridBuilder;
        }

        public static GridBuilder operator +(GridBuilder gridBuilder, IEnumerable<View> views)
        {
            gridBuilder.AddRow(views.ToArray());

            return gridBuilder;
        }
    }

    public class GridCommand
    {
        public View View { get; set; }

        public int Column { get; set; }
        public int Row { get; set; }

        public int ColumnSpan { get; set; }
        public int RowSpan { get; set; }

        public bool SpanAllColumnsAndRows { get; set; }
        public bool SpanAllColumns { get; set; }
        public bool SpanAllRows { get; set; }

        public GridCommand()
            : this(null)
        {
        }

        public GridCommand(View view)
        {
            View = view;
        }

        public static implicit operator GridCommand(View view)
        {
            return new GridCommand(view);
        }
    }

    public static class GridCommandExtensions
    {
        public static GridCommand WithColumn(GridCommand gridCommand, int column)
        {
            gridCommand.Column = column;
            return gridCommand;
        }

        public static GridCommand WithRow(GridCommand gridCommand, int row)
        {
            gridCommand.Row = row;
            return gridCommand;
        }

        public static GridCommand WithColumnSpan(GridCommand gridCommand, int columnSpan)
        {
            gridCommand.ColumnSpan = columnSpan;
            return gridCommand;
        }

        public static GridCommand WithRowSpan(GridCommand gridCommand, int rowSpan)
        {
            gridCommand.RowSpan = rowSpan;
            return gridCommand;
        }
    }
}
