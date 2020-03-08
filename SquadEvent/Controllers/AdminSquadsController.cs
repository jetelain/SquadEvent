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
    public class AdminSquadsController : Controller
    {
        private readonly SquadEventContext _context;

        public AdminSquadsController(SquadEventContext context)
        {
            _context = context;
        }

        // GET: RoundSquads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roundSquad = await _context.RoundSquads
                .Include(r => r.Side).ThenInclude(r => r.MatchSide).ThenInclude(r => r.Match)
                .Include(r => r.Side).ThenInclude(r => r.Round).ThenInclude(r => r.Match)
                .Include(r => r.Side).ThenInclude(r => r.Round).ThenInclude(r => r.GameMap)
                .Include(r => r.Slots)
                .FirstOrDefaultAsync(m => m.RoundSquadID == id);
            if (roundSquad == null)
            {
                return NotFound();
            }

            EnsurePolicy(roundSquad);

            return View(roundSquad);
        }

        // GET: RoundSquads/Create
        public async Task<IActionResult> Create(int roundSideID)
        {
            var vm = new RoundSquadFormViewModel();
            vm.Squad = new RoundSquad()
            {
                RoundSideID = roundSideID,
                Slots = Enumerable.Range(1, 9).Select(num => new RoundSlot() { SlotNumber = num }).ToList()
            };

            vm.Squad.Slots[0].AssignedKit = Kit.SquadLeader;
            vm.Squad.Slots[0].Role = FireTeamRole.TeamLeader;

            await PrepareViewModel(vm);

            return View(vm);
        }

        private static void EnsurePolicy(RoundSquad roundSquad)
        {
            if (roundSquad.Side.MatchSide.SquadsPolicy == SquadsPolicy.SquadsAndSlotsRestricted)
            {
                roundSquad.RestrictTeamComposition = true;
            }
        }

        private async Task PrepareViewModel(RoundSquadFormViewModel vm)
        {
            vm.Squad.Side = await _context.RoundSides
                .Include(r => r.Round).ThenInclude(r => r.Match)
                .Include(r => r.Round).ThenInclude(r => r.GameMap)
                .Include(r => r.MatchSide).ThenInclude(r => r.Match)
                .FirstOrDefaultAsync(r => r.RoundSideID == vm.Squad.RoundSideID);

            EnsurePolicy(vm.Squad);
        }

        // POST: RoundSquads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoundSquadFormViewModel vm)
        {
            await PrepareViewModel(vm);

            if (ModelState.IsValid)
            {
                for (int i = 0; i < vm.Squad.Slots.Count; ++i)
                {
                    var slot = vm.Squad.Slots[i];
                    slot.Squad = vm.Squad;
                }

                await ComputeSquadNumber(vm.Squad);

                vm.Squad.Slots = vm.Squad.Slots.Where(s => s.Role != null).ToList();
                vm.Squad.SlotsCount = vm.Squad.Slots.Count();

                _context.Add(vm.Squad);

                var slotNumber = 1;
                foreach (var slot in vm.Squad.Slots)
                {
                    slot.SlotNumber = slotNumber;
                    _context.Add(slot);
                    slotNumber++;
                }

                await _context.SaveChangesAsync();
                return RedirectToRound(vm.Squad);
            }
            return View(vm);
        }

        private async Task ComputeSquadNumber(RoundSquad squad)
        {
            var others = await _context.RoundSquads.Where(rs => rs.RoundSideID == squad.RoundSideID).ToListAsync();
            if (others.Count == 0)
            {
                squad.Number = 1;
            }
            else
            {
                squad.Number = Enumerable.Range(1, 40).First(num => !others.Any(t => t.Number == num));
            }
            if (string.IsNullOrEmpty(squad.Name))
            {
                squad.Name = $"Squad {squad.Number}";
            }
        }

        // GET: RoundSquads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var roundSquad = await _context.RoundSquads
                .Include(r => r.Side).ThenInclude(r => r.MatchSide).ThenInclude(r => r.Match)
                .Include(r => r.Side).ThenInclude(r => r.Round).ThenInclude(r => r.Match)
                .Include(r => r.Side).ThenInclude(r => r.Round).ThenInclude(r => r.GameMap)
                .Include(r => r.Slots)
                .FirstOrDefaultAsync(r => r.RoundSquadID == id);
            if (roundSquad == null)
            {
                return NotFound();
            }
            var vm = new RoundSquadFormViewModel()
            {
                Squad = roundSquad
            };
            vm.Squad.Slots = vm.Squad.Slots.Concat(Enumerable.Range(vm.Squad.Slots.Count, 9 - vm.Squad.Slots.Count).Select(num => new RoundSlot() { SlotNumber = num })).ToList();
            EnsurePolicy(vm.Squad);
            return View(vm);
        }

        // POST: RoundSquads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoundSquadFormViewModel vm)
        {
            if (id != vm.Squad.RoundSquadID)
            {
                return NotFound();
            }

            await PrepareViewModel(vm);

            if (ModelState.IsValid)
            {
                try
                {
                    var removed = vm.Squad.Slots.Where(s => s.Role == null && s.RoundSlotID != 0).ToList();

                    vm.Squad.Slots = vm.Squad.Slots.Where(s => s.Role != null).ToList();
                    vm.Squad.SlotsCount = vm.Squad.Slots.Count();

                    _context.Update(vm.Squad);

                    var slotNumber = 1;
                    foreach (var slot in vm.Squad.Slots)
                    {

                        slot.SlotNumber = slotNumber;
                        if (slot.RoundSlotID == 0)
                        {
                            _context.Add(slot);
                        }
                        else
                        {
                            // Il y a un risque de concurrence d'accès, on s'assure que si un utilisateur s'est affecté entre temps que ce n'est pas perdu
                            var existing = await _context.RoundSlots.AsNoTracking().FirstOrDefaultAsync(s => s.RoundSlotID == slot.RoundSlotID);
                            slot.AssignedUser = existing.AssignedUser;
                            slot.MatchUserID = existing.MatchUserID;

                            _context.Update(slot);
                        }
                        slotNumber++;
                    }

                    foreach (var slot in removed)
                    {
                        _context.Remove(slot);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoundSquadExists(vm.Squad.RoundSquadID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToRound(vm.Squad);
            }
            return View(vm);
        }

        private IActionResult RedirectToRound(RoundSquad rs)
        {
            return RedirectToAction(nameof(AdminMatchsController.Details), ControllersName.AdminMatchs, new { id = rs.Side.Round.MatchID }, "round-" + rs.Side.RoundID);
        }

        // GET: RoundSquads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roundSquad = await _context.RoundSquads
                .Include(r => r.Side).ThenInclude(r => r.MatchSide).ThenInclude(r => r.Match)
                .Include(r => r.Side).ThenInclude(r => r.Round).ThenInclude(r => r.Match)
                .Include(r => r.Side).ThenInclude(r => r.Round).ThenInclude(r => r.GameMap)
                .FirstOrDefaultAsync(m => m.RoundSquadID == id);
            if (roundSquad == null)
            {
                return NotFound();
            }

            return View(roundSquad);
        }

        // POST: RoundSquads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roundSquad = await _context.RoundSquads
                .Include(r => r.Side).ThenInclude(r => r.Round)
                .FirstOrDefaultAsync(r => r.RoundSquadID == id);
            _context.RoundSquads.Remove(roundSquad);
            await _context.SaveChangesAsync();
            return RedirectToRound(roundSquad);
        }

        private bool RoundSquadExists(int id)
        {
            return _context.RoundSquads.Any(e => e.RoundSquadID == id);
        }
    }
}
