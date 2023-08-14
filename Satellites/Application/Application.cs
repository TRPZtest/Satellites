using Satellites.Application.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satellites.Application
{
    public class Application
    {    
        public List<JobBase> Jobs { get; set; }

        public Application() 
        { 
            Jobs = new List<JobBase>();
        }

        public async Task Run()
        {
            for (int i = 0; i < Jobs.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {Jobs[i].Name}");
            }

            int cursorTop = Console.CursorTop + 1;
            int userInput;

            // Get the user input
            do
            {           
                Console.SetCursorPosition(0, cursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, cursorTop);

                Console.Write($"Enter a choice (1 - {Jobs.Count}): ");
            } while (!int.TryParse(Console.ReadLine(), out userInput) ||
                     userInput < 1 || userInput > Jobs.Count);

            // Execute the menu item function
            await Jobs[userInput - 1].ExecuteJobAsync();
        }
    }
}
