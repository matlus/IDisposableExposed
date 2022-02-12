using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DomainLayer
{
    internal sealed class ImdbServiceGateway : IDisposable
    {
        private HttpClient _httpClient;
        private bool _disposed;

        public ImdbServiceGateway(string baseUrl)
        {
            _httpClient = CreateHttpClient(baseUrl);
        }

        private HttpClient CreateHttpClient(string baseUrl)
        {
            var httpMessageHandler = new HttpClientHandler();

            if (httpMessageHandler is HttpClientHandler httpClientHandler)
            {
                httpClientHandler.PreAuthenticate = true;
                httpClientHandler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
            }

            var httpClient = new HttpClient(httpMessageHandler)
            {
                BaseAddress = new Uri(baseUrl),
            };

            return httpClient;
        }

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            var httpResponseMessage = await _httpClient.GetAsync("movies");
            var imdbMovies = await httpResponseMessage.Content.ReadAsAsync<IEnumerable<ImdbMovie>>();
            return MapToMovies(imdbMovies);
        }

        private IEnumerable<Movie> MapToMovies(IEnumerable<ImdbMovie> imdbMovies)
        {
            foreach (var imdbMovie in imdbMovies)
            {
                yield return new Movie(
                                    title: imdbMovie.Title,
                                    imageUrl: imdbMovie.ImageUrl,
                                    genre: GenreParser.Parse(imdbMovie.Category),
                                    year: imdbMovie.Year);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _httpClient.Dispose();
            }

            _disposed = true;
        }
    }
}
