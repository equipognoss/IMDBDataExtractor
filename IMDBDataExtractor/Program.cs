using IMDBDataExtractor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBDataExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            string originalFile = @"..\..\..\title.basics.tsv\data.tsv";
            string ratingFile = @"..\..\..\title.ratings.tsv\data.tsv";
            string filmsFile = @"..\..\data\films.tsv";
            string seriesFile = @"..\..\data\series.tsv";
            

            IMDBService imdbDataReader = new IMDBService(originalFile, ratingFile);
            List<string> filmsIds = imdbDataReader.GetFilmsIdsFromIMDB(filmsFile);
            //List<string> seriesIds = imdbDataReader.GetTvSeriesIdsFromIMDB(seriesFile);
        }
    }
}
