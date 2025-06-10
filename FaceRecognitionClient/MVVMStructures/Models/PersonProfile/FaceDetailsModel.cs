using DataProtocols.FaceRecognitionMessages;
using DataProtocols.FaceRecognitionMessages.Models;
using FaceRecognitionClient.InternalDataModels;

namespace FaceRecognitionClient.MVVMStructures.Models.PersonProfile
{
    /// <summary>
    /// Handles saving or updating a known person’s details (name, government ID, etc.).
    /// Performs local validation and sends update requests to the server.
    /// </summary>
    public class FaceDetailsModel
    {
        private readonly INetworkFacade m_NetworkFacade;
        private readonly Mapper m_Mapper;

        public FaceDetailsModel(INetworkFacade networkFacade, Mapper mapper)
        {
            m_NetworkFacade = networkFacade;
            m_Mapper = mapper;
        }

        /// <summary>
        /// Validates input, builds the request, and sends it to the server.
        /// Returns a DTO indicating whether the update was successful.
        /// </summary>
        public async Task<UpdatePersonDataResponseDTO> SaveOrUpdateAsync(AdvancedPersonData record)
        {
            var validationResults = PersonDataLegitimacyCheck.Validate(record.FirstName, record.LastName, record.GovernmentID, record.Sex);

            if (!validationResults.IsValid)
            {
                return new UpdatePersonDataResponseDTO(false, m_Mapper.Map<PersonDataValidationResult, PersonDataValidationResultDTO>(validationResults));
            }

            var request = new UpdatePersonDataRequestDTO
            {
                Id = record.Id,
                FaceEmbedding = record.FaceEmbedding,
                GovernmentID = record.GovernmentID,
                FirstName = record.FirstName,
                LastName = record.LastName,
                HeightCm = record.HeightCm,
                Sex = record.Sex,
                Notes = record.Notes
            };

            return await m_NetworkFacade.SendRequestAsync<UpdatePersonDataRequestDTO, UpdatePersonDataResponseDTO>(request);
        }
    }
}
