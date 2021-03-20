using System;
using System.Threading;

namespace FlexConnectors
{
    internal interface IRobotCell
    {
        void Add(Strip[] strips);
        void Trigger();
        void Index();
    }

    internal class RobotCell : IRobotCell
    {
        private Strip[,] _nests;

        private readonly IMoulder _moulder;

        public RobotCell(int cavities, IMoulder moulder)
        {
            _moulder = moulder;
            Cavities = cavities;
            _nests = new Strip[6, cavities];
        }

        internal int Cavities { get; }

        public void Add(Strip[] strips)
        {
            if (strips.Length > Cavities)
            {
                throw new Exception("Too many strips");
            }

            for (int i = 0; i < strips.Length; i++)
            {
                _nests[0, i] = strips[i];
            }
        }

        public void Trigger()
        {
            for (int i = _nests.GetLength(0) - 1; i > 0; i--)
            {
                for (int j = 0; j < _nests.GetLength(1); j++)
                {
                    _nests[i, j] = _nests[i - 1, j];
                }
            }

            Crop();
            PlaceSpade();
            Weld();
            RobotPick();
        }

        public void Index()
        {
            for (int i = 0; i < _nests.GetLength(1); i++)
            {
                _nests[0, i] = null;
            }

            Trigger();
        }

        private void Crop()
        {
            if (_nests[1, 0] != null)
            {
                for (int j = 0; j < _nests.GetLength(1); j++)
                {
                    Thread.Sleep(1000);
                    _nests[1, j].IsCropped = true;
                }

                System.Console.WriteLine("Cropped strips");
            }
        }

        private void PlaceSpade()
        {
            if (_nests[2, 0] != null)
            {
                Thread.Sleep(1500);
                System.Console.WriteLine("Placed spades on strips");
            }
        }

        private void Weld()
        {
            if (_nests[3, 0] != null)
            {
                for (int j = 0; j < _nests.GetLength(1); j++)
                {
                    Thread.Sleep(2000);
                    _nests[3, j].IsWelded = true;
                }

                System.Console.WriteLine("Welded strips");
            }
        }

        private void RobotPick()
        {

            if (_nests[4, 0] != null)
            {
                Strip[] strips = new Strip[Cavities];
                for (int i = 0; i < strips.Length; i++)
                {
                    strips[i] = _nests[4, i];
                }

                System.Console.WriteLine("Placing strips in the moulder");
                Thread.Sleep(1500);
                _moulder.Add(strips);
            }
        }
    }
}