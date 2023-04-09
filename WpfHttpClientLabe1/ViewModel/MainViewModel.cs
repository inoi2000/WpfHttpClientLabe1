using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfHttpClientLabe1.Commands;

namespace WpfHttpClientLabe1.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Properties
        private string? adressTextBox;
        public string? AdressTextBox
        {
            get => adressTextBox;
            set { adressTextBox = value; OnPropertyChanged(); }
        }

        private string? statusCodeTextBlock;
        public string? StatusCodeTextBlock
        {
            get => statusCodeTextBlock;
            set { statusCodeTextBlock = value; OnPropertyChanged(); }
        }

        private string? responseBodyTextBox;
        public string? ResponseBodyTextBox
        {
            get => responseBodyTextBox;
            set { responseBodyTextBox = value; OnPropertyChanged();}
        }
        #endregion

        #region Commands
        public ActionCommand GetHttpInfoCommand => new ActionCommand(x => GetHttpInfo().GetAwaiter(), x => CanGetHttpInfo());
        public ActionCommand SaveHttpAsCommand => new ActionCommand(x => SaveHttpAs().GetAwaiter(), x => CanSaveHttpAs());

        private async Task GetHttpInfo()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var httpResponse = await httpClient.GetAsync(adressTextBox))
                    {
                        ResponseBodyTextBox = await httpResponse.Content.ReadAsStringAsync();
                        StatusCodeTextBlock = $"{(double)httpResponse.StatusCode} {httpResponse.StatusCode}";
                    }
                }
            }
            catch (ArgumentNullException)
            {
                ResponseBodyTextBox = "Запрос является null";
            }
            catch (InvalidOperationException)
            {
                ResponseBodyTextBox = "Запос URL не является верным, проверте поле ввода";
            }
            catch (HttpRequestException)
            {
                ResponseBodyTextBox = "Сбой запроса из-за основной проблемы, такой как сетевое подключение, сбой DNS, проверка сертификата сервера или тайм-аута";
            }
            catch (TaskCanceledException)
            {
                ResponseBodyTextBox = "Истекло время ожидания запроса";
            }
            catch (Exception ex)
            {
                ResponseBodyTextBox = ex.Message;
            }
            finally
            {
                OnPropertyChanged(nameof(SaveHttpAsCommand));
            }
        }

        private bool CanGetHttpInfo()
        {
            return !string.IsNullOrEmpty(adressTextBox);
        }

        private async Task SaveHttpAs()
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog() { Filter = "Text Files(*.txt)|*.txt|All(*.*)|*" };
                if (dialog.ShowDialog() == true)
                {
                    await File.WriteAllTextAsync(dialog.FileName, ResponseBodyTextBox);
                }
            }
            catch (Exception ex)
            {
                StatusCodeTextBlock = ex.Message;
            }
        }
        private bool CanSaveHttpAs()
        {
            return !string.IsNullOrEmpty(ResponseBodyTextBox);
        }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string property = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
