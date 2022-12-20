using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectNokia.Data;
using ProjectNokia.Models;
using RestSharp;

namespace ProjectNokia.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
              return View(await _context.Tickets.ToListAsync());
        }

        // GET: Tickets/Details/5
        // aici fac query la tabela de commenturi in momentul in care se face click pe detalii
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,LimitDate,Priority,Reporter,Assigne")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                string? name = HttpContext.User.Identity.Name;
                ticket.Reporter = name;
                ticket.CreatedDate = DateTime.Now;
                ticket.State = "To Do";

                //assign ticket to userid
                //first check the name of the assigne to 
                var _db = new ApplicationDbContext();

                //IQueryable<AppUser> query = _db.AppUsers.AsQueryable(); // from IdentityDbContext
                //if (ticket.Reporter == ticket.Assigne)
                //{
                //    var query =
                //        from AppUser in _db.Users
                //        where AppUser.UserName == name
                //        select AppUser;
                //    foreach (var item in query)
                //    {
                //        ticket.AppUserId = item.Id;
                //    }
                //}
                //else
                //{
                //    var querySecond =
                //    from AppUser in _db.Users
                //    where AppUser.UserName == ticket.Reporter
                //    select AppUser;
                //    foreach (var item in querySecond)
                //    {
                //        ticket.AppUserId = item.Id;
                //    }
                //}
                // search for matching user

                var query =
                    from AppUser in _db.Users
                    select AppUser;
                foreach(var user in query)
                {
                    if(ticket.Assigne == user.UserName.ToString())
                    {
                        ticket.AppUserId = user.Id;
                        /*var to = new { email = ticket.Assigne };
                        var from = new { email = "vlad_andrey1@yahoo.com", name = "Mailtrap Test" };
                        var args = new
                        {
                            from = from,
                            to = new[] { to },
                            subject = "You are awesome",
                            text = "Congrats for sending a test email with Mailtrap!",
                            category = "Integration Test"
                        };

                        var client = new RestClient("https://send.api.mailtrap.io/api");
                        var request = new RestRequest("/send", RestSharp.Method.Post);

                        request.AddHeader("Authorization", "Bearer YOUR_TOKEN");
                        request.AddHeader("Content-Type", "application/json");
                        request.AddParameter("application/json",
                        JsonSerializer.Serialize(args), ParameterType.RequestBody);

                        RestResponse response = client.Execute(request);*/
                       /* var smtpClient = new SmtpClient("smtp.gmail.com")
                        {
                            Port = 587,
                            Credentials = new NetworkCredential("username", "password"),
                            EnableSsl = true,
                        };

                        smtpClient.Send("email", "recipient", "subject", "body");*/
                        break;
                    }
                }

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,CreatedDate,LimitDate,State,Description,Priority,Reporter,Assigne")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
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
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tickets == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tickets'  is null.");
            }
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
          return _context.Tickets.Any(e => e.Id == id);
        }
    }
}
