using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConcertCompanion.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using SpotifyAPI.Web.Auth;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ConcertCompanion.Controllers
{
    public static class AuthHelper
    {
        // test values
        static string clientId = "dummy";
        static string clientSecret = "dummy";
        static string redirectUri = "concert-companion://callback";
        static TokenSwapWebAPIFactory webApiFactory;
        static SpotifyWebAPI spotify;
        // real values

        [HttpGet]
        public static async void DoAuthAsync()
        {
            AuthorizationCodeAuth auth = new AuthorizationCodeAuth(
  clientId,
  clientSecret,
  // "https://concert-companion.appspot.com",
  // "https://concert-companion.appspot.com",
  "concert-companion:/",
  // "http://localhost:4002",
  "http://localhost:4002",
  Scope.UserFollowRead | Scope.UserReadPrivate | Scope.UserTopRead | Scope.UserReadEmail | Scope.PlaylistReadCollaborative
);

            auth.AuthReceived += async (sender, payload) =>
            {
                auth.Stop();
                Token token = await auth.ExchangeCode(payload.Code);
                SpotifyWebAPI api = new SpotifyWebAPI()
                {
                    TokenType = token.TokenType,
                    AccessToken = token.AccessToken
                };

                PrivateProfile profile = api.GetPrivateProfile();
                Paging<FullArtist> artists = api.GetUsersTopArtists();
                UserProfile userProfile = new UserProfile()
                {
                    Id = profile.Id,
                    Name = profile.DisplayName,
                    Email = profile.Email,
                    TopArtists = artists.Items.Select(artist => new Artist() { Id = artist.Id, Name = artist.Name }).ToList()
                };

                SaveUser(userProfile);

                // Do requests with API client
            };
            Console.WriteLine("Starting the server");
            auth.Start(); // Starts an internal HTTP Server
            Console.WriteLine("Opening the browser");
            auth.OpenBrowser();
        }

        private static void SaveUser(UserProfile profile)
        {
            // 34.83.48.61:27017
            var client = new MongoClient("dummy");
            var database = client.GetDatabase("TSpotifyDB");
            var collection = database.GetCollection<UserProfile>("users");
            //var deleteFilter = Builders<UserProfile>.Filter.Eq("Id", "dnwk7y2o86oy14bwmmzgwetfu");
            //collection.DeleteOne(deleteFilter);
            try
            {
                collection.InsertOne(profile);
            }
            catch
            { }
        }
    }
}
