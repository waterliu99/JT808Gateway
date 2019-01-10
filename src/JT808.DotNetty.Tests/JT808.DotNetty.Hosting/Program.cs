﻿using JT808.DotNetty.Core;
using JT808.DotNetty.Core.Handlers;
using JT808.DotNetty.Hosting.Handlers;
using JT808.DotNetty.Tcp;
using JT808.DotNetty.Udp;
using JT808.DotNetty.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace JT808.DotNetty.Hosting
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //7E020000220138123456780085000000010000000101EA2A3F08717931000C015400201901032000020104000000E6F87E
            var serverHostBuilder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConsole();  
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<ILoggerFactory, LoggerFactory>();
                    services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));             
                    services.AddJT808Core(hostContext.Configuration)
                            .AddJT808TcpHost()
                            .AddJT808UdpHost()
                            .AddJT808WebApiHost();
                    // 自定义Tcp消息处理业务
                    services.Replace(new ServiceDescriptor(typeof(JT808MsgIdTcpHandlerBase), typeof(JT808MsgIdTcpCustomHandler), ServiceLifetime.Singleton));
                    // 自定义Udp消息处理业务
                    services.Replace(new ServiceDescriptor(typeof(JT808MsgIdUdpHandlerBase), typeof(JT808MsgIdUdpCustomHandler), ServiceLifetime.Singleton));
                });

            await serverHostBuilder.RunConsoleAsync();
        }
    }
}