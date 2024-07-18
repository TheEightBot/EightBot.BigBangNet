using System;
using System.Linq;
using Microsoft.Maui;

namespace EightBot.BigBang.Maui
{
    public static class IGridListViewExtensions
    {
        public static View ElementAtOrDefault(this Microsoft.Maui.Controls.Compatibility.Grid.IGridList<View> gridList, int column, int row)
        {
            return gridList.FirstOrDefault(x => Grid.GetColumn(x) == column && Grid.GetRow(x) == row);
        }

        public static bool RemoveAt(this Microsoft.Maui.Controls.Compatibility.Grid.IGridList<View> gridList, int column, int row)
        {
            var item = gridList.ElementAtOrDefault(column, row);

            if (item == null)
                return false;

            return gridList.Remove(item);
        }
    }
}
