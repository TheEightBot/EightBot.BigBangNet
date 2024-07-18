using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EightBot.BigBangNet.MauiSample;

public partial class ListViewContent : ViewCell
{
    public ListViewContent()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        Console.WriteLine($"ViewCell Property: {propertyName}");
    }
}