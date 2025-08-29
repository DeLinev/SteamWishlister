namespace SteamWishlister;

static class ConsoleSpinner
{
    static private readonly char[] LoaderChars = ['/', '-', '\\', '|'];
    static private readonly int AnimSpeed = 100;

    static public void Spin(int milliseconds)
    {
        int position = 0;
        int timeLeft = milliseconds;

        do
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(LoaderChars[position++]);
            position = position % LoaderChars.Length;
            Thread.Sleep(AnimSpeed);
            timeLeft -= AnimSpeed;
        } while (timeLeft > 0);

        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop);
    }
}