namespace DataProtocols.RetrievingPersonDataMessages
{
    public class GetAdvancedPersonDataWithProfilePictureByIdRequestDTO : Data
    {
        public int PersonId { get; set; }

        public GetAdvancedPersonDataWithProfilePictureByIdRequestDTO()
        {
            DataType = DataType.GetAdvancedPersonDataWithProfilePictureByIdRequest;
        }

        public GetAdvancedPersonDataWithProfilePictureByIdRequestDTO(int personId) : this()
        {
            PersonId = personId;
        }
    }
}
