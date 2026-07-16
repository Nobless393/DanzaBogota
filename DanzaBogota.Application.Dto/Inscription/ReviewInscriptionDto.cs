namespace DanzaBogota.Application.Dto.Inscription
{
    public class ReviewInscriptionDto
    {
        public int InscriptionId { get; set; }
        public bool Approved { get; set; }
        public string? Observations { get; set; }
    }
}
