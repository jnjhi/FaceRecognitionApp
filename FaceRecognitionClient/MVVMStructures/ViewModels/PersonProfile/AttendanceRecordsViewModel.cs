using FaceRecognitionClient.Commands;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.MVVMStructures.Models.PersonProfile;
using FaceRecognitionClient.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.PersonProfile
{
    public class AttendanceRecordsViewModel : BaseViewModel
    {
        private readonly AttendanceModel m_Model;
        private readonly AdvancedPersonData m_Person;
        private readonly AttendanceExportService m_ExportService;

        public ObservableCollection<AttendanceRecord> AttendanceRecords { get; } = new();

        public AsyncRelayCommand RefreshCommand { get; }
        public RelayCommand ExportCommand { get; }

        public AttendanceRecordsViewModel(INetworkFacade network, Mapper mapper, AdvancedPersonData person)
        {
            m_Model = new AttendanceModel(network, mapper);
            m_Person = person;
            m_ExportService = new AttendanceExportService();

            RefreshCommand = new AsyncRelayCommand(_ => LoadAsync());
            ExportCommand = new RelayCommand(_ => ExportAttendance());
        }

        public async Task LoadAsync()
        {
            try
            {
                var records = await m_Model.GetAttendanceAsync(m_Person);

                AttendanceRecords.Clear();
                foreach (var record in records)
                    AttendanceRecords.Add(record);
            }
            catch (Exception ex)
            {
                ClientLogger.ClientLogger.LogException(ex, "Failed to load attendance in AttendanceRecordsViewModel.");
            }
        }

        private void ExportAttendance()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                FileName = "attendance.txt"
            };

            if (dialog.ShowDialog() != true)
                return;

            m_ExportService.Export(AttendanceRecords, dialog.FileName);
        }
    }
}
