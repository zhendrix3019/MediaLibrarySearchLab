using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediaLibrary
{
    class Program
    {
        private static readonly NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();

        private static readonly string scrubbedFile = FileScrubber.ScrubMovies("movies.csv");
        private static readonly MovieFile movieFile = new MovieFile(scrubbedFile);

        static void Main(string[] args)
        {
            logger.Info("Program started");

            int choice;
            do
            {
                Console.WriteLine("[1] Display All movies");
                Console.WriteLine("[2] Add a movie");
                Console.WriteLine("[3] Search for a movie");
                Console.WriteLine("[0] Quit");
                choice = Int32.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        movieFile.DisplayMovies();
                        break;
                    case 2:
                        AddMovie();
                        break;
                    case 3:
                        SearchMovies();
                        break;
                    default:
                        break;
                }
            } while (choice != 0);

            logger.Info("Program ended");
        }

        private static void AddMovie()
        {
            Console.WriteLine("Enter movie title:");
            string title = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(title))
            {
                Console.WriteLine("Invalid title. Please try again.");
                return;
            }

            if (FindDupeMovieByTitle(title))
            {
                Console.WriteLine("That movie is already in the list!");
                return;
            }

            Console.WriteLine("Enter director:");
            string director = Console.ReadLine().Trim();

            Console.WriteLine("Enter genres separated by '|':");
            string genresString = Console.ReadLine().Trim();

            Console.WriteLine("Enter run time in the format 'hh:mm:ss':");
            string runTime = Console.ReadLine().Trim();
            if (!TimeSpan.TryParse(runTime, out TimeSpan duration))
            {
                Console.WriteLine("Invalid duration. Please try again.");
                return;
            }

            Movie movie = new Movie
            {
                mediaId = movieFile.GetNewID(),
                title = title,
                director = director,
                genres = genresString.Split('|').Select(x => x.Trim()).ToList(),
                runningTime = duration
            };

            movieFile.AddMovie(movie);

            logger.Info("Movie added:");
            Console.WriteLine(movie.Display());
        }

        private static bool FindDupeMovieByTitle(string title)
        {
            return movieFile.MovieList.Any(x => x.title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }

        private static void SearchMovies()
        {
            Console.WriteLine("Enter a title to search for:");
            string title = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(title))
            {
                Console.WriteLine("Invalid title. Please try again.");
                return;
            }

            List<Movie> result = movieFile.SearchMovieByTitle(title);

            Console.WriteLine($"Found {result.Count} movies with title containing \"{title}\":");
            foreach (Movie movie in result)
            {
                Console.WriteLine(movie.Display());
            }
        }
    }
}
