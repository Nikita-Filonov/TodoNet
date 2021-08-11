﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using WebApi.Models;


namespace WebApi.Utils
{
    public static class DbSetup
    {
        public static bool Debug = true;
        public static string GetHerokuConnectionString()
        {
            string connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            var databaseUri = new Uri(connectionUrl);

            string db = databaseUri.LocalPath.TrimStart('/');
            string[] userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);

            return $"User ID={userInfo[0]};Password={userInfo[1]};Host={databaseUri.Host};Port={databaseUri.Port};Database={db};Pooling=true;SSL Mode=Require;Trust Server Certificate=True;";
        }

        public static string DatabaseConnectionString =>
           Debug
            ? "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=Coool12345"
            : GetHerokuConnectionString();
    }
}
