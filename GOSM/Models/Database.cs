using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GOSM.Models
{
    public class Database : DbContext
    {
        public Database(DbContextOptions<Database> options)
            : base(options)
        {

        }

        public DbSet<Post> PostTable { get; set; }
        public DbSet<User> UserTable { get; set; }
        public DbSet<Comment> CommentTable { get; set; }
        public DbSet<RelevantGames> RelevantGamesTable { get; set; }
        public DbSet<FriendRequest> FriendRequestTable { get; set; }
    }
}
