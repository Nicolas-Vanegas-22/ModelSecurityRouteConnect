namespace Entity.Model
{
    public class Permission    
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }
        public string DeleteAt { get; set; }
        public string CreateAt { get; set; }
    }
}