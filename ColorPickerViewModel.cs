using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AOP_Homework;

public partial class ColorPickerViewModel : ObservableObject
{
    
    public bool editsPending;  // Flag to check if there are any unsaved changes
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
    "#00FF00", // Lime
    "#FFC0CB", // Pink
    "#000080", // Navy
    "#A52A2A", // Brown
    "#87CEEB"  // Sky Blue
];

    // [ObservableProperty]
    private ObservableCollection<NamedColor> _customColors;

    public ObservableCollection<NamedColor> customColors
    {
        get => _customColors; 
        set { SetProperty(ref _customColors, value); }
    }
    
    

// public ObservableCollection<Color> CustomPalette { get; set; } = new ObservableCollection<Color>(customColors);
    public ColorPickerViewModel(){
        customColors = new ObservableCollection<NamedColor> (colorCodes.Select(colorCode => 
        new NamedColor{Name = colorCode, Value = Color.Parse(colorCode)}));
    }

}
