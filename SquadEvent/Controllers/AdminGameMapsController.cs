using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SquadEvent.Entities;

namespace SquadEvent.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminGameMapsController : Controller
    {
        private readonly SquadEventContext _context;

        public AdminGameMapsController(SquadEventContext context)
        {
            _context = context;
        }

        // GET: AdminGameMaps
        public async Task<IActionResult> Index()
        {
            return View(await _context.Maps.ToListAsync());
        }

        // GET: AdminGameMaps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameMap = await _context.Maps
                .FirstOrDefaultAsync(m => m.GameMapID == id);
            if (gameMap == null)
            {
                return NotFound();
            }

            return View(gameMap);
        }

        // GET: AdminGameMaps/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminGameMaps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GameMapID,Name,Image,Region")] GameMap gameMap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gameMap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gameMap);
        }

        // GET: AdminGameMaps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameMap = await _context.Maps.FindAsync(id);
            if (gameMap == null)
            {
                return NotFound();
            }
            return View(gameMap);
        }

        // POST: AdminGameMaps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GameMapID,Name,Image,Region")] GameMap gameMap)
        {
            if (id != gameMap.GameMapID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameMap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameMapExists(gameMap.GameMapID))
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
            return View(gameMap);
        }

        // GET: AdminGameMaps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameMap = await _context.Maps
                .FirstOrDefaultAsync(m => m.GameMapID == id);
            if (gameMap == null)
            {
                return NotFound();
            }

            return View(gameMap);
        }

        // POST: AdminGameMaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gameMap = await _context.Maps.FindAsync(id);
            _context.Maps.Remove(gameMap);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameMapExists(int id)
        {
            return _context.Maps.Any(e => e.GameMapID == id);
        }
    }
}
