using System.Linq;
using Microsoft.Win32;
using System.IO;
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
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                ValidateNames = false,
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FileName = "Select folder or file"
            };

            if (dialog.ShowDialog() != true)
                return;

            var path = dialog.FileName;
            if (Directory.Exists(path))
            {
                var fileName = $"attendance_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                path = Path.Combine(path, fileName);
            }
            else if (string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path += ".txt";
            }

            var exporter = new AttendanceExportService();
            var records = AttendanceRecords.Cast<AttendanceRecord>().ToList();
            await exporter.ExportAsync(records, path);
        }

    }
}
