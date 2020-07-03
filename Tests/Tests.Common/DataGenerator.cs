using Common.Models;
using System;
using System.Collections.Generic;

namespace Tests.Common
{
    public static class DataGenerator
    {
        public static List<Movie> GetMovieList()
        {
            return new List<Movie>() {
            new Movie(){
                IsAdult= false,
                ImdbId="tt0114709",
                Overview ="Led by Woody, Andy's toys live happily in his room until Andy's birthday brings Buzz Lightyear onto the scene. Afraid of losing his place in Andy's heart, Woody plots against Buzz. But when circumstances separate Buzz and Woody from their owner, the duo eventually learns to put aside their differences.",
                Popularity = 21.946943,
                ReleaseDate = DateTime.Parse("1995/10/30"),
                Revenue = 373554033,
                Runtime = 81,
                Title = "Toy Story",
                VoteAverage=7.7
            },
            new Movie(){
                IsAdult= true,
                ImdbId="tt0093870",
                Overview ="In a violent, near-apocalyptic Detroit, evil corporation Omni Consumer Products wins a contract from the city government to privatize the police force. To test their crime-eradicating cyborgs, the company leads street cop Alex Murphy into an armed confrontation with crime lord Boddicker so they can use his body to support their untested RoboCop prototype. But when RoboCop learns of the company's nefarious plans, he turns on his masters.",
                Popularity = 13.485583,
                ReleaseDate = DateTime.Parse("1987/07/17"),
                Revenue = 53000000,
                Runtime = 102,
                TagLine= "Part man. Part machine. All cop. The future of law enforcement.",
                Title = "RoboCop",
                VoteAverage=7.1
            }
            };
        }
    }
}
