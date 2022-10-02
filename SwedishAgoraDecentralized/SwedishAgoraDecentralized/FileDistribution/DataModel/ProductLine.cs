using System.Collections.ObjectModel;

namespace SwedishAgoraDecentralized.FileDistribution.DataModel
{
    public sealed class ProductLine : IHasInsertMetadata
    {
        public string TableName => "productlines";
        public IEnumerable<KeyValuePair<string, object>> FieldParameters => new ReadOnlyDictionary<string, object> (new Dictionary<string, object>()
        {
            { "@Id", Id },
            { "@Name", Name },
            { "@Description", Description },
            { "@ImagePath", ImagePath },
            { "@PublicKey", PublicKey },
            { "@LatestChangeSignature", string.IsNullOrEmpty(LatestChangeSignature) ? DBNull.Value : LatestChangeSignature },
            { "@QuantityRemaining", QuantityRemaining },
            { "@UnitPrice", UnitPrice },
        });

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImagePath { get; set; } = "";
        public string PublicKey { get; set; } = "";
        public string LatestChangeSignature { get; set; } = "";
        public int QuantityRemaining { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
