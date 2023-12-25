using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using FinalYearProject.Models.ViewModels;
using Microsoft.AspNetCore.SignalR;

namespace FinalYearProject.Controllers
{
    [Area("Staff")]
    public class ChatboxController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ChatboxController(UserManager<IdentityUser> userManager, ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var aspId = User.Identity?.Name;

            if (aspId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            else
            {
                var currentUser = await _db.EmployeeDetails
                    .FirstOrDefaultAsync(e => e.user_id == aspId);

                var currentAdmin = await _db.Admin
                    .FirstOrDefaultAsync(a => a.admin_id == aspId);

                if (currentUser == null && currentAdmin == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
                else
                {
                    var chatList = await _db.Chatboxs
                        .Include(c => c.EmployeeDetails)
                        .Where(c =>
                            (currentUser != null && (c.staff_id == currentUser.employee_id || c.receive_id == currentUser.employee_id)) ||
                            (currentAdmin != null && (c.admin_id == currentAdmin.admin_id || c.receive_id == currentAdmin.admin_id)))
                        .ToListAsync();

                    var chatboxVM = new ChatboxVM
                    {
                        owner = currentAdmin,
                        owners = currentUser,
                        ChatCreated = chatList,
                        displayChatbox = new Models.Chatbox(),
                    };

                    return View(chatboxVM);
                }
            }
        }
        public async Task<IActionResult> Create()
        {
            var aspId = User.Identity?.Name;

            if (aspId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var currentUser = await _db.EmployeeDetails
                .FirstOrDefaultAsync(e => e.user_id == aspId);

            var currentAdmin = await _db.Admin
                .FirstOrDefaultAsync(a => a.admin_id == aspId);

            if (currentUser == null && currentAdmin == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var recipients = await _db.EmployeeDetails
                .Where(e => e.user_id != aspId)
                .Select(e => new SelectListItem { Value = e.employee_id, Text = $"{e.employee_name} (Staff)" })
                .ToListAsync();

            recipients.AddRange(await _db.Admin
                .Where(a => a.admin_id != aspId)
                .Select(a => new SelectListItem { Value = a.admin_id, Text = $"{a.admin_id} (Admin)" })
                .ToListAsync());

            var chatbox = new Chatbox
            {
                send_id = currentUser?.employee_id ?? currentAdmin?.admin_id,
                send_name = currentUser?.employee_name ?? currentAdmin?.admin_id,
                timestamp = DateTime.Now,
                chat_id = await GenerateChatID()
            };

            ViewBag.ReceiveOptions = recipients;
            chatbox.chat_ctn = "Hey";
            return View(chatbox);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Chatbox chatbox)
        {
            var aspid = User.Identity?.Name;

            if (ModelState.IsValid)
            {
                var aspId = User.Identity?.Name;
                var currentUser = await _db.EmployeeDetails.FirstOrDefaultAsync(e => e.user_id == aspId);
                var currentAdmin = await _db.Admin.FirstOrDefaultAsync(a => a.admin_id == aspId);

                if (currentUser != null)
                {
                    chatbox.staff_id = currentUser.employee_id;
                    chatbox.send_id = currentUser.employee_id;
                    chatbox.send_name = currentUser.employee_name;
                    chatbox.send_userid = currentUser.user_id;
                }
                else if (currentAdmin != null)
                {
                    chatbox.admin_id = currentAdmin.admin_id;
                    chatbox.send_id = currentAdmin.admin_id;
                    chatbox.send_name = currentAdmin.admin_id;
                    chatbox.send_userid= currentAdmin.admin_id;
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }

                if (!string.IsNullOrEmpty(chatbox.receive_id))
                {
                    var receiveEmployee = await _db.EmployeeDetails.FirstOrDefaultAsync(e => e.employee_id == chatbox.receive_id);
                    var receiveAdmin = await _db.Admin.FirstOrDefaultAsync(a => a.admin_id == chatbox.receive_id);

                    chatbox.receive_name = receiveEmployee?.employee_name ?? receiveAdmin?.admin_id;
                }

                chatbox.chat_id = await GenerateChatID();

                var initialChatMessage = new ChatMessage
                {
                    chatmsg_id = GenerateChatMessageID(),
                    chat_id = chatbox.chat_id,
                    send_id = chatbox.send_id,
                    send_name = chatbox.send_name,
                    send_userid = chatbox.send_userid,
                    receive_id = chatbox.receive_id,
                    receive_name = chatbox.receive_name,
                    chat_ctn = chatbox.chat_ctn,
                    timestamp = DateTime.Now
                };
                _db.ChatMessages.Add(initialChatMessage);

                _db.Chatboxs.Add(chatbox);

                await _db.SaveChangesAsync();

                return RedirectToAction("Chat", new { id = initialChatMessage.chat_id });
            }

            var recipients = await _db.EmployeeDetails
                .Where(e => e.user_id != aspid)
                .Select(e => new SelectListItem { Value = e.employee_id, Text = $"{e.employee_name} (Staff)" })
                .ToListAsync();

            recipients.AddRange(await _db.Admin
                .Where(a => a.admin_id != aspid)
                .Select(a => new SelectListItem { Value = a.admin_id, Text = $"{a.admin_id} (Admin)" })
                .ToListAsync());

            ViewBag.ReceiveOptions = recipients;

            return View(chatbox);
        }

        public async Task<IActionResult> Chat(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var chatMessages = await _db.ChatMessages
                .Where(c => c.chat_id == id)
                .OrderBy(c => c.timestamp)
                .ToListAsync();

            ViewBag.ChatId = id;

            return View(chatMessages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Chat(ChatMessage chatMessage)
        {
            if (ModelState.IsValid)
            {
                var aspid = User.Identity?.Name;
                var currentUser = await _db.EmployeeDetails.FirstOrDefaultAsync(e => e.user_id == aspid);
                var currentAdmin = await _db.Admin.FirstOrDefaultAsync(a => a.admin_id == aspid);

                chatMessage.send_name = currentUser?.employee_name ?? currentAdmin?.admin_id;
                chatMessage.chatmsg_id = GenerateChatMessageID();
                chatMessage.timestamp = DateTime.Now;

                _db.ChatMessages.Add(chatMessage);
                await _db.SaveChangesAsync();

                return RedirectToAction("Chat", new { id = chatMessage.chat_id });
            }

            var chatId = Request.Form["chat_id"];
            ViewBag.ChatId = chatId;

            return View("Chat", chatMessage);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chat = await _db.Chatboxs.FindAsync(id);

            if (chat == null)
            {
                return NotFound();
            }

            return View(chat);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var chat = await _db.Chatboxs.FindAsync(id);

            if (chat == null)
            {
                return NotFound();
            }

            _db.Chatboxs.Remove(chat);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private string GenerateChatMessageID()
        {
            string newId;
            string prefix = "CM";
            do
            {
                newId = prefix + Guid.NewGuid().ToString("N").Substring(0, 10);
            } while (_db.ChatMessages.Any(c => c.chatmsg_id == newId));

            return newId;
        }

        private async Task<string> GenerateChatID()
        {
            string newId;
            string prefix = "CT";

            var totalChats = await _db.Chatboxs.CountAsync();

            newId = prefix + (totalChats + 1).ToString("00000");

            return newId;
        }
    }
}
