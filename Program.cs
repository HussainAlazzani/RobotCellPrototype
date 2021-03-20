using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexConnectors
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IMoulder moulder = new PolypropyleneMoulder(4);
            RobotCellAsync robotCell = new RobotCellAsync(2, moulder);

            Stack<Strip> boxOfStrips = new Stack<Strip>();

            int numberOfStrips = 3000;
            for (int i = 0; i < numberOfStrips; i++)
            {
                boxOfStrips.Push(new Strip { Name = "CX4005RH" });
            }

            Strip[] strips = new Strip[2];

            while (boxOfStrips.Count > 1)
            {
                strips[0] = boxOfStrips.Pop();
                strips[1] = boxOfStrips.Pop();

                robotCell.Add(strips);

                await robotCell.TriggerAsync();
            }

            // Clear remaining strips from table cell
            for (int i = 0; i < 4; i++)
            {
                await robotCell.Index();
            }
        }
    }
}
