using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsJammerLocator
{
    public class ProgressBar
    {
        private readonly int _barLength;
        private int _lastPercentage = -1;
        private int _lastDots = -1;
        private readonly int _total;
        private bool complete = false;

        public ProgressBar(int total, int barLength = 50)
        {
            _total = total;
            _barLength = barLength;
        }

        private readonly object _updateLock = new object();

        public void Update(int currentCount)
        {
            lock (_updateLock) // Ensure only one thread can enter this block at any time
            {
                if (complete) return;

                double percentage = currentCount * 100.0 / _total;
                int currentDots = (int)(percentage / 100 * _barLength);

                if (currentDots != _lastDots || (int)percentage != _lastPercentage)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("[");
                    Console.Write(new string('.', currentDots) + new string(' ', _barLength - currentDots));
                    Console.Write($"] {(int)percentage}% ");
                    _lastDots = currentDots;
                    _lastPercentage = (int)percentage;
                    if (percentage == 100) Complete();
                }
            }
        }

        private void Complete()
        {
            Console.WriteLine();
            complete = true;
        }
    }
}
