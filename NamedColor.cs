
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AOP_Homework;

public class NamedColor : ObservableObject{
    private string _name;
    private Color _color;

    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }
    public Color Value
    {
        get { return _color; }
        set { SetProperty(ref _color, value); }
    }


    public NamedColor(){}

}