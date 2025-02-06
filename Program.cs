namespace SpinningCube
{
    internal class Program
    {
        // Rotation angles for the cube's movement
        static float A, B, C;

        // Size of the cube
        static float cubeWidth = 15;

        // Dimensions of the console screen
        static int width = 100, height = 20;

        // Arrays to store z-buffer and the actual characters to be printed
        static float[] zBuffer = new float[100 * 20];
        static char[] buffer = new char[100 * 20];

        // Background character to clear the screen
        static char backgroundCode = ' ';

        // Camera distance for projection calculations
        static int distanceFromCam = 150;

        // Speed at which the cube is incremented in space
        static float incrementSpeed = 0.4f;

        // Constant for scaling the projection
        static float K1 = 40f;

        // Variables for storing the cube's 3D coordinates
        static float x, y, z;

        // Variable to store the normalized depth (inverse of Z)
        static float ozz;

        // Variables for 2D screen projection coordinates
        static int xp, yp;

        // Index for accessing the buffer
        static int idx;

        // Function to calculate X projection based on cube coordinates and rotation angles
        static float CalculateX(float i, float j, float k)
        {
            return j * MathF.Sin(A) * MathF.Sin(B) * MathF.Cos(C) - k * MathF.Cos(A) * MathF.Sin(B) * MathF.Cos(C) +
                   j * MathF.Cos(A) * MathF.Sin(C) + k * MathF.Sin(A) * MathF.Sin(C) + i * MathF.Cos(B) * MathF.Cos(C);
        }

        // Function to calculate Y projection based on cube coordinates and rotation angles
        static float CalculateY(float i, float j, float k)
        {
            return j * MathF.Cos(A) * MathF.Cos(C) + k * MathF.Sin(A) * MathF.Cos(C) -
                   j * MathF.Sin(A) * MathF.Sin(B) * MathF.Sin(C) + k * MathF.Cos(A) * MathF.Sin(B) * MathF.Sin(C) -
                   i * MathF.Cos(B) * MathF.Sin(C);
        }

        // Function to calculate Z projection based on cube coordinates and rotation angles
        static float CalculateZ(float i, float j, float k)
        {
            return k * MathF.Cos(A) * MathF.Cos(B) - j * MathF.Sin(A) * MathF.Cos(B) + i * MathF.Sin(B);
        }

        // Function that calculates the projection for a surface of the cube
        static void CalculateForSurface(float cubeX, float cubeY, float cubeZ, char ch)
        {
            // Calculate the 3D coordinates of the surface
            x = CalculateX(cubeX, cubeY, cubeZ);
            y = CalculateY(cubeX, cubeY, cubeZ);
            z = CalculateZ(cubeX, cubeY, cubeZ) + distanceFromCam; // Add distance from the camera to Z

            // Normalize Z to avoid large depth values
            ozz = 1 / z;

            // Project 3D coordinates onto the 2D screen
            xp = (int)(width / 2 + K1 * ozz * x * 2);
            yp = (int)(height / 2 + K1 * ozz * y);

            // Calculate the buffer index based on the 2D projection
            idx = xp + yp * width;

            // Check if the calculated index is within valid screen bounds
            if (idx >= 0 && idx < width * height)
            {
                // If the new point is closer than the current point at the same screen position, update it
                if (ozz > zBuffer[idx])
                {
                    zBuffer[idx] = ozz; // Update the z-buffer
                    buffer[idx] = ch;   // Update the screen buffer with the character
                }
            }
        }

        static void Main(string[] args)
        {
            // Clear the console screen at the start
            Console.WriteLine("\x1b[2");

            while (true)
            {
                // Fill the screen buffer with background code to clear previous frame
                Array.Fill(buffer, backgroundCode, 0, width * height);

                // Reset the z-buffer
                Array.Fill(zBuffer, 0.0f);

                // Loop through the cube's surface and calculate projections for each point
                for (float cubeX = -cubeWidth; cubeX < cubeWidth; cubeX += incrementSpeed)
                {
                    for (float cubeY = -cubeWidth; cubeY < cubeWidth; cubeY += incrementSpeed)
                    {
                        // Calculate and render the surfaces of the cube with different characters
                        CalculateForSurface(cubeX, cubeY, -cubeWidth, '*');
                        CalculateForSurface(cubeWidth, cubeY, cubeX, '$');
                        CalculateForSurface(-cubeWidth, cubeY, -cubeX, '~');
                        CalculateForSurface(-cubeX, cubeY, cubeWidth, '#');
                        CalculateForSurface(cubeX, -cubeWidth, -cubeY, '"');
                        CalculateForSurface(cubeX, cubeWidth, cubeY, '+');
                    }
                }

                // Reset the cursor to the top of the console
                Console.WriteLine("\x1b[H");

                // Print the buffer to the console, rendering the cube
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Console.Write(buffer[x + y * width]);  // Print each character at the correct position
                    }
                    Console.WriteLine();  // Move to the next line after printing a row
                }

                // Update the rotation angles to animate the cube
                A += 0.3f;  // Adjust A to control the rotation on X-axis
                B += 0.3f;  // Adjust B to control the rotation on Y-axis

                // Small delay to control the speed of the animation
                Thread.Sleep(50);
            }
        }
    }
}
