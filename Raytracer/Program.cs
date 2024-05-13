using System;
class Program
{
    static void Main(string[] args)
    {
        //set initial values for image width and height if none provided
        int width = 512;
        int height = 512;

        //If values are provided, set the values
        if (args.Length == 2) {
            width = int.Parse(args[0]);
            height = int.Parse(args[1]);
        }

        //verify values set
        Console.WriteLine( width + " " + height);

        //create file with a name
        File.WriteAllText("Pic.ppm", "P3\n");
        File.AppendAllText("Pic.ppm", $"{width} {height}\n");
        File.AppendAllText("Pic.ppm", "255\n");

        Pixel[] pixelArr = new Pixel[width * height];

        //Write Pixel Data loop
        for(int i = 0; i < height; i++) {
            for(int j = 0; j < width; j++) {
                pixelArr[j+(i*width)] = new Pixel(120, 120, 120);
            }
        }

        //Write to file loops
        for(int i = 0; i < height; i++) {
            for(int j = 0; j < width; j++) {
                if(j != (width - 1)) {
                    File.AppendAllText("Pic.ppm", 
                    $"{pixelArr[j+(i*width)].r} {pixelArr[j+(i*width)].g} {pixelArr[j+(i*width)].b} ");
                }

                else {
                    File.AppendAllText("Pic.ppm", 
                    $"{pixelArr[j+(i*width)].r} {pixelArr[j+(i*width)].g} {pixelArr[j+(i*width)].b}\n");
                }
            }
        }

    }
} 
