using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

public partial class NotificationsPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SiteMaster master = Master as SiteMaster;
        if (master != null)
        {
            master.PageTitle = "Notifications";
            master.PageHint = "Alerts";
        }

        int userId = Session["UserId"] is int ? (int)Session["UserId"] : 0;
        if (userId <= 0)
        {
            litNotices.Text = "<li class=\"notice\">Not signed in.</li>";
            return;
        }

        try
        {
            List<NotificationItem> notices = AppDb.GetNotifications(userId);
            StringBuilder html = new StringBuilder();
            foreach (NotificationItem notice in notices)
            {
                string css = notice.IsRead ? "notice" : "notice unread";
                html.Append("<li class=\"").Append(css).Append("\">")
                    .Append("<strong>").Append(Server.HtmlEncode(notice.Title)).Append("</strong>")
                    .Append("<span>").Append(Server.HtmlEncode(notice.Body)).Append("</span>")
                    .Append("</li>");
            }

            if (html.Length == 0)
            {
                html.Append("<li class=\"notice\"><strong>No notifications</strong><span>You're all caught up.</span></li>");
            }

            litNotices.Text = html.ToString();
        }
        catch (Exception)
        {
            litNotices.Text = "<li class=\"notice\"><strong>Database unavailable</strong><span>Could not load notifications.</span></li>";
        }
    }
}
