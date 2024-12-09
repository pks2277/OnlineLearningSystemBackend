using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace OnlineLearningPlatform.Models
{
    public class Contact
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("FirstName")]
        public string FirstName { get; set; }

        [BsonElement("LastName")]
        public string LastName { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("CountryCode")]
        public string CountryCode { get; set; }

        [BsonElement("Phone")]
        public string Phone { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; }
    }
}
