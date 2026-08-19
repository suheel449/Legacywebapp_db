<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="DashboardPage" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Dashboard — Legacy Auth</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <p class="lead"><asp:Literal ID="litWelcome" runat="server" /></p>

  <section class="stat-grid">
    <article class="card stat">
      <p class="stat-label">Open tasks</p>
      <p class="stat-value">8</p>
      <p class="stat-note">3 due this week</p>
    </article>
    <article class="card stat">
      <p class="stat-label">Reports</p>
      <p class="stat-value">12</p>
      <p class="stat-note">Last export yesterday</p>
    </article>
    <article class="card stat">
      <p class="stat-label">Notifications</p>
      <p class="stat-value">4</p>
      <p class="stat-note">2 unread</p>
    </article>
    <article class="card stat">
      <p class="stat-label">Team members</p>
      <p class="stat-value">6</p>
      <p class="stat-note">In-memory demo data</p>
    </article>
  </section>

  <section class="split-grid">
    <article class="card">
      <h2>Quick actions</h2>
      <div class="action-list">
        <a class="action" href="Profile.aspx">Update profile</a>
        <a class="action" href="Reports.aspx">View weekly reports</a>
        <a class="action" href="Notifications.aspx">Review alerts</a>
        <a class="action" href="Settings.aspx">Adjust preferences</a>
      </div>
    </article>
    <article class="card">
      <h2>Recent activity</h2>
      <ul class="timeline">
        <li><strong>Signed in</strong><span>Just now</span></li>
        <li><strong>Weekly summary generated</strong><span>Yesterday</span></li>
        <li><strong>Profile viewed</strong><span>2 days ago</span></li>
        <li><strong>Password last changed</strong><span>Demo only</span></li>
      </ul>
    </article>
  </section>
</asp:Content>
