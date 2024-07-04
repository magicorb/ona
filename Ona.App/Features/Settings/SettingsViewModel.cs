using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ona.App.Controls;
using Ona.App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ona.App.Features.Settings
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly IDateRepository dateRepository;
        private readonly IUserConfirmationService userConfirmationService;

        private Task? deleteTask;

        public SettingsViewModel(
            IDateRepository dateRepository,
            IUserConfirmationService userConfirmationService)
        {
            this.dateRepository = dateRepository;
            this.userConfirmationService = userConfirmationService;
            DeleteDataCommand = new RelayCommand(ExecuteDeleteData, CanExecuteDeleteData);
        }

        public ICommand DeleteDataCommand { get; }

        private async void ExecuteDeleteData()
        {
            var isConfirmrd = await userConfirmationService.ConfirmAsync(
                title: "Confirm Data Deletion",
                message: "This action can’t be undone.",
                accept: "Confirm & Delete Data",
                cancel: "Cancel");

            if (!isConfirmrd)
                return;

            deleteTask = dateRepository.DeleteAllDateRecordsAsync();
            await deleteTask;
            deleteTask = null;
        }

        private bool CanExecuteDeleteData()
            => deleteTask == null;
    }
}
