using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using SDL2;
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

        double aspectRatio = 16.0/9.0;
        height = (int) (width / aspectRatio);

        // Initilizes SDL.
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"There was an issue initilizing SDL. {SDL.SDL_GetError()}");
        }

        // Create a new window given a title, size, and passes it a flag indicating it should be shown.
        var window = SDL.SDL_CreateWindow("Image", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, 
        width, height, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

        if (window == IntPtr.Zero)
        {
            Console.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
        }

        // Creates a new SDL hardware renderer using the default graphics device with VSYNC enabled.
        var renderer = SDL.SDL_CreateRenderer(window,
                                                -1,
                                                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                                                SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

        if (renderer == IntPtr.Zero)
        {
            Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
        }

        // Initilizes SDL_image for use with png files.
        if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
        {
            Console.WriteLine($"There was an issue initilizing SDL2_Image {SDL_image.IMG_GetError()}");
        }


        //Create New array with the number of pixels needed
        Pixel[] pixelArr = new Pixel[width * height];
        
        var shapes = new ArrayList();
        
        Sphere sphere1 = new(new vec4(0, 0, -1, 0), 0.5, new Pixel(1, 1, 1));
        Sphere sphere2 = new(new vec4(0, -100.5, -1, 0), 100, new Pixel(1, 1, 1));
        shapes.Add(sphere1);
        shapes.Add(sphere2);
   
        hitRecord record = new hitRecord();

        //Camera Code
        double focalLength = 1.0;
        double viewportHeight = 2.0;
        double viewportWidth = viewportHeight * (double) ((double) width/ (double) height);
        vec4 cameraCenter = new(0, 0, 0, 0);

        //vectors across horizontal and down vertical
        vec4 viewportU = new(viewportWidth, 0, 0, 0);
        vec4 viewportV = new(0, -viewportHeight, 0 , 0);

        //vectors from pixel to pixel
        vec4 pixelDeltaU = vec4.Div3(viewportU, (double) width);
        vec4 pixelDeltaV = vec4.Div3(viewportV, (double) height);

        //Location of upper left pixel
        vec4 viewport_upper_left = vec4.Sub3( vec4.Sub3( vec4.Sub3(cameraCenter, new vec4(0, 0, focalLength, 0)), vec4.Div3(viewportU, 2)), vec4.Div3(viewportV, 2));
        vec4 pixel00_loc = vec4.Add3(viewport_upper_left, vec4.Mul3(vec4.Add3(pixelDeltaU, pixelDeltaV), 0.5));

        //Render
        //Write Pixel Data loop
        Console.WriteLine("Beginning Raytracing");
        for(int i = 0; i < height; i++) {
            Console.WriteLine($"{i+1} / {height} lines");
            for(int j = 0; j < width; j++) {
                vec4 pixel_center = vec4.Add3(vec4.Add3(pixel00_loc, vec4.Mul3( pixelDeltaU, (double) j)), vec4.Mul3(pixelDeltaV, (double) i));
                vec4 ray_direction = vec4.Sub3(pixel_center, cameraCenter);
                Ray ray = new(cameraCenter, ray_direction);
                
                //Reset record
                record.t = Double.MaxValue;

                pixelArr[j+(i*width)] = ray_color(ray, shapes, record);
            }
        }

        WriteToFile(height, width, pixelArr);

        //SDL stuff
        var running = true;

        // Main loop for the program
        while (running)
        {
            for(int i = 0; i < height; i++) {
                //Console.WriteLine($"{i+1} / {height} lines");
                for(int j = 0; j < width; j++) {
                    while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
                    {
                        switch (e.type)
                        {
                            case SDL.SDL_EventType.SDL_QUIT:
                                running = false;
                                break;
                        }
                    }

                    if(running == false) { 
                        break;
                    }
                    Render(renderer, width, height, pixelArr[j + (i * width)], j, i);
                }
                if (running == false)
                {
                    break;
                }
            }
            SDL.SDL_RenderPresent(renderer);
        }

        // Clean up the resources that were created.
        CleanUp(renderer, window);
    }

    static Pixel ray_color(Ray ray, ArrayList shapes, hitRecord record) {
        //If any of the items hit, take info from the rayhit struct and color pixels
        bool hitMade = false;
        foreach(Shape item in shapes) {
           if(item.RayHit(ray, 0, Double.MaxValue, record)) {
                hitMade = true;
           }
        }

        if(hitMade) {
            return new Pixel(
                (int) (0.5 * (record.normal.e[0] + record.color.r) * 255.999),
                (int) (0.5 * (record.normal.e[1] + record.color.g) * 255.999),
                (int) (0.5 * (record.normal.e[2] + record.color.b) * 255.999));
        }

        vec4 unit_direction = vec4.Unit3(ray.Direction());
        double a = 0.5 * (unit_direction.e[1] + 1.0);
        return new Pixel((int) ((((1.0 - a) * 1.0) + (a * 0.5)) * 255.999), 
                         (int) ((((1.0 - a) * 1.0) + (a * 0.7)) * 255.999),
                         (int) ((((1.0 - a) * 1.0) + (a * 1.0)) * 255.999));
    }

    static void Render(nint renderer, int width, int height, Pixel pixel, int x, int y) {
        SDL.SDL_SetRenderDrawColor(renderer, (byte) pixel.r, (byte) pixel.g, (byte) pixel.b, 255);
        SDL.SDL_RenderDrawPoint(renderer, x, y);
        //SDL.SDL_RenderPresent(renderer);
    }

    static void CleanUp(nint renderer, nint window) {
        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }

    static void WriteToFile(int height, int width, Pixel[] pixelArr) {
        File.WriteAllText("Pic.ppm", "P3\n");
        File.AppendAllText("Pic.ppm", $"{width} {height}\n");
        File.AppendAllText("Pic.ppm", "255\n");
        
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (j != (width - 1))
                {
                    File.AppendAllText("Pic.ppm",
                    $"{pixelArr[j + (i * width)].r} {pixelArr[j + (i * width)].g} {pixelArr[j + (i * width)].b} ");
                }

                else
                {
                    File.AppendAllText("Pic.ppm",
                    $"{pixelArr[j + (i * width)].r} {pixelArr[j + (i * width)].g} {pixelArr[j + (i * width)].b}\n");
                }
            }
        }

    }
} 
