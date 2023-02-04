using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChatChallenge.Pages
{
    public class ChatModel : PageModel
    {
        public string UserName { get; set; } = string.Empty;
        public void OnGet()
        {
            if(User.Identity is not null){
                UserName = User.Identity.Name!.Split('@')[0];
            }
        }
    }
}
