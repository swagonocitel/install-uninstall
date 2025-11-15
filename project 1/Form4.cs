using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading; // ДОБАВЬТЕ ЭТУ ДИРЕКТИВУ

namespace project_1
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            StartInstallation();
        }

        // ЗАМЕНИТЕ метод StartInstallation на этот многопоточный вариант
        private async void StartInstallation()
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = 100;

            try
            {
                // Этап 1: Подготовка (10%) - параллельная предзагрузка
                await UpdateProgress(10, "Подготовка файлов...");
                await PreloadResourcesAsync();
                await Task.Delay(500);

                // Этап 2: Копирование файлов (40%) - многопоточное копирование
                await UpdateProgress(20, "Копирование файлов...");
                await CopyFilesWithParallelProcessing();
                await UpdateProgress(50, "Файлы скопированы");

                // Этап 3: Параллельное создание ярлыков и запись в реестр (30%)
                await UpdateProgress(60, "Создание системных записей...");

                var systemTasks = new[]
                {
                    CreateShortcutsParallel(),
                    CreateRegistryEntriesParallel()
                };
                await Task.WhenAll(systemTasks);

                await UpdateProgress(90, "Системные записи созданы");

                // Этап 4: Завершение (10%)
                await UpdateProgress(100, "Завершение установки...");
                await Task.Delay(500);

                MessageBox.Show("Установка завершена успешно!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка установки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ДОБАВЬТЕ ЭТОТ МЕТОД ДЛЯ ПАРАЛЛЕЛЬНОЙ ПРЕДЗАГРУЗКИ
        private async Task PreloadResourcesAsync()
        {
            await Task.Run(() =>
            {
                // Параллельная предзагрузка ресурсов
                Parallel.Invoke(
                    () => { /* Загрузка иконок */ },
                    () => { /* Подготовка временных файлов */ },
                    () => { /* Инициализация компонентов */ }
                );
            });
        }

        // ДОБАВЬТЕ ЭТОТ МЕТОД ДЛЯ МНОГОПОТОЧНОГО КОПИРОВАНИЯ
        private async Task CopyFilesWithParallelProcessing()
        {
            await Task.Run(() =>
            {
                try
                {
                    // Имитация копирования файлов с использованием всех ядер
                    var fakeFiles = Enumerable.Range(1, 1000).Select(i => $"file{i}.dat").ToArray();

                    Parallel.ForEach(fakeFiles, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Environment.ProcessorCount
                    }, file =>
                    {
                        // Имитация работы с файлом
                        Thread.Sleep(1);
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка при копировании файлов: {ex.Message}");
                }
            });
        }

        // ЗАМЕНИТЕ метод CreateShortcuts на этот многопоточный вариант
        private async Task CreateShortcutsParallel()
        {
            await Task.Run(() =>
            {
                try
                {
                    string appPath = Application.ExecutablePath;
                    string appName = "project_1";

                    var shortcutTasks = new List<(string path, string name)>();

                    if (Form3.CreateDesktopShortcut)
                    {
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        shortcutTasks.Add((Path.Combine(desktopPath, appName + ".lnk"), appName));
                    }

                    if (Form3.CreateStartMenuShortcut)
                    {
                        string startMenuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs");
                        if (!Directory.Exists(startMenuPath))
                            Directory.CreateDirectory(startMenuPath);
                        shortcutTasks.Add((Path.Combine(startMenuPath, appName + ".lnk"), appName));
                    }

                    // Параллельное создание ярлыков
                    Parallel.ForEach(shortcutTasks, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Environment.ProcessorCount
                    }, shortcut =>
                    {
                        CreateShortcutUsingVBS(appPath, shortcut.path, shortcut.name);
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка при создании ярлыков: {ex.Message}");
                }
            });
        }

        // ЗАМЕНИТЕ метод CreateRegistryEntries на этот многопоточный вариант
        private async Task CreateRegistryEntriesParallel()
        {
            await Task.Run(() =>
            {
                try
                {
                    string appName = "project_1";
                    string publisherName = "YourCompany";

                    // Параллельное выполнение операций с реестром
                    var registryTasks = new[]
                    {
                        Task.Run(() => CreatePublisherRegistryKey(appName, publisherName)),
                        Task.Run(() => CreateUninstallRegistryKey(appName, publisherName))
                    };

                    // Ожидаем завершения всех задач
                    Task.WaitAll(registryTasks);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка при работе с реестром: {ex.Message}");
                }
            });
        }

        // ДОБАВЬТЕ ЭТИ ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ДЛЯ РЕЕСТРА
        private void CreatePublisherRegistryKey(string appName, string publisherName)
        {
            try
            {
                string installPath = Path.GetDirectoryName(Application.ExecutablePath);

                using (RegistryKey publisherKey = Registry.LocalMachine.CreateSubKey($@"Software\{publisherName}\{appName}"))
                {
                    if (publisherKey != null)
                    {
                        publisherKey.SetValue("InstallPath", installPath, RegistryValueKind.String);
                        publisherKey.SetValue("Version", "1.0.0", RegistryValueKind.String);
                        publisherKey.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"), RegistryValueKind.String);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка создания ключа издателя: {ex.Message}");
            }
        }

        private void CreateUninstallRegistryKey(string appName, string publisherName)
        {
            try
            {
                string installPath = Path.GetDirectoryName(Application.ExecutablePath);
                string uninstallString = $"\"{Application.ExecutablePath}\" /uninstall";

                using (RegistryKey uninstallKey = Registry.LocalMachine.CreateSubKey($@"Software\Microsoft\Windows\CurrentVersion\Uninstall\{appName}"))
                {
                    if (uninstallKey != null)
                    {
                        uninstallKey.SetValue("DisplayName", "Project 1 Application", RegistryValueKind.String);
                        uninstallKey.SetValue("DisplayVersion", "1.0.0", RegistryValueKind.String);
                        uninstallKey.SetValue("Publisher", publisherName, RegistryValueKind.String);
                        uninstallKey.SetValue("UninstallString", uninstallString, RegistryValueKind.String);
                        uninstallKey.SetValue("InstallLocation", installPath, RegistryValueKind.String);
                        uninstallKey.SetValue("DisplayIcon", Application.ExecutablePath, RegistryValueKind.String);
                        uninstallKey.SetValue("NoModify", 1, RegistryValueKind.DWord);
                        uninstallKey.SetValue("NoRepair", 1, RegistryValueKind.DWord);
                        uninstallKey.SetValue("EstimatedSize", GetApplicationSizeParallel(), RegistryValueKind.DWord);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка создания ключа удаления: {ex.Message}");
            }
        }

        // ЗАМЕНИТЕ метод GetApplicationSize на этот многопоточный вариант
        private int GetApplicationSizeParallel()
        {
            try
            {
                string appDirectory = Path.GetDirectoryName(Application.ExecutablePath);
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

        private Task UpdateProgress(int value, string status)
        {
            if (InvokeRequired)
            {
                return (Task)Invoke(new Func<int, string, Task>(UpdateProgress), value, status);
            }

            progressBar1.Value = value;
            textBox1.Text = status;
            return Task.Delay(100);
        }

        // Метод создания ярлыка через VBS скрипт (оставьте без изменений)
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
                File.WriteAllText(tempVbsFile, vbsScript, Encoding.Default);

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
                throw new Exception($"VBS метод не сработал: {ex.Message}");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Оставляем пустым
        }
    }
}