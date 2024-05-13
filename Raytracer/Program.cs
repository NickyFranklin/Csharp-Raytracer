using System;
using Microsoft.VisualBasic;
using SDL2;
class Program
{
    static void Main(string[] args)
    {
        // Initilizes SDL.
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"There was an issue initilizing SDL. {SDL.SDL_GetError()}");
        }

        // Create a new window given a title, size, and passes it a flag indicating it should be shown.
        var window = SDL.SDL_CreateWindow("Image", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, 1024, 1024, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

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
        Console.WriteLine("Beginning Raytracing");
        for(int i = 0; i < height; i++) {
            Console.WriteLine($"{i+1} / {height} lines");
            for(int j = 0; j < width; j++) {
                double tempRed = ((double) j) / (width-1);
                double tempGreen = ((double) i) / (height - 1);
                double tempBlue = 0;
                pixelArr[j+(i*width)] = 
                    new Pixel((int) (tempRed * 255.999), (int) (tempGreen * 255.999), (int) (tempBlue * 255.999));
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


        //SDL stuff
        var running = true;

        // Main loop for the program
        while (running)
        {
            // Check to see if there are any events and continue to do so until the queue is empty.
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        running = false;
                        break;
                }
            }

            // Sets the color that the screen will be cleared with.
            if (SDL.SDL_SetRenderDrawColor(renderer, 135, 206, 235, 255) < 0)
            {
                Console.WriteLine($"There was an issue with setting the render draw color. {SDL.SDL_GetError()}");
            }

            // Clears the current render surface.
            if (SDL.SDL_RenderClear(renderer) < 0)
            {
                Console.WriteLine($"There was an issue with clearing the render surface. {SDL.SDL_GetError()}");
            }

            // Switches out the currently presented render surface with the one we just did work on.
            SDL.SDL_RenderPresent(renderer);
        }

        // Clean up the resources that were created.
        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();

    }

    void Render(nint renderer, int width, int height, Pixel pixel, int x, int y) {
        SDL.SDL_SetRenderDrawColor(renderer, (byte) pixel.r, (byte) pixel.g, (byte) pixel.b, 255);
        SDL.SDL_RenderClear(renderer);
        SDL.SDL_RenderPresent(renderer);
        SDL.SDL_RenderDrawPoint(renderer, x, y);
    }

    

} 
