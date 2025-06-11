using System.Linq;
using Microsoft.Win32;
using FaceRecognitionClient.Services.AttendanceExportService;
﻿using FaceRecognitionClient.Commands;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.MVVMStructures.Models.PersonProfile;
using System.Collections.ObjectModel;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.PersonProfile
{
    public class AttendanceRecordsViewModel : BaseViewModel
    {
        private readonly AttendanceModel m_Model;
        private readonly AdvancedPersonData m_Person;

        public ObservableCollection<AttendanceRecord> AttendanceRecords { get; } = new();

        public AsyncRelayCommand RefreshCommand { get; }
        public AsyncRelayCommand ExportCommand { get; }

        public AttendanceRecordsViewModel(INetworkFacade network, Mapper mapper, AdvancedPersonData person)
        {
            m_Model = new AttendanceModel(network, mapper);
            m_Person = person;

            RefreshCommand = new AsyncRelayCommand(_ => LoadAsync());
            ExportCommand = new AsyncRelayCommand(_ => ExportAsync());
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
        private async Task ExportAsync()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FileName = "attendance.txt",
                DefaultExt = ".txt",
                AddExtension = true,
            };

            if (dialog.ShowDialog() != true)
                return;

            var exporter = new AttendanceExportService();
            var records = AttendanceRecords.Cast<AttendanceRecord>().ToList();
            await exporter.ExportAsync(records, dialog.FileName);
        }

    }
}
