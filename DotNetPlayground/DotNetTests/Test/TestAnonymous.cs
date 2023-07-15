using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TimerTimer = System.Timers.Timer;
using ThreadingTimer = System.Threading.Timer;

namespace DotNetPlayground
{
    internal class TestAnonymous
    {
        int _value = 0;
        
        private ElapsedEventHandler _elapsedHandler;
        private TimerTimer timer;
        private ThreadingTimer anotherTimer;

        public void StartTimerTimer()
        {
            timer = new TimerTimer(1000);
            _elapsedHandler = (sender, args) =>
            {
                _value++; // 捕获了成员变量 _value
                Console.WriteLine(_value);
            };
            timer.Elapsed += _elapsedHandler;
            timer.Start();
        }

        public void StopTimerTimer()
        {
            if(timer != null)
                timer.Elapsed -= _elapsedHandler;
            _elapsedHandler = null;
        }

        public void StartThreadingTimer()
        {
            ThreadingTimer t = new ThreadingTimer(o => 
            {
                Console.WriteLine("ThreadingTimer done");
            }, null, 0, 500);
        }
    }
}
