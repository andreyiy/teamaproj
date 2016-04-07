using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BusinessLayer.Timer
{
    public class Check
    {
        private HomeworkService hwService = new HomeworkService();
        public void Tim()
        {
            while (true)
            {
                hwService.CheckHomeworkDeadLine();
                Thread.Sleep(120000);
            }

        }
    }
}
