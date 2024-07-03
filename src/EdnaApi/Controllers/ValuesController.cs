using EdnaApi.DTOs;
using InStep.eDNA.EzDNAApiNet;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Globalization;

namespace EdnaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController(ILogger<ValuesController> logger, IConfiguration configuration) : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger = logger;
        private readonly bool _useRandom = configuration.GetValue<bool>("UseRandom");

        // GET api/values
        [HttpGet(Name = "ValuesGetTest")]
        public object Get()
        {
            return new RealResult { Dval = 10, Timestamp = DateTime.Now, Status = "asfsa", Units = "hrht" };
        }

        // GET api/values/history?type=snap&pnt=something&strtime=30/11/2016/00:00:00&endtime=30/11/2016/23:59:00&secs=60
        // GET api/values/real?pnt=something
        [HttpGet("{id}", Name = "GetData")]
        public object? GetData(string id, string? pnt = "", string? strtime = "30/11/2023/00:00:00", string? endtime = "30/11/2023/23:59:00", int secs = 60, string? type = "snap", string? service = "")
        {
            int nret = 0;
            string format = "dd/MM/yyyy/HH:mm:ss";
            if (id == "history")
            {
                DateTime startDt = DateTime.ParseExact(strtime, format, CultureInfo.InvariantCulture);
                DateTime endDt = DateTime.ParseExact(endtime, format, CultureInfo.InvariantCulture);
                if (_useRandom)
                {
                    return FetchRandomHistData(startDt, endDt, secs);
                }
                //get history values
                ArrayList historyResults = [];
                try
                {
                    uint s = 0;
                    double dval = 0;
                    DateTime timestamp = DateTime.Now;
                    string status = "";
                    TimeSpan period = TimeSpan.FromSeconds(secs);
                    //history request initiation
                    if (type == "raw")
                    { nret = History.DnaGetHistRaw(pnt, startDt, endDt, out s); }
                    else if (type == "snap")
                    { nret = History.DnaGetHistSnap(pnt, startDt, endDt, period, out s); }
                    else if (type == "average")
                    { nret = History.DnaGetHistAvg(pnt, startDt, endDt, period, out s); }
                    else if (type == "min")
                    { nret = History.DnaGetHistMin(pnt, startDt, endDt, period, out s); }
                    else if (type == "max")
                    { nret = History.DnaGetHistMax(pnt, startDt, endDt, period, out s); }

                    while (nret == 0)
                    {
                        nret = History.DnaGetNextHist(s, out dval, out timestamp, out status);
                        if (!string.IsNullOrWhiteSpace(status))
                        {
                            historyResults.Add(new HistResult { Dval = dval, Timestamp = timestamp, Status = status });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while fetching history results " + ex.Message);
                    historyResults = [];
                }
                return historyResults;
            }
            else if (id == "real")
            {
                if (_useRandom)
                {
                    return new RealResult
                    {
                        Dval = new Random().Next(50, 100),
                        Timestamp = DateTime.Now,
                        Status = "GOOD",
                        Units = "MW"
                    };
                }

                RealResult realVal;
                try
                {
                    nret = RealTime.DNAGetRTAll(pnt, out double dval, out DateTime timestamp, out string status, out string desc, out string units);//get RT value
                    if (nret == 0)
                    {
                        realVal = new RealResult { Dval = dval, Timestamp = timestamp, Status = status, Units = units };
                        return realVal;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while fetching realtime result " + ex.Message);
                    return null;
                }
                return null;
            }
            else if (id == "longtoshort")
            {
                string shortId;
                try
                {
                    Configuration.ShortIdFromLongId(service, pnt, out shortId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while fetching longtoshort result " + ex.Message);
                    shortId = "";
                }
                return new { shortId };
            }
            else
            {
                return null;
            }
        }

        private static List<HistResult> FetchRandomHistData(DateTime startTime, DateTime endTime, int samplingPeriod)
        {
            List<HistResult> reslt = [];
            DateTime curTime = startTime;
            int resFreq = (samplingPeriod > 0) ? samplingPeriod : 60;
            DateTime targetEndTime = endTime;
            while (curTime <= targetEndTime)
            {
                reslt.Add(new HistResult { Dval = new Random().Next(50, 100), Status = "GOOD", Timestamp = curTime });
                curTime += TimeSpan.FromSeconds(resFreq);
            }
            return reslt;
        }
    }
}
