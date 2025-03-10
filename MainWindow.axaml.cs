using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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
    private ColorPickerViewModel _viewModel;
    int height; // Stores the dimensions of the grid (height)
    int width; // Stores the dimensions of the grid (width)
    private int[][] data; // 2D array to store the grid data (binary or color)
    // bool isColor = false; // Made a method to check
    int heightMultiplier=50, widthMultiplier=50; // Size of each grid cell in pixels


    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new ColorPickerViewModel();
        // comboBox = this.Find<ComboBox>("ColorList");
        DataContext = _viewModel;
        // comboBox.ItemsSource = _viewModel.CustomColors;
        // comboBox.SelectedIndex = 3;
        // ColorList.ItemsSource = new string[] { "a", "s", "d",};
        foreach (var item in ColorList.Items) Debug.WriteLine(item.ToString() + "Gotten From the list");
        Main_Window.Title = "No file opened";
        Canvas.Width = 400;
        Canvas.Height = 300;
        Canvas.Background = Brushes.White;
        ColorList.SelectedIndex = 0;
    }

    private bool IsMonochrome(int[][] data)
    {
        foreach (var row in data)
        {
            if (row.Any(x => x != 0 && x != 1)) return false;
        }
        return true;
    }

    // Save the file
    private async void Save(object sender, RoutedEventArgs e)
    {
        if (data == null)
        {
            return;
        }
        var result = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save a b2img.txt file",
            FileTypeChoices = IsMonochrome(data) ? new List<FilePickerFileType>
            {
                // Monochrome file types
                new FilePickerFileType("b2img file (*.b2img)")
                {
                    Patterns = ["*.b2img"]
                },
                new FilePickerFileType("b2img.txt file (*.b2img.txt)")
                {
                    Patterns = ["*.b2img.txt"]
                }
            } : new List<FilePickerFileType>
            {
                // Non-monochrome file types
                new FilePickerFileType("b16img file (*.b16img.txt)")
                {
                    Patterns = ["*.b16img.txt"]
                }
            },
            DefaultExtension = IsMonochrome(data) ? ".b2img.txt" : ".b16img.txt",
            SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(Environment.CurrentDirectory),
            SuggestedFileName = Main_Window.Title,
            ShowOverwritePrompt = true  // Show a prompt if the file already exists
        });

        if (result != null)
        {
            var LocalPath = result.Path.AbsolutePath;
            Main_Window.Title = result.Name;
            SaveFile(LocalPath.Replace("%20"," "), result.Name.EndsWith(".b2img")); // Save the file and replacing "%20" with spaces
        }
    }

    // Open the file
    private async void Load(object sender, RoutedEventArgs e)
    {
        IReadOnlyList<IStorageFile> result = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open a b2img.txt file",
            // File type picker
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("All supported formats (*.b2img *.b2img.txt *.b16img.txt)")
                {
                    Patterns = ["*.b2img", "*.b2img.txt", "*.b16img.txt"]
                },
                new FilePickerFileType("b2img file (*.b2img)")
                {
                    Patterns = ["*.b2img"]
                },
                new FilePickerFileType("b2img.txt file (*.b2img.txt)")
                {
                    Patterns = ["*.b2img.txt"]
                },
                new FilePickerFileType("b16img file (*.b16img.txt)")
                {
                    Patterns = ["*.b16img.txt"]
                }
            },
            SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(Environment.CurrentDirectory), // Open the file dialog in the current directory
            SuggestedFileName = Main_Window.Title,
            AllowMultiple = false // Allow only one file to be selected
        });

        if (result != null && result.Count > 0) 
        {
            // Get the path of the selected file
            var LocalPath = result[0].Path.AbsolutePath;
            // Set the title of the window to the name of the file, removes extensions from name
            Main_Window.Title = result[0].Name.Replace(".txt", "").Replace(".b2img", "").Replace(".b16img", "");
            // Load the file and replacing "%20" with spaces, determines whether the file is in binary depending on the extension
            LoadFile(LocalPath.Replace("%20", " "), result[0].Name.EndsWith(".b2img"));
        }
    }

    private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetPosition(Canvas); // Get the position of the pointer
        int x = (int)point.X / widthMultiplier; // Calculate grid columm
        int y = (int)point.Y / heightMultiplier; // Calculate grid row
        if (x < width && y < height)
        {
            data[y][x] =  ColorList.SelectedIndex; // Changes pixel color
            UpdateCanvas(data); // Redraw the canvas with updated data
        }
    }

    // Redraws the grid on the canvas using the stored data
    private void DrawCanvas(int[][] drawData)
    {
        // if (IsColor) {
        for (int i = 0; i < height; i++) // Loop through each row
        {
            for (int j = 0; j < width; j++) // Loop through each column
            {
                Rectangle rect = new Rectangle(); // Create a new rectangle to represent a grid cell
                rect.Width = widthMultiplier; // Set the width of the rectangle
                rect.Height = widthMultiplier; // Set the height of the rectangle
                rect.Fill = _viewModel.CustomColors[drawData[i][j]].Value; // Set the fill color of the rectangle
                Canvas.SetLeft(rect, j * widthMultiplier);
                Canvas.SetTop(rect, i * heightMultiplier);
                Canvas.Children.Add(rect); // Add the rectangle to the canvas
            }
        }
    }

    // Very similar to the "DrawCanvas" method, but it only updates the grid cells that have changed
    private void UpdateCanvas(int[][] drawData)
    {
        data = drawData;
        DrawCanvas(data);
    }

    private void LoadFile(string file, bool isBinary =  false)
    {
        string pixelString = "";
        if (isBinary)
        {
            using BinaryReader br = new BinaryReader(File.Open(file, FileMode.Open));
            height = br.ReadInt32();
            width = br.ReadInt32();

            // Converts bytes into binary string
            for (int i = 0; i < height*width/8 + 1; i++)
            {
                pixelString += Convert.ToString(br.ReadByte(), 2).PadLeft(8, '0');
            }

            // Removes filler bits
            pixelString = pixelString[..(height * width)];
            Console.WriteLine($"{pixelString}");

            br.Close();
        }
        else
        {
            using StreamReader sr = new StreamReader(file);
            string? resline = sr.ReadLine();
            if (resline == null) return;
            string[] line = resline.Split();
            height = int.Parse(line[0]);
            width = int.Parse(line[1]);
            pixelString = sr.ReadToEnd();
            sr.Close();
        }
        
        // Canvas Prep
        data = new int[height][];
        Canvas.Height = height * heightMultiplier;
        Canvas.Width = width * widthMultiplier;
        Canvas.Children.Clear();

        for (int i = 0; i < height; i++)
        {
            data[i] = new int[width];
            for (int j = 0; j < width; j++)
            {
                data[i][j] = Convert.ToInt32(pixelString[i * width + j].ToString(), 16);
            }
        }

        DrawCanvas(data);
    }

    protected internal void SaveFile(string file, bool isBinary = false)
    {
        // Gets pixel values as string
        string pixelString = "";
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                pixelString += Convert.ToString(data[i][j], 16).ToUpper();
            }
        }

        // Saving as binary
        if (isBinary)
        {
            // Adds filler bits to complete last byte
            for (int _ = 0; _ < pixelString.Length%8; _++)
            {
                pixelString += "0";
            }
            // Turns string into bytes
            byte[] pixelBytes = new byte[pixelString.Length / 8];
            for (int i = 0; i < pixelBytes.Length; i++)
            {
                string byteString = pixelString.Substring(i * 8, 8);
                pixelBytes[i] = Convert.ToByte(byteString, 2);
            }
            // Writes binary file
            using (BinaryWriter bw = new BinaryWriter(File.Open(file, FileMode.Create)))
            {
                bw.Write(height);
                bw.Write(width);
                bw.Write(pixelBytes);
            }
            return;
        }

        // Saving not as binary
        using (StreamWriter sw = new StreamWriter(file))
        {
            sw.WriteLine($"{height} {width}");
            sw.Write(pixelString);
        }
    }
}