using Ats.Service;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ats.Program
{
    class Program
    {
        static readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                Task cancelTask = Task.Run(() =>
                {
                    var totalSeconds = 600;

                    while (stopwatch.Elapsed.TotalSeconds < totalSeconds)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));

                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            return Task.CompletedTask;
                        }

                        LogHelper.WriteLine($"Programın sonlanmasına {totalSeconds - stopwatch.Elapsed.TotalSeconds:F0} saniye kaldı!", ConsoleColor.Cyan);
                    }

                    cancellationTokenSource.Cancel();

                    return Task.CompletedTask;
                }, cancellationTokenSource.Token);

                Task mainTask = Task.Run(() =>
                {
                    using (var announcementService = new AtsService())
                    {
                        announcementService.GetNewDefinitionsAndSendMail();
                    }

                    cancellationTokenSource.Cancel();

                    return Task.CompletedTask;
                }, cancellationTokenSource.Token);

                var taskList = new List<Task> {
                cancelTask,
                mainTask
                };

                await Task.WhenAny(taskList);

                LogHelper.WriteLine($"Main Task IsCompleted: {mainTask.IsCompleted}", ConsoleColor.DarkMagenta);
                LogHelper.WriteLine($"Cancel Task IsCompleted: {cancelTask.IsCompleted}", ConsoleColor.DarkMagenta);

                LogHelper.WriteLine("İşlem bitti, kapanıyor....", ConsoleColor.Cyan);

                Process.GetProcesses().Where(pr => pr.ProcessName.Contains("phantom")).ToList().ForEach(x => x.Kill());
            }
            catch (Exception exception)
            {
                LogHelper.WriteLine(exception, null);
                LogHelper.WriteLine("Bir hata oluştu, kapanıyor....", ConsoleColor.Red);
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}