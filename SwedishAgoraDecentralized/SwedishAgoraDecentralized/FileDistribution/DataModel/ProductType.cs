namespace SwedishAgoraDecentralized.FileDistribution.DataModel
{
    public sealed class ProductType
    {
        public static Dictionary<int, ProductType> Dictionary { get; } = new() 
        {
            //Level 1 (Top level)
            { 1, new ProductType { Id = 1, Name = "Substances" } },
            { 2, new ProductType { Id = 2, Name = "Utilities" } },
            { 3, new ProductType { Id = 3, Name = "Counterfeits" } },
            { 4, new ProductType { Id = 4, Name = "Firearms" } },
            { 5, new ProductType { Id = 5, Name = "Legal products" } },
        };

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int? ParentId { get; set; }
    }
}
