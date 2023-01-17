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
using System.Web;
using PusherServer;

namespace ProjectNokia.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        //{
            
        //    var tickets = _context.Tickets.AsQueryable();
        //    List<string> list = new List<string>();
        //    foreach(var ticket in tickets)
        //    {
        [Authorize]
        public async Task<IActionResult> Index(string? SearchString,string? SelectAssigne,DateTime? startdate,DateTime? enddate,string SelectStatus,string SelectPriority,int pg=1)
        //    }

            var tickets = from t in _context.Tickets
                          select t;
            if (!String.IsNullOrEmpty(SearchString))
            {
                tickets = tickets.Where(t=>t.Title.Contains(SearchString) || t.Description.Contains(SearchString));
            }
            if (startdate != null && enddate == null)
            {
                tickets = tickets.Where(t => DateTime.Compare((DateTime)startdate,t.CreatedDate) <= 0 );
            }
            else if (startdate != null && enddate != null)
            {
                tickets = tickets.Where(t => DateTime.Compare((DateTime)startdate, t.CreatedDate) <= 0 && DateTime.Compare(t.LimitDate, (DateTime)enddate) <= 0);
            }
            else if (startdate == null && enddate != null)
            {
                tickets = tickets.Where(t => DateTime.Compare(t.LimitDate, (DateTime)enddate) <= 0  && DateTime.Compare(t.CreatedDate, DateTime.Now) <= 0);
            }
            if(!String.IsNullOrEmpty(SelectStatus))
            {
                tickets = tickets.Where(t => t.State == SelectStatus);
            }
            if (!String.IsNullOrEmpty(SelectPriority))
            {
                tickets = tickets.Where(t => t.Priority == SelectPriority);
            }
            if (!String.IsNullOrEmpty(SelectAssigne))
            {
                tickets = tickets.Where(t => t.Assigne == SelectAssigne);
            }
            }
            }
            }
            }
            }
            }
            {
                tickets = tickets.Where(t => t.State == SelectStatus);
            }
            if (!String.IsNullOrEmpty(SelectPriority))
            {
                tickets = tickets.Where(t => t.Priority == SelectPriority);
            }
            if (!String.IsNullOrEmpty(SelectAssigne))
            {
                tickets = tickets.Where(t => t.Assigne == SelectAssigne);
            }
            {
                tickets = tickets.Where(t => t.State == SelectStatus);
            }
            if (!String.IsNullOrEmpty(SelectPriority))
            {
                tickets = tickets.Where(t => t.Priority == SelectPriority);
            }
            if (!String.IsNullOrEmpty(SelectAssigne))
            {
                tickets = tickets.Where(t => t.Assigne == SelectAssigne);
            }
            const int pageSize = 5;
            if (pg < 1)
                pg=1;
            int recsCount = tickets.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = tickets.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            return View(data);
        }

        // GET: Tickets/Details/5
        // aici fac query la tabela de commenturi in momentul in care se face click pe detalii
        [Authorize]
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

        public JsonResult Graph()
        {
            var tickets = from t in _context.Tickets
                          select t;
            return Json(tickets);
        }




        // GET: Tickets/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
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
                var _db = new ApplicationDbContext();
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
