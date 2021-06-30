using System;
using System.Collections.Generic;

namespace RateLimiter
{
    public class RateLimiter
    {
        private DateTime _periodStartTimeStamp;
        private int _intervalInSeconds;
        private int _requestLimit;
        private Dictionary<string,int> _requests;

        public RateLimiter(int intervalInSeconds, int requestLimit)
        {
            _periodStartTimeStamp = DateTime.Now;
            _intervalInSeconds = intervalInSeconds;
            _requestLimit = requestLimit;
            _requests = new Dictionary<string, int>();
        }

        private void resetLimiter()
        {
            _periodStartTimeStamp = DateTime.Now;
            _requests.Clear();
        }

        public bool acceptRequest(string id)
        {
            bool isResetDone = false;

            // Verify if current time slot has elapsed
            // If slot has elapsed -> perform reset
            var currentTimeStamp = DateTime.Now;
            var timeSlot = currentTimeStamp - _periodStartTimeStamp;
            if (timeSlot.Seconds > _intervalInSeconds)
            {
                resetLimiter();
                isResetDone = true;
            }

            if (isResetDone)
            {
                // When reset was performed, we don't have to check limits
                // We store request details and accept it
                _requests.Add(id, 1);
                return true;
            }
            else
            {
                // Address already existing in requests records
                if (_requests.ContainsKey(id))
                {
                    _requests[id]++;
                    if (_requests[id] <= _requestLimit)
                    {
                        // Limit was not exceeded -> request is accepted
                        return true;
                    }
                }
                // Address not existing in requests records
                else
                {
                    _requests.Add(id, 1);
                    return true;
                }
            }

            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //string clients = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string clients = "ABC";
            int clientIndex;
            Random random = new Random();
            RateLimiter limiter = new RateLimiter(5, 2);
            for (int i = 0; i < 1000; i++)
            {
                clientIndex = random.Next(0, 3);
                if (limiter.acceptRequest(clients[clientIndex].ToString()))
                {
                    Console.WriteLine("Client {0} request: ACCEPTED", clients[clientIndex]);
                }
                else
                {
                    Console.WriteLine("Client {0} request: REJECTED", clients[clientIndex]);
                }
                System.Threading.Thread.Sleep(250);
            }
        }
    }
}
