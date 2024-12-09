using MongoDB.Driver;
using OnlineLearningPlatform.Models;

namespace OnlineLearningPlatform.Services
{
    public class ContactService
    {
        private readonly IMongoCollection<Contact> _contacts;

        public ContactService(MongoDbContext context)
        {

            _contacts = context.Contacts;
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            await _contacts.InsertOneAsync(contact);
            return contact;
        }
    }
}
