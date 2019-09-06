using CsvHelper;
using CsvHelper.Configuration;
using IMDBDataExtractor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBDataExtractor.Services
{
    public class IMDBService
    {
        private const string MOVIE_TYPE = "movie";
        private const string TV_SERIE_TYPE = "tvSeries";
        List<string> NOT_ALLOWED_GENRES = new List<string>() { "Documentary", "Short", "News", "\\N", "Sport", "Biography", "Music", "History", "Adult", "Reality-TV", "Talk-Show", "Game-Show", "Film-Noir", "Musical" };

        private string _originalFilePath;
        private string _ratingFilePath;

        public IMDBService(string originalFilePath, string ratingFilePath)
        {
            _originalFilePath = originalFilePath;
            _ratingFilePath = ratingFilePath;
        }

        public List<string> GetFilmsIdsFromIMDB(string destinationFilePath)
        {
            List<string> ids = new List<string>();
            CsvReader csvReader = new CsvReader(new StreamReader(_originalFilePath), new Configuration() { Delimiter = "\t", HasHeaderRecord = true, BadDataFound = null, IgnoreQuotes = true });

            var csvWriter = new CsvWriter(new StreamWriter(destinationFilePath), new Configuration() { Delimiter = "\t", IgnoreQuotes = true, HasHeaderRecord = true });
            csvWriter.WriteHeader(typeof(ImdbTitleBasicsWithRating));
            csvWriter.Flush();
            csvWriter.Context.Writer.WriteLine();

            int total = File.ReadLines(_originalFilePath).Count();

            Dictionary<string, ImdbTitleBasics> filmsList = new Dictionary<string, ImdbTitleBasics>();

            while (csvReader.Read())
            {
                var record = csvReader.GetRecord<ImdbTitleBasics>();

                if (record.titleType.Equals(MOVIE_TYPE) && !record.isAdultBool && record.startYearInt >= 2000 && record.startYearInt < 2019 && record.runtimeMinutesInt > 90 && !record.GenresList.Any(t2 => NOT_ALLOWED_GENRES.Contains(t2)))
                {
                    filmsList.Add(record.tconst, record);
                }
            }

            FillRatings(filmsList);

            foreach (ImdbTitleBasics basicFilm in filmsList.Values)
            {
                if (basicFilm is ImdbTitleBasicsWithRating)
                {
                    ImdbTitleBasicsWithRating film = (ImdbTitleBasicsWithRating)basicFilm;
                    if (film.averageRatingFloat > 7.5 && film.numVotesInt > 1000)
                    {
                        csvWriter.WriteRecord(film);
                        csvWriter.Flush();
                        csvWriter.Context.Writer.WriteLine();
                        csvWriter.Flush();

                        ids.Add(film.tconst);
                    }
                }
            }

            csvWriter.Dispose();
            csvReader.Dispose();

            return ids;

        }

        public List<string> GetTvSeriesIdsFromIMDB(string destinationFilePath)
        {
            List<string> ids = new List<string>();
            CsvReader csvReader = new CsvReader(new StreamReader(_originalFilePath), new Configuration() { Delimiter = "\t", HasHeaderRecord = true, BadDataFound = null, IgnoreQuotes = true });

            var csvWriter = new CsvWriter(new StreamWriter(destinationFilePath), new Configuration() { Delimiter = "\t", IgnoreQuotes = true, HasHeaderRecord = true });
            csvWriter.WriteHeader(typeof(ImdbTitleBasicsWithRating));
            csvWriter.Flush();
            csvWriter.Context.Writer.WriteLine();

            int total = File.ReadLines(_originalFilePath).Count();

            Dictionary<string, ImdbTitleBasics> filmsList = new Dictionary<string, ImdbTitleBasics>();
            while (csvReader.Read())
            {
                var record = csvReader.GetRecord<ImdbTitleBasics>();

                if (record.titleType.Equals(TV_SERIE_TYPE) && !record.isAdultBool && record.startYearInt >= 2010 && record.startYearInt < 2019 && !record.GenresList.Any(t2 => NOT_ALLOWED_GENRES.Contains(t2)))
                {
                    filmsList.Add(record.tconst, record);
                }
            }

            FillRatings(filmsList);

            foreach (ImdbTitleBasics basicFilm in filmsList.Values)
            {
                if (basicFilm is ImdbTitleBasicsWithRating)
                {
                    ImdbTitleBasicsWithRating film = (ImdbTitleBasicsWithRating)basicFilm;
                    if (film.averageRatingFloat > 8.0 && film.numVotesInt > 10000)
                    {
                        csvWriter.WriteRecord(film);
                        csvWriter.Flush();
                        csvWriter.Context.Writer.WriteLine();
                        csvWriter.Flush();

                        ids.Add(film.tconst);
                    }
                }
            }

            csvWriter.Dispose();
            csvReader.Dispose();

            return ids;

        }

        private void FillRatings(Dictionary<string, ImdbTitleBasics> filmsList)
        {
            CsvReader csvReader = new CsvReader(new StreamReader(_ratingFilePath), new Configuration() { Delimiter = "\t", HasHeaderRecord = true, BadDataFound = null, IgnoreQuotes = true });

            while (csvReader.Read())
            {
                var record = csvReader.GetRecord<ImdbTitleRating>();

                if (filmsList.ContainsKey(record.tconst))
                {
                    ImdbTitleBasicsWithRating filmWithRating = new ImdbTitleBasicsWithRating(filmsList[record.tconst]);
                    filmsList[record.tconst] = filmWithRating;
                    filmWithRating.averageRating = record.averageRating;
                    filmWithRating.numVotes = record.numVotes;
                }
            }
        }
    }
}
