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
                foreach (var comment in context.CommentTable)
                {
                    context.CommentTable.Remove(comment);
                }
                foreach (var post in context.PostTable)
                {
                    context.PostTable.Remove(post);
                }
                foreach(var game in context.RelevantGamesTable)
                {
                    context.RelevantGamesTable.Remove(game);
                }
                foreach(var friend in context.FriendRequestTable)
                {
                    context.FriendRequestTable.Remove(friend);
                }
                context.SaveChanges();

                context.RelevantGamesTable.AddRange(
                    new RelevantGames
                    {
                        Title = "Counter Strike: Global Offensive",
                        Genre = GameGenre.FPS
                    },
                    new RelevantGames
                    {
                        Title = "DOTA",
                        Genre = GameGenre.MOBA
                    },
                    new RelevantGames
                    {
                        Title = "World of Warcraft",
                        Genre = GameGenre.MMORPG
                    },
                    new RelevantGames
                    {
                        Title = "The Witcher 3: Wild Hunt",
                        Genre = GameGenre.RPG
                    },
                    new RelevantGames
                    {
                        Title = "Fortnite",
                        Genre = GameGenre.Other
                    },
                    new RelevantGames
                    {
                        Title = "Minecraft",
                        Genre = GameGenre.Sandbox
                    }
                );
                context.SaveChanges();

                context.UserTable.AddRange(
                    new User
                    {
                        Username = "Mike",
                        Password = "Wazowski",
                        Email = "wazowski@gmail.com",
                        CreationDate = DateTime.Now,
                    },
                    new User
                    {
                        Username = "Darko",
                        Password = "poggers123",
                        Email = "paradzikovic@gmail.com",
                        CreationDate = DateTime.Now
                    }
                );
                context.SaveChanges();

                context.UserRelevantGamesTable.AddRange(
                    new UserRelevantGames
                    {
                        User = (from u in context.UserTable
                                where u.Username == "Darko"
                                select u).FirstOrDefault(),

                        RelevantGames = (from g in context.RelevantGamesTable
                                         where g.Title == "Minecraft"
                                         select g).FirstOrDefault()
                    },
                    new UserRelevantGames
                    {
                        User = (from u in context.UserTable
                                where u.Username == "Darko"
                                select u).FirstOrDefault(),

                        RelevantGames = (from g in context.RelevantGamesTable
                                         where g.Title == "World of Warcraft"
                                         select g).FirstOrDefault()
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

                context.CommentTable.AddRange(
                    new Comment
                    {
                        Text = "This is comment number one",

                        TimeStamp = DateTime.Now,

                        User = (from u in context.UserTable
                                where u.Username == "Darko"
                                select u).FirstOrDefault(),

                        PostID = (from p in context.PostTable
                                select p.ID).FirstOrDefault()
                    },
                    new Comment
                    {
                        Text = "This is comment number two",

                        TimeStamp = DateTime.Now,

                        User = (from u in context.UserTable
                                where u.Username == "Mike"
                                select u).FirstOrDefault(),

                        PostID = (from p in context.PostTable
                                select p.ID).FirstOrDefault()
                    },
                    new Comment
                    {
                        Text = "This is comment number three",
                        TimeStamp = DateTime.Now,
                        User = (from u in context.UserTable
                                where u.Username == "Darko"
                                select u).FirstOrDefault(),

                        PostID = (from p in context.PostTable
                                select p.ID).FirstOrDefault()
                    }

                );
                context.SaveChanges();
            }
        }
    }
}
