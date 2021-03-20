using System;
using System.Threading.Tasks;

namespace FlexConnectors
{
    internal interface IRobotCellAsync
    {
        void Add(Strip[] strips);
        Task TriggerAsync();
        Task Index();
    }

    internal class RobotCellAsync : IRobotCellAsync
    {
        private Strip[,] _nests;

        private readonly IMoulder _moulder;

        public RobotCellAsync(int cavities, IMoulder moulder)
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

        public async Task TriggerAsync()
        {
            for (int i = _nests.GetLength(0) - 1; i > 0; i--)
            {
                for (int j = 0; j < _nests.GetLength(1); j++)
                {
                    _nests[i, j] = _nests[i - 1, j];
                }
            }

            Task crop = CropAsync();
            Task placeSpade = PlaceSpadeAsync();
            Task weld = WeldAsync();
            Task pickStrips = PickStripsAsync();

            Task[] tasks = {crop, placeSpade, weld, pickStrips};

            await Task.WhenAll(tasks);
        }

        public async Task Index()
        {
            for (int i = 0; i < _nests.GetLength(1); i++)
            {
                _nests[0, i] = null;
            }

            await TriggerAsync();
        }

        private async Task CropAsync()
        {
            if (_nests[1, 0] != null)
            {
                for (int j = 0; j < _nests.GetLength(1); j++)
                {
                    await Task.Delay(1000);
                    _nests[1, j].IsCropped = true;
                }

                System.Console.WriteLine("Cropped strips");
            }
        }

        private async Task PlaceSpadeAsync()
        {
            if (_nests[2, 0] != null)
            {
                await Task.Delay(1500);
                System.Console.WriteLine("Placed spades on strips");
            }
        }

        private async Task WeldAsync()
        {
            if (_nests[3, 0] != null)
            {
                for (int j = 0; j < _nests.GetLength(1); j++)
                {
                    await Task.Delay(2000);
                    _nests[3, j].IsWelded = true;
                }

                System.Console.WriteLine("Welded strips");
            }
        }

        private async Task PickStripsAsync()
        {
            if (_nests[4, 0] != null)
            {
                Strip[] strips = new Strip[Cavities];
                for (int i = 0; i < strips.Length; i++)
                {
                    strips[i] = _nests[4, i];
                }

                System.Console.WriteLine("Placing strips in the moulder");
                await Task.Delay(1500);
                _moulder.Add(strips);
            }
        }
    }
}