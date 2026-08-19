<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Reports.aspx.cs" Inherits="ReportsPage" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Reports — Legacy Auth</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <article class="card">
    <h2>Weekly reports</h2>
    <p class="subtitle">Sample activity for this demo workspace.</p>

    <div class="table-wrap">
      <table class="data-table">
        <thead>
          <tr>
            <th>Report</th>
            <th>Owner</th>
            <th>Period</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>Login activity</td>
            <td>Ops</td>
            <td>This week</td>
            <td><span class="pill ok">Ready</span></td>
          </tr>
          <tr>
            <td>New registrations</td>
            <td>HR</td>
            <td>This week</td>
            <td><span class="pill ok">Ready</span></td>
          </tr>
          <tr>
            <td>Failed sign-ins</td>
            <td>Security</td>
            <td>Last 7 days</td>
            <td><span class="pill warn">Review</span></td>
          </tr>
          <tr>
            <td>Session summary</td>
            <td>IT</td>
            <td>Last 30 days</td>
            <td><span class="pill muted">Draft</span></td>
          </tr>
        </tbody>
      </table>
    </div>
  </article>
</asp:Content>
