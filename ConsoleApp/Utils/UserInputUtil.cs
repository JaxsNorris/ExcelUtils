
using System;

namespace ConsoleApp.Utils
{
    public static class UserInputUtil
    {
        public static bool GetUserBoolInput(string question)
        {
            Console.WriteLine(question);
            Console.WriteLine("Press n for no. any other key will be a consider yes");
            var userInput = Console.ReadKey();
            Console.WriteLine();
            if (userInput.KeyChar == 'n' || userInput.KeyChar == 'N')
                return false;

            return true;
        }
    }
}
