using System;

namespace ExtraUtils
{
    /// <summary>
    /// Improves on Console.Write methods by adding colours
    /// </summary>
    public class ColourWriter
    {
        /// <summary>
        /// Flag idicator for using colours
        /// </summary>
        public bool USE_COLOURS;
        /// <summary>
        /// Single colour setting
        /// </summary>
        public int MULTI_LINES = 37;
        /// <summary>
        /// Single colour for user input
        /// </summary>
        public int INPUT_COLOUR = 37;

        /// <summary>
        /// ColourWriter instance
        /// </summary>
        public ColourWriter()
        {
            USE_COLOURS = false;
        }

        /// <summary>
        /// Writes single line of text (without breaking the line).</summary>
        /// <param name="text"> Text to print</param>
        /// <seealso cref="Console.Write(string)">
        /// Similar to Console output Write() method. </seealso>
        public void Write(string text)
        {
            if (USE_COLOURS)
            {
                Console.Write("\x1b[" + MULTI_LINES + "m");
                Console.Write(text);
                Console.Write("\x1b[0m");
            }
            else
            {
                Console.Write(text);
            }
        }

        /// <summary>
        /// Writes single line of text (without breaking the line).</summary>
        /// <param name="text"> Text to print</param>
        /// <param name="colour"> Colour to apply to text</param>
        /// <seealso cref="Console.Write(string)">
        /// Similar to Console output Write() method. </seealso>
        public void Write(string text, int colour)
        {
            if (USE_COLOURS)
            {
                    Console.Write("\x1b[" + colour + "m");
                    Console.Write(text);
                    Console.Write("\x1b[0m");
            }
            else
            {
                Console.Write(text);
            }
        }

        /// <summary>
        /// Writes single line of text (without breaking the line).</summary>
        /// <param name="text"> Text to print</param>
        /// <param name="colour"> Colour to apply to text</param>
        /// <param name="formatting"> formattign to apply to text (bold, inverse, etc.)</param>
        /// <seealso cref="Console.Write(string)">
        /// Similar to Console output Write() method. </seealso>
        public void Write(string text, int colour, int formatting)
        {
            if (USE_COLOURS)
            {
                Console.Write("\x1b[" + formatting + "m");
                    Console.Write("\x1b[" + colour + "m");
                        Console.Write(text);
                    Console.Write("\x1b[0m");
                Console.Write("\x1b[0m");
            }
            else
            {
                Console.Write(text);
            }
        }

        /// <summary>
        /// Writes single line of text (without breaking the line).</summary>
        /// <param name="text"> Text to print</param>
        /// <param name="text2"> Text to print without colour</param>
        /// <param name="colour"> Colour to apply to text</param>
        /// <seealso cref="Console.Write(string)">
        /// Similar to Console output Write() method. </seealso>
        public void Write(string text, string text2, int colour)
        {
            if (USE_COLOURS)
            {
                Console.Write("\x1b[" + colour + "m");
                Console.Write(text);
                Console.Write("\x1b[0m");
                Console.Write(text2);
            }
            else
            {
                Console.Write(text + text2);
            }
        }

        /// <summary>
        /// Writes single line of text (without breaking the line).</summary>
        /// <param name="text"> Text to print</param>
        /// <param name="text2"> Text to print </param>
        /// <param name="colour"> Colour to apply to text</param>
        /// <param name="colour2"> Colour to apply to text2</param>
        /// <seealso cref="Console.Write(string)">
        /// Similar to Console output Write() method. </seealso>
        public void Write(string text, string text2, int colour, int colour2)
        {
            if (USE_COLOURS)
            {
                Console.Write("\x1b[" + colour + "m");
                Console.Write(text);
                Console.Write("\x1b[0m");
                Console.Write("\x1b[" + colour2 + "m");
                Console.Write(text2);
                Console.Write("\x1b[0m");
            }
            else
            {
                Console.Write(text + text2);
            }
        }

        /// <summary>
        /// Writes single line of text (breaking the line).
        /// Uses MULTI_LINE colour code to print each line. </summary>
        /// <param name="text"> Text to print</param>
        /// <seealso cref="Console.WriteLine()">
        /// Similar to Console output WriteLine() method. </seealso>
        public void WriteLine(string text)
        {
            if (USE_COLOURS)
            {
                Console.Write("\x1b[" + MULTI_LINES + "m");
                Console.WriteLine(text);
                Console.Write("\x1b[0m");
            }
            else
            {
                Console.WriteLine(text);
            }
        }

        /// <summary>
        /// Writes single line of text (breaking the line).</summary>
        /// <param name="text"> Text to print</param>
        /// <param name="colour"> Colour to apply to text</param>
        /// <seealso cref="Console.WriteLine()">
        /// Similar to Console output WriteLine() method. </seealso>
        public void WriteLine(string text, int colour)
        {
            if (USE_COLOURS)
            {
                    Console.Write("\x1b[" + colour + "m");
                    Console.WriteLine(text);
                    Console.Write("\x1b[0m");                
            }
            else
            {
                Console.WriteLine(text);
            }
        }

        /// <summary>
        /// Writes single line of text (breaking the line).</summary>
        /// <param name="text"> Text to print</param>
        /// <param name="colour"> Colour to apply to text</param>
        /// <param name="formatting"> formattign to apply to text (bold, inverse, etc.)</param>
        /// <seealso cref="Console.Write(string)">
        /// Similar to Console output Write() method. </seealso>
        public void WriteLine(string text, int colour, int formatting)
        {
            if (USE_COLOURS)
            {
                Console.Write("\x1b[" + formatting + "m");
                Console.Write("\x1b[" + colour + "m");
                Console.WriteLine(text);
                Console.Write("\x1b[0m");
                Console.Write("\x1b[0m");
            }
            else
            {
                Console.Write(text);
            }
        }

        /// <summary>
        /// Writes single line of text (breaking the line).</summary>
        /// <param name="text"> Text to print with colour</param>
        /// <param name="text2"> Text to print without colour</param>
        /// <param name="colour"> Colour to apply to text</param>
        /// <seealso cref="Console.WriteLine()">
        /// Similar to Console output WriteLine() method. </seealso>
        public void WriteLine(string text, string text2, int colour)
        {
            if (USE_COLOURS)
            {
                Console.Write("\x1b[" + colour + "m");
                Console.Write(text);
                Console.Write("\x1b[0m");
                Console.WriteLine(text2);
            }
            else
            {
                Console.WriteLine(text+text2);
            }
        }

        /// <summary>
        /// Writes single line of text (breaking the line).</summary>
        /// <param name="text"> Text to print with colour</param>
        /// <param name="text2"> Text to print  with colour</param>
        /// <param name="colour"> Colour to apply to text</param>
        /// <param name="colour2"> Colour to apply to text2</param>
        /// <seealso cref="Console.WriteLine()">
        /// Similar to Console output WriteLine() method. </seealso>
        public void WriteLine(string text, string text2, int colour, int colour2)
        {
            if (USE_COLOURS)
            {
                Console.Write("\x1b[" + colour + "m");
                Console.Write(text);
                Console.Write("\x1b[0m");
                Console.Write("\x1b[" + colour2 + "m");
                Console.Write(text2);
                Console.WriteLine("\x1b[0m");
            }
            else
            {
                Console.WriteLine(text + text2);
            }
        }

        /// <summary>
        /// Reads user input from console.
        /// Uses INPUT_COLOUR colour code to print user's characters. </summary>
        /// <seealso cref="Console.ReadLine()"></seealso>
        public string ReadLine()
        {
            string output = "";
            if (USE_COLOURS)
            {
                Console.Write("\x1b[" + INPUT_COLOUR + "m");
                output = Console.ReadLine();
                Console.Write("\x1b[0m");
            }
            else
            {
                output = Console.ReadLine();
            }
            return output;
        }

        /// <summary>
        /// Reads user input from console but masks typed characters. </summary>
        /// <seealso cref="Console.ReadLine()"></seealso>
        public string ReadPassword()
        {
            string pass = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                }
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine("");
                }
            }
            while (key.Key != ConsoleKey.Enter);
            return pass;
        }
    }

    /// <summary>
    /// Colour codes 
    /// </summary>
    public struct WriterColours
    {
        public const int BOLD       = 1;
        public const int UNDERLINE  = 4;
        public const int INVERSE    = 7;
        public const int BLACK      = 30;
        public const int RED        = 31;
        public const int GREEN      = 32;
        public const int YELLOW     = 33;
        public const int BLUE       = 34;
        public const int MAGENTA    = 35;
        public const int CYAN       = 36;
        public const int WHITE      = 37;
        public const int HIGHLIGHT_BLACK    = 40;
        public const int HIGHLIGHT_RED      = 41;
        public const int HIGHLIGHT_GREEN    = 42;
        public const int HIGHLIGHT_YELLOW   = 43;
        public const int HIGHLIGHT_BLUE     = 44;
        public const int HIGHLIGHT_MAGENTA  = 45;
        public const int HIGHLIGHT_CYAN     = 46;
        public const int HIGHLIGHT_WHITE    = 47;
        public const int STRONG_BLACK   = 90;
        public const int STRONG_RED     = 91;
        public const int STRONG_GREEN   = 92;
        public const int STRONG_YELLOW  = 93;
        public const int STRONG_BLUE    = 94;
        public const int STRONG_MAGENTA = 95;
        public const int STRONG_CYAN    = 96;
        public const int STRONG_WHITE   = 97;

        public const int HIGHLIGHT_ERROR    = 101;
        public const int STANDARD = 37;
        public const int ERROR = 91;
        public const int SUCCESS = 92;
        public const int WARNING = 93;
        public const int COMPONENT = 95;
        public const int INFO = 96;
    }

}
