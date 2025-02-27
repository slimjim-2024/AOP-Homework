using Avalonia.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AOP_Homework;

public class ColorPickerViewModel : INotifyPropertyChanged
{
    private static readonly Color[] customColors = {
        Colors.White,
        Colors.Black,
        Colors.Red,
        Colors.Cyan,
        Colors.Green,
        Colors.Magenta,
        Colors.Blue,
        Colors.Yellow,
        Colors.Orange,
        Colors.Teal,
        Colors.Purple,
        Colors.Lime,
        Colors.Pink,
        Colors.Navy,
        Colors.Brown,
        Colors.SkyBlue
    };

public ObservableCollection<Color> CustomPalette { get; set; } = new ObservableCollection<Color>(customColors);

    private Color _selectedColor;
    public Color SelectedColor
    {
        get => _selectedColor;
        set
        {
            _selectedColor = value;
            OnPropertyChanged(nameof(SelectedColor));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
