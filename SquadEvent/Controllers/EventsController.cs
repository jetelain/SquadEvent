using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SquadEvent.Entities;
using SquadEvent.Models;
using SquadEvent.SquadGameInfos;

namespace SquadEvent.Controllers
{
    public class EventsController : Controller
    {
        private readonly SquadEventContext _context;

        public EventsController(SquadEventContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Matchs.Include(m => m.Rounds).ToListAsync());
        }

        [Authorize(Policy = "SteamID")]
        [HttpGet]
        public async Task<IActionResult> Subscription(int? id, int? matchSideID, int? roundSquadID)
        {
            if (id == null)
            {
                return NotFound();
            }
            var match = await _context.Matchs
                .Include(m => m.Sides)
                .Include(m => m.Users).ThenInclude(u => u.User)
                .Include(m => m.Rounds).ThenInclude(r => r.GameMap)
                .Include(m => m.Rounds).ThenInclude(r => r.Sides).ThenInclude(s => s.Squads).ThenInclude(s => s.Slots).ThenInclude(s => s.AssignedUser).ThenInclude(u => u.User)
                .FirstOrDefaultAsync(m => m.MatchID == id);
            if (match == null)
            {
                return NotFound();
            }

            var user = await GetUser();
            if (user == null)
            {
                return View("SubscriptionInitial", new SubscriptionInitialViewModel()
                {
                    Match = match,
                    MatchSideID = matchSideID,
                    RoundSquadID = roundSquadID,
                    User = new User() { SteamName = User.Identity.Name }
                });
            }
            var matchUser = match.Users.FirstOrDefault(u => u.UserID == user.UserID);
            if (matchUser == null)
            {
                return View("SubscriptionInitial", new SubscriptionInitialViewModel()
                {
                    Match = match,
                    MatchSideID = matchSideID,
                    RoundSquadID = roundSquadID,
                    User = user
                });
            }
            var vm = new SubscriptionViewModel();
            vm.Match = match;
            vm.User = user;
            vm.MatchUser = matchUser;
            return View("Subscription", vm);
        }

        private async Task<User> GetUser()
        {
            var steamId = SteamHelper.GetSteamId(User);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.SteamId == steamId);
            return user;
        }

        //
        [Authorize(Policy = "SteamID")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubscriptionSide(int id, int matchSideID)
        {
            var user = await GetUser();
            if (user == null)
            {
                return NotFound();
            }

            var matchUser = await _context.MatchUsers.FirstOrDefaultAsync(u => u.MatchID == id && u.UserID == user.UserID);
            if (matchUser == null || matchUser.MatchSideID != null )
            {
                return NotFound();
            }

            matchUser.MatchSideID = await _context.MatchSides.Where(s => s.MatchSideID == matchSideID && s.MatchID == id).Select(s => s.MatchSideID).FirstOrDefaultAsync();
            if (matchUser.MatchSideID != null)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    if (await CanJoin(matchUser))
                    {
                        _context.Update(matchUser);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                }
            }
            return RedirectToAction(nameof(Subscription), new { id });
        }

        private async Task<bool> CanJoin(MatchUser user)
        {
            var max = await _context.MatchSides.Where(s => s.MatchSideID == user.MatchSideID).Select(s => s.MaxUsersCount).FirstOrDefaultAsync();
            var count = await _context.MatchUsers.Where(s => s.MatchSideID == user.MatchSideID && s.MatchUserID != user.MatchUserID).CountAsync();
            return count < max;
        }

        [Authorize(Policy = "SteamID")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubscriptionInitial(int id, SubscriptionInitialViewModel vm)
        {
            var match = await _context.Matchs.Include(m => m.Sides).FirstOrDefaultAsync(m => m.MatchID == id);
            if (match == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                vm.Match = match;
                return View("SubscriptionInitial", vm);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var user = await GetUser();
                if (user == null)
                {
                    vm.User.SteamId = SteamHelper.GetSteamId(User);
                    vm.User.SteamName = User.Identity.Name;
                    vm.User.Name = vm.User.NamePrefix + User.Identity.Name;
                    vm.User.UserID = 0;
                    _context.Add(vm.User);
                    await _context.SaveChangesAsync();
                    user = vm.User;
                }

                var matchUser = await _context.MatchUsers.FirstOrDefaultAsync(u => u.MatchID == id && u.UserID == user.UserID);
                if (matchUser == null)
                {
                    if (vm.RoundSquadID != null)
                    {
                        // Vérifie que RoundSquadID appartient bien à MatchID
                        vm.RoundSquadID = await _context.RoundSquads.Where(s => s.RoundSquadID == vm.RoundSquadID && s.Side.MatchSide.MatchID == id).Select(s => s.RoundSquadID).FirstOrDefaultAsync();
                        // Calcule MatchSideID
                        vm.MatchSideID = await _context.RoundSquads.Where(s => s.RoundSquadID == vm.RoundSquadID && s.Side.MatchSide.MatchID == id).Select(s => s.Side.MatchSideID).FirstOrDefaultAsync();
                    }
                    else if (vm.MatchSideID != null)
                    {
                        // Vérifie que MatchSideID appartient bien à MatchID
                        vm.MatchSideID = await _context.MatchSides.Where(s => s.MatchSideID == vm.MatchSideID && s.MatchID == id).Select(s => s.MatchSideID).FirstOrDefaultAsync();
                    }
                    matchUser = new MatchUser() { MatchID = id, UserID = vm.User.UserID, MatchSideID = vm.MatchSideID };
                    if (matchUser.MatchSideID == null || await CanJoin(matchUser))
                    {
                        _context.Add(matchUser);
                        await _context.SaveChangesAsync();
                    }
                }
                await transaction.CommitAsync();
            }
            return RedirectToAction(nameof(Subscription), new { id });
        }

        [Authorize(Policy = "SteamID")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetSide(int id)
        {
            var user = await GetUser();
            if (user == null)
            {
                return NotFound();
            }

            var matchUser = await _context.MatchUsers.FirstOrDefaultAsync(u => u.MatchID == id && u.UserID == user.UserID);
            if (matchUser == null || matchUser.MatchSideID == null)
            {
                return NotFound();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                matchUser.MatchSideID = null;
                _context.Update(matchUser);

                var olderSlots = await _context.RoundSlots.Where(s => s.MatchUserID == matchUser.MatchUserID).ToListAsync();
                foreach (var olderSlot in olderSlots)
                {
                    olderSlot.MatchUserID = null;
                    _context.Update(olderSlot);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }

            return RedirectToAction(nameof(Subscription), new { id });
        }

        [Authorize(Policy = "SteamID")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubscriptionSlot(int id, int? roundSquadID, int? roundSlotID, int? roundSideID, string squadName)
        {
            var user = await GetUser();
            if (user == null)
            {
                return NotFound();
            }

            var matchUser = await _context.MatchUsers.FirstOrDefaultAsync(u => u.MatchID == id && u.UserID == user.UserID);
            if (matchUser == null || matchUser.MatchSideID == null)
            {
                return NotFound();
            }


            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                RoundSlot roundSlot = null;

                if (roundSlotID != null)
                {
                    roundSlot = await _context.RoundSlots.Include(s => s.Squad).ThenInclude(s => s.Side).FirstOrDefaultAsync(s => s.Squad.Side.Round.MatchID == id && s.Squad.Side.MatchSideID == matchUser.MatchSideID && s.RoundSlotID == roundSlotID);
                    if (roundSlot == null || roundSlot.MatchUserID != null)
                    {
                        return RedirectToAction(nameof(Subscription), new { id });
                    }
                }
                else if (roundSquadID != null)
                {
                    var roundSquad = await _context.RoundSquads.Include(s => s.Side).FirstOrDefaultAsync(s => s.Side.Round.MatchID == id && s.Side.MatchSideID == matchUser.MatchSideID && s.RoundSquadID == roundSquadID);
                    if (roundSquad == null || roundSquad.SlotsCount >= 9 || roundSquad.RestrictTeamComposition)
                    {
                        return RedirectToAction(nameof(Subscription), new { id });
                    }
                    roundSlot = new RoundSlot() { Squad = roundSquad, SlotNumber = roundSquad.SlotsCount, Role = FireTeamRole.Alpha };
                    roundSlot.SetTimestamp();
                    roundSquad.SlotsCount++;
                    _context.Add(roundSlot);
                    _context.Update(roundSquad);
                    await _context.SaveChangesAsync();
                }

                await AssignSlot(matchUser, roundSlot);

                await transaction.CommitAsync();
            }
            

            return RedirectToAction(nameof(Subscription), new { id });
        }

        private async Task AssignSlot(MatchUser matchUser, RoundSlot roundSlot)
        {
            var olderSlots = await _context.RoundSlots.Where(s => s.Squad.Side.RoundID == roundSlot.Squad.Side.RoundID && s.MatchUserID == matchUser.MatchUserID).ToListAsync();
            foreach (var olderSlot in olderSlots)
            {
                olderSlot.MatchUserID = null;
                _context.Update(olderSlot);
            }

            roundSlot.MatchUserID = matchUser.MatchUserID;
            _context.Update(roundSlot);
            await _context.SaveChangesAsync();
        }

        [Authorize(Policy = "SteamID")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveSlot(int id, int roundSlotID)
        {
            var user = await GetUser();
            if (user == null)
            {
                return NotFound();
            }

            var matchUser = await _context.MatchUsers.FirstOrDefaultAsync(u => u.MatchID == id && u.UserID == user.UserID);
            if (matchUser == null || matchUser.MatchSideID == null)
            {
                return NotFound();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var roundSlot = await _context.RoundSlots.FirstOrDefaultAsync(s => s.RoundSlotID == roundSlotID);
                if (roundSlot.MatchUserID == matchUser.MatchUserID)
                {
                    roundSlot.MatchUserID = null;
                    _context.Update(roundSlot);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }

            return RedirectToAction(nameof(Subscription), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var match = await _context.Matchs
                .Include(m => m.Sides)
                .Include(m => m.Users).ThenInclude(u => u.User)
                .Include(m => m.Rounds).ThenInclude(r => r.GameMap)
                .Include(m => m.Rounds).ThenInclude(r => r.Sides).ThenInclude(s => s.Squads).ThenInclude(s => s.Slots).ThenInclude(s => s.AssignedUser).ThenInclude(u => u.User)
                .FirstOrDefaultAsync(m => m.MatchID == id);
            if (match == null)
            {
                return NotFound();
            }
            var vm = new EventDetailsViewModel();
            vm.Match = match;
            vm.User = await GetUser(); 
            if (vm.User != null)
            {
                vm.MatchUser = match.Users.FirstOrDefault(u => u.UserID == vm.User.UserID);
            }
            AdminMatchsController.SortModel(match);
            return View(vm);
        }

    }
}