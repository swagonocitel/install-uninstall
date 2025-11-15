using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading; // ДОБАВЬТЕ ЭТУ ДИРЕКТИВУ

namespace project_1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Оптимизация многопоточности - ДОБАВЬТЕ ЭТО В НАЧАЛО
            ConfigureThreadPool();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Обработка аргументов командной строки
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.Equals("/install", StringComparison.OrdinalIgnoreCase))
                    {
                        // Автоматическая установка с показом окон
                        RunSilentInstallation(false);
                        return;
                    }
                    else if (arg.Equals("/silent", StringComparison.OrdinalIgnoreCase))
                    {
                        // Полностью автоматическая установка без окон
                        RunSilentInstallation(true);
                        return;
                    }
                    else if (arg.Equals("/uninstall", StringComparison.OrdinalIgnoreCase))
                    {
                        // Запуск деинсталляции
                        Application.Run(new Form5());
                        return;
                    }
                }
            }

            // Обычный запуск с GUI
            Application.Run(new Form1());
        }

        // ДОБАВЬТЕ ЭТОТ МЕТОД ДЛЯ НАСТРОЙКИ МНОГОПОТОЧНОСТИ
        private static void ConfigureThreadPool()
        {
            try
            {
                // Увеличиваем минимальное количество рабочих потоков
                int workerThreads, completionPortThreads;
                ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);

                // Устанавливаем минимальное количество потоков = количество ядер * 2
                int minThreads = Environment.ProcessorCount * 2;
                ThreadPool.SetMinThreads(minThreads, minThreads);

                Debug.WriteLine($"ThreadPool настроен: MinThreads = {minThreads}, ProcessorCount = {Environment.ProcessorCount}");

                // Настраиваем политику потоков для лучшей производительности
                TaskScheduler.UnobservedTaskException += (sender, e) =>
                {
                    // Логируем необработанные исключения задач
                    Debug.WriteLine($"Unobserved task exception: {e.Exception}");
                    e.SetObserved();
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка настройки ThreadPool: {ex.Message}");
            }
        }

        /// <summary>
        /// Запуск автоматической установки
        /// </summary>
        /// <param name="fullySilent">true - полностью без окон, false - с показом прогресса</param>
        static void RunSilentInstallation(bool fullySilent)
        {
            if (fullySilent)
            {
                // Полностью автоматический режим - выполняем установку в фоне
                PerformSilentInstallation();
            }
            else
            {
                // Режим с показом окна прогресса
                Application.Run(new Form4());
            }
        }

        /// <summary>
        /// Выполнение полностью автоматической установки
        /// </summary>
        static void PerformSilentInstallation()
        {
            try
            {
                // Устанавливаем флаги для создания ярлыков (в автоматическом режиме создаем оба)
                Form3.CreateDesktopShortcut = true;
                Form3.CreateStartMenuShortcut = true;

                // Создаем и запускаем установку
                SilentInstaller installer = new SilentInstaller();
                installer.Install();

                // Завершаем приложение после установки
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                // В случае ошибки завершаем с кодом ошибки
                Environment.Exit(1);
            }
        }
    }

    // Класс SilentInstaller определен внутри того же namespace
    internal class SilentInstaller
    {
        public void Install()
        {
            // Имитация процесса установки с задержками
            System.Threading.Thread.Sleep(2000);

            // Создание ярлыков
            CreateShortcuts();

            // Запись в реестр
            CreateRegistryEntries();

            System.Threading.Thread.Sleep(1000);
        }

        // ЗАМЕНИТЕ метод CreateShortcuts на этот многопоточный вариант
        private void CreateShortcuts()
        {
            try
            {
                string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string appName = "installer";

                var shortcutTasks = new List<Task>();

                // Создаем ярлык на рабочем столе в отдельном потоке
                if (Form3.CreateDesktopShortcut)
                {
                    shortcutTasks.Add(Task.Run(() =>
                    {
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        string shortcutPath = Path.Combine(desktopPath, appName + ".lnk");
                        CreateShortcutUsingVBS(appPath, shortcutPath, appName);
                    }));
                }

                // Создаем ярлык в меню Пуск в отдельном потоке
                if (Form3.CreateStartMenuShortcut)
                {
                    shortcutTasks.Add(Task.Run(() =>
                    {
                        string startMenuPath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                            "Programs");

                        if (!Directory.Exists(startMenuPath))
                            Directory.CreateDirectory(startMenuPath);

                        string shortcutPath = Path.Combine(startMenuPath, appName + ".lnk");
                        CreateShortcutUsingVBS(appPath, shortcutPath, appName);
                    }));
                }

                // Ожидаем завершения всех задач создания ярлыков
                if (shortcutTasks.Count > 0)
                {
                    Task.WaitAll(shortcutTasks.ToArray(), TimeSpan.FromSeconds(10));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при создании ярлыков: {ex.Message}");
            }
        }

        // ЗАМЕНИТЕ метод CreateRegistryEntries на этот многопоточный вариант
        private void CreateRegistryEntries()
        {
            try
            {
                string appName = "installer";
                string publisherName = "synergy programm";
                string installPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string version = "1.0.0";
                string uninstallString = $"\"{System.Reflection.Assembly.GetExecutingAssembly().Location}\" /uninstall";

                // Параллельное выполнение операций с реестром
                var registryTasks = new[]
                {
                    Task.Run(() => CreatePublisherRegistryKey(appName, publisherName, installPath, version)),
                    Task.Run(() => CreateUninstallRegistryKey(appName, publisherName, installPath, version, uninstallString))
                };

                // Ожидаем завершения всех задач с реестром
                Task.WaitAll(registryTasks, TimeSpan.FromSeconds(5));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при работе с реестром: {ex.Message}");
            }
        }

        // ДОБАВЬТЕ ЭТИ ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        private void CreatePublisherRegistryKey(string appName, string publisherName, string installPath, string version)
        {
            try
            {
                using (RegistryKey publisherKey = Registry.LocalMachine.CreateSubKey($@"Software\{publisherName}\{appName}"))
                {
                    if (publisherKey != null)
                    {
                        publisherKey.SetValue("InstallPath", installPath, RegistryValueKind.String);
                        publisherKey.SetValue("Version", version, RegistryValueKind.String);
                        publisherKey.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"), RegistryValueKind.String);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка создания ключа издателя: {ex.Message}");
            }
        }

        private void CreateUninstallRegistryKey(string appName, string publisherName, string installPath, string version, string uninstallString)
        {
            try
            {
                using (RegistryKey uninstallKey = Registry.LocalMachine.CreateSubKey($@"Software\Microsoft\Windows\CurrentVersion\Uninstall\{appName}"))
                {
                    if (uninstallKey != null)
                    {
                        uninstallKey.SetValue("DisplayName", appName, RegistryValueKind.String);
                        uninstallKey.SetValue("DisplayVersion", version, RegistryValueKind.String);
                        uninstallKey.SetValue("Publisher", publisherName, RegistryValueKind.String);
                        uninstallKey.SetValue("UninstallString", uninstallString, RegistryValueKind.String);
                        uninstallKey.SetValue("InstallLocation", installPath, RegistryValueKind.String);
                        uninstallKey.SetValue("DisplayIcon", System.Reflection.Assembly.GetExecutingAssembly().Location, RegistryValueKind.String);
                        uninstallKey.SetValue("NoModify", 1, RegistryValueKind.DWord);
                        uninstallKey.SetValue("NoRepair", 1, RegistryValueKind.DWord);
                        uninstallKey.SetValue("EstimatedSize", GetApplicationSize(), RegistryValueKind.DWord);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка создания ключа удаления: {ex.Message}");
            }
        }

        // ОБНОВИТЕ метод GetApplicationSize для многопоточности
        private int GetApplicationSize()
        {
            try
            {
                string appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (Directory.Exists(appDirectory))
                {
                    long totalSize = 0;
                    var files = Directory.GetFiles(appDirectory, "*.*", SearchOption.AllDirectories);

                    // Параллельный расчет размера файлов
                    Parallel.ForEach(files, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Environment.ProcessorCount
                    }, file =>
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            Interlocked.Add(ref totalSize, fileInfo.Length);
                        }
                        catch
                        {
                            // Игнорируем ошибки доступа к файлам
                        }
                    });

                    return (int)(totalSize / 1024);
                }
            }
            catch
            {
                // В случае ошибки возвращаем приблизительный размер
            }
            return 1024;
        }

        private void CreateShortcutUsingVBS(string targetPath, string shortcutPath, string shortcutName)
        {
            try
            {
                string vbsScript = $@"
Set WshShell = WScript.CreateObject(""WScript.Shell"")
Set shortcut = WshShell.CreateShortcut(""{shortcutPath}"")
shortcut.TargetPath = ""{targetPath}""
shortcut.WorkingDirectory = ""{Path.GetDirectoryName(targetPath)}""
shortcut.Description = ""Ярлык для {shortcutName}""
shortcut.Save
";

                string tempVbsFile = Path.GetTempFileName() + ".vbs";
                File.WriteAllText(tempVbsFile, vbsScript, System.Text.Encoding.Default);

                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = "wscript.exe",
                    Arguments = $"\"{tempVbsFile}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit(3000);
                }

                try { File.Delete(tempVbsFile); } catch { }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка создания ярлыка {shortcutPath}: {ex.Message}");
            }
        }
    }
}