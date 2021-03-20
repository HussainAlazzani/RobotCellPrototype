using System;
using System.Collections.Generic;
using System.Threading;

namespace FlexConnectors
{
    internal interface IMoulder
    {
        int Cavities { get; }
        void Add(Strip[] strips);
        void Mould();
    }

    /// <summary>
    /// This moulder is limited to 2 and 4 cavity tools by design.
    /// </summary>
    internal class PolypropyleneMoulder : IMoulder
    {
        private List<Strip> _stripsInTool;
        
        internal PolypropyleneMoulder(int cavities)
        {
            if (cavities != 2 && cavities != 4)
            {
                throw new Exception("Can only select tools with 2 and 4 cavities.");
            }

            Cavities = cavities;
            _stripsInTool = new List<Strip>(cavities);
        }

        public int Cavities { get; }
        public int Counter { get; set; }

        /// <summary>
        /// Add parts to mould tool. When all cavaties are occuppied by strips, mould the strips.
        /// </summary>
        public void Add(Strip[] strips)
        {
            for (int i = 0; i < strips.Length; i++)
            {
                _stripsInTool.Add(strips[i]);
            }
            if (_stripsInTool.Count == _stripsInTool.Capacity)
            {
                Mould();
            }
        }

        public void Mould()
        {
            foreach (var strip in _stripsInTool)
            {
                Thread.Sleep(1000);
                strip.IsMoulded = true;
            }
            System.Console.WriteLine("Finished moulding");

            // Remove strips from mould tool
            _stripsInTool.Clear();

            this.Counter += this.Cavities;
            System.Console.WriteLine("Number of parts moulded: " + this.Counter);
            System.Console.WriteLine();
        }
    }
}