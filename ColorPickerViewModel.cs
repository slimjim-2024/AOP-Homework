using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace AOP_Homework;

public partial class ColorPickerViewModel : ObservableObject
{
    
    public bool editsPending;  // Flag to check if there are any unsaved changes
    [ObservableProperty]
    private static string[] colorCodes = [
    "#FFFFFF", // White
    "#000000", // Black
    "#FF0000", // Red
    "#00FFFF", // Cyan
    "#00FF00", // Green
    "#FF00FF", // Magenta
    "#0000FF", // Blue
    "#FFFF00", // Yellow
    "#FFA500", // Orange
    "#008080", // Teal
    "#800080", // Purple
    "#40D61A", // Lime
    "#FFC0CB", // Pink
    "#000080", // Navy
    "#A52A2A", // Brown
    "#87CEEB"  // Sky Blue
];

    [ObservableProperty]
    private List<NamedColor> _customColors;    
    // private ObservableCollection<string> _customColors;    
        

// public ObservableCollection<Color> CustomPalette { get; set; } = new ObservableCollection<Color>(customColors);
    // [RelayCommand]
    public ColorPickerViewModel(){
        CustomColors = new List<NamedColor> (colorCodes.Select(colorCode =>
        {
            Debug.WriteLine("" + colorCode);
            return new NamedColor{Name = colorCode, Value = Brush.Parse(colorCode)};
        }
        ));
        // CustomColors = new ObservableCollection<string>(colorCodes);
    }

}
