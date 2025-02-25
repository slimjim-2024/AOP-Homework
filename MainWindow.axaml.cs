using System;
using System.Collections.Generic;
using System.IO;
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
                    Patterns = new[] { "*.b2img.txt" }
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
    private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetPosition(Canvas);
        int x = (int)point.X / 100;
        int y = (int)point.Y / 100;
        if (x < width && y < height)
        {
            data[y][x] = data[y][x] == '0' ? '1' : '0';
            UpdateCanvas(data);
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
                    Patterns = new[] { "*.b2img.txt" }
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
            LoadFile(LocalPath.Replace("%20"," "));
        }
    }
    private void UpdateCanvas(char[][] drawData)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Rectangle rect = new Rectangle();
                rect.Width = 100;
                rect.Height = 100;
                rect.Fill = drawData[i][j] switch
                {
                    '0' => Brushes.White,
                    '1' => Brushes.Black,
                    _ => Brushes.Red
                };
                Canvas.SetLeft(rect, j * 100);
                Canvas.SetTop(rect, i * 100);
                Canvas.Children.Add(rect);

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
        Canvas.Height = height * 100;
        Canvas.Width = width * 100;
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