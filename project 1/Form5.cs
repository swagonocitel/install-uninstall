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
    public partial class Form5 : Form
    {
        private bool _isUninstallMode = false;

        public Form5()
        {
            InitializeComponent();
            buttonDelete.Enabled = false;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            string[] args = Environment.GetCommandLineArgs();
            if (args.Any(arg => arg.Equals("/uninstall", StringComparison.OrdinalIgnoreCase)))
            {
                _isUninstallMode = true;
                checkBoxConfirm.Checked = true;
                buttonDelete.PerformClick();
            }
        }

        private void checkBoxConfirm_CheckedChanged(object sender, EventArgs e)
        {
            buttonDelete.Enabled = checkBoxConfirm.Checked;
        }

        // ОБНОВИТЕ метод buttonDelete_Click для многопоточности
        private async void buttonDelete_Click(object sender, EventArgs e)
        {
            if (!checkBoxConfirm.Checked && !_isUninstallMode)
            {
                MessageBox.Show("Пожалуйста, подтвердите удаление программы.", "Подтверждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            buttonDelete.Enabled = false;
            checkBoxConfirm.Enabled = false;
            buttonCancel.Enabled = false;

            await StartUninstallation();
        }

        // ОБНОВИТЕ метод StartUninstallation для многопоточности
        private async Task StartUninstallation()
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = 100;

            try
            {
                // Параллельное выполнение операций удаления
                var deletionTasks = new[]
                {
                    DeleteShortcutsParallel(),
                    DeleteRegistryEntriesParallel(),
                    DeleteProgramFilesParallel()
                };

                // Запускаем все задачи параллельно и отслеживаем прогресс
                await UpdateProgress(20, "Удаление ярлыков...");
                await deletionTasks[0];

                await UpdateProgress(50, "Удаление записей реестра...");
                await deletionTasks[1];

                await UpdateProgress(80, "Удаление файлов программы...");
                await deletionTasks[2];

                await UpdateProgress(100, "Завершение удаления...");
                await Task.Delay(500);

                MessageBox.Show("Программа успешно удалена!", "Удаление завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (!_isUninstallMode)
                {
                    buttonDelete.Enabled = true;
                    checkBoxConfirm.Enabled = true;
                    buttonCancel.Enabled = true;
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        // ЗАМЕНИТЕ метод DeleteShortcuts на этот многопоточный вариант
        private async Task DeleteShortcutsParallel()
        {
            await Task.Run(() =>
            {
                try
                {
                    string appName = "installer";

                    var deletionTasks = new[]
                    {
                        Task.Run(() => DeleteDesktopShortcut(appName)),
                        Task.Run(() => DeleteStartMenuShortcut(appName))
                    };

                    Task.WaitAll(deletionTasks);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка при удалении ярлыков: {ex.Message}");
                }
            });
        }

        // ДОБАВЬТЕ ЭТИ ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ДЛЯ УДАЛЕНИЯ ЯРЛЫКОВ
        private void DeleteDesktopShortcut(string appName)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string desktopShortcut = Path.Combine(desktopPath, appName + ".lnk");
                if (File.Exists(desktopShortcut))
                {
                    File.Delete(desktopShortcut);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Не удалось удалить ярлык с рабочего стола: {ex.Message}");
            }
        }

        private void DeleteStartMenuShortcut(string appName)
        {
            try
            {
                string startMenuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs");
                string startMenuShortcut = Path.Combine(startMenuPath, appName + ".lnk");
                if (File.Exists(startMenuShortcut))
                {
                    File.Delete(startMenuShortcut);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Не удалось удалить ярлык из меню Пуск: {ex.Message}");
            }
        }

        // ЗАМЕНИТЕ метод DeleteRegistryEntries на этот многопоточный вариант
        private async Task DeleteRegistryEntriesParallel()
        {
            await Task.Run(() =>
            {
                try
                {
                    string appName = "project_1";
                    string publisherName = "YourCompany";

                    var registryDeletionTasks = new[]
                    {
                        Task.Run(() => DeletePublisherRegistryKey(appName, publisherName)),
                        Task.Run(() => DeleteUninstallRegistryKey(appName))
                    };

                    Task.WaitAll(registryDeletionTasks);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка при удалении записей реестра: {ex.Message}");
                }
            });
        }

        // ДОБАВЬТЕ ЭТИ ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ДЛЯ УДАЛЕНИЯ РЕЕСТРА
        private void DeletePublisherRegistryKey(string appName, string publisherName)
        {
            try
            {
                string publisherKeyPath = $@"Software\{publisherName}\{appName}";
                Registry.LocalMachine.DeleteSubKeyTree(publisherKeyPath, false);
            }
            catch (ArgumentException)
            {
                // Ключ не существует - это нормально
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Не удалось удалить ключ издателя: {ex.Message}");
            }
        }

        private void DeleteUninstallRegistryKey(string appName)
        {
            try
            {
                string uninstallKeyPath = $@"Software\Microsoft\Windows\CurrentVersion\Uninstall\{appName}";
                Registry.LocalMachine.DeleteSubKeyTree(uninstallKeyPath, false);
            }
            catch (ArgumentException)
            {
                // Ключ не существует - это нормально
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Не удалось удалить ключ удаления: {ex.Message}");
            }
        }

        // ЗАМЕНИТЕ метод DeleteProgramFiles на этот многопоточный вариант
        private async Task DeleteProgramFilesParallel()
        {
            await Task.Run(() =>
            {
                try
                {
                    string installPath = Path.GetDirectoryName(Application.ExecutablePath);
                    string appName = "project_1";

                    // Проверяем, что мы не пытаемся удалить системные папки
                    if (string.IsNullOrEmpty(installPath) ||
                        installPath.Equals(Environment.GetFolderPath(Environment.SpecialFolder.Windows), StringComparison.OrdinalIgnoreCase) ||
                        installPath.Equals(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), StringComparison.OrdinalIgnoreCase) ||
                        installPath.Equals(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), StringComparison.OrdinalIgnoreCase))
                    {
                        if (_isUninstallMode)
                            return;
                        else
                            throw new Exception("Невозможно удалить файлы из системной папки.");
                    }

                    // Удаляем папку с программой, если она существует
                    if (Directory.Exists(installPath))
                    {
                        DeleteFilesAndFoldersParallel(installPath);
                    }

                    // Параллельная очистка временных файлов
                    CleanTempFilesParallel();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка при удалении файлов программы: {ex.Message}");
                }
            });
        }

        // ДОБАВЬТЕ ЭТОТ МЕТОД ДЛЯ МНОГОПОТОЧНОГО УДАЛЕНИЯ ФАЙЛОВ И ПАПОК
        private void DeleteFilesAndFoldersParallel(string directoryPath)
        {
            try
            {
                // Получаем все файлы и папки
                var allFiles = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
                var allDirectories = Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories);

                // Параллельное удаление файлов
                Parallel.ForEach(allFiles, new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }, file =>
                {
                    try
                    {
                        // Не удаляем текущий исполняемый файл в режиме uninstall
                        if (!_isUninstallMode || !file.Equals(Application.ExecutablePath, StringComparison.OrdinalIgnoreCase))
                        {
                            File.SetAttributes(file, FileAttributes.Normal);
                            File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Не удалось удалить файл {file}: {ex.Message}");
                    }
                });

                // Параллельное удаление папок
                var sortedDirectories = allDirectories.OrderByDescending(d => d.Length).ToArray();
                Parallel.ForEach(sortedDirectories, new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }, dir =>
                {
                    try
                    {
                        if (Directory.Exists(dir))
                        {
                            Directory.Delete(dir, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Не удалось удалить папку {dir}: {ex.Message}");
                    }
                });

                // Пытаемся удалить корневую папку
                if (!_isUninstallMode && Directory.Exists(directoryPath))
                {
                    try
                    {
                        Directory.Delete(directoryPath, true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Не удалось удалить корневую папку: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка параллельного удаления: {ex.Message}");
            }
        }

        // ЗАМЕНИТЕ метод CleanTempFiles на этот многопоточный вариант
        private void CleanTempFilesParallel()
        {
            try
            {
                string tempPath = Path.GetTempPath();
                string appName = "project_1";

                // Ищем файлы, связанные с нашей программой
                var tempFiles = Directory.GetFiles(tempPath, "*" + appName + "*", SearchOption.AllDirectories)
                                        .Concat(Directory.GetFiles(tempPath, "*project_1*", SearchOption.AllDirectories))
                                        .Distinct()
                                        .ToArray();

                // Параллельное удаление временных файлов
                Parallel.ForEach(tempFiles, new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }, file =>
                {
                    try
                    {
                        if (File.Exists(file))
                        {
                            File.SetAttributes(file, FileAttributes.Normal);
                            File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Не удалось удалить временный файл {file}: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка очистки временных файлов: {ex.Message}");
            }
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

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}