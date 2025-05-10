using System.Security.Claims;
using AnquetteLTD.Controllers;
using AnquetteLTD.Data;
using AnquetteLTD.Models;
using AnquetteLTD.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AnquetteLTD.Tests;
public class FillActionTests
{
    [Fact]
    public async Task SecondVote_FromSameUser_OverwritesFirst()
    {
        // arrange
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase("vote-overwrite"));
        var sp = services.BuildServiceProvider();
        var db = sp.GetRequiredService<ApplicationDbContext>();

        // act
        var fakeAuthor = Guid.NewGuid().ToString();
        var poll = new Anquette
        {
            Id = Guid.NewGuid(),
            Description = "Unit-test poll",
            UserId = fakeAuthor,
            IsEnabled = true
        }
        ; poll.Answers.Add(new Answer { Id = 1, Value = "A" });
        poll.Answers.Add(new Answer { Id = 2, Value = "B" });
        db.Anquettes.Add(poll);
        await db.SaveChangesAsync();

        // controller with fake user
        var controller = new AnquettesController(db, null!)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) }, "mock"))
                }
            }
        };

        var vm = new AnquetteVoteViewModel
        {
            AnquetteId = poll.Id,
            SelectedAnswerIds = { 1 }
        };

        // first vote
        await controller.Fill(vm);

        // second vote (choose other answer)
        vm.SelectedAnswerIds.Clear();
        vm.SelectedAnswerIds.Add(2);
        await controller.Fill(vm);

        // assert
        var subs = await db.Submissions.Where(s => s.AnquetteId == poll.Id).Include(s => s.SelectedAnswers).ToListAsync();
        Assert.Single(subs);                         // only one submission
        Assert.Single(subs[0].SelectedAnswers);      // one one answer
        Assert.Equal(2, subs[0].SelectedAnswers.First().AnswerId);  // latest choice
    }
}
