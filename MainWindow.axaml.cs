using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;

namespace AOP_Homework;

public partial class MainWindow : Window
{
    int height;     // Stores the dimensions of the grid (height and width)
    int width;
    private char[][] data;
    private bool editsPending = false;
    private string[] colorCodes = {
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
};
bool IsColor = false;
int heightMultiplier=50, widthMultiplier=50;


    public MainWindow()
    {
        InitializeComponent();
        Main_Window.Title = "No file opened";
        Canvas.Width = 400;
        Canvas.Height = 300;
        Canvas.Background = Brushes.White;
    }
    private async void Save(object sender, RoutedEventArgs e)
    {
        var result = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Open a b2img.txt file",
            FileTypeChoices = new List<FilePickerFileType>
            {
                new FilePickerFileType("b2img.txt files")
                {
                    Patterns = new[] { "*.b2img.txt", "*.b2img" },
                }
            },
            DefaultExtension=".b2img.txt",
            SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(Environment.CurrentDirectory),
            SuggestedFileName = Main_Window.Title,
            ShowOverwritePrompt = true
        });

        if (result != null)
        {
            var LocalPath = result.Path.AbsolutePath;
            Main_Window.Title = result.Name;
            SaveFile(LocalPath.Replace("%20"," "));
        }
    }
    private async void Load(object sender, RoutedEventArgs e)
    {
        var result = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open a b2img.txt file",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("b2img.txt files")
                {
                    Patterns = ["*.b2img.txt"]
                },
                new FilePickerFileType("b16img.txt color files")
                {
                    Patterns = ["*.b16img.txt"]
                }
            },
            SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(Environment.CurrentDirectory),
            SuggestedFileName = Main_Window.Title,
            AllowMultiple = false
        });

        if (result != null && result.Count > 0)
        {
            var LocalPath = result[0].Path.AbsolutePath;
            Main_Window.Title = result[0].Name;
            IsColor = result[0].Name.Contains("b16img.txt");
            LoadFile(LocalPath.Replace("%20"," "));
        }
    }
    private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetPosition(Canvas);
        int x = (int)point.X / widthMultiplier;
        int y = (int)point.Y / heightMultiplier;
        if (x < width && y < height)
        {
            data[y][x] = data[y][x] == '0' ? '1' : '0';
            UpdateCanvas(data);
        }
    }
    private void UpdateCanvas(char[][] drawData)
    {
        if (IsColor){
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Rectangle rect = new Rectangle();
                rect.Width = widthMultiplier;
                rect.Height = widthMultiplier;
                rect.Fill = drawData[i][j] switch 
                {
                    '0' => Brush.Parse(colorCodes[0]),
                    '1' => Brush.Parse(colorCodes[1]),
                    '2' => Brush.Parse(colorCodes[2]),
                    '3' => Brush.Parse(colorCodes[3]),
                    '4' => Brush.Parse(colorCodes[4]),
                    '5' => Brush.Parse(colorCodes[5]),
                    '6' => Brush.Parse(colorCodes[6]),
                    '7' => Brush.Parse(colorCodes[7]),
                    '8' => Brush.Parse(colorCodes[8]),
                    '9' => Brush.Parse(colorCodes[9]),
                    'A' => Brush.Parse(colorCodes[0xA]),
                    'B' => Brush.Parse(colorCodes[0xB]),
                    'C' => Brush.Parse(colorCodes[0xC]),
                    'D' => Brush.Parse(colorCodes[0xD]),
                    'E' => Brush.Parse(colorCodes[0xE]),
                    'F' => Brush.Parse(colorCodes[0xF]),
                    _ => Brushes.Black

                };
                Canvas.SetLeft(rect, j * widthMultiplier);
                Canvas.SetTop(rect, i * heightMultiplier);
                Canvas.Children.Add(rect);

            }
        }
        }
        else {
            for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Rectangle rect = new Rectangle();
                rect.Width = widthMultiplier;
                rect.Height = heightMultiplier;
                rect.Fill = drawData[i][j] switch 
                {
                    '0' => Brush.Parse(colorCodes[0]),
                    '1' => Brush.Parse(colorCodes[1]),
                    _ => Brushes.Black
                };
                Canvas.SetLeft(rect, j * widthMultiplier);
                Canvas.SetTop(rect, i * heightMultiplier);
                Canvas.Children.Add(rect);

            }
        }
        }
        editsPending=true;
    }
    private void LoadFile(string file)
    {
        using StreamReader sr = new StreamReader(file);
        string[] line = sr.ReadLine().Split(' ');
        height = int.Parse(line[0]);
        width = int.Parse(line[1]);
        char[] chardata = sr.ReadToEnd().ToCharArray();
        data = new char[height][];
        Canvas.Height = height * heightMultiplier;
        Canvas.Width = width * widthMultiplier;
        Canvas.Children.Clear();
        for (int i = 0; i < height; i++)
        {
            data[i] = new char[width];
            for (int j = 0; j < width; j++)
            {
                data[i][j] = chardata[i * width + j];
            }
        }
        sr.Close();
        UpdateCanvas(data);
        editsPending=false;
    }
    protected internal void SaveFile(string file)
    {
        using StreamWriter sw = new StreamWriter(file);
        sw.WriteLine($"{height} {width}");
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                sw.Write(data[i][j]);
            }
        }
        sw.Close();
        editsPending=false;
    }
}