using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SquadEvent.Entities;
using SquadEvent.Models;
using SquadEvent.SquadGameInfos;

namespace SquadEvent.Controllers
{
    public class AdminMatchsController : Controller
    {
        private readonly SquadEventContext _context;

        public AdminMatchsController(SquadEventContext context)
        {
            _context = context;
        }

        // GET: Matches
        public async Task<IActionResult> Index()
        {
            return View(await _context.Matchs.ToListAsync());
        }

        // GET: Matches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var match = await _context.Matchs
                .Include(m => m.Sides)
                .Include(m => m.Rounds).ThenInclude(r => r.GameMap)
                .Include(m => m.Rounds).ThenInclude(r => r.Sides).ThenInclude(s => s.Squads)
                .Include(m => m.Users).ThenInclude(u => u.User)
                .Include(m => m.Users).ThenInclude(u => u.Slots)
                .FirstOrDefaultAsync(m => m.MatchID == id);
            if (match == null)
            {
                return NotFound();
            }
            SortModel(match);
            return View(match);
        }


        public async Task<IActionResult> DuplicateFromPrevious(int roundSideID)
        {
            var roundSide = _context.RoundSides
                .Include(rs => rs.Round)
                .Include(rs => rs.Squads).ThenInclude(s => s.Slots)
                .FirstOrDefault(s => s.RoundSideID == roundSideID);
            if (roundSide == null)
            {
                return NotFound();
            }
            var round = roundSide.Round;

            var previousRound= _context.Rounds.FirstOrDefault(r => r.MatchID == round.MatchID && r.Number == round.Number - 1);
            if (previousRound == null)
            {
                return NotFound();
            }

            var previousRoundSide = _context.RoundSides
                .Include(rs => rs.Round)
                .Include(rs => rs.Squads).ThenInclude(s => s.Slots)
                .FirstOrDefault(s => s.RoundID == previousRound.RoundID && s.MatchSideID == roundSide.MatchSideID);
            if (previousRoundSide == null)
            {
                return NotFound();
            }

            await DuplicateSquadsAndSlots(previousRoundSide, roundSide);

            return RedirectToAction(nameof(Details), ControllersName.AdminMatchs, new { id = round.MatchID }, "round-" + round.RoundID);
        }

        private async Task DuplicateSquadsAndSlots(RoundSide source, RoundSide target)
        {
            _context.RemoveRange(target.Squads);

            target.Squads = source.Squads.Select(s => CuplicateSquadAndSlots(target, s)).ToList();

            _context.AddRange(target.Squads);

            await _context.SaveChangesAsync();
        }

        private static RoundSquad CuplicateSquadAndSlots(RoundSide target, RoundSquad s)
        {
            var copy = new RoundSquad()
            {
                InviteOnly = s.InviteOnly,
                Name = s.Name,
                Number = s.Number,
                RestrictTeamComposition = s.RestrictTeamComposition,
                SlotsCount = s.SlotsCount,
                RoundSideID = target.RoundSideID,
            };
            copy.Slots = s.Slots.Select(p => new RoundSlot()
            {
                AssignedKit = p.AssignedKit,
                Label = p.Label,
                MatchUserID = p.MatchUserID,
                Role = p.Role,
                SlotNumber = p.SlotNumber,
                Squad = copy,
                Timestamp = p.Timestamp
            }).ToList();
            return copy;
        }


        // GET: Matches/Create
        public async Task<IActionResult> Create()
        {
            var vm = new MatchFormViewModel();

            vm.Match = new Match()
            {
                Date = DateTime.Today,
                StartTime = new DateTime(1,1,1,21,0,0),
                Sides = new List<MatchSide>()
                {
                    new MatchSide() { Name = "Equipe A", Number = 1 },
                    new MatchSide() { Name = "Equipe B", Number = 2 }
                },
                Rounds = new List<Round>()
                {
                    new Round()
                    {
                        Number = 1,
                        Sides = new List<RoundSide>()
                        {
                            new RoundSide(),
                            new RoundSide()
                        }
                    }
                }
            };

            await PrepareDrowdownLists(vm);

            return View(vm);
        }

        private async Task PrepareDrowdownLists(MatchFormViewModel vm)
        {
            var maps = await _context.Layouts.Include(m => m.GameMap).Where(m => m.Left != null && m.Right != null).OrderBy(m => m.Name).ToListAsync();
            vm.MapsDropdownList = maps.GroupBy(m => m.GameMap).SelectMany(g => {
                var grp = new SelectListGroup() { Name = g.Key.Name };
                return g.Select(m => new SelectListItem(m.Name, m.GameLayoutID.ToString()) { Group = grp });
            }).ToList();
            vm.MapsDropdownList.Insert(0, new SelectListItem("(Autre / Non précisé)", ""));
            vm.MapsData = maps;
        }

        public async Task<IActionResult> AddRamdomUsers(int id)
        {
            var rnd = new Random();
            var users = (await _context.Users.ToListAsync()).OrderBy(elem => rnd.Next()).Take(80);

            _context.AddRange(users.Select(u => new MatchUser() { UserID = u.UserID, MatchID = id }));
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), ControllersName.AdminMatchs, new { id }, "users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetUserSide(int matchUserId, [FromForm]int matchSideId)
        {
            var matchUser = await _context.MatchUsers.FindAsync(matchUserId);

            matchUser.MatchSideID = matchSideId;

            _context.Update(matchUser);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), ControllersName.AdminMatchs, new { id = matchUser.MatchID }, "unassigned");
        }
        
        // POST: Matches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MatchFormViewModel vm, string addmap, string removemap)
        {
            if (!string.IsNullOrEmpty(addmap))
            {
                vm.Match.Rounds.Add(
                    new Round()
                    {
                        Number = vm.Match.Rounds.Count() + 1,
                        Sides = new List<RoundSide>()
                        {
                            new RoundSide(),
                            new RoundSide()
                        }
                    }
                );
            }
            else if (!string.IsNullOrEmpty(removemap))
            {
                vm.Match.Rounds.RemoveAt(vm.Match.Rounds.Count - 1);
            }
            else if (ModelState.IsValid)
            {
                await ConsolidateMatchForm(vm);

                _context.Add(vm.Match);
                foreach (var side in vm.Match.Sides)
                {
                    _context.Add(side);
                }
                foreach (var round in vm.Match.Rounds)
                {
                    _context.Add(round);
                }
                foreach (var roundSide in vm.Match.Rounds.SelectMany(r => r.Sides))
                {
                    _context.Add(roundSide);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PrepareDrowdownLists(vm);

            return View(vm);
        }

        private async Task ConsolidateMatchForm(MatchFormViewModel vm)
        {
            var maps = await _context.Layouts.Where(m => m.Left != null && m.Right != null).ToListAsync();
            for (var s = 0; s < 2; ++s)
            {
                var side = vm.Match.Sides[s];
                side.Match = vm.Match;
                side.Number = s + 1;
            }
            for (var r = 0; r < vm.Match.Rounds.Count; ++r)
            {
                var round = vm.Match.Rounds[r];
                round.Number = r + 1;
                round.Match = vm.Match;
                var map = maps.FirstOrDefault(m => m.GameLayoutID == round.GameLayoutID);
                for (var s = 0; s < 2; ++s)
                {
                    var roundSide = round.Sides[s];
                    roundSide.Round = round;
                    roundSide.MatchSide = vm.Match.Sides[s];
                    roundSide.GameSide = GetSide(r, s);
                    if (map != null)
                    {
                        roundSide.Faction = roundSide.GameSide == GameSide.Left ? map.Left : map.Right;
                    }
                }
            }
        }

        private GameSide GetSide(int r, int s)
        {
            if (s == r % 2)
            {
                return GameSide.Left;
            }
            return GameSide.Right;
        }

        // GET: Matches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var match = await _context.Matchs
                .Include(m => m.Sides)
                .Include(m => m.Rounds)
                .ThenInclude(r => r.Sides)
                .FirstOrDefaultAsync(m => m.MatchID == id);
            if (match == null)
            {
                return NotFound();
            }
            SortModel(match);
            var vm = new MatchFormViewModel();
            vm.Match = match;
            await PrepareDrowdownLists(vm);
            return View(vm);
        }

        private static void SortModel(Match match)
        {
            match.Sides = match.Sides.OrderBy(s => s.Number).ToList();
            match.Rounds = match.Rounds.OrderBy(s => s.Number).ToList();
            foreach (var r in match.Rounds)
            {
                r.Sides = r.Sides.OrderBy(s => s.MatchSide.Number).ToList();
                foreach (var s in r.Sides)
                {
                    if (s.Squads != null)
                    {
                        s.Squads = s.Squads.OrderBy(a => a.Number).ToList();
                    }
                }
            }

            foreach (var s in match.Sides)
            {
                if (s.Users != null)
                {
                    s.Users = s.Users.OrderBy(s => s.User.Name).ToList();
                }
            }
            if (match.Users != null)
            {
                match.Users = match.Users.OrderBy(s => s.User.Name).ToList();
            }
        }

        // POST: Matches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MatchFormViewModel vm, string addmap, string removemap)
        {
            if (id != vm.Match.MatchID)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(addmap))
            {
                vm.Match.Rounds.Add(
                    new Round()
                    {
                        Number = vm.Match.Rounds.Count() + 1,
                        Sides = new List<RoundSide>()
                        {
                            new RoundSide(),
                            new RoundSide()
                        }
                    }
                );
            }
            else if (!string.IsNullOrEmpty(removemap))
            {
                vm.Match.Rounds.RemoveAt(vm.Match.Rounds.Count - 1);
            }
            else if (ModelState.IsValid)
            {
                await ConsolidateMatchForm(vm);

                try
                {
                    _context.Update(vm.Match);
                    foreach (var side in vm.Match.Sides)
                    {
                        _context.Update(side);
                    }
                    foreach (var round in vm.Match.Rounds)
                    {
                        if (round.RoundID == 0)
                        {
                            _context.Add(round);
                        }
                        else
                        {
                            _context.Update(round);
                        }
                    }
                    foreach (var roundSide in vm.Match.Rounds.SelectMany(r => r.Sides))
                    {
                        if (roundSide.RoundSideID == 0)
                        {
                            _context.Add(roundSide);
                        }
                        else
                        {
                            _context.Update(roundSide);
                        }
                    }

                    var remains = vm.Match.Rounds.Where(r => r.RoundID != 0).Select(r => r.RoundID).ToList();

                    foreach (var removed in _context.Rounds.Where(r => r.MatchID == vm.Match.MatchID && !remains.Contains(r.RoundID)).ToList())
                    {
                        _context.Remove(removed);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatchExists(vm.Match.MatchID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            await PrepareDrowdownLists(vm);

            return View(vm);
        }

        // GET: Matches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var match = await _context.Matchs
                .FirstOrDefaultAsync(m => m.MatchID == id);
            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var match = await _context.Matchs.FindAsync(id);
            _context.Matchs.Remove(match);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatchExists(int id)
        {
            return _context.Matchs.Any(e => e.MatchID == id);
        }
    }
}
