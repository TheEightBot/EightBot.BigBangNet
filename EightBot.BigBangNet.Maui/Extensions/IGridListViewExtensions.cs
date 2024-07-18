using System;
using System.Linq;
using Xamarin.Forms;

namespace EightBot.BigBang.XamForms
{
    public static class IGridListViewExtensions
    {
        public static View ElementAtOrDefault(this Grid.IGridList<View> gridList, int column, int row) {
            return gridList.FirstOrDefault(x => Grid.GetColumn(x) == column && Grid.GetRow(x) == row);
        }

        public static bool RemoveAt(this Grid.IGridList<View> gridList, int column, int row)
        {
            var item = gridList.ElementAtOrDefault(column, row);

            if (item == null)
                return false;

            return gridList.Remove(item);
        }
    }
}
