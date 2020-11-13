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

                var query = from g in context.GameGenreTable
                            orderby g.ID descending
                            select g;
                var genreList = query.ToList();

                context.RelevantGamesTable.AddRange(
                    new RelevantGames
                    {
                        Title = "Counter Strike: Global Offensive",
                        GameGenre = genreList[0]
                    },
                    new RelevantGames
                    {
                        Title = "DOTA",
                        GameGenre = genreList[1]
                    },
                    new RelevantGames
                    {
                        Title = "World of Warcraft",
                        GameGenre = genreList[2]
                    },
                    new RelevantGames
                    {
                        Title = "The Witcher 3: Wild Hunt",
                        GameGenre = genreList[3]
                    },
                    new RelevantGames
                    {
                        Title = "Fortnite",
                        GameGenre = genreList[4]
                    },
                    new RelevantGames
                    {
                        Title = "Minecraft",
                        GameGenre = genreList[5]
                    }
                );
                context.SaveChanges();

                var newQuery = from g in context.RelevantGamesTable
                                select g;
                var gamesList = newQuery.ToList();

                context.UserTable.AddRange(
                    new User
                    {
                        Username = "Mike",
                        Password = "Wazowski",
                        Email = "wazowski@gmail.com",
                        CreationDate = DateTime.Now,
                        RelevantGamesList = gamesList
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
