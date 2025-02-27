
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AOP_Homework;

public partial class NamedColor : ObservableObject{
[ObservableProperty]
    private string _name;
    [ObservableProperty]
    private IBrush _value;


    // public string Name
    // {
    //     get { return _name; }
    //     set { SetProperty(ref _name, value); }
    // }
    // public Color Value
    // {
    //     get { return _color; }
    //     set { SetProperty(ref _color, value); }
    // }


    public NamedColor(){}

    public override string ToString(){
        return $"{Name} {Value.ToString()}";
    }
}