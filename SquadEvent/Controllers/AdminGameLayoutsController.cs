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
    public class AdminGameLayoutsController : Controller
    {
        private readonly SquadEventContext _context;

        public AdminGameLayoutsController(SquadEventContext context)
        {
            _context = context;
        }

        // GET: AdminGameLayouts
        public async Task<IActionResult> Index()
        {
            var squadEventContext = _context.Layouts.Include(g => g.GameMap);
            return View(await squadEventContext.ToListAsync());
        }

        // GET: AdminGameLayouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameLayout = await _context.Layouts
                .Include(g => g.GameMap)
                .FirstOrDefaultAsync(m => m.GameLayoutID == id);
            if (gameLayout == null)
            {
                return NotFound();
            }

            return View(gameLayout);
        }

        // GET: AdminGameLayouts/Create
        public IActionResult Create()
        {
            ViewData["GameMapID"] = new SelectList(_context.Maps, "GameMapID", "Name");
            return View();
        }

        // POST: AdminGameLayouts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GameLayoutID,Name,Left,Right,Image,Thumbnail,MapFull,GameMapID")] GameLayout gameLayout)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gameLayout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameMapID"] = new SelectList(_context.Maps, "GameMapID", "Name", gameLayout.GameMapID);
            return View(gameLayout);
        }

        // GET: AdminGameLayouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameLayout = await _context.Layouts.FindAsync(id);
            if (gameLayout == null)
            {
                return NotFound();
            }
            ViewData["GameMapID"] = new SelectList(_context.Maps, "GameMapID", "Name", gameLayout.GameMapID);
            return View(gameLayout);
        }

        // POST: AdminGameLayouts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GameLayoutID,Name,Left,Right,Image,Thumbnail,MapFull,GameMapID")] GameLayout gameLayout)
        {
            if (id != gameLayout.GameLayoutID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameLayout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameLayoutExists(gameLayout.GameLayoutID))
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
            ViewData["GameMapID"] = new SelectList(_context.Maps, "GameMapID", "Name", gameLayout.GameMapID);
            return View(gameLayout);
        }

        // GET: AdminGameLayouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameLayout = await _context.Layouts
                .Include(g => g.GameMap)
                .FirstOrDefaultAsync(m => m.GameLayoutID == id);
            if (gameLayout == null)
            {
                return NotFound();
            }

            return View(gameLayout);
        }

        // POST: AdminGameLayouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gameLayout = await _context.Layouts.FindAsync(id);
            _context.Layouts.Remove(gameLayout);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameLayoutExists(int id)
        {
            return _context.Layouts.Any(e => e.GameLayoutID == id);
        }
    }
}
