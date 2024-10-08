﻿using EdnaApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace EdnaApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SoeController(IConfiguration configuration) : ControllerBase
{
    private readonly string? _dbConnStr = configuration.GetValue<string>("DbConnStr");
    private readonly string? _soeSql = configuration.GetValue<string>("SoeSql");

    // GET api/soe?strtime=30/11/2016/00:00:00&endtime=30/11/2016/23:59:00
    [HttpGet(Name = "GetSoeData")]
    public List<SoeResult> GetData(string? strtime = "16/08/2024/00:00:00", string? endtime = "16/08/2024/23:59:00")
    {
        List<SoeResult> results = [];
        string format = "dd/MM/yyyy/HH:mm:ss";
        DateTime startDt = DateTime.ParseExact(strtime, format, CultureInfo.InvariantCulture);
        DateTime endDt = DateTime.ParseExact(endtime, format, CultureInfo.InvariantCulture);
        //string sqlQuery = "SELECT area, CATEGORY, LOCATION, TEXT, time_soe FROM eta_user.SOE22 where time_soe between @st and @et order by time_soe desc";
        using (var sqlConnection1 = new SqlConnection(_dbConnStr))
        {
            using var cmd = new SqlCommand(_soeSql, sqlConnection1);
            cmd.Parameters.Add("@st", SqlDbType.DateTime).Value = startDt;
            cmd.Parameters.Add("@et", SqlDbType.DateTime).Value = endDt;
            sqlConnection1.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string area = (string)reader[0];
                string catgry = (string)reader[1];
                string loctn = (string)reader[2];
                string soeTxt = (string)reader[3];
                DateTime soeTimestamp = (DateTime)reader[4];
                results.Add(new SoeResult { Area = area, Category = catgry, Location = loctn, Text = soeTxt, SoeTime = soeTimestamp });
            }
        }
        return results;
    }
}
