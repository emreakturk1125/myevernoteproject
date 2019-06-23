using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MyEvernote.Entities;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class MyInitiallizer : CreateDatabaseIfNotExists<DatabaseContext>
    {
        //her çalıştırdıgında çalışmayacaktır. veritabanında yoksa çalışacaktır.

        protected override void Seed(DatabaseContext context)
        {
            EvernoteUser admin = new EvernoteUser()
            {
                Name = "Emre",
                Surname = "Aktürk",
                Email = "emre@hotmail.com",
                ActiveGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = true,
                Username = "emreakturk11",
                ProfileImageFileName = "user.jpg",
                Password = "123456",
                CreatedOn = DateTime.Now,
                ModifieOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "emreakturk11"
            };

            EvernoteUser standartUser = new EvernoteUser()
            {
                Name = "Mahmut",
                Surname = "Aktürk",
                Email = "mahmut@hotmail.com",
                ActiveGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false,
                ProfileImageFileName = "user.jpg",
                Username = "mahmut25",
                Password = "123456",
                CreatedOn = DateTime.Now,
                ModifieOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "mahmut25"
            };

            context.EvernoteUsers.Add(admin);
            context.EvernoteUsers.Add(standartUser);


            for (int i = 0; i < 8; i++)
            {
                EvernoteUser user = new EvernoteUser()
                {
                    Name = FakeData.NameData.GetFirstName(),
                    Surname = FakeData.NameData.GetSurname(),
                    Email = FakeData.NetworkData.GetEmail(),
                    ActiveGuid = Guid.NewGuid(),
                    IsActive = true,
                    IsAdmin = false,
                    Username = $"user{i}",
                    ProfileImageFileName = "user.jpg",
                    Password = "123456",
                    CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifieOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedUsername = $"user{i}"
                };
                context.EvernoteUsers.Add(user);
            }

            context.SaveChanges();

            // userlist for 
            List<EvernoteUser> userlist = context.EvernoteUsers.ToList();

            //Adding fake category
            for (int i = 0; i < 10; i++)
            {
                Category cat = new Category()
                {
                    Title = FakeData.PlaceData.GetStreetName(),
                    Description = FakeData.PlaceData.GetAddress(),
                    CreatedOn = DateTime.Now,
                    ModifieOn = DateTime.Now,
                    ModifiedUsername = "emreakturk11"

                };

                context.Categories.Add(cat);

                // Adding fake notes..
                for (int k = 0; k < FakeData.NumberData.GetNumber(5,9); k++)
                {
                    EvernoteUser owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count - 1)];
                    Note note = new Note()
                    {
                        Title = FakeData.TextData.GetAlphabetical(FakeData.NumberData.GetNumber(5, 25)),
                        Text = FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(1, 3)),
                        Category = cat,
                        IsDraft = false,
                        LikeCount = FakeData.NumberData.GetNumber(1, 9),
                        Owner =  userlist[FakeData.NumberData.GetNumber(0,userlist.Count-1)],
                        CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifieOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedUsername = owner.Username,
                    };

                    cat.Notes.Add(note);

                    // adding fake comment

                    for (int j = 0; j < FakeData.NumberData.GetNumber(3,5); j++)
                    {
                        EvernoteUser comment_owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count - 1)];

                        Comment comment = new Comment()
                        {
                            Text = FakeData.TextData.GetSentence(), 
                            Owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count - 1)],
                            CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifieOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedUsername = comment_owner.Username,

                        };

                        note.Comments.Add(comment);
                    }
                    //Adding fake likes
                   
                    for (int m = 0; m < note.LikeCount; m++)
                    {
                        Liked liked = new Liked()
                        {
                            LikedUser = userlist[m]
                        };
                        note.Likes.Add(liked);
                    }
                  
                }
            }
            context.SaveChanges();
        }
    }
}
