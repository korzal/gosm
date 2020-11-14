using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace GOSM.Models
{
    public class SeedData
    {
        public static void Init(IServiceProvider serviceProvider)
        {
            using (Database context = new Database(serviceProvider.GetRequiredService<DbContextOptions<Database>>()))
            {
                foreach (var user in context.UserTable)
                {
                    context.UserTable.Remove(user);
                }
                foreach(var post in context.PostTable)
                {
                    context.PostTable.Remove(post);
                }
                foreach (var comment in context.CommentTable)
                {
                    context.CommentTable.Remove(comment);
                }
                foreach(var game in context.RelevantGamesTable)
                {
                    context.RelevantGamesTable.Remove(game);
                }
                foreach(var friend in context.FriendRequestTable)
                {
                    context.FriendRequestTable.Remove(friend);
                }
                foreach(var genre in context.GameGenreTable)
                {
                    context.GameGenreTable.Remove(genre);
                }
                context.SaveChanges();

                context.GameGenreTable.AddRange(
                    new GameGenre
                    {
                        Value = "FPS"
                    },
                    new GameGenre
                    {
                        Value = "MOBA"
                    },
                    new GameGenre
                    {
                        Value = "MMORPG"
                    },
                    new GameGenre
                    {
                        Value = "RPG"
                    },
                    new GameGenre
                    {
                        Value = "Battle royale"
                    },
                    new GameGenre
                    {
                        Value = "Sandbox"
                    }
                );
                context.SaveChanges();

                context.RelevantGamesTable.AddRange(
                    new RelevantGames
                    {
                        Title = "Counter Strike: Global Offensive",
                        GameGenre = (from g in context.GameGenreTable
                                     where g.Value == "FPS"
                                     select g).FirstOrDefault()
                    },
                    new RelevantGames
                    {
                        Title = "DOTA",
                        GameGenre = (from g in context.GameGenreTable
                                     where g.Value == "MOBA"
                                     select g).FirstOrDefault()
                    },
                    new RelevantGames
                    {
                        Title = "World of Warcraft",
                        GameGenre = (from g in context.GameGenreTable
                                     where g.Value == "MMORPG"
                                     select g).FirstOrDefault()
                    },
                    new RelevantGames
                    {
                        Title = "The Witcher 3: Wild Hunt",
                        GameGenre = (from g in context.GameGenreTable
                                     where g.Value == "RPG"
                                     select g).FirstOrDefault()
                    },
                    new RelevantGames
                    {
                        Title = "Fortnite",
                        GameGenre = (from g in context.GameGenreTable
                                     where g.Value == "Battle royale"
                                     select g).FirstOrDefault()
                    },
                    new RelevantGames
                    {
                        Title = "Minecraft",
                        GameGenre = (from g in context.GameGenreTable
                                     where g.Value == "Sandbox"
                                     select g).FirstOrDefault()
                    }
                );
                context.SaveChanges();

                var newQuery = from g in context.RelevantGamesTable
                                select g;
                var gamesList = newQuery.ToList();

                var newQuery2 = from g in context.RelevantGamesTable
                                where g.Title == "Minecraft"
                                select g;
                var gamesList2 = newQuery2.ToList();

                context.UserTable.AddRange(
                    new User
                    {
                        Username = "Mike",
                        Password = "Wazowski",
                        Email = "wazowski@gmail.com",
                        CreationDate = DateTime.Now,
                        RelevantGamesList = gamesList
                    },
                    new User
                    {
                        Username = "Darko",
                        Password = "poggers123",
                        Email = "paradzikovic@gmail.com",
                        CreationDate = DateTime.Now,
                        RelevantGamesList = gamesList2
                    }
                );
                context.SaveChanges();

                context.PostTable.AddRange(
                    new Post
                    {
                        Text = "Test post by Mike",
                        TimeStamp = DateTime.Now,
                        User = (from u in context.UserTable
                                where u.Username == "Mike"
                                select u).FirstOrDefault()
                    },
                    new Post
                    {
                        Text = "Test post by Darko",
                        TimeStamp = DateTime.Now,
                        User = (from u in context.UserTable
                                where u.Username == "Darko"
                                select u).FirstOrDefault()
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
