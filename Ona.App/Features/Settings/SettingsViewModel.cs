using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ona.App.Controls;
using Ona.App.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ona.App.Features.Settings
{
	public class SettingsViewModel : ObservableObject
	{
		private readonly IDateRepository dateRepository;
		private readonly IFileSaver fileSaver;
		private readonly IFilePicker filePicker;

		private Task? deleteDataTask;
		private Task<FileSaverResult>? exportDataTask;
		private Task<bool>? importDataTask;

		public SettingsViewModel(
			IDateRepository dateRepository,
			IFileSaver fileSaver,
			IFilePicker filePicker)
		{
			this.dateRepository = dateRepository;
			this.fileSaver = fileSaver;
			this.filePicker = filePicker;

			VersionNumber = AppInfo.Current.VersionString;
			
			DeleteDataCommand = new RelayCommand(ExecuteDeleteData, CanExecuteDeleteData);
			ExportDataCommand = new RelayCommand(ExecuteExportData, CanExecuteExportData);
			ImportDataCommand = new RelayCommand(ExecuteImportData, CanExecuteImportData);
		}

		public IUserNotificationService UserNotificationService { get; set; }

		public string VersionNumber { get; }

		public ICommand DeleteDataCommand { get; }

		public ICommand ExportDataCommand { get; }

		public ICommand ImportDataCommand { get; }

		private async void ExecuteDeleteData()
		{
			var isConfirmrd = await UserNotificationService.ConfirmAsync(
				title: "Confirm Data Deletion",
				message: "This action can’t be undone.",
				accept: "Confirm & Delete Data",
				cancel: "Cancel");

			if (!isConfirmrd)
				return;

			this.deleteDataTask = this.dateRepository.DeleteAllDateRecordsAsync();
			await this.deleteDataTask;
			this.deleteDataTask = null;

			await UserNotificationService.NotifyAsync("Data deleted");
		}

		private bool CanExecuteDeleteData()
			=> this.deleteDataTask == null;

		private async void ExecuteExportData()
		{
			this.exportDataTask = ExportDataAsync();
			var taskResult = await this.exportDataTask;
			this.exportDataTask = null;

			var message = taskResult.IsSuccessful ? "Data exported" : "Error exporting data";
			await UserNotificationService.NotifyAsync(message);
		}

		private bool CanExecuteExportData()
			=> this.exportDataTask == null;

		private async void ExecuteImportData()
		{
			this.importDataTask = ImportDataAsync();
			var taskResult = await this.importDataTask;
			this.importDataTask = null;

			var message = taskResult ? "Data imported" : "Error importing data";
			await UserNotificationService.NotifyAsync(message);
		}

		private bool CanExecuteImportData()
			=> this.importDataTask == null;

		private async Task<FileSaverResult> ExportDataAsync()
		{
			var dateRecords = await this.dateRepository.GetDateRecordsAsync();

			using var stream = new MemoryStream();
			using var writer = new StreamWriter(stream);

			foreach (var dateRecord in dateRecords)
				await writer.WriteLineAsync(dateRecord.Date.ToString("yyyy-MM-dd"));

			await writer.FlushAsync();

			var fileSaverResult = await FileSaver.Default.SaveAsync("data.ona", stream);
			return fileSaverResult;
		}

		private async Task<bool> ImportDataAsync()
		{
			var filePickerResult = await this.filePicker.PickAsync();

			if (filePickerResult == null)
				return false;

			try
			{
				using var stream = await filePickerResult.OpenReadAsync();
				using var streamReader = new StreamReader(stream);

				var dates = new List<DateTime>();
				while (await streamReader.ReadLineAsync() is string line)
					dates.Add(DateTime.Parse(line));

				await this.dateRepository.DeleteAllDateRecordsAsync();

				foreach (var date in dates)
					await this.dateRepository.AddDateRecordAsync(date);

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
