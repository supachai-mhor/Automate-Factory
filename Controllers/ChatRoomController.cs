using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutomateBussiness.Models;
using Newtonsoft.Json;

namespace AutomateBussiness.Controllers
{

    public class ChatRoomController : Controller
    {
        //[Authorize]
        [Route("chat")]
        public IActionResult Index()
        {
            Random random = new Random();
            List<DataPoint> dataPoints1 = new List<DataPoint>();
            List<DataPoint> dataPoints2 = new List<DataPoint>();

            int updateInterval = 500;

            // initial value
            double yValue1 = 0;
            double yValue2 = 0;
            double time;

            DateTime dateNow = DateTime.Now;
            DateTime date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 9, 30, 00);
            time = ((DateTimeOffset)date).ToUnixTimeSeconds() * 1000;
            //addData(1);

            ViewBag.DataPoints1 = JsonConvert.SerializeObject(dataPoints1);
            ViewBag.DataPoints2 = JsonConvert.SerializeObject(dataPoints2);
            ViewBag.YValue1 = yValue1;
            ViewBag.YValue2 = yValue2;
            ViewBag.Time = time;
            ViewBag.UpdateInterval = updateInterval;

            return View();

            void addData(int count)
            {
                double deltaY1, deltaY2;
                for (int i = 0; i < count; i++)
                {
                    time += updateInterval;
                    deltaY1 = .5 + random.NextDouble() * (-.5 - .5);
                    deltaY2 = .5 + random.NextDouble() * (-.5 - .5);

                    // adding random value and rounding it to two digits.
                    yValue1 = Math.Round((yValue1 + deltaY1) * 100) / 100;
                    yValue2 = Math.Round((yValue2 + deltaY2) * 100) / 100;

                    // pushing the new values
                    dataPoints1.Add(new DataPoint(time, yValue1));
                    dataPoints2.Add(new DataPoint(time, yValue2));
                }
            }
        }

        [Authorize]
        public IActionResult Streamming()
        {
            return View();
        }
    }
}