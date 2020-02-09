using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConcertCompanion.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Models;
using SpotifyAPI.Web;

namespace ConcertCompanion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : Controller
    {
        [HttpGet]
        public IEnumerable<UserProfile> Get(string userId)
        {
            // dnwk7y2o86oy14bwmmzgwetfu
            var client = new MongoClient("mongodb://adminUser:Thinkpad@34.83.48.61:27017");
            var database = client.GetDatabase("TSpotifyDB");
            var filter = Builders<UserProfile>.Filter.Eq("Id", userId);
            var currentUser = database.GetCollection<UserProfile>("users").Find(filter).FirstOrDefault();

            CredentialsAuth auth = new CredentialsAuth("192d8e42c7c4403c972a5403aff35e5f", "52f6c46978c841869bdb5bb660ea86fa");
            Token token = auth.GetToken().Result;
            SpotifyWebAPI api = new SpotifyWebAPI()
            {
                TokenType = token.TokenType,
                AccessToken = token.AccessToken
            };


            return database.GetCollection<UserProfile>("users").Find(new BsonDocument()).ToList();
        }
    }
}