namespace Entity.Model
{
    public class Form
    {
        public int FormId { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string DeleteAt { get; set; }
        public string CreateAt { get; set; }
    }
}
