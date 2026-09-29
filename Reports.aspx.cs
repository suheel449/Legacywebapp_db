using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

public partial class ReportsPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SiteMaster master = Master as SiteMaster;
        if (master != null)
        {
            master.PageTitle = "Reports";
            master.PageHint = "Insights";
        }

        try
        {
            List<ReportItem> reports = AppDb.GetReports();
            StringBuilder rows = new StringBuilder();
            foreach (ReportItem report in reports)
            {
                string pillClass = StatusCss(report.Status);
                rows.Append("<tr>")
                    .Append("<td>").Append(Server.HtmlEncode(report.Title)).Append("</td>")
                    .Append("<td>").Append(Server.HtmlEncode(report.OwnerTeam)).Append("</td>")
                    .Append("<td>").Append(Server.HtmlEncode(report.PeriodLabel)).Append("</td>")
                    .Append("<td><span class=\"pill ").Append(pillClass).Append("\">")
                    .Append(Server.HtmlEncode(report.Status))
                    .Append("</span></td>")
                    .Append("</tr>");
            }

            if (rows.Length == 0)
            {
                rows.Append("<tr><td colspan=\"4\">No reports found.</td></tr>");
            }

            litRows.Text = rows.ToString();
        }
        catch (Exception)
        {
            litRows.Text = "<tr><td colspan=\"4\">Could not load reports from LegacyBizDb.</td></tr>";
        }
    }

    private static string StatusCss(string status)
    {
        if (string.Equals(status, "Ready", StringComparison.OrdinalIgnoreCase))
        {
            return "ok";
        }

        if (string.Equals(status, "Review", StringComparison.OrdinalIgnoreCase))
        {
            return "warn";
        }

        return "muted";
    }
}
