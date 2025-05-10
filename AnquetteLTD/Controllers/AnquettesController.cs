using AnquetteLTD.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnquetteLTD.Data;
using AnquetteLTD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Humanizer;
using System.Security.Claims;

namespace AnquetteLTD.Controllers
{
    public class AnquettesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AnquettesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Anquettes
        public async Task<IActionResult> Index()
        {
            var polls = await _context.Anquettes
                .Include(q => q.User)
                .Include(q => q.Answers)
                .Select(q => new AnquetteListItemViewModel
                {
                    Id = q.Id,
                    Description = q.Description,
                    AuthorUserName = q.User!.UserName ?? q.User!.Email,
                    AnswersCount = q.Answers.Count,
                    IsMultipleChoice = q.IsMultipleChoice,
                    AllowAnonymous = q.AllowAnonymous,
                    IsEnabled = q.IsEnabled,
                    StartsAt = q.StartsAt,
                    EndsAt = q.EndsAt,
                    IsOpen = (
                        (q.StartsAt == null && q.EndsAt == null)
                            ? q.IsEnabled
                            : (DateTimeOffset.UtcNow >= (q.StartsAt ?? DateTimeOffset.MinValue) &&
                               DateTimeOffset.UtcNow <= (q.EndsAt ?? DateTimeOffset.MaxValue))
                        )
                })
                .ToListAsync();

            return View(polls);
        }

        // GET: Anquettes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anquette = await _context.Anquettes
                .Include(a => a.User)
                .Include(a => a.Answers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (anquette == null)
            {
                return NotFound();
            }

            return View(anquette);
        }

        // Only authenticated users may manage polls
        [Authorize]
        // GET: Anquettes/Create
        public IActionResult Create() => View(new AnquetteCreateViewModel());

        // POST: Anquettes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnquetteCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var userId = _userManager.GetUserId(User)!;

            var anquette = new Anquette
            {
                Id = Guid.NewGuid(),
                Description = vm.Description,
                UserId = userId,
                IsMultipleChoice = vm.IsMultipleChoice,
                AllowAnonymous = vm.AllowAnonymous,
            };
            anquette.IsEnabled = vm.IsEnabled;
            anquette.StartsAt = vm.StartsAt?.ToUniversalTime();
            anquette.EndsAt = vm.EndsAt?.ToUniversalTime();
            // create Answer entities for every non-empty value
            foreach (var text in vm.Answers.Where(a => !string.IsNullOrWhiteSpace(a)))
                anquette.Answers.Add(new Answer { Value = text.Trim() });

            _context.Anquettes.Add(anquette);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Anquettes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anquette = await _context.Anquettes
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);

            if (anquette == null) return NotFound();

            AnquetteEditViewModel vm = new AnquetteEditViewModel
            {
                Id = anquette.Id,
                Description = anquette.Description,
                IsMultipleChoice = anquette.IsMultipleChoice,
                AllowAnonymous = anquette.AllowAnonymous,
                IsEnabled = anquette.IsEnabled,
                StartsAt = anquette.StartsAt?.ToLocalTime(),
                EndsAt = anquette.EndsAt?.ToLocalTime(),
                Answers = anquette.Answers
                    .OrderBy(a => a.Id)
                    .Select(a => new AnswerItemViewModel
                    {
                        Id = a.Id,
                        Value = a.Value
                    }).ToList()
            };

            // ensure at least one empty row if no answers exist
            if (vm.Answers.Count == 0) vm.Answers.Add(new AnswerItemViewModel());

            return View(vm);
        }

        // POST: Anquettes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AnquetteEditViewModel vm)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var anquette = await _context.Anquettes
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id &&
                                          q.UserId == userId);

            if (anquette == null) return Forbid();

            if (!ModelState.IsValid) return View(vm);

            anquette.Description = vm.Description.Trim();
            anquette.IsMultipleChoice = vm.IsMultipleChoice;
            anquette.IsEnabled = vm.IsEnabled;
            anquette.StartsAt = vm.StartsAt?.ToUniversalTime();
            anquette.EndsAt = vm.EndsAt?.ToUniversalTime();
            var posted = vm.Answers ?? new();

            foreach (var dbAnswer in anquette.Answers.ToList())
            {
                var dto = posted.FirstOrDefault(a => a.Id == dbAnswer.Id);
                if (dto == null || dto.IsDeleted || string.IsNullOrWhiteSpace(dto.Value))
                {
                    _context.Answers.Remove(dbAnswer);
                    continue;
                }

                if (dbAnswer.Value != dto.Value.Trim())
                    dbAnswer.Value = dto.Value.Trim();
            }

            foreach (var dto in posted.Where(a => a is { Id: 0, IsDeleted: false }))
            {
                if (!string.IsNullOrWhiteSpace(dto.Value))
                    anquette.Answers.Add(new Answer { Value = dto.Value.Trim() });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Anquettes/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var poll = await _context.Anquettes
                .Include(q => q.Answers)
                .Include(q => q.Submissions)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (poll == null) return NotFound();
            if (poll.UserId != userId) return Forbid();

            return View(poll);
        }

        // POST: Anquettes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var poll = await _context.Anquettes
                .Where(q => q.Id == id && q.UserId == userId)
                .FirstOrDefaultAsync();

            if (poll == null) return Forbid();

            _context.Anquettes.Remove(poll);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Anquettes/My
        [Authorize]
        public async Task<IActionResult> My()
        {
            var userId = _userManager.GetUserId(User)!;             // current user

            var myPolls = await _context.Anquettes
                                                           .Where(q => q.UserId == userId)
                                       .Include(q => q.Answers)     // eager-load answers
                                       .ToListAsync();

            return View("My", myPolls);     // will render Views/Anquettes/My.cshtml
        }

        // GET: /Anquettes/Fill/5
        [HttpGet]
        public async Task<IActionResult> Fill(Guid id)
        {
            var poll = await _context.Anquettes
                                     .Include(q => q.Answers)
                                     .FirstOrDefaultAsync(q => q.Id == id);
            if (poll is null) return NotFound();

            /* 1️⃣  Access check */
            if (!User.Identity!.IsAuthenticated && !poll.AllowAnonymous)
                return Challenge();                         // ⇢ sign-in page

            /* 2️⃣  Is the poll open? */
            bool isOpen = IsPollOpen(poll);

            var vm = new AnquetteVoteViewModel
            {
                AnquetteId = poll.Id,
                Description = poll.Description,
                IsMultipleChoice = poll.IsMultipleChoice,
                IsOpen = isOpen,
                Options = poll.Answers
                                        .Select(a => new AnswerOptionVm(a.Id, a.Value))
                                        .ToList()
            };

            return View("Fill", vm);                        // Views/Anquettes/Fill.cshtml
        }

        // POST: /Anquettes/Fill/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Fill(AnquetteVoteViewModel vm)
        {
            var poll = await _context.Anquettes
                                     .Include(q => q.Answers)
                                     .FirstOrDefaultAsync(q => q.Id == vm.AnquetteId);
            if (poll is null) return NotFound();

            if (!User.Identity!.IsAuthenticated && !poll.AllowAnonymous)
                return Forbid();

            if (!IsPollOpen(poll))
            {
                ModelState.AddModelError(string.Empty, "This poll is currently closed.");
                vm.IsOpen = false;
                vm.Options = poll.Answers.Select(a => new AnswerOptionVm(a.Id, a.Value)).ToList();
                return View("Fill", vm);
            }
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = userIdStr != null ? Guid.Parse(userIdStr) : null;

            /* if already voted -> delete old vote */
            if (userId is { } nonNull)
            {
                var previous = await _context.Submissions
                    .Where(s => s.AnquetteId == poll.Id && s.UserId == nonNull)
                    .Include(s => s.SelectedAnswers)
                    .FirstOrDefaultAsync();
                if (previous != null)
                    _context.Submissions.Remove(previous);              // cascade deletes children
            }
            var submission = new AnquetteSubmission
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) is { } uid ? Guid.Parse(uid) : null,
                AnquetteId = poll.Id
            };

            var distinctIds = poll.IsMultipleChoice
                              ? vm.SelectedAnswerIds.Distinct()
                              : vm.SelectedAnswerIds.Take(1);

            foreach (var aid in distinctIds)
                if (poll.Answers.Any(a => a.Id == aid))
                    submission.SelectedAnswers.Add(new SubmissionAnswer { AnswerId = aid });

            if (submission.SelectedAnswers.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Please pick at least one answer.");
                vm.IsOpen = true;
                vm.Options = poll.Answers.Select(a => new AnswerOptionVm(a.Id, a.Value)).ToList();
                return View("Fill", vm);
            }

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = poll.Id });
        }

        // GET: /Anquettes/Results/5
        public async Task<IActionResult> Results(Guid id)
        {
            var poll = await _context.Anquettes
                                     .Include(q => q.Answers)
                                     .Include(q => q.Submissions)
                                        .ThenInclude(s => s.SelectedAnswers)
                                     .FirstOrDefaultAsync(q => q.Id == id);

            if (poll == null) return NotFound();

            var counts = poll.Answers
                             .GroupJoin(poll.Submissions.SelectMany(s => s.SelectedAnswers),
                                        a => a.Id,
                                        sa => sa.AnswerId,
                                        (a, grp) => new { a.Value, Count = grp.Count() })
                             .OrderByDescending(x => x.Count)
                             .ToList();

            var voterGroups = await _context.Submissions
                .Where(s => s.AnquetteId == id)
                .GroupBy(s => s.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Votes = g.Count()
                })
                .ToListAsync();

            var userIdStrings = voterGroups
                .Where(v => v.UserId != null)
                .Select(v => v.UserId!.Value.ToString())
                .ToList();

            var userMap = await _context.Users
                .Where(u => userIdStrings.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Email);

            var voterVm = voterGroups.Select(v => new VoterGroupVm
            {
                Display = v.UserId == null
                        ? "Guest"
                        : userMap.TryGetValue(v.UserId.Value.ToString(), out var name) ? name! : "User",
                Votes = v.Votes
            })
                .OrderByDescending(v => v.Votes)
                .ToList();

            var vm = new AnquetteResultsViewModel
            {
                AnquetteId = poll.Id,
                Description = poll.Description,
                Labels = counts.Select(c => c.Value).ToList(),
                Counts = counts.Select(c => c.Count).ToList(),
                Voters = voterVm
            };

            return View("Results", vm);               // Views/Anquettes/Results.cshtml
        }


        private static bool IsPollOpen(Anquette q)
        {
            var now = DateTimeOffset.UtcNow;

            if (q.StartsAt is null && q.EndsAt is null)
                return q.IsEnabled;                                        // no window ⇒ flag decides

            if (q.StartsAt is { } start && now < start) return false;
            if (q.EndsAt is { } end && now > end) return false;

            return true;
        }

        private bool AnquetteExists(Guid id)
        {
            return _context.Anquettes.Any(e => e.Id == id);
        }
    }
}
