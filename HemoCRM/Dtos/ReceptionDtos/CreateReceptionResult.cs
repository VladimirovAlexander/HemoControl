namespace HemoCRM.Web.Dtos.ReceptionDtos
{
    public class CreateReceptionResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Guid? ReceptionId { get; set; }

        public static CreateReceptionResult Fail(string message) => new() { Success = false, Message = message };
        public static CreateReceptionResult Ok(Guid id) => new() { Success = true, ReceptionId = id };
    }
}
